using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Text;

namespace Test
{
    public class ML_Blog
    {
        public ML_Blog()
        {
            string sampleFile = "test\\KRW-BTC_sample.csv";
            string laterFile  = "test\\KRW-BTC_later.csv";

            var predictor = GetPredictionEngine(out var mlContext, sampleFile);

            var lines = File.ReadAllLines(laterFile);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"1m,2m,3m,4m,5m");
            int count = 1;
            foreach (var line in lines)
            {
                if (!float.TryParse(line, out float price))
                    continue;

                var prediction = predictor.Predict(new CoinPrice { Price = price });
                string scores = string.Join(',', prediction.Score);
                sb.AppendLine(scores);

                Console.WriteLine($"[{count++}] {scores}");
            }

            // predictor.CheckPoint(mlContext, "TrainedModel");

            string marketCode = Path.GetFileNameWithoutExtension(sampleFile);
            string saveName = $"result\\{marketCode}.csv";
            File.WriteAllText(saveName, sb.ToString());
        }

        TimeSeriesPredictionEngine<CoinPrice, CoinPricePrediction> GetPredictionEngine(out MLContext mlContext, string csvpath)
        {
            mlContext = new MLContext(seed: 0);
            // csv 파일 로드
            var lines = File.ReadAllLines(csvpath);

            if (File.Exists("TrainedModel"))
            {
                // Load the forecast engine that has been previously saved.
                using var file = File.OpenRead("TrainedModel");
                ITransformer forecaster;
                forecaster = mlContext.Model.Load(file, out DataViewSchema schema);

                // We must create a new prediction engine from the persisted model.
                return forecaster.CreateTimeSeriesEngine<CoinPrice, CoinPricePrediction>(mlContext);
            }
            else
            {
                // STEP 1: Common data loading configuration
                IDataView baseTrainingDataView = mlContext.Data.LoadFromTextFile<CoinPrice>(csvpath, hasHeader: true, separatorChar: ',');

                var trainer = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: nameof(CoinPricePrediction.Score),
                    inputColumnName: nameof(CoinPrice.Price),
                    windowSize: 30,
                    seriesLength: lines.Length,
                    trainSize: lines.Length,
                    horizon: 5,
                    confidenceLevel: 0.8f,
                    // confidenceLevel: 1f,
                    confidenceLowerBoundColumn: "Features",
                    confidenceUpperBoundColumn: "Features");

                //var trainingPipeline = dataProcessPipeline.Append(trainer);

                ITransformer trainedModel = trainer.Fit(baseTrainingDataView);

                return trainedModel.CreateTimeSeriesEngine<CoinPrice, CoinPricePrediction>(mlContext);
            }
        }
    }

    public class CoinPricePrediction
    {
        [ColumnName("Score")]
        public float[] Score;
    }

    public class CoinPrice
    {
        [LoadColumn(0)]
        public float Price { get; set; }
    }
}

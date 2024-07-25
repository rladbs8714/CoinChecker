using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Generalibrary;

namespace Test
{
    public class ML
    {
        public ML()
        {
            var files = Directory.GetFiles("csv");

            for (int loop = 0; loop < files.Length; loop++)
            {
                string file = files[loop];

                // csv 파일 로드
                var lines = File.ReadAllLines(file);
                var prices = new List<TimeSeriesData>();

                // 헤더 줄을 건너뛰고 가격 데이터를 파싱
                foreach (var line in lines.Skip(1))
                {
                    var columns = line.Split(',');
                    if (float.TryParse(columns[6], out float price))  // 네 번째 컬럼이 가격 데이터라고 가정
                    {
                        prices.Add(new TimeSeriesData { Value = price });
                    }
                }

                // MLContext 생성
                var mlContext = new MLContext();

                // 데이터 로드
                var data = mlContext.Data.LoadFromEnumerable(prices);

                // 학습용 데이터와 테스트용 데이터 분리
                int trainSize = (int)(prices.Count * 0.9);
                int horizon = prices.Count - trainSize;
                var trainData = mlContext.Data.TakeRows(data, trainSize);
                var testData = mlContext.Data.SkipRows(data, trainSize);

                // 시계열 예측 파이프라인 생성
                var pipeline = mlContext.Forecasting.ForecastBySsa(
                    outputColumnName: nameof(ForecastedData.ForecastedValues),
                    inputColumnName: nameof(TimeSeriesData.Value),
                    windowSize: 30,  // 윈도우 크기
                    seriesLength: prices.Count,  // 시계열 길이
                    trainSize: trainSize,  // 학습 데이터 크기
                    horizon: horizon,  // 예측할 기간
                    confidenceLevel: 0.95f,  // 신뢰 수준
                    confidenceLowerBoundColumn: nameof(ForecastedData.ConfidenceLowerBound),
                    confidenceUpperBoundColumn: nameof(ForecastedData.ConfidenceUpperBound));

                // 모델 학습
                var model = pipeline.Fit(data);

                // 예측
                var forecastEngine = model.CreateTimeSeriesEngine<TimeSeriesData, ForecastedData>(mlContext);
                var forecast = forecastEngine.Predict();

                // 예측 결과 출력
                //for (int i = 0; i < forecast.ForecastedValues.Length; i++)
                //{
                //    Console.WriteLine($"예측된 가격 {i + 1} 단계: {forecast.ForecastedValues[i]}");
                //}

                // 예측된 값 CSV 파일로 저장
                var forecastLines = new List<string> { "Step,ForecastedPrice,ConfidenceLowerBound,ConfidenceUpperBound" };
                for (int i = 0; i < forecast.ForecastedValues.Length; i++)
                {
                    forecastLines.Add($"{i + 1},{forecast.ForecastedValues[i]},{forecast.ConfidenceLowerBound[i]},{forecast.ConfidenceUpperBound[i]}");
                }
                string marketCode = Path.GetFileNameWithoutExtension(file);
                string saveName = $"result\\{marketCode}.csv";
                File.WriteAllLines(saveName, forecastLines);

                LogManager.Instance.Info("TEST", "MAIN", $"[{marketCode}] ({loop + 1}/{files.Length}) 예측 완료 및 {marketCode}.csv 파일로 저장됨");
            }
        }

        public class TimeSeriesData
        {
            public float Value { get; set; }
        }

        public class ForecastedData
        {
            public float[] ForecastedValues { get; set; }
            public float[] ConfidenceLowerBound { get; set; }
            public float[] ConfidenceUpperBound { get; set; }
        }
    }
}

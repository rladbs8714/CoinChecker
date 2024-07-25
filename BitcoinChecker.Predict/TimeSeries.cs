using Generalibrary;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using System.Reflection;

namespace BitcoinChecker.Predict
{

    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.20
     *  
     *  < 목적 >
     *  - ML.Net 으로 코인의 미래 가격을 예측한다.
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.07.20 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class TimeSeries
    {

        // ====================================================================
        // INNER CLASS
        // ====================================================================

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


        // ====================================================================
        // CONSTANTS
        // ====================================================================

        /// <summary>
        /// log
        /// </summary>
        private readonly ILogManager LOG = LogManager.Instance;

        /// <summary>
        /// log type
        /// </summary>
        private const string LOG_TYPE = "TimeSeries";

        /// <summary>
        /// 예측 결과 파일 이름 포멧 <br />
        /// 0: 마켓명 (e.g. KRW-BTC) <br />
        /// 1: 파일 생성 시간        <br />
        /// 2: 예측 범위(m) (e.g. 1m, 3m, 5m 등) <br />
        /// 사용은 <see cref="string.Format(string, object?[])"/> 으로 한다.
        /// </summary>
        private const string RESULT_FILE_NAME_FORMAT = "result\\{0}_{1}_{2}.csv";

        // ====================================================================
        // FIELDS
        // ====================================================================

        /// <summary>
        /// 예측 엔진
        /// </summary>
        private TimeSeriesPredictionEngine<CoinPrice, CoinPricePrediction> _predictor;

        /// <summary>
        /// 과거 코인 가격을 기록한 csv
        /// </summary>
        private StreamWriter? _historyCsvStream;

        /// <summary>
        /// 예측 결과 csv
        /// </summary>
        private StreamWriter? _resultCsvStream;


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        /// <summary>
        /// 시계열 예측 클래스의 생성자
        /// </summary>
        /// <param name="marketName">코인 이름 (e.g. KRW-BTC)</param>
        /// <param name="historyCsvFileName">과거 코인 가격을 기록한 csv 파일 이름</param>
        /// <param name="hasHeader">csv 파일 헤더 유무</param>
        /// <param name="predictionRange">분(minutes) 기준 예측 범위. 1보다 작을 수 없다.</param>
        /// <param name="saveToFile">예측된 가격 저장 여부</param>
        /// <param name="appendPriceToCsv">코인의 갱신된 가격 저장 여부</param>
        /// <exception cref="ArgumentException"><paramref name="historyCsvFileName"/><paramref name="predictionRange"/></exception>
        public TimeSeries(string marketName, string historyCsvFileName, bool hasHeader, int predictionRange = 5, bool saveToFile = false, bool appendPriceToCsv = true)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            if (string.IsNullOrEmpty(historyCsvFileName))
                throw new ArgumentException($"\'{nameof(historyCsvFileName)}\'의 값이 공백이거나 null 입니다.");
            if (!Uri.TryCreate(historyCsvFileName, UriKind.Absolute, out _))
                historyCsvFileName = Path.Combine(Environment.CurrentDirectory, historyCsvFileName);
            
            if (predictionRange < 1)
                throw new ArgumentException($"\'{nameof(predictionRange)}\'의 값은 1보다 작을 수 없습니다.");

            _predictor = GetPredictionEngine(out MLContext mlContext, historyCsvFileName, hasHeader, predictionRange);

            // 예측 결과 저장 파일 및 스트림 생성
            if (saveToFile)
            {
                string fileName = string.Format(RESULT_FILE_NAME_FORMAT, marketName, DateTime.Now.ToString("yyyyMMddHHmmss"), $"{predictionRange}m");
                string? folder = Path.GetDirectoryName(fileName);

                if (string.IsNullOrEmpty(folder))
                {
                    LOG.Error(LOG_TYPE, doc, $"{nameof(fileName)}의 형식이 유효하지 않습니다.{Environment.NewLine}{nameof(fileName)}:{fileName}");
                    return;
                }

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                fileName = Path.Combine(Environment.CurrentDirectory, fileName);

                if (File.Exists(fileName))
                {
                    LOG.Error(LOG_TYPE, doc, $"{fileName} 파일이 이미 존재합니다.");
                    return;
                }

                _resultCsvStream = new FileInfo(fileName).AppendText();
                _resultCsvStream.Write($"Update Time");
                for (int i = 0; i < predictionRange; i++)
                    _resultCsvStream.Write($",Later {i}");
                _resultCsvStream.Write(Environment.NewLine);
            }

            // 갱신된 가격 저장 스트림 생성
            if (appendPriceToCsv)
            {
                _historyCsvStream = new FileInfo(historyCsvFileName).AppendText();
                _historyCsvStream.AutoFlush = true;
            }
        }


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        /// <summary>
        /// 예측된 가격을 반환한다.
        /// </summary>
        /// <param name="price">갱신된 가격</param>
        /// <returns>예측된 가격</returns>
        public float[] GetPredictedPrice(float price)
        {
            var prediction = _predictor.Predict(new CoinPrice { Price = price });
            string predictedPrices = string.Join(',', prediction.Score);

            _historyCsvStream?.Write($"{price}{Environment.NewLine}");
            _resultCsvStream?.Write($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},{predictedPrices}{Environment.NewLine}");

            return prediction.Score;
        }

        /// <summary>
        /// 예측 엔진을 생성한다
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="csvpath">과거 가격 기록이 담긴 csv</param>
        /// <param name="hasHeader">csv 헤더 유무</param>
        /// <param name="predictionRange">분 기준 예측 범위. 1보다 작을 수 없다.</param>
        /// <returns>예측 엔진</returns>
        private TimeSeriesPredictionEngine<CoinPrice, CoinPricePrediction> GetPredictionEngine(out MLContext mlContext, string csvpath, bool hasHeader, int predictionRange = 5)
        {
            mlContext = new MLContext(seed: 0);

            int dataSize = File.ReadAllLines(csvpath).Length;
            dataSize = hasHeader ? dataSize - 1 : dataSize;

            // Common data loading configuration
            IDataView baseTrainingDataView = mlContext.Data.LoadFromTextFile<CoinPrice>(csvpath, hasHeader: hasHeader, separatorChar: ',');

            var trainer = mlContext.Forecasting.ForecastBySsa(
                outputColumnName: nameof(CoinPricePrediction.Score),
                inputColumnName: nameof(CoinPrice.Price),
                windowSize: 30,
                seriesLength: dataSize,
                trainSize: dataSize,
                horizon: predictionRange,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "Features",
                confidenceUpperBoundColumn: "Features");

            ITransformer trainedModel = trainer.Fit(baseTrainingDataView);

            return trainedModel.CreateTimeSeriesEngine<CoinPrice, CoinPricePrediction>(mlContext);
        }
    }
}

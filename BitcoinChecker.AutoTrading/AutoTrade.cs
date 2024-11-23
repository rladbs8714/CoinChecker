using BitcoinChecker.Core;
using BItcoinChecker.Core;
using Generalibrary;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;

namespace BitcoinChecker.AutoTrading
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.11.23
     *  
     *  < 목적 >
     *  - 업비트 자동 트레이딩 기능을 작성한다
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.11.23 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class AutoTrade
    {
        // ====================================================================
        // CLASS
        // ====================================================================

        /// <summary>
        /// 스캘핑 메서드
        /// </summary>
        private class ScalpingExecuteParameters
        {
            public string Market { get; set; }

            public int Count { get; set; }

            public double Threshold { get; set; }

            public double TradeVolume { get; set; }
        }


        // ====================================================================
        // ENUMS
        // ====================================================================

        /// <summary>
        /// 매수/중립/매도 신호
        /// </summary>
        private enum ESignal
        {
            /// <summary>
            /// 매수
            /// </summary>
            Buy,
            /// <summary>
            /// 중립
            /// </summary>
            Hold,
            /// <summary>
            /// 매도
            /// </summary>
            Sell
        }


        // ====================================================================
        // FIELDS
        // ====================================================================

        private const string LOG_TYPE = "Auto_Trade";

        private readonly ILogManager LOG = LogManager.Instance;

        private readonly UpbitCore _upbitCore;

        private readonly string Market;

        private readonly string To;

        private readonly int Count;

        private double _buyPrice = 0;

        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        public AutoTrade(string market, string to, int count, bool isDebug = false, bool isTest = false)
        {
            _upbitCore = new UpbitCore(isDebug);

            if (isTest)
                return;

            Market = market;
            To = to;
            Count = count;

            ScalpingExecuteParameters @params = new ScalpingExecuteParameters()
            {
                Market = Market,
                Count = Count,
                Threshold = 3.0,
                TradeVolume = 0.01
            };
            _ = new System.Threading.Timer(ExecuteSecondScalping, @params, 1000, 0);
        }


        // ====================================================================
        // METHODS
        // ====================================================================

        /// <summary>
        /// 거래량 비율을 계산한다
        /// </summary>
        /// <param name="candles">이전의 캔들 평균 거래량</param>
        /// <returns>거래량 비율</returns>
        /// <exception cref="ArgumentException"></exception>
        private double CalculateSecondVolumeRatio(SecondsCandles_VO[] candles)
        {
            if (candles == null || candles.Length < 2)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            // 현재 캔들 거래량
            double currentVolume = candles[0].CandleAccTradeVolume;

            // 이전 N개의 평균 거래량 계산
            double averageVolume = candles.Skip(1).Average(c => c.CandleAccTradeVolume);

            return currentVolume / averageVolume;
        }

        /// <summary>
        /// 거래량 비율이 특정 임계값을 초과하면 매수 또는 매도신호 생성
        /// </summary>
        /// <param name="volumeRatio">거래량 비율</param>
        /// <param name="threshold">임계값</param>
        /// <returns>매수/중립/매도 신호</returns>
        private ESignal GenerateSecondTradeSignal(SecondsCandles_VO[] candles, double volumeRatio, double threshold)
        {
            double targetProfit = 0.5;
            double stopLoss     = 0.2;
            double currentPrice = candles[0].TradePrice;
            double rsi = GetRSI(candles, candles.Length);
            if (volumeRatio > threshold && rsi < 30)
            {
                return ESignal.Buy; // 상승 신호 시 매수
            }
            else
            {
                double profitRate = (currentPrice - _buyPrice) / _buyPrice;
                if (profitRate  >= targetProfit ||   // 목표 수익률 도달
                    profitRate  <= -stopLoss ||      // 손절 조건
                    volumeRatio < 1 && rsi > 70)    // 매도 신호 발생
                {
                    return ESignal.Sell;
                }
            }
            return ESignal.Hold; // 신호 없음
        }

        /// <summary>
        /// RSI를 계산하여 가져온다
        /// </summary>
        /// <param name="candles">초봉 캔들</param>
        /// <param name="period">기간</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public double GetRSI(CandlesBase_VO[] candles, int period)
        {
            if (candles == null || candles.Length < period + 1)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            double gain = 0; // 상승 합계
            double loss = 0; // 하락 합계

            // 초기 상승/하락 계산
            for (int i = 1; i <= period; i++)
            {
                double change = candles[i - 1].TradePrice - candles[i].TradePrice;

                if (change > 0)
                {
                    gain += change;
                }
                else
                {
                    loss -= change; // 하락값은 양수로 전환
                }
            }

            // 초기 평균 상승/하락 계산
            double avgGain = gain / period;
            double avgLoss = loss / period;

            // RSI 계산
            for (int i = period + 1; i < candles.Length; i++)
            {
                double change = candles[i - 1].TradePrice - candles[i].TradePrice;

                if (change > 0)
                {
                    avgGain = ((avgGain * (period - 1)) + change) / period;
                    avgLoss = (avgLoss * (period - 1)) / period;
                }
                else
                {
                    avgGain = (avgGain * (period - 1)) / period;
                    avgLoss = ((avgLoss * (period - 1)) - change) / period;
                }
            }

            double rs = avgGain / avgLoss;
            double rsi = 100 - (100 / (1 + rs));

            return rsi;
        }

        /// <summary>
        /// 초 기준 스캘핑을 실행한다
        /// </summary>
        /// <param name="o"></param>
        private void ExecuteSecondScalping(object? o)
        {
            if (o is not ScalpingExecuteParameters parameters)
                return;

            string doc         = MethodBase.GetCurrentMethod().Name;
            string market      = parameters.Market;
            int    count       = parameters.Count;
            double threshold   = parameters.Threshold;
            double tradeVolume = parameters.TradeVolume;

            try
            {
                if (!_upbitCore.TryGetSecondCandles(out SecondsCandles_VO[]? candles, market, "", count) || candles == null)
                {
                    LOG.Warning(LOG_TYPE, doc, "초봉 데이터를 가져오지 못했습니다.");
                    return;
                }

                double  volumeRatio = CalculateSecondVolumeRatio(candles);
                ESignal signal      = GenerateSecondTradeSignal(candles, volumeRatio, threshold);

                if (signal == ESignal.Buy)
                {
                    LOG.Info(LOG_TYPE, doc, "매수 신호 발생! 주문을 실행합니다.");

                    if (_upbitCore.TryOrders(
                        out Orders_VO? orderResult,
                        market,
                        UpbitCore.ESide.bid,
                        tradeVolume,
                        candles[0].TradePrice, // 현재가로 매수
                        UpbitCore.EOrderType.market))
                    {
                        LOG.Info(LOG_TYPE, doc, $"주문 성공! 주문 ID: {orderResult?.UUID}");
                    }
                    else
                    {
                        LOG.Warning(LOG_TYPE, doc, "주문 실패.");
                    }
                }
                else if (signal == ESignal.Sell)
                {
                    LOG.Info(LOG_TYPE, doc, "매도 신호 발생! 주문을 실행합니다.");
                    if (_upbitCore.TryOrders(
                        out Orders_VO? orderResult,
                        market,
                        UpbitCore.ESide.ask,
                        tradeVolume,
                        candles[0].TradePrice, // 현재가로 매도
                        UpbitCore.EOrderType.market))
                    {
                        LOG.Info(LOG_TYPE, doc, $"주문 성공! 주문 ID: {orderResult?.UUID}");
                    }
                    else
                    {
                        LOG.Warning(LOG_TYPE, doc, "주문 실패.");
                    }
                }
                else
                {
                    LOG.Warning(LOG_TYPE, doc, "신호 없음.");
                }
            }
            catch (Exception ex)
            {
                LOG.Error(LOG_TYPE, doc, $"에러 발생: {ex.Message}");
            }
        }


        // ====================================================================
        // METHODS - TEST
        // ====================================================================

        /// <summary>
        /// 업비트 api 테스트 객체
        /// </summary>
        private UpbitApiTest _upbitApiTest;

        /// <summary>
        /// 업비트 사용자 프로필을 반환한다
        /// </summary>
        private UpbitProfile _profile => _upbitApiTest.GetProfile();

        private double Money; 

        /// <summary>
        /// 시뮬레이션 시작
        /// </summary>
        /// <param name="sampleFilePath">참조할 데이터 경로</param>
        public void StartSimulationTest(double money)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            LOG.Info(LOG_TYPE, doc, $"시뮬레이션 설정 완료");
            Money = money;
            _upbitApiTest = new UpbitApiTest(money);

            LOG.Info(LOG_TYPE, doc, $"시뮬레이션 시작");

            Stopwatch sw = Stopwatch.StartNew();
            ExecuteSecondScalping_TEST();
            sw.Stop();

            LOG.Info(LOG_TYPE, doc, $"시뮬레이션 작동 시간: {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// 거래량 비율을 계산한다
        /// </summary>
        /// <param name="tradeVols">이전의 캔들 평균 거래량</param>
        /// <returns>거래량 비율</returns>
        /// <exception cref="ArgumentException"></exception>
        private double CalculateSecondVolumeRatio_TEST(List<double> tradeVols)
        {
            if (tradeVols == null || tradeVols.Count < 2)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            // 현재 캔들 거래량
            double currentVolume = tradeVols.Last();

            // 이전 N개의 평균 거래량 계산
            double averageVolume = tradeVols.SkipLast(1).Average();

            return currentVolume / averageVolume;
        }

        /// <summary>
        /// 거래량 비율이 특정 임계값을 초과하면 매수 또는 매도신호 생성
        /// </summary>
        /// <param name="volumeRatio">거래량 비율</param>
        /// <param name="threshold">임계값</param>
        /// <returns>매수/중립/매도 신호</returns>
        private ESignal GenerateSecondTradeSignal_TEST(List<double> prices, double volumeRatio, double threshold)
        {
            double targetProfit = 0.5;
            double stopLoss = 0.2;
            double currentPrice = prices.Last();
            double rsi = GetRSI_TEST(prices, prices.Count - 1);
            if (volumeRatio > threshold && rsi < 30)
            {
                return ESignal.Buy; // 상승 신호 시 매수
            }
            else
            {
                double profitRate = (currentPrice - _buyPrice) / _buyPrice;
                if (profitRate >= targetProfit ||   // 목표 수익률 도달
                    profitRate <= -stopLoss    ||   // 손절 조건
                    volumeRatio < 1 && rsi > 70)    // 매도 신호 발생
                {
                    return ESignal.Sell;
                }
            }
            return ESignal.Hold; // 신호 없음
        }

        /// <summary>
        /// <b>[테스트]</b> <br />
        /// RSI를 계산하여 가져온다
        /// </summary>
        /// <param name="prices">초봉 캔들</param>
        /// <param name="period">기간</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public double GetRSI_TEST(List<double> prices, int period)
        {
            if (prices == null || prices.Count < period + 1)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            double gain = 0; // 상승 합계
            double loss = 0; // 하락 합계

            // 초기 상승/하락 계산
            for (int i = 1; i <= period; i++)
            {
                double change = prices[i - 1] - prices[i];

                if (change > 0)
                {
                    gain += change;
                }
                else
                {
                    loss -= change; // 하락값은 양수로 전환
                }
            }

            // 초기 평균 상승/하락 계산
            double avgGain = gain / period;
            double avgLoss = loss / period;

            // RSI 계산
            for (int i = period + 1; i < prices.Count; i++)
            {
                double change = prices[i - 1] - prices[i];

                if (change > 0)
                {
                    avgGain = ((avgGain * (period - 1)) + change) / period;
                    avgLoss = (avgLoss * (period - 1)) / period;
                }
                else
                {
                    avgGain = (avgGain * (period - 1)) / period;
                    avgLoss = ((avgLoss * (period - 1)) - change) / period;
                }
            }

            double rs = avgGain / avgLoss;
            double rsi = 100 - (100 / (1 + rs));

            return rsi;
        }

        /// <summary>
        /// 초 기준 스캘핑을 실행한다
        /// </summary>
        private void ExecuteSecondScalping_TEST()
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            List<double> prices = new List<double>();
            List<double> tradeVols = new List<double>();
            int lineCount = 0;
            using FileStream fs = new FileStream("test.csv", FileMode.Open, FileAccess.Read, FileShare.Read, 65536);
            using (StreamReader sr = new StreamReader(fs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sp = line.Split(',');
                    prices   .Add(double.Parse(sp[1]));
                    tradeVols.Add(double.Parse(sp[2]));
                    ++lineCount;
                }
            }

            const int criteria = 1_000;
            for (int i = criteria; i < lineCount; i++)
            {
                List<double> newPrice = prices.GetRange(i - criteria, criteria);
                List<double> newTradeVols = tradeVols.GetRange(i - criteria, criteria);

                double volumeRatio = CalculateSecondVolumeRatio_TEST(newTradeVols);
                ESignal signal = GenerateSecondTradeSignal_TEST(newPrice, volumeRatio, 3.0);

                if (signal == ESignal.Buy)
                {
                    LOG.Info(LOG_TYPE, doc, "매수 신호 발생! 주문을 실행합니다.");

                    double volume = _profile.Money / prices[i];
                    if (_upbitApiTest.TryOrder(Market, UpbitCore.ESide.bid, volume, prices[i]))
                    {
                        LOG.Info(LOG_TYPE, doc, $"매수 성공!");
                    }
                }
                else if (signal == ESignal.Sell)
                {
                    LOG.Info(LOG_TYPE, doc, "매도 신호 발생! 주문을 실행합니다.");
                    if (_upbitApiTest.TryOrder("", UpbitCore.ESide.ask, 0, 0))
                    {
                        LOG.Info(LOG_TYPE, doc, $"매도 성공!");
                    }
                }
                else
                {
                    LOG.Warning(LOG_TYPE, doc, "신호 없음.");
                }
            }

            _profile.SellAll();
            double change = ((_profile.Money - Money) / Money) * 100;
            LOG.Info(LOG_TYPE, doc, $"최종 현금: {_profile.Money}");
            string str = change < Money ? "잃었습니다" : "벌었습니다";
            LOG.Info(LOG_TYPE, doc, $"{change}% {str}");
        }
    }
}

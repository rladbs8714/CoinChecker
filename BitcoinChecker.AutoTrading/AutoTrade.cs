using BitcoinChecker.Core;
using BItcoinChecker.Core;
using Generalibrary;
using System.Diagnostics;
using System.Reflection;

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
            public string? Market { get; set; }

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
            Sell,
            /// <summary>
            /// 손절
            /// </summary>
            StopLoss,
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

        public AutoTrade(string market, string to, int count = 10, bool isDebug = false, bool isTest = false)
        {
            _upbitCore = new UpbitCore(isDebug);

            if (isTest)
                return;

            Market = market;
            To = to;
            Count = count;

            ScalpingExecuteParameters @params = new ScalpingExecuteParameters()
            {
                Market = Market.Split('-')[1],
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
        private double CalculateSecondVolumeRatio(SecondsCandles_VO[] tradeVols, double multiplier)
        {
            if (tradeVols == null || tradeVols.Length < 2)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            // 현재 캔들 거래량
            double currentVolume = tradeVols[tradeVols.Length - 1].CandleAccTradeVolume;

            // 이전 N개의 평균 거래량 계산
            double averageVolume = tradeVols.SkipLast(1).Average(c => c.CandleAccTradeVolume);

            // return currentVolume / averageVolume;

            return averageVolume * multiplier;
        }

        /// <summary>
        /// 거래량 비율이 특정 임계값을 초과하면 매수 또는 매도신호 생성
        /// </summary>
        /// <param name="volumeRatio">거래량 비율</param>
        /// <param name="threshold">임계값</param>
        /// <returns>매수/중립/매도 신호</returns>
        private ESignal GenerateSecondTradeSignal(SecondsCandles_VO[] prices, double volume, double threshold)
        {
            double targetProfit = 0.5;
            double stopLoss = 0.2;
            double currentPrice = prices[prices.Length - 1].TradePrice;
            //double rsi = GetRSI_TEST(prices, prices.Count - 1);
            double rsi = GetRSI(prices, prices.Length - 1);
            if (volume > threshold && rsi < 30)
            {
                return ESignal.Buy; // 상승 신호 시 매수
            }
            else
            {
                double profitRate = (currentPrice - _buyPrice) / _buyPrice;
                if (double.IsInfinity(profitRate) || double.IsNaN(profitRate))
                    profitRate = 0;

                if (profitRate <= -stopLoss)        // 손절 조건
                    return ESignal.StopLoss;
                if (profitRate >= targetProfit ||   // 목표 수익률 도달
                    volume < 1 && rsi > 70)         // 매도 신호 발생
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
        public double GetRSI(SecondsCandles_VO[] prices, int period)
        {
            if (prices == null || prices.Length < period + 1)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            double gain = 0; // 상승 합계
            double loss = 0; // 하락 합계

            // 초기 상승/하락 계산 (첫 번째 기간)
            for (int i = 1; i <= period; i++)
            {
                double change = prices[i].TradePrice - prices[i - 1].TradePrice;

                if (change > 0)
                {
                    gain += change;
                }
                else
                {
                    loss -= change; // 하락값을 양수로 전환
                }
            }

            // 초기 평균 상승/하락 계산
            double avgGain = gain / period;
            double avgLoss = loss / period;

            // RSI 계산 루프
            for (int i = period + 1; i < prices.Length; i++)
            {
                double change = prices[i].TradePrice - prices[i - 1].TradePrice;

                if (change > 0)
                {
                    avgGain = ((avgGain * (period - 1)) + change) / period;
                    avgLoss = (avgLoss * (period - 1)) / period; // 하락값 없음
                }
                else
                {
                    avgGain = (avgGain * (period - 1)) / period; // 상승값 없음
                    avgLoss = ((avgLoss * (period - 1)) - change) / period;
                }
            }

            // 안정성을 위한 예외 처리 (avgLoss가 0인 경우)
            if (avgLoss == 0)
            {
                return 100; // 완전한 과매수 상태
            }

            // RSI 계산
            double rs = avgGain / avgLoss;
            double rsi = 100 - (100 / (1 + rs));

            return rsi;
        }

        /// <summary>
        /// 초 기준 스캘핑을 실행한다
        /// </summary>
        /// <param name="o"><see cref="ScalpingExecuteParameters"/> 주문 정보</param>
        private void ExecuteSecondScalping(object? o)
        {
            if (o is not ScalpingExecuteParameters parameters)
                return;
            if (string.IsNullOrEmpty(parameters.Market))
                return;

            string doc         = MethodBase.GetCurrentMethod().Name;
            string market      = parameters.Market;
            int    count       = parameters.Count;
            double threshold   = parameters.Threshold;
            double tradeVolume = parameters.TradeVolume;

            try
            {
                if (!_upbitCore.TryGetSecondCandles(out SecondsCandles_VO[]? candles, market, "", count) || 
                    candles == null)
                {
                    LOG.Warning(LOG_TYPE, doc, "초봉 데이터를 가져오지 못했습니다.");
                    return;
                }

                candles = candles.Reverse().ToArray();

                double currentVolume = candles[candles.Length - 1].CandleAccTradeVolume;
                double dynamicVolume = CalculateSecondVolumeRatio(candles, 3);
                ESignal signal  = GenerateSecondTradeSignal(candles, currentVolume, dynamicVolume);

                if (signal == ESignal.Buy)
                {
                    LOG.Info(LOG_TYPE, doc, "매수 신호 발생! 주문을 실행합니다.");

                    if (!_upbitCore.TryGetAccounts(out Accounts_VO? accounts))
                    {
                        LOG.Warning(LOG_TYPE, doc, "주문 실패. - 사용자 정보를 불러올 수 없었습니다.");
                        return;
                    }

                    string currency = Market.Split('-')[0];
                    Accounts_VO.Account? account = accounts.Accounts.Find(x => x.Currency == currency);
                    if (account == null)
                    {
                        LOG.Warning(LOG_TYPE, doc, $"주문 실패. - {currency} 화폐가 없습니다.");
                        return;
                    }
                    if (!double.TryParse(account.Balance, out double balance) || balance < 5000)
                    {
                        LOG.Warning(LOG_TYPE, doc, $"주문 실패. - {currency} 화폐 단위가 잘못되었거나 최소 주문 금액(5,000원) 미만입니다.");
                        return;
                    }

                    double currentPrice = candles[candles.Length - 1].TradePrice;
                    double volume = balance / currentPrice;

                    if (_upbitCore.TryOrders(
                        out Orders_VO? orders,
                        market,
                        UpbitCore.ESide.bid,
                        volume,
                        candles[candles.Length - 1].TradePrice, // 현재가로 매수
                        UpbitCore.EOrderType.market) && orders != null)
                    {
                        LOG.Info(LOG_TYPE, doc, $"주문 성공! 주문 ID: {orders.UUID}");
                    }
                    else
                    {
                        LOG.Warning(LOG_TYPE, doc, "주문 실패.");

                        // 주문 실패 시 주문 취소
                        if (!_upbitCore.TryDeleteOrder(out Order_VO? order, orders.UUID, string.Empty))
                        {
                            LOG.Error(LOG_TYPE, doc, "주문 취소가 정상적으로 이루어지지 않았습니다.");
                        }
                        else
                        {
                            LOG.Info(LOG_TYPE, doc, "주문 취소 완료");
                        }

                        return;
                    }
                }
                else if (signal == ESignal.Sell || signal == ESignal.StopLoss)
                {
                    LOG.Info(LOG_TYPE, doc, "매도 신호 발생! 주문을 실행합니다.");

                    if (!_upbitCore.TryGetAccounts(out Accounts_VO? accounts))
                        LOG.Warning(LOG_TYPE, doc, "주문 실패. - 사용자 정보를 불러올 수 없었습니다.");

                    string currency = market.Split('-')[1];
                    Accounts_VO.Account? account = accounts.Accounts.Find(x => x.Currency == currency);
                    if (account == null)
                        LOG.Warning(LOG_TYPE, doc, $"주문 실패. - {currency} 화폐가 없습니다.");
                    if (!double.TryParse(account.Balance,     out double balance)     || 
                        !double.TryParse(account.AvgBuyPrice, out double avgBuyPrice) || 
                        balance == 0 || balance * avgBuyPrice < 5000)
                    {
                        LOG.Warning(LOG_TYPE, doc, $"주문 실패. - {currency} 화폐 단위 혹은 수량 단위가 잘못되었거나, 수량이 없거나, 최소 주문 금액(5,000원) 미만입니다.");
                        return;
                    }
                        

                    if (_upbitCore.TryOrders(
                        out Orders_VO? orders,
                        market,
                        UpbitCore.ESide.ask,
                        tradeVolume,
                        candles[0].TradePrice, // 현재가로 매도
                        UpbitCore.EOrderType.market))
                    {
                        LOG.Info(LOG_TYPE, doc, $"주문 성공! 주문 ID: {orders?.UUID}");
                    }
                    else
                    {
                        LOG.Warning(LOG_TYPE, doc, "주문 실패.");

                        // 주문 실패 시 주문 취소
                        if (!_upbitCore.TryDeleteOrder(out Order_VO? order, orders.UUID, string.Empty))
                        {
                            LOG.Error(LOG_TYPE, doc, "주문 취소가 정상적으로 이루어지지 않았습니다.");
                        }
                        else
                        {
                            LOG.Info(LOG_TYPE, doc, "주문 취소 완료");
                        }

                        return;
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
        private UpbitProfile? Profile => _upbitApiTest.GetProfile();

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
            ExecuteSecondScalping_TEST("BTC");
            sw.Stop();

            LOG.Info(LOG_TYPE, doc, $"시뮬레이션 작동 시간: {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// 거래량 비율을 계산한다
        /// </summary>
        /// <param name="tradeVols">이전의 캔들 평균 거래량</param>
        /// <param name="multiplier">동적 임계값 계산을 위한 배수</param>
        /// <returns>거래량 비율</returns>
        /// <exception cref="ArgumentException"></exception>
        private double CalculateSecondVolumeRatio_TEST(List<double> tradeVols, double multiplier)
        {
            if (tradeVols == null || tradeVols.Count < 2)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            // 현재 캔들 거래량
            double currentVolume = tradeVols[tradeVols.Count - 1];

            // 이전 N개의 평균 거래량 계산
            double averageVolume = tradeVols.SkipLast(1).Average();

            // return currentVolume / averageVolume;

            return averageVolume * multiplier;
        }

        /// <summary>
        /// 거래량이 특정 임계값을 초과하면 매수 또는 매도신호 생성
        /// </summary>
        /// <param name="volume">거래량</param>
        /// <param name="threshold">임계값</param>
        /// <returns>매수/중립/매도 신호</returns>
        private ESignal GenerateSecondTradeSignal_TEST(List<double> prices, double volume, double threshold)
        {
            double targetProfit = 0.5;
            double stopLoss = 0.2;
            double currentPrice = prices[prices.Count - 1];
            //double rsi = GetRSI_TEST(prices, prices.Count - 1);
            double rsi = GetRSI_TEST2(prices, prices.Count - 1);
            if (volume > threshold && rsi < 30)
            {
                return ESignal.Buy; // 상승 신호 시 매수
            }
            else
            {
                double profitRate = (currentPrice - _buyPrice) / _buyPrice;
                if (double.IsInfinity(profitRate) || double.IsNaN(profitRate))
                    profitRate = 0;

                if (profitRate <= -stopLoss)        // 손절 조건
                    return ESignal.StopLoss;
                if (profitRate >= targetProfit ||   // 목표 수익률 도달
                    volume < 1 && rsi > 70)         // 매도 신호 발생
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
        /// <returns>rsi</returns>
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

        public double GetRSI_TEST2(List<double> prices, int period)
        {
            if (prices == null || prices.Count < period + 1)
                throw new ArgumentException("캔들 데이터가 부족합니다.");

            double gain = 0; // 상승 합계
            double loss = 0; // 하락 합계

            // 초기 상승/하락 계산 (첫 번째 기간)
            for (int i = 1; i <= period; i++)
            {
                double change = prices[i] - prices[i - 1];

                if (change > 0)
                {
                    gain += change;
                }
                else
                {
                    loss -= change; // 하락값을 양수로 전환
                }
            }

            // 초기 평균 상승/하락 계산
            double avgGain = gain / period;
            double avgLoss = loss / period;

            // RSI 계산 루프
            for (int i = period + 1; i < prices.Count; i++)
            {
                double change = prices[i] - prices[i - 1];

                if (change > 0)
                {
                    avgGain = ((avgGain * (period - 1)) + change) / period;
                    avgLoss = (avgLoss * (period - 1)) / period; // 하락값 없음
                }
                else
                {
                    avgGain = (avgGain * (period - 1)) / period; // 상승값 없음
                    avgLoss = ((avgLoss * (period - 1)) - change) / period;
                }
            }

            // 안정성을 위한 예외 처리 (avgLoss가 0인 경우)
            if (avgLoss == 0)
            {
                return 100; // 완전한 과매수 상태
            }

            // RSI 계산
            double rs = avgGain / avgLoss;
            double rsi = 100 - (100 / (1 + rs));

            return rsi;
        }


        /// <summary>
        /// 초 기준 스캘핑을 실행한다
        /// </summary>
        private void ExecuteSecondScalping_TEST(string market)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            List<double> prices = new List<double>();
            List<double> tradeVols = new List<double>();
            int lineCount = 0;
            using FileStream fs = new FileStream("test_small.csv", FileMode.Open, FileAccess.Read, FileShare.Read, 65536);
            using (StreamReader sr = new StreamReader(fs))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sp = line.Split(',');
                    if (!double.TryParse(sp[1], out _))
                        continue;

                    prices   .Add(double.Parse(sp[1]));
                    tradeVols.Add(double.Parse(sp[2]));
                    ++lineCount;
                }
            }

            const int criteria = 3;
            int stopLossCount = 0;
            for (int i = criteria; i < lineCount; i++)
            {
                List<double> newPrice = prices.GetRange(i - criteria, criteria);
                List<double> newTradeVols = tradeVols.GetRange(i - criteria, criteria);
                double currentPrice = prices[i];
                double currentVolume = tradeVols[i];

                double dynamicVolume = CalculateSecondVolumeRatio_TEST(newTradeVols, 3);
                ESignal signal = GenerateSecondTradeSignal_TEST(newPrice, currentVolume, dynamicVolume);

                if (signal == ESignal.Buy)
                {
                    LOG.Info(LOG_TYPE, doc, "매수 신호 발생! 주문을 실행합니다.");

                    double volume = Profile.Money / prices[i];
                    double total = volume * prices[i];

                    if (double.IsNaN(total) || double.IsInfinity(total) || total == 0)
                        continue;

                    if (_upbitApiTest.TryOrder(market, UpbitCore.ESide.bid, volume, currentPrice))
                    {
                        _buyPrice = currentPrice;
                        LOG.Info(LOG_TYPE, doc, $"매수 성공!");
                    }
                }
                else if (signal == ESignal.Sell || signal == ESignal.StopLoss)
                {
                    LOG.Info(LOG_TYPE, doc, "매도 신호 발생! 주문을 실행합니다.");

                    if (_upbitApiTest.TryOrder(market, UpbitCore.ESide.ask, -1, currentPrice))
                    {
                        LOG.Info(LOG_TYPE, doc, $"매도 성공!");
                    }

                    if (signal == ESignal.StopLoss)
                        stopLossCount++;
                }
                else
                {
                    LOG.Warning(LOG_TYPE, doc, "신호 없음.");
                }
            }

            // Profile.SellAll();
            double change = ((Profile.Money - Money) / Money) * 100;
            LOG.Info(LOG_TYPE, doc, $"최종 현금: {Profile.Money}");
            string str = change < Money ? "잃었습니다" : "벌었습니다";
            LOG.Info(LOG_TYPE, doc, $"{change}% {str}");
            LOG.Info(LOG_TYPE, doc, $"손절한 횟수 : {stopLossCount}");
        }
    }
}

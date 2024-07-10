using System.Text.Json.Serialization;

namespace AutoNewBitcoinChecker
{
    public class TickerCollection_VO
    {
        public class Ticker_VO
        {
            /// <summary>
            /// 종목 구분 코드
            /// </summary>
            [JsonPropertyName("market")]
            public string Market { get; set; }

            /// <summary>
            /// 최근 거래 일자(UTC) <br />
            /// 포맷: yyyyMMdd
            /// </summary>
            [JsonPropertyName("trade_date")]
            public string TradeDate { get; set; }

            /// <summary>
            /// 최근 거래 시각(UTC) <br />
            /// 포맷: HHmmss
            /// </summary>
            [JsonPropertyName("trade_time")]
            public string TradeTime { get; set; }

            /// <summary>
            /// 최근 거래 일자(KST) <br />
            /// 포맷: yyyyMMdd
            /// </summary>
            [JsonPropertyName("trade_date_kst")]
            public string TradeDateKST { get; set; }

            /// <summary>
            /// 최근 거래 시각(KTC) <br />
            /// 포맷: HHmmss
            /// </summary>
            [JsonPropertyName("trade_time_kst")]
            public string TradeTimeKST { get; set; }

            /// <summary>
            /// 최근 거래 일시(UTC) <br />
            /// 포맷: Unix Timestamp
            /// </summary>
            [JsonPropertyName("trade_timestamp")]
            public long TradeTimeStamp { get; set; }

            /// <summary>
            /// 시가
            /// </summary>
            [JsonPropertyName("opening_price")]
            public double OpeningPrice { get; set; }

            /// <summary>
            /// 고가
            /// </summary>
            [JsonPropertyName("high_price")]
            public double HighPrice { get; set; }

            /// <summary>
            /// 저가
            /// </summary>
            [JsonPropertyName("low_price")]
            public double LowPrice { get; set; }

            /// <summary>
            /// 종가 (현재가)
            /// </summary>
            [JsonPropertyName("trade_price")]
            public double TradePrice { get; set; }

            /// <summary>
            /// 전일 종가 (UTC 0시 기준)
            /// </summary>
            [JsonPropertyName("prev_closing_price")]
            public double PrevClosingPrice { get; set; }

            /// <summary>
            /// EVEN: 보합 <br />
            /// RISE: 상승 <br />
            /// FALL: 하락
            /// </summary>
            [JsonPropertyName("change")]
            public string Change { get; set; }

            /// <summary>
            /// 변화액의 절대값
            /// </summary>
            [JsonPropertyName("change_price")]
            public double ChangePrice { get; set; }

            /// <summary>
            /// 변화율의 절대값
            /// </summary>
            [JsonPropertyName("change_rate")]
            public double ChangeRate { get; set; }

            /// <summary>
            /// 부호가 있는 변화액
            /// </summary>
            [JsonPropertyName("signed_change_price")]
            public double SignedChangePrice { get; set; }

            /// <summary>
            /// 부호가 있는 변화율
            /// </summary>
            [JsonPropertyName("signed_change_rate")]
            public double SignedChangeRate { get; set; }

            /// <summary>
            /// 가장 최근 거래량
            /// </summary>
            [JsonPropertyName("trade_volume")]
            public double TradeVolume { get; set; }

            /// <summary>
            /// 누적 거래대금 (UTC 0시 기준)
            /// </summary>
            [JsonPropertyName("acc_trade_price")]
            public double AccTradePrice { get; set; }

            /// <summary>
            /// 24시간 누적 거래대금
            /// </summary>
            [JsonPropertyName("acc_trade_price_24h")]
            public double AccTradePrice24h { get; set; }

            /// <summary>
            /// 누적 거래량 (UTC 0시 기준)
            /// </summary>
            [JsonPropertyName("acc_trade_volume")]
            public double AccTradeVolume { get; set; }

            /// <summary>
            /// 24시간 누적 거래량
            /// </summary>
            [JsonPropertyName("acc_trade_volume_24h")]
            public double AccTradeVolume24h { get; set; }

            /// <summary>
            /// 52주 신고가
            /// </summary>
            [JsonPropertyName("highest_52_week_price")]
            public double Highest52WeekPrice { get; set; }

            /// <summary>
            /// 52주 신고가 달성일 <br />
            /// 포맷: yyyy-MM-dd
            /// </summary>
            [JsonPropertyName("highest_52_week_date")]
            public string Highest52WeekDate { get; set; }

            /// <summary>
            /// 52주 신저가
            /// </summary>
            [JsonPropertyName("lowest_52_week_price")]
            public double Lowest52WeekPrice { get; set; }

            /// <summary>
            /// 52주 신저가 달성일 <br />
            /// 포맷: yyyy-MM-dd
            /// </summary>
            [JsonPropertyName("lowest_52_week_date")]
            public string Lowest52WeekDate { get; set; }

            /// <summary>
            /// 타임스탬프
            /// </summary>
            [JsonPropertyName("timestamp")]
            public long TimeStamp { get; set; }
        }

        public List<Ticker_VO> Tickers { get; private set; }

        public TickerCollection_VO()
        {
            Tickers = new List<Ticker_VO>();
        }

        public TickerCollection_VO(List<Ticker_VO> tickers)
        {
            Tickers = tickers;
        }
    }
}

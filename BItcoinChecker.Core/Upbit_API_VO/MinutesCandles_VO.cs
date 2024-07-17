using System.Text.Json.Serialization;

namespace BitcoinChecker.Core
{

    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.16
     *  
     *  < 목적 >
     *  - 분(minute) 캔들 VO 클래스
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.07.16 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class MinutesCandles_VO
    {
        /// <summary>
        /// 마켓명
        /// </summary>
        [JsonPropertyName("market")]
        public string? Market { get; set; }

        /// <summary>
        /// 캔들 기준 시각(UTC 기준) <br/>
        /// 포맷: yyyy-MM-dd'T'HH:mm:ss
        /// </summary>
        [JsonPropertyName("candle_date_time_utc")]
        public string? CandleDateTimeUTC { get; set; }

        /// <summary>
        /// 캔들 기준 시각(KST 기준) <br/>
        /// 포맷: yyyy-MM-dd'T'HH:mm:ss
        /// </summary>
        [JsonPropertyName("candle_date_time_kst")]
        public string? CandleDateTimeKST { get; set; }

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
        /// 종가
        /// </summary>
        [JsonPropertyName("trade_price")]
        public double TradePrice { get; set; }

        /// <summary>
        /// 해당 캔들에서 마지막 틱이 저장된 시각
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long TimeStamp { get; set; }

        /// <summary>
        /// 누적 거래 금액
        /// </summary>
        [JsonPropertyName("candle_acc_trade_price")]
        public double CandleAccTradePrice { get; set; }

        /// <summary>
        /// 누적 거래량
        /// </summary>
        [JsonPropertyName("candle_acc_trade_volume")]
        public double CandleAccTradeVolume { get; set; }

        /// <summary>
        /// 분 단위(유닛)
        /// </summary>
        [JsonPropertyName("unit")]
        public int Unit { get; set; }
    }
}

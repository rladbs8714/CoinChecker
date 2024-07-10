using System.Text.Json.Serialization;

namespace AutoNewBitcoinChecker
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.10
     *  
     *  < 목적 >
     *  - 마켓 코드 조회 VO 클래스
     *  - 업비트에서 거래 가능한 마켓 목록
     *  
     *  < TODO >
     *  - 
     *  
     *  < History >
     *  2024.06.10 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class MarketAll_VO
    {
        public class Market_VO
        {
            public class MarketEvent_VO
            {
                /// <summary>
                /// 업비트 시장경보 > 주의종목 지정 여부
                /// </summary>
                public class Caution_VO
                {
                    /// <summary>
                    /// 가격 급등락 경보 발령 여부
                    /// </summary>
                    [JsonPropertyName("PRICE_FLUCTUATIONS")]
                    public bool PriceFluctuations { get; set; }
                    /// <summary>
                    /// 거래량 급등 경보 발령 여부
                    /// </summary>
                    [JsonPropertyName("TRADING_VOLUME_SOARING")]
                    public bool TradingVolumeSoaring { get; set; }
                    /// <summary>
                    /// 입금량 급등 경보 발령 여부
                    /// </summary>
                    [JsonPropertyName("DEPOSIT_AMOUNT_SOARING")]
                    public bool DepositAmountSoaring { get; set; }
                    /// <summary>
                    /// 가격 차이 경보 발령 여부
                    /// </summary>
                    [JsonPropertyName("GLOBAL_PRICE_DIFFERENCES")]
                    public bool GlobalPriceDifferences { get; set; }
                    /// <summary>
                    /// 소수 계정 집중 경보 발령 여부
                    /// </summary>
                    [JsonPropertyName("CONCENTRATION_OF_SMALL_ACCOUNTS")]
                    public bool ConcentrationOfSmallAccounts { get; set; }
                }

                /// <summary>
                /// 업비트 시장경보 > 유의종목 지정 여부
                /// </summary>
                [JsonPropertyName("warning")]
                public bool Warning { get; set; }

                [JsonPropertyName("caution")]
                public Caution_VO Caution { get; set; }
            }

            /// <summary>
            /// 업비트에서 제공중인 시장 정보
            /// </summary>
            [JsonPropertyName("market")]
            public string Market { get; set; }
            /// <summary>
            /// 거래 대상 디지털 자산 한글명
            /// </summary>
            [JsonPropertyName("korean_name")]
            public string KoreanName { get; set; }
            /// <summary>
            /// 거래 대상 디지털 자산 영문명
            /// </summary>
            [JsonPropertyName("english_name")]
            public string EnglishName { get; set; }
            /// <summary>
            /// 유의 종목 여부
            /// NONE(해당 사항 없음), CAUTION(투자유의)
            /// *deprecated
            /// </summary>
            [JsonPropertyName("market_warning")]
            public string MarketWarning { get; set; }

            [JsonPropertyName("market_event")]
            public MarketEvent_VO MarketEvent { get; set; }
        }


        // ====================================================================
        // PROPERTY
        // ====================================================================

        /// <summary>
        /// 업비트에 등록된 모든 코인 마켓 상태 리스트
        /// </summary>
        public List<Market_VO> Markets { get; private set; }


        // ====================================================================
        // CONSTRUCTOR
        // ====================================================================

        public MarketAll_VO(List<Market_VO> markets)
        {
            Markets = markets;
        }
    }
}

using System.Text.Json.Serialization;

namespace BitcoinChecker.Core
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.10
     *  
     *  < 목적 >
     *  - 주문 가능 정보 VO 클래스
     *  - 마켓별 주문 가능 정보를 확인한다.
     *  
     *  < TODO >
     *  - 
     *  
     *  < History >
     *  2024.06.10 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class OrdersChance_VO
    {
        public class Market_VO
        {
            public class Bid_VO
            {
                /// <summary>
                /// 화폐를 의미하는 영문 대문자 코드
                /// </summary>
                [JsonPropertyName("currency")]
                public string Currency { get; set; }
                
                /// <summary>
                /// 최소 매도/매수 금액
                /// </summary>
                [JsonPropertyName("min_total")]
                public string MinTotal { get; set; }
            }

            public class Ask_VO
            {
                /// <summary>
                /// 화폐를 의미하는 영문 대문자 코드
                /// </summary>
                [JsonPropertyName("currency")]
                public string Currency { get; set; }

                /// <summary>
                /// 최소 매도/매수 금액
                /// </summary>
                [JsonPropertyName("min_total")]
                public string MinTotal { get; set; }
            }

            /// <summary>
            /// 마켓의 유일 키
            /// </summary>
            [JsonPropertyName("id")]
            public string ID { get; set; }

            /// <summary>
            /// 마켓 이름
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; }

            /// <summary>
            /// 지원 주문 방식 (만료)
            /// </summary>
            [JsonPropertyName("order_types")]
            public string[] OrderTypes { get; set; }

            /// <summary>
            /// 지원 주문 종류
            /// </summary>
            [JsonPropertyName("order_sides")]
            public string[] OrderSides { get; set; }

            /// <summary>
            /// 매수 주문 지원 방식
            /// </summary>
            [JsonPropertyName("bid_types")]
            public string[] BidTypes { get; set; }

            /// <summary>
            /// 매도 주문 지원 방식
            /// </summary>
            [JsonPropertyName("ask_types")]
            public string[] AskTypes { get; set; }

            /// <summary>
            /// 매수 시 제약사항
            /// </summary>
            [JsonPropertyName("bid")]
            public Bid_VO Bid { get; set; }

            /// <summary>
            /// 매도 시 제약사항
            /// </summary>
            [JsonPropertyName("ask")]
            public Ask_VO Ask { get; set; }

            /// <summary>
            /// <b>[NumberString]</b><br/>
            /// 최대 매도/매수 금액
            /// </summary>
            [JsonPropertyName("max_total")]
            public string MaxTotal { get; set; }

            /// <summary>
            /// 마켓 운영 상태
            /// </summary>
            [JsonPropertyName("state")]
            public string State { get; set; }
        }

        public class BidAccount_VO
        {
            /// <summary>
            /// 화폐를 의미하는 영문 대문자 코드
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            /// 주문가능 금액/수량
            /// </summary>
            [JsonPropertyName("balance")]
            public string Balance { get; set; }

            /// <summary>
            /// 주문 중 묶여있는 금액 수량
            /// </summary>
            [JsonPropertyName("locked")]
            public string Locked { get; set; }

            /// <summary>
            /// 매수 평균가
            /// </summary>
            [JsonPropertyName("avg_buy_price")]
            public string AvgBuyPrice { get; set; }

            /// <summary>
            /// 매수 평균가 수정 여부
            /// </summary>
            [JsonPropertyName("avg_buy_price_modified")]
            public bool AvgBuyPriceModified { get; set; }

            /// <summary>
            /// 평단가 기준 화폐
            /// </summary>
            [JsonPropertyName("unit_currency")]
            public string UnitCurrency { get; set; }
        }

        public class AskAccount_VO
        {
            /// <summary>
            /// 화폐를 의미하는 영문 대문자 코드
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            /// 주문가능 금액/수량
            /// </summary>
            [JsonPropertyName("balance")]
            public string Balance { get; set; }

            /// <summary>
            /// 주문 중 묶여있는 금액 수량
            /// </summary>
            [JsonPropertyName("locked")]
            public string Locked { get; set; }

            /// <summary>
            /// 매수 평균가
            /// </summary>
            [JsonPropertyName("avg_buy_price")]
            public string AvgBuyPrice { get; set; }

            /// <summary>
            /// 매수 평균가 수정 여부
            /// </summary>
            [JsonPropertyName("avg_buy_price_modified")]
            public bool AvgBuyPriceModified { get; set; }

            /// <summary>
            /// 평단가 기준 화폐
            /// </summary>
            [JsonPropertyName("unit_currency")]
            public string UnitCurrency { get; set; }
        }

        /// <summary>
        /// 매수 수수료 비율
        /// </summary>
        [JsonPropertyName("bid_fee")]
        public string BidFee { get; set; }

        /// <summary>
        /// 매도 수수료 비율
        /// </summary>
        [JsonPropertyName("ask_fee")]
        public string AskFee { get; set; }

        [JsonPropertyName("maker_bid_fee")]
        public string MakerBidFee { get; set; }

        [JsonPropertyName("maker_ask_fee")]
        public string MakerAskFee { get; set; }

        /// <summary>
        /// 마켓에 대한 정보
        /// </summary>
        [JsonPropertyName("market")]
        public Market_VO Market { get; set; }

        /// <summary>
        /// 매수 시 사용하는 화폐의 계좌 상태
        /// </summary>
        [JsonPropertyName("bid_account")]
        public BidAccount_VO BidAccount { get; set; }

        /// <summary>
        /// 매도 시 사용하는 화폐의 계좌 상태
        /// </summary>
        [JsonPropertyName("ask_account")]
        public AskAccount_VO AskAccount { get; set; }
    }
}

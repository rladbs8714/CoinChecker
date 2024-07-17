using System.Text.Json.Serialization;

namespace BitcoinChecker.Core
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.10
     *  
     *  < 목적 >
     *  - 전체 계좌 조회 VO 클래스
     *  - 계정이 보유한 자산 리스트를 보여준다.
     *  
     *  < TODO >
     *  - NumberString 형식의 속성들을 float나 long같이 정규화 된 변수로 바로 사용할 수 있게 만들기
     *  
     *  < History >
     *  2024.06.10 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class Accounts_VO
    {
        public class Account
        {
            /// <summary>
            /// 화폐를 의미하는 영문 대문자 코드
            /// </summary>
            [JsonPropertyName("currency")]
            public string Currency { get; set; }

            /// <summary>
            /// <b>[NumberString]</b><br/>
            /// 주문가능 금액 / 수량
            /// </summary>
            [JsonPropertyName("balance")]
            public string Balance { get; set; }

            /// <summary>
            /// <b>[NumberString]</b><br/>
            /// 주문 중 묶여있는 금액 / 수량
            /// </summary>
            [JsonPropertyName("locked")]
            public string Locked { get; set; }

            /// <summary>
            /// <b>[NumberString]</b><br/>
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
        /// 보유한 자산 리스트
        /// </summary>
        public List<Account> Accounts { get; private set; }

        public Accounts_VO(List<Account> accounts)
        {
            Accounts = accounts;
        }
    }
}

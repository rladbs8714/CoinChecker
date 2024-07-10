using System.Text.Json.Serialization;

namespace AutoNewBitcoinChecker
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.10
     *  
     *  < 목적 >
     *  - 주문하기 VO 클래스
     *  - 주문 요청을 한다.
     *  
     *  < TODO >
     *  - 
     *  
     *  < History >
     *  2024.06.10 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class Orders_VO
    {
        /// <summary>
        /// 주문의 고유 아이디
        /// </summary>
        [JsonPropertyName("uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// 주문 종류
        /// </summary>
        [JsonPropertyName("side")]
        public string Side { get; set; }

        /// <summary>
        /// 주문 방식
        /// </summary>
        [JsonPropertyName("ord_type")]
        public string OrderType { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 주문 당시 화폐 가격
        /// </summary>
        [JsonPropertyName("price")]
        public string Price { get; set; }

        /// <summary>
        /// 주문 상태
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// 마켓의 유일 키
        /// </summary>
        [JsonPropertyName("market")]
        public string Market { get; set; }

        /// <summary>
        /// 주문 생성 시간
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 사용자가 입력한 주문 양
        /// </summary>
        [JsonPropertyName("volume")]
        public string Volume { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 체결 후 남은 주문 양
        /// </summary>
        [JsonPropertyName("remaining_volume")]
        public string RemainingVolume { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 수수료로 예약된 비용
        /// </summary>
        [JsonPropertyName("reserved_fee")]
        public string ReservedFee { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 남은 수수료
        /// </summary>
        [JsonPropertyName("remaining_fee")]
        public string RemainingFee { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 사용된 수수료
        /// </summary>
        [JsonPropertyName("paid_fee")]
        public string PaidFee { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 거래에 사용중인 비용
        /// </summary>
        [JsonPropertyName("locked")]
        public string Locked { get; set; }

        /// <summary>
        /// <b>[NumberString]</b><br />
        /// 체결된 양
        /// </summary>
        [JsonPropertyName("executed_volume")]
        public string ExecutedVolume { get; set; }

        /// <summary>
        /// 해당 주문에 걸린 체결 수
        /// </summary>
        [JsonPropertyName("trades_count")]
        public int TradesCount { get; set; }

        /// <summary>
        /// IOC, FOK 설정
        /// </summary>
        [JsonPropertyName("time_in_force")]
        public string TimeInForce { get; set; }
    }
}

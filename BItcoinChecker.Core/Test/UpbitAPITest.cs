using BitcoinChecker.Core;
using BItcoinChecker.Core;

namespace BitcoinChecker.AutoTrading
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.11.23
     *  
     *  < 목적 >
     *  - 업비트 API 테스트를 위한 클래스
     *  - 작정하고 만든게 아니므로 그때그때 필요한 메서드를 만들어 이용한다
     *  - 해당 메서드는 시뮬레이션 용이므로 실제 서비스에는 이용하면 안된다
     *  
     *  < TODO >
     *  - 제대로 된 구조화
     *  
     *  < History >
     *  2024.11.23 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class UpbitApiTest
    {
        // ====================================================================
        // FIELDS
        // ====================================================================

        private readonly UpbitProfile _profile;


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        public UpbitApiTest(double money)
        {
            _profile = new UpbitProfile(money);
        }


        // ====================================================================
        // METHODS
        // ====================================================================

        /// <summary>
        /// 사용자 프로필을 반환한다
        /// </summary>
        /// <returns>사용자 프로필</returns>
        public UpbitProfile GetProfile()
        {
            return _profile;
        }

        /// <summary>
        /// 주문을 시도한다
        /// </summary>
        /// <param name="market">마켓명</param>
        /// <param name="side">주문 종류</param>
        /// <param name="volume">주문량</param>
        /// <param name="price">가격</param>
        /// <returns>주문에 성공했다면 true, 그렇지 않다면 false</returns>
        public bool TryOrder(string market, UpbitCore.ESide side, double volume, double price)
        {
            if (side == UpbitCore.ESide.bid)
            {
                // 매수
                PurchaseData data = new PurchaseData()
                {
                    Market = market,
                    Price = price,
                    Volume = volume,
                };

                _profile.AddPurchaseData(data);
            }
            else // UpbitCore.ESide.ask
            {
                // 매도

                // 1. 전량 매도
                _profile.SellAll(price);

                // 2. 분할 매도
                // 구현하고 테스트 해봐야 함
            }

            return true;
        }
    }
}

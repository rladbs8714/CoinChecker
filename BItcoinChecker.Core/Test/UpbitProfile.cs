using BItcoinChecker.Core;

namespace BitcoinChecker.AutoTrading
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.11.23
     *  
     *  < 목적 >
     *  - 업비트 자동 트레이딩 테스트를 위한 유저 프로필
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.11.23 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class UpbitProfile
    {
        // ====================================================================
        // PROPERTIES
        // ====================================================================

        /// <summary>
        /// 현금 자산
        /// </summary>
        public double Money { get; private set; }

        /// <summary>
        /// 구매 목록
        /// </summary>
        private Dictionary<string, List<PurchaseData>> PurchaseList = new Dictionary<string, List<PurchaseData>>();


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        public UpbitProfile(double money)
            => Money = money;


        // ====================================================================
        // METHOD
        // ====================================================================

        /// <summary>
        /// 가지고 있는 현금에 <paramref name="money"/>만큼 차감한다
        /// </summary>
        /// <param name="money">차감할 값</param>
        /// <returns>빼기에 성공했다면 true, 그렇지 않다면 false</returns>
        public bool TryUseMoney(double money)
        {
            if (Money < money)
                return false;

            Money -= money;
            return true;
        }

        /// <summary>
        /// 가지고 있는 현금에 <paramref name="money"/>만큼 가감한다
        /// </summary>
        /// <param name="money">가감할 값</param>
        public void TryGetMoney(double money)
        {
            Money += money;
        }

        /// <summary>
        /// 구매 내역을 추가한다
        /// </summary>
        /// <param name="data">구매 내역</param>
        public void AddPurchaseData(PurchaseData data)
        {
            if (data == null || string.IsNullOrEmpty(data.Market))
                return;

            if (!PurchaseList.ContainsKey(data.Market))
                PurchaseList.Add(data.Market, new List<PurchaseData>());

            PurchaseList[data.Market].Add(data);
            Money -= (data.Price * data.Volume);
        }

        /// <summary>
        /// 마지막에 구매한 내역에서 <see cref="per"/>만큼 매도한다
        /// </summary>
        /// <param name="per">매도 비율 (0.0 ~ 1.0)</param>
        /// <returns>매도에 성공했다면 true, 그렇지 않다면 false</returns>
        public bool TrySellLast(string market, double per)
        {
            if (!PurchaseList.ContainsKey(market))
                return false;
            if (per < 0 || per > 1)
                return false;

            Money += PurchaseList[market].Last().SplitSell(per);

            return true;
        }

        /// <summary>
        /// 구매한 모든 종목을 매도한다
        /// </summary>
        public void SellAll()
        {
            if (PurchaseList.Count == 0)
                return;

            foreach (List<PurchaseData> list in PurchaseList.Values)
            {
                foreach (PurchaseData data in list)
                {
                    Money = data.SplitSell(1);
                }
            }

            PurchaseList.Clear();
        }
    }
}

namespace BItcoinChecker.Core
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.11.23
     *  
     *  < 목적 >
     *  - 구매 데이터 클래스
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.11.23 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class PurchaseData
    {
        public string? Market { get; set; }

        public double Price { get; set; }

        public double Volume { get; set; }


        /// <summary>
        /// <see cref="per"/>% 만큼 분할하여 매도한다 <br />
        /// = Volume - (Volume * per)
        /// </summary>
        /// <param name="per">매도 비율 (0.0 ~ 1.0)</param>
        /// <returns>매도한 만큼의 가격</returns>
        public double SplitSell(double per)
        {
            double r = (Price * Volume) * per;
            Volume -= (Volume * per);
            return r;
        }
    }
}

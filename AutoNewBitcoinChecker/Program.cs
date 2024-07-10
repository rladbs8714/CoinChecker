using System.Linq;
using static AutoNewBitcoinChecker.Accounts_VO;
using Market = AutoNewBitcoinChecker.MarketAll_VO.Market_VO;

namespace AutoNewBitcoinChecker
{
    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.06.12
     *  
     *  < 목적 >
     *  - 
     *  
     *  < TODO >
     *  - 새로운 코인을 탐지하고 구매하는 로직을 다른 클래스를 만들어 본 클래스의 구문을 최대한 간결하게 유지한다.
     *  
     *  < History >
     *  2024.06.12 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public static class Program
    {
        public static MarketAll_VO? StartingMarketAll;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            //new UpbitCore(isDebug: true).TryGetAccounts(out var accounts);

            // new UpbitCore(isDebug: true).TryGetMarketAll(out var marketAll);

            // new UpbitCore(isDebug: true).TryGetOrdersChance(out var orderChance, "KRW-BTC");

            // new UpbitCore(isDebug: true).TryGetTicker(out var ticker, "KRW-SHIB");

            // var account = accounts?.Accounts.Find(x => x.Currency == "KRW");
            // double krw = double.Parse(account?.Balance);
            // new UpbitCore(isDebug: true).TryOrders(out var order, "KRW-SHIB", UpbitCore.ESide.bid, (krw / ticker.Tickers[0].TradePrice) * 0.9, ticker.Tickers[0].TradePrice, UpbitCore.EOrderType.limit);

            // 디스코드 봇 클라이언트 시작
            // DiscordHelper.Instance.Start();
            // 프로그램 시작
            bool withUI = false;
            if (!withUI)
            {
                new Upbit().Start(withUI);

                Thread.Sleep(-1);
            }
            else
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
        }
    }
}
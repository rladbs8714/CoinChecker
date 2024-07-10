using System.Linq;
using static AutoNewBitcoinChecker.Accounts_VO;
using Market = AutoNewBitcoinChecker.MarketAll_VO.Market_VO;

namespace AutoNewBitcoinChecker
{
    /*
     *  ===========================================================================
     *  �ۼ���     : @yoon
     *  ���� �ۼ���: 2024.06.12
     *  
     *  < ���� >
     *  - 
     *  
     *  < TODO >
     *  - ���ο� ������ Ž���ϰ� �����ϴ� ������ �ٸ� Ŭ������ ����� �� Ŭ������ ������ �ִ��� �����ϰ� �����Ѵ�.
     *  
     *  < History >
     *  2024.06.12 @yoon
     *  - ���� �ۼ�
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

            // ���ڵ� �� Ŭ���̾�Ʈ ����
            // DiscordHelper.Instance.Start();
            // ���α׷� ����
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
using BitcoinChecker.Core;
using System.Linq;

namespace BitcoinChecker
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
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // ���ڵ� �� Ŭ���̾�Ʈ ����
            // DiscordHelper.Instance.Start();
            // ���α׷� ����
            bool withUI = false;
            if (!withUI)
            {
                var upbit = new Upbit(discordPipeStart: false);
                // upbit.StartAutoProcess(withUI);

                if (new UpbitCore().TryGetMarketAll(out var marketCollection))
                {
                    DateTime time = DateTime.Now;
                    var marketsAll = marketCollection.Markets.Select(x => x.Market);
                    var krwMarkets = marketsAll.ToList().FindAll(x => x.Contains("KRW"));
                    
                    for (int i = 0; i < krwMarkets.Count; i++)
                    {
                        string marketCode = krwMarkets[i];
                        
                        upbit.TryGetCandlesCsv(marketCode, 1,
                                       time.AddMonths(-1),
                                       time);

                        Console.WriteLine($"���� ���൵ {i + 1}/{krwMarkets.Count}");
                    }
                }

                // Thread.Sleep(-1);
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
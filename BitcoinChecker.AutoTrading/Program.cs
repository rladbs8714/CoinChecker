using Generalibrary;

namespace BitcoinChecker.AutoTrading
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            double money = 10_000;

            AutoTrade at = new AutoTrade("", "", 0, isDebug: true, isTest: true);
            at.StartSimulationTest(money);

            Thread.Sleep(-1);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new frmMain());
        }
    }
}
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
                var upbit = new Upbit();
                upbit.StartAutoProcess(withUI);

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
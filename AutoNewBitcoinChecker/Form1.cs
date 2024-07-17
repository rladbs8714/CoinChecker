namespace BitcoinChecker
{
    public partial class Form1 : Form
    {
        private const    string     URL     = "https://api.upbit.com/v1";

        private const    string     MARKET  = "market";
        private const    string     ALL     = "all";
        private readonly CheckBox?   IsDetail;

        public Form1()
        {
            InitializeComponent();
        }
    }
}

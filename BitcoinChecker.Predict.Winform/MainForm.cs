using BitcoinChecker.Core;
using Timer = System.Windows.Forms.Timer;
using Generalibrary;
using System.Reflection;
using ScottPlot.WinForms;

namespace BitcoinChecker.Predict.Winform
{

    /*
     *  ===========================================================================
     *  �ۼ���     : @yoon
     *  ���� �ۼ���: 2024.07.21
     *  
     *  < ���� >
     *  - ������ ������ �׷����� ǥ���Ѵ�.
     *  
     *  < TODO >
     *  - @yoon �� ���� ���� ���� ������ �񵿱�� ó���Ѵ�. (history �����Ͱ� ���� ���� ���� �ε��� �ſ� ������)
     *  - @yoon ���� ���� �׷����� ���� ���� �׷����� �̾� ������. (���� �׷��� ��Ȳ�� �ſ� �˾ƺ��� �����.) ���� �������� ������ �ٸ��� �� �� �ִٸ� ����Ʈ.
     *  - @yoon �� ������ ���� ���� ���۽� ��� ���� 200�� �����͸� ������ ���� ������ �Ѵ�.
     *  
     *  < History >
     *  2024.07.21 @yoon
     *  - ���� �ۼ�
     *  ===========================================================================
     */

    public partial class frmMain : Form
    {

        // ====================================================================
        // CONSTANTS
        // ====================================================================

        private readonly ILogManager LOG = LogManager.Instance;

        private const string LOG_TYPE = "frmMain";


        // ====================================================================
        // FIELDS
        // ====================================================================

        private Dictionary<string, PredictPackage> _predictPackages = new Dictionary<string, PredictPackage>();

        private TabControl _mainTab;


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        public frmMain()
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            _mainTab = new TabControl()
            {
                Parent = this,
                Dock= DockStyle.Fill
            };

            Timer test = new Timer() { Interval = 100, Enabled = true };
            Task.Run(() =>
            {
                // �⺻ �� 1�� KRW-BTC ����
                string krwBtc = "KRW-BTC";
                var krwBtcTab = AddTab(krwBtc);
                TimeSeries ts = new TimeSeries(krwBtc, $"history\\{krwBtc}.csv", false, predictionRange: 10, appendPriceToCsv: false);
                Invoke((Action)(() =>
                {
                    _predictPackages.Add(krwBtc, new PredictPackage(krwBtc, krwBtcTab, ts));
                }));


                //UpdateData(this, new EventArgs());

                //Timer mainTimer = new Timer() { Interval = 1000 * 60, Enabled = true };
                //mainTimer.Tick += UpdateData;


                if (!new UpbitCore(isDebug: true).TryGetMinuteCandles(out var mCandles, 1, krwBtc, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 200))
                {
                    LOG.Warning(LOG_TYPE, doc, $"'{krwBtc}'�� �����͸� �������� ���߽��ϴ�.");
                    return;
                }

                if (mCandles == null || mCandles.Length < 1)
                {
                    LOG.Warning(LOG_TYPE, doc, $"'{krwBtc}'�� �����Ͱ� ����ְų� null �Դϴ�.");
                    return;
                }

                Queue<double> ps = new Queue<double>(mCandles.Select(e => e.TradePrice));
                test.Tick += (object? s, EventArgs e) =>
                {
                    if (!ps.TryDequeue(out double p)) return;

                    Invoke((Action)(() =>
                    {
                        _predictPackages[krwBtc].PredictPrice(Convert.ToSingle(p));
                    }));
                };
            });

            // Test
            //string later = "D:\\Project\\CoinChecker\\Test\\bin\\Debug\\net8.0\\history\\KRW-BTC_later.csv";
            //string later_p = "D:\\Project\\CoinChecker\\Test\\bin\\Debug\\net8.0\\history\\KRW-BTC_result.csv";
            //SetPlot(new string[] { later, later_p });

            InitializeComponent();
        }


        // ====================================================================
        // METHODS
        // ====================================================================

        /// <summary>
        /// ������ �����͸� ������ �� �ǿ� �����Ų��.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateData(object? sender, EventArgs e)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            foreach (string key in _predictPackages.Keys)
            {
                // 1. get data
                if (!new UpbitCore(isDebug: true).TryGetMinuteCandles(out var mCandles, 1, key, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 1))
                {
                    LOG.Warning(LOG_TYPE, doc, $"'{key}'�� �����͸� �������� ���߽��ϴ�.");
                    continue;
                }

                if (mCandles == null || mCandles.Length < 1)
                {
                    LOG.Warning(LOG_TYPE, doc, $"'{key}'�� �����Ͱ� ����ְų� null �Դϴ�.");
                    continue;
                }

                double price = mCandles[0].TradePrice;

                // 2. set data
                PredictPackage package = _predictPackages[key];
                package.PredictPrice(Convert.ToSingle(price));
            }
        }

        private TabPage AddTab(string tabName)
        {
            _mainTab.SuspendLayout();

            TabPage tab = new TabPage()
            {
                Name = tabName.Replace('-', '_'),
                Text = tabName
            };

            _mainTab.TabPages.Add(tab);

            _mainTab.Update();
            _mainTab.ResumeLayout();

            return tab;
        }

        private bool TryRemoveTab()
        {
            return false;
        }


        // ====================================================================
        // METHODS - TEST
        // ====================================================================

        /// <summary>
        /// plot�� ���� �����͸� Ȯ���ϰ� ���� ��� ����Ѵ�.
        /// </summary>
        /// <param name="fileNames">Ȯ���� ������ ���� �̸�</param>
        private void SetPlot(string[] fileNames)
        {
            string testTab = "Test Tab";
            TabPage tp = AddTab(testTab);

            FormsPlot fp = new FormsPlot()
            {
                Parent = tp,
                Dock = DockStyle.Fill,
            };
            fp.SuspendLayout();

            foreach (string fileName in fileNames)
            {
                string newFileName = fileName;
                if (!Uri.TryCreate(fileName, UriKind.Absolute, out _))
                    newFileName = Path.Combine(Environment.CurrentDirectory, fileName);

                if (!Path.Exists(newFileName))
                    continue;

                string[] lines = File.ReadAllLines(newFileName);
                float[] datas = Array.ConvertAll(lines, float.Parse);

                fp.Plot.Add.Signal(datas);
            }

            fp.Plot.Axes.AutoScale();
            fp.Refresh();
            fp.ResumeLayout();
        }
    }
}

using BitcoinChecker.Core;
using Timer = System.Windows.Forms.Timer;
using Generalibrary;
using System.Reflection;
using ScottPlot.WinForms;

namespace BitcoinChecker.Predict.Winform
{

    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.21
     *  
     *  < 목적 >
     *  - 예측된 코인을 그래프로 표시한다.
     *  
     *  < TODO >
     *  - @yoon 각 코인 예측 시작 구문을 비동기로 처리한다. (history 데이터가 많아 지면 최초 로딩이 매우 느려짐)
     *  - @yoon 실제 가격 그래프와 예측 가격 그래프를 이어 붙힌다. (현재 그래프 상황은 매우 알아보기 어려움.) 만약 구간마다 색상을 다르게 할 수 있다면 베스트.
     *  - @yoon 각 코인의 최초 예측 시작시 당시 이전 200분 데이터를 가져와 예열 예측을 한다.
     *  
     *  < History >
     *  2024.07.21 @yoon
     *  - 최초 작성
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
                // 기본 탭 1개 KRW-BTC 생성
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
                    LOG.Warning(LOG_TYPE, doc, $"'{krwBtc}'의 데이터를 가져오지 못했습니다.");
                    return;
                }

                if (mCandles == null || mCandles.Length < 1)
                {
                    LOG.Warning(LOG_TYPE, doc, $"'{krwBtc}'의 데이터가 비어있거나 null 입니다.");
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
        /// 코인의 데이터를 가져와 각 탭에 적용시킨다.
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
                    LOG.Warning(LOG_TYPE, doc, $"'{key}'의 데이터를 가져오지 못했습니다.");
                    continue;
                }

                if (mCandles == null || mCandles.Length < 1)
                {
                    LOG.Warning(LOG_TYPE, doc, $"'{key}'의 데이터가 비어있거나 null 입니다.");
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
        /// plot을 통해 데이터를 확인하고 싶을 경우 사용한다.
        /// </summary>
        /// <param name="fileNames">확인할 데이터 파일 이름</param>
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

using BitcoinChecker.Core;
using Generalibrary;
using Microsoft.VisualBasic.Logging;
using ScottPlot;
using ScottPlot.WinForms;
using System.Reflection;
using Color = System.Drawing.Color;

namespace BitcoinChecker.Predict.Winform
{

    /*
     *  ===========================================================================
     *  작성자     : @yoon
     *  최초 작성일: 2024.07.21
     *  
     *  < 목적 >
     *  - 예측과 그래프 표시에 필요한 모델을 관리한다.
     *  
     *  < TODO >
     *  
     *  < History >
     *  2024.07.22 @yoon
     *  - 최초 작성
     *  ===========================================================================
     */

    public class PredictPackage
    {

        // ====================================================================
        // CONSTANTS
        // ====================================================================

        private readonly ILogManager LOG = LogManager.Instance;

        private const string LOG_TYPE = "frmMain";

        // ====================================================================
        // PROPERTIES
        // ====================================================================

        public FormsPlot FormsPlot { get; private set; }

        public TimeSeries TimeSeries { get; private set; }


        // ====================================================================
        // FIELDS
        // ====================================================================

        private Queue<float> _prices = new Queue<float>();


        // ====================================================================
        // CONSTRUCTORS
        // ====================================================================

        public PredictPackage(FormsPlot fp, TimeSeries ts)
        {
            FormsPlot = fp;
            TimeSeries = ts;
        }

        public PredictPackage(string marketCode, Control parent, TimeSeries ts)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            FormsPlot = new FormsPlot()
            {
                Parent = parent,
                Dock = DockStyle.Fill,
            };
            FormsPlot.Plot.Axes.Bottom.FrameLineStyle.Color = ScottPlot.Color.FromColor(Color.Yellow);
            FormsPlot.Plot.Axes.Bottom.FrameLineStyle.Rounded = true;

            TimeSeries = ts;

            //if (!new UpbitCore(isDebug: true).TryGetMinuteCandles(out var mCandles, 1, marketCode, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 200))
            //{
            //    LOG.Warning(LOG_TYPE, doc, $"'{marketCode}'의 데이터를 가져오지 못했습니다.");
            //    return;
            //}

            //if (mCandles == null || mCandles.Length < 1)
            //{
            //    LOG.Warning(LOG_TYPE, doc, $"'{marketCode}'의 데이터가 비어있거나 null 입니다.");
            //    return;
            //}

            //foreach (double tradePrice in mCandles.Select(e => e.TradePrice))
            //{
            //    float price = Convert.ToSingle(tradePrice);
            //    PredictPrice(price);
            //}
        }


        // ====================================================================
        // METHODS
        // ====================================================================

        /// <summary>
        /// 가격을 예상한다. <br />
        /// 내부에서 그래프 표시까지 진행한다.
        /// </summary>
        /// <param name="price">현재 가격</param>
        public void PredictPrice(float price)
        {
            FormsPlot.SuspendLayout();
            FormsPlot.Plot.Clear();

            Plot plot = FormsPlot.Plot;

            float[] predicts = TimeSeries.GetPredictedPrice(price);

            _prices.Enqueue(price);
            if (_prices.Count > 5)
                _prices.Dequeue();

            List<float> datas = new List<float>(_prices.ToArray());
            datas.AddRange(predicts);

            var sig1 = plot.Add.Signal(datas);
            sig1.Color = ScottPlot.Color.FromColor(Color.DarkGreen);

            // var sig2 = plot.Add.Signal(predicts);
            // sig2.Color = ScottPlot.Color.FromColor(Color.DarkRed);

            FormsPlot.Plot.Axes.AutoScale();
            FormsPlot.Refresh();
            FormsPlot.ResumeLayout();
        }
    }
}

using Accord.Math.Geometry;
using BitcoinChecker.Predict;
using System.Text;

using Extreme.Mathematics;
using Extreme.Statistics;
using Extreme.Statistics.TimeSeriesAnalysis;
using System.Diagnostics;

namespace Test
{

    public class Test
    {
        static void Main(string[] args)
        {
            // new ML_Blog();

            //TimeSeries ts = new TimeSeries("KRW-BTC", "history\\KRW-BTC_sample.csv", false, predictionRange:1440, appendPriceToCsv: false);

            //string[] lines = File.ReadAllLines("history\\KRW-BTC_later.csv");
            //float[] datas = Array.ConvertAll(lines, float.Parse);

            //StringBuilder sb = new StringBuilder();
            //int count = 0;
            //foreach (float data in datas)
            //{
            //    float p = ts.GetPredictedPrice(data)[1439];
            //    sb.AppendLine(p.ToString());
            //    Console.WriteLine(++count);
            //}

            //File.AppendAllText("history\\KRW-BTC_result.csv", sb.ToString());

            //Extreme.License.Verify("24241-22245-03934-21588");
            //ARIMA();
            //FindArimaParameterValue();

            using (StreamReader sr = new StreamReader("test.csv"))
            {
                string? line = sr.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    string[] sp = line.Split(',');
                    prices.Add(double.Parse(sp[1]));
                    tradeVols.Add(double.Parse(sp[2]));
                    ++lineCount;
                }
            }
        }

        public static void ARIMA()
        {
            string dataPath = "history\\KRW-BTC_sample.csv";
            double[] datas = Array.ConvertAll(File.ReadAllLines(dataPath), double.Parse);

            // The time series data is stored in a numerical variable:
            var coinData = Vector.Create(datas);

            ArimaModel model = new ArimaModel(coinData, 4, 2, 4)
            {
                EstimateMean = true
            };

            model.Fit();

            var nextValues = model.Forecast(1440);
            StreamWriter sw = new FileInfo("result\\result.csv").AppendText();
            sw.AutoFlush = true;

            foreach (var value in nextValues)
            {
                sw.WriteLine(value.ToString());
            }
        }

        public static void FindArimaParameterValue()
        {
            string[] rawDatas = File.ReadAllLines("history\\KRW-BTC_org.csv");

            double minAIC = double.MaxValue;
            var coinDatas = Vector.Create(Array.ConvertAll(rawDatas, double.Parse));

            bool changeMin = false;
            int minP = 0, minD = 0, minQ = 0;
            for (int p = 0; p < 10; p++)
            {
                for (int d = 0; d < 10; d++)
                {
                    for (int q = 0; q < 10; q++)
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        ArimaModel model = new ArimaModel(coinDatas, p, d, q);
                        model.Fit();

                        double aic = model.GetAkaikeInformationCriterion();
                        if (aic < minAIC)
                        {
                            minAIC = aic;
                            changeMin = true;
                            minP = p;
                            minD = d;
                            minQ = q;
                        }

                        sw.Stop();
                        Console.WriteLine($"Elapsed: {sw.Elapsed.TotalSeconds} s");
                        Console.WriteLine($"AIC    : {aic}");
                        Console.WriteLine($"[CURRENT] p: {p} | d: {d} | q: {q}");
                        if (changeMin)
                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"    [MIN] p: {minP} | d: {minD} | q: {minQ}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("-------------------------");
                    }
                }
            }
        }

        public static void GetData()
        {
            string path = "C:\\Users\\rladb\\OneDrive\\바탕 화면\\data.txt";
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                if (!line.Contains("AIC"))
                    continue;

                string[] s = line.Split(':');
                Console.WriteLine(double.Parse(s[1]));
            }
        }
    }
}

using BitcoinChecker.Core;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Generalibrary;

namespace BitcoinChecker.Test
{
    internal class Program
    {
        public static ILogManager LOG = LogManager.Instance;

        public static string LOG_TYPE = "Test";

        public static void Main(string[] args)
        {
            string doc = MethodBase.GetCurrentMethod().Name;

            // new UpbitCore(isDebug: true).TryGetAccounts(out var accounts);

            // new UpbitCore(isDebug: true).TryGetMarketAll(out var marketAll);

            // new UpbitCore(isDebug: true).TryGetOrdersChance(out var orderChance, "KRW-BTC");

            // new UpbitCore(isDebug: true).TryGetTicker(out var ticker, "KRW-SHIB");

            // var account = accounts?.Accounts.Find(x => x.Currency == "KRW");
            // double krw = double.Parse(account?.Balance);
            // new UpbitCore(isDebug: true).TryOrders(out var order, "KRW-SHIB", UpbitCore.ESide.bid, (krw / ticker.Tickers[0].TradePrice) * 0.9, ticker.Tickers[0].TradePrice, UpbitCore.EOrderType.limit);

            bool loop = true;
            int count = 200;
            int loopCount = 1;
            var startTime = new DateTime(2017, 09, 26, 00, 01, 01);
            var endTime = DateTime.Now;
            string marketCode = "KRW-BTC";
            List<MinutesCandles_VO> candles = new List<MinutesCandles_VO>();
            StreamWriter sw = new FileInfo($"result\\{marketCode}.csv").AppendText();
            sw.AutoFlush = true;
            while (loop)
            {
                int tmpMinutes = (int)(endTime - startTime).TotalMinutes;
                if (tmpMinutes < count)
                {
                    count = tmpMinutes;
                    loop = false;
                }

                startTime = startTime.AddMinutes(count);
                string time = startTime.ToString("yyyy-MM-dd HH:mm:ss");

                new UpbitCore(isDebug: true).TryGetMinuteCandles(out var mCandles, 1, marketCode, time, count);
                if (mCandles == null)
                    break;

                // candles.AddRange(mCandles);
                // LogManager.Instance.Info(LOG_TYPE, doc, $"[완료:{loopCount++,3}] {time}");
                Console.WriteLine(time);

                foreach (var price in mCandles.Reverse().Select(e => e.TradePrice))
                    sw.WriteLine(price);

                Thread.Sleep(110);
            }
            Console.WriteLine("완료");

            // ConvertJsonToCsv<MinutesCandles_VO>(candles, true);
            // Console.WriteLine(csv);
        }

        // =====================================================================
        // METHODS
        // =====================================================================

        /// <summary>
        /// Json to csv <br />
        /// 이 메서드는 동일한 요소를 가지고 있는 항목이 배열로 들고 있는 Json일 때 정상적으로 작동한다. <br />
        /// <code>
        /// e.g.
        /// [
        ///     {
        ///         "market": "KRW-ENS",
        ///         "price": 38040
        ///     },
        ///     {
        ///         "market": "KRW-ENS",
        ///         "price": 38050
        ///     },
        ///     {
        ///         "market": "KRW-ENS",
        ///         "price": 38060
        ///     }
        /// ]
        /// </code>
        /// </summary>
        /// <param name="json">origianl json</param>
        /// <returns>csv or <seealso cref="string.Empty"/></returns>
        public static string ConvertJsonToCsv<T>(List<T> json, bool saveFile = false)
        {
            if (json == null)
                return string.Empty;

            StringBuilder csv = new StringBuilder();
            List<string> properties = new List<string>();

            var members = typeof(T).GetProperties();
            foreach (MemberInfo member in members)
            {
                JsonPropertyNameAttribute? attr = member.GetCustomAttribute<JsonPropertyNameAttribute>();

                if (attr == null)
                    continue;

                csv.Append($"{attr.Name},");
                properties.Add(member.Name);
            }
            csv.Length--;
            csv.Append(Environment.NewLine);

            foreach (var elem in json)
            {
                var type = elem.GetType();
                foreach (string header in properties)
                {
                    string? value = type.GetProperty(header).GetValue(elem).ToString();

                    if (string.IsNullOrEmpty(value))
                        continue;

                    csv.Append($"{value},");
                }

                csv.Length--;
                csv.Append(Environment.NewLine);
            }

            if (saveFile)
            {
                if (!Directory.Exists("file"))
                    Directory.CreateDirectory("file");

                using var sw = File.CreateText($"file\\{DateTime.Now.ToString("yyyyMMddHHmmssfffff")}.csv");
                sw.Write(csv.ToString());
            }

            return csv.ToString();
        }
    }
}

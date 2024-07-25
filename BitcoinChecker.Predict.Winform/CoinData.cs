namespace BitcoinChecker.Predict.Winform
{
    public class CoinData
    {
        public DateTime DateTime { get; set; }

        public double Price { get; set; }

        public CoinData(DateTime dateTime, double price)
        {
            DateTime = dateTime;
            Price = price;
        }
    }
}

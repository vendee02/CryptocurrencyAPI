using CryptoAPI.DTOModels;
using CryptoAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CryptoAPI.UnitTest
{
    public class TestData
    {
        public static IList<string> CurrencyList = new List<string> { "EUR", "USD", "BRL", "GBP" };
        public const string BTCsymbol = "BTC";

        public static Cryptocurrency CreateCryptoQuote(string symbol, string currency, double price)
        {
            return new Cryptocurrency
            {
                Data = { { symbol, new CryptocurrencyDetails { Quotes = { { currency, new CryptocurrencyQuote { Price = price } } } } } }
            };
        }

        public static IList<Cryptocurrency> CryptoQuoteList(string symbol, IList<string> currencies)
        {
            var rnd = new Random();
            IList<Cryptocurrency> cryptoModels = new List<Cryptocurrency>();
            foreach (var currency in currencies)
            {
                cryptoModels.Add(CreateCryptoQuote(symbol, currency, rnd.NextDouble()));
            }
            return cryptoModels;
        }

        public static async IAsyncEnumerable<CryptocurrencyPrice> GetStreamCryptoPrice()
        {
            var rnd = new Random();

            yield return new CryptocurrencyPrice(CurrencyList[0]) { Price = rnd.NextDouble() };
            yield return new CryptocurrencyPrice(CurrencyList[1]) { Price = rnd.NextDouble() };
            yield return new CryptocurrencyPrice(CurrencyList[2]) { Price = rnd.NextDouble() };
            yield return new CryptocurrencyPrice(CurrencyList[3]) { Price = rnd.NextDouble() };
            await Task.CompletedTask;
        }

        public Stream GetStream<T>(T objModel)
        {
            string jsonText = JsonConvert.SerializeObject(objModel, Formatting.Indented);
            var ms = new MemoryStream();
            var encoding = new UTF8Encoding(false, true);
            using (var writer = new StreamWriter(ms, encoding, 8192, true))
            {
                writer.Write(jsonText);
            }
            ms.Position = 0;
            return ms;
        }
    }
}

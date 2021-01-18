using System.Collections.Generic;

namespace CryptoAPI.Options
{
    public class CurrenciesOptions
    {
        public const string OutputContent = "OutputContent";
        public const string CryptoApiCurrencies = "CryptoApi";
        public CurrenciesOptions()
        {
            Currencies = new List<string>();
        }
        public IList<string> Currencies { get; set; }
    }
}
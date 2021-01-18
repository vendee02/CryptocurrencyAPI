#nullable enable
using System.Collections.Generic;

namespace CryptoAPI.DTOModels
{
    public class CryptoQuoteDto
    {
        public CryptoQuoteDto(string cryptocurrencyCode)
        {
            CryptocurrencyCode = cryptocurrencyCode;
            ExchangeRate = new Dictionary<string, double>();
        }

        public string CryptocurrencyCode { get; private set; }

        public Dictionary<string,double> ExchangeRate { get; set; }
    }
}
#nullable restore

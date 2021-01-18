#nullable enable

namespace CryptoAPI.DTOModels
{
    public class CryptocurrencyPrice
    {
        public CryptocurrencyPrice(string currency)
        {
            Currency = currency;
        }

        public string Currency { get; private set; }

        public double Price { get; set; }
    }
}
#nullable restore
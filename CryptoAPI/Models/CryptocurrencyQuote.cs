using Newtonsoft.Json;
using System;

namespace CryptoAPI.Models
{
    public class CryptocurrencyQuote
    {
        [JsonProperty("price")]
        public double Price { get; set; }
        public float volume_24h { get; set; }
        public float percent_change_1h { get; set; }
        public float percent_change_24h { get; set; }
        public float percent_change_7d { get; set; }
        public float market_cap { get; set; }
        public DateTime last_updated { get; set; }
    }
}
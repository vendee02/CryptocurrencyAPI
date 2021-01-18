using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CryptoAPI.Models
{
    public class CryptocurrencyDetails
    {
        public CryptocurrencyDetails()
        {
            Quotes = new Dictionary<string, CryptocurrencyQuote>();
        }
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string slug { get; set; }
        public int num_market_pairs { get; set; }
        public DateTime date_added { get; set; }
        public string[] tags { get; set; }
        public object max_supply { get; set; }
        public float circulating_supply { get; set; }
        public float total_supply { get; set; }
        public int is_active { get; set; }
        public object platform { get; set; }
        public int cmc_rank { get; set; }
        public int is_fiat { get; set; }
        public DateTime last_updated { get; set; }

        [JsonProperty("quote")]
        public Dictionary<string, CryptocurrencyQuote> Quotes { get; set; }
    }
}

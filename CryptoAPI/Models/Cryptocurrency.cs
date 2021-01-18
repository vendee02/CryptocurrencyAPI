using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CryptoAPI.Models
{
    public class Cryptocurrency
    {
        public Cryptocurrency()
        {
            Data = new Dictionary<string, CryptocurrencyDetails>();
        }
        public object status { get; set; }

        [JsonProperty("data")]
        public Dictionary<string, CryptocurrencyDetails> Data { get; set; }
    }
}

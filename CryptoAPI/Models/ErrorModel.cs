using Newtonsoft.Json;

namespace CryptoAPI.DTOModels
{
    public class ErrorModel
    {
        [JsonProperty("status")]
        public ErrorStatus ErrorStatus { get; set; }
    }
}
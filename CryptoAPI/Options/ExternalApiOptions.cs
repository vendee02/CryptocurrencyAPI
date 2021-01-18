namespace CryptoAPI.Options
{
    public class ExternalApiOptions
    {
        public const string ExternalApis = "ExternalApis";
        public const string CryptoApi = "CryptoApi";

        public string Uri { get; set; }
        public string ApiKey { get; set; }
    }
}

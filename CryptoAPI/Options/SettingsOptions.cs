using System.Collections.Generic;

namespace CryptoAPI.Options
{
    public class SettingsOptions
    {
        public const string Settings = "Settings";
        public const string Security = "Security";

        public SettingsOptions()
        {
            ApiKeys = new List<string>();
        }
        public IList<string> ApiKeys { get; set; }
    }
}
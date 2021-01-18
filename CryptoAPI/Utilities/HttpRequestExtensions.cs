using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoAPI.Utilities
{
    public static class HttpRequestExtensions
    {
        public static Uri SetParameters(this Uri uri, IDictionary<string, string> parameters, Uri RequestUri)
        {
            IDictionary<string, string> Parameters = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                Parameters.Add(parameter.Key, parameter.Value);
            }

            var queryString = string.Join("&", Parameters.Select((parameter) => $"{parameter.Key}={parameter.Value}"));
            var query = string.IsNullOrEmpty(queryString) ? null : $"?{queryString}";

            var uriBuilder = new UriBuilder(RequestUri)
            {
                Query = query
            };

            return uriBuilder.Uri;
        }
    }
}
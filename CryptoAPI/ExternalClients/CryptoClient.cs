using CryptoAPI.DTOModels;
using CryptoAPI.Models;
using CryptoAPI.Options;
using CryptoAPI.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoAPI.ExternalClients
{
    public class CryptoClient : ICryptoClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CryptoClient> _logger;
        private ExternalApiOptions _externalApiOptions;

        private readonly string BaseUri;
        private readonly string ApiKey;

        private const string UriQuotesLatest = "v1/cryptocurrency/quotes/latest";
        private const string UriSymbolInfo = "v1/cryptocurrency/map";

        public CryptoClient(HttpClient httpClient,
                            IOptionsMonitor<ExternalApiOptions> externalApiOptions,
                            ILogger<CryptoClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _externalApiOptions = externalApiOptions.CurrentValue;
            externalApiOptions.OnChange(config =>
            {
                _externalApiOptions = config;
                logger.LogInformation("The ExternalApiOptions has been updated.");
            });

            BaseUri = _externalApiOptions.Uri;
            ApiKey = _externalApiOptions.ApiKey;
        }

        public async Task<bool> VerifyCryptocurrencyCode(string cryptocurrencyCode, CancellationToken cancellationToken)
        {
            var requestUri = new Uri(BaseUri + UriQuotesLatest);
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Task is cancelled before verifying Cryptocurreny Code: {cryptocurrencyCode}");
                cancellationToken.ThrowIfCancellationRequested();
            }
            var httpRequestMsg = new HttpRequestMessage(HttpMethod.Head, UriSymbolInfo);
            BuildHttpRequestHeaders(ref httpRequestMsg, ApiKey);
            var parameters = new Dictionary<string, string>();
            parameters.Add("symbol", cryptocurrencyCode);
            httpRequestMsg.RequestUri = requestUri.SetParameters(parameters, requestUri); ;

            var response = await _httpClient.SendAsync(httpRequestMsg, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        public async IAsyncEnumerable<CryptocurrencyPrice> GetLatestQuotes(string cryptocurrencyCode, IList<string> currencies, 
                                                                [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var requestUri = new Uri(BaseUri + UriQuotesLatest);
            foreach (var currency in currencies)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation($"Task is cancelled before getting latest quote for Currency: {currency}");
                    cancellationToken.ThrowIfCancellationRequested();
                }
                var httpRequestMsg = new HttpRequestMessage(HttpMethod.Get, requestUri);
                BuildHttpRequestHeaders(ref httpRequestMsg, ApiKey);
                var parameters = new Dictionary<string, string>();
                parameters.Add("symbol", cryptocurrencyCode);
                parameters.Add("convert", currency);
                httpRequestMsg.RequestUri = requestUri.SetParameters(parameters, requestUri); ;

                var response = await _httpClient.SendAsync(httpRequestMsg, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var contentSteam = await response.Content.ReadAsStreamAsync();
                    response.ValidateClientErrorStatusCodes(contentSteam);
                }

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStreamAsync();
                var cryptoModel = content.ReadAndDeserializeFromJson<Cryptocurrency>();

                yield return new CryptocurrencyPrice(currency) { Price = cryptoModel.Data[cryptocurrencyCode].Quotes[currency].Price };
            }
        }

        private HttpRequestMessage BuildHttpRequestHeaders(ref HttpRequestMessage request, string apiKey)
        {
            request.Headers.Add("X-CMC_PRO_API_KEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return request;
        }
    }
}

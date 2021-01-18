using CryptoAPI.ExternalClients;
using CryptoAPI.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoAPI.Services
{
    public class CryptoService : ICryptoService
    {
        private readonly ICryptoClient _cryptoClient;
        private readonly ILogger<CryptoService> _logger;
        private CurrenciesOptions _currenciesOptions;

        public CryptoService(ICryptoClient cryptoClient, IOptionsMonitor<CurrenciesOptions> currenciesOptions, ILogger<CryptoService> logger)
        {
            _cryptoClient = cryptoClient;
            _logger = logger;
            _currenciesOptions = currenciesOptions.CurrentValue;
            currenciesOptions.OnChange(config =>
            {
                _currenciesOptions = config;
                logger.LogInformation("The CurrenciesOptions has been updated.");
            });
        }

        private async Task<bool> VerifyCryptocurrencyCode(string cryptocurrencyCode, CancellationToken cancellationToken)
        {
            return await _cryptoClient.VerifyCryptocurrencyCode(cryptocurrencyCode, cancellationToken);
        }

        public async Task<Dictionary<string, double>> GetLatesQuotes(string cryptocurrencyCode, CancellationToken cancellationToken)
        {
            var isCryptocurrencyCodeValid = await VerifyCryptocurrencyCode(cryptocurrencyCode, cancellationToken);
            if (!isCryptocurrencyCodeValid)
            {
                var errorMessage = $"The Cryptocurrency Code is invalid {cryptocurrencyCode}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var currencyList = _currenciesOptions.Currencies;
            var currencies = currencyList.ToDictionary(x => x, y => 0.0);

            await foreach (var quote in _cryptoClient.GetLatestQuotes(cryptocurrencyCode, currencyList, cancellationToken))
            {
                currencies[quote.Currency] = quote.Price;
            }

            return currencies;
        }
    }
}

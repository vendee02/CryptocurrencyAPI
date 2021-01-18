using CryptoAPI.DTOModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoAPI.ExternalClients
{
    public interface ICryptoClient
    {
        Task<bool> VerifyCryptocurrencyCode(string symbol, CancellationToken ct);
        IAsyncEnumerable<CryptocurrencyPrice> GetLatestQuotes(string symbol, IList<string> currencies, CancellationToken ct);
    }
}
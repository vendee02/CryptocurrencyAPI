using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoAPI.Services
{
    public interface ICryptoService
    {
        Task<Dictionary<string, double>> GetLatesQuotes(string symbol, CancellationToken cancellationToken);
    }
}
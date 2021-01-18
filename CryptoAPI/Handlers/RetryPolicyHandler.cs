using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoAPI.Handlers
{
    public class RetryPolicyHandler : DelegatingHandler
    {
        private readonly int _maxRetries;

        public RetryPolicyHandler(int maxRetries)
            : base()
        {
            _maxRetries = maxRetries;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            var counter = 0;
            var statusCodes = new List<HttpStatusCode> { HttpStatusCode.ServiceUnavailable,
                              HttpStatusCode.GatewayTimeout, HttpStatusCode.RequestTimeout };
            while (counter < _maxRetries)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (!statusCodes.Contains(response.StatusCode))
                {
                    break;
                }
                counter++;
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            };
            return response;
        }
    }
}

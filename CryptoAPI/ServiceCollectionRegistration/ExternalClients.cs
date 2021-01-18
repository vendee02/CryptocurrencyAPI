using CryptoAPI.ExternalClients;
using CryptoAPI.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;

namespace CryptoAPI.ServiceCollectionRegistration
{
    public static class ExternalClients
    {
        public static IServiceCollection RegisterExternalClients(this IServiceCollection services)
        {
            services.AddHttpClient<ICryptoClient, CryptoClient>()
                 .AddHttpMessageHandler(handler => new RetryPolicyHandler(3));

            return services;
        }
    }
}

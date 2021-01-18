using CryptoAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoAPI.ServiceCollectionRegistration
{
    public static class Services
    {
        public static IServiceCollection RegisterServicess(this IServiceCollection services)
        {
            services.AddScoped<ICryptoService, CryptoService>();

            return services;
        }
    }
}

using CryptoAPI.Options;
using CryptoAPI.Options.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace CryptoAPI.ServiceCollectionRegistration
{
    public static class Options
    {
        public static IServiceCollection RegisterOptions(this IServiceCollection services, IConfiguration Configuration)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ExternalApiOptions>, ExternalServicesConfigurationValidation>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<CurrenciesOptions>, CurrenciesOptionsConfigurationValidation>());

            services.Configure<ExternalApiOptions>(Configuration.GetSection($"{ExternalApiOptions.ExternalApis}:{ExternalApiOptions.CryptoApi}"));
            services.Configure<CurrenciesOptions>(Configuration.GetSection($"{CurrenciesOptions.OutputContent}:{CurrenciesOptions.CryptoApiCurrencies}"));
            services.Configure<SettingsOptions> (Configuration.GetSection($"{SettingsOptions.Settings}:{SettingsOptions.Security}"));
            
            return services;
        }
    }
}

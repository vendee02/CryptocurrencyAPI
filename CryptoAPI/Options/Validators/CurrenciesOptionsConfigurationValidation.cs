using Microsoft.Extensions.Options;
using System.Linq;

namespace CryptoAPI.Options.Validators
{
    public class CurrenciesOptionsConfigurationValidation : IValidateOptions<CurrenciesOptions>
    {
        public ValidateOptionsResult Validate(string name, CurrenciesOptions options)
        {
            if (!options.Currencies.Any())
            {
                return ValidateOptionsResult.Fail("List of currencies is required.");
            }

            return ValidateOptionsResult.Success;
        }
    }
}
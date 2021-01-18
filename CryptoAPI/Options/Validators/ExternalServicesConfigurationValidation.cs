using Microsoft.Extensions.Options;

namespace CryptoAPI.Options.Validators
{
    public class ExternalServicesConfigurationValidation : IValidateOptions<ExternalApiOptions>
    {
        public ValidateOptionsResult Validate(string name, ExternalApiOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Uri))
            {
                return ValidateOptionsResult.Fail("A URL for the product API is required.");
            }
            switch (name)
            {
                case ExternalApiOptions.CryptoApi:
                    if (string.IsNullOrWhiteSpace(options.ApiKey))
                    {
                        return ValidateOptionsResult.Fail("A URL for the product API is required.");
                    }
                    break;
                default:
                    return ValidateOptionsResult.Skip;
            }

            return ValidateOptionsResult.Success;
        }
    }


}
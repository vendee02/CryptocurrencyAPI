using CryptoAPI.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CryptoAPI.Security
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly ILogger _logger;
        private SettingsOptions _settingsOptions;
        private const string ApiKeyHeaderName = "X-API-KEY";
        private const string KeyName = "key";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock, IOptionsMonitor<SettingsOptions> settingsOptions) : base(options, logger, encoder, clock)
        {
            _logger = logger.CreateLogger(nameof(ApiKeyAuthenticationHandler));
            _settingsOptions = settingsOptions.CurrentValue;
            settingsOptions.OnChange(config =>
            {
                _settingsOptions = config;
                _logger.LogInformation("The SettingsOptions has been updated.");
            });
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeader))
            {
                var errorMessage = $"Request Header {ApiKeyHeaderName} is not existing.";
                _logger.LogError(errorMessage);
                return Task.FromResult(AuthenticateResult.Fail(errorMessage));
            }

            var requestHeaderApiKey = apiKeyHeader.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(requestHeaderApiKey))
            {
                var errorMessage = $"Request Header {ApiKeyHeaderName} is empty.";
                _logger.LogError(errorMessage);
                return Task.FromResult(AuthenticateResult.Fail(errorMessage));
            }

            var validApiKeys = _settingsOptions.ApiKeys;
            if (validApiKeys.Contains(requestHeaderApiKey))
            {
                var claims = new List<Claim>
                {
                    new Claim(KeyName, requestHeaderApiKey)
                };
                var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
                var identities = new List<ClaimsIdentity> { identity };
                var principal = new ClaimsPrincipal(identities);
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key is provided."));
        }
    }

}

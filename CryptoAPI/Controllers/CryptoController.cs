using System;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using CryptoAPI.DTOModels;
using CryptoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CryptoAPI.Controllers
{
    [ApiController]
    [Route("api/v1/cryptocurrency/latestquotes")]
    [Authorize]
    public class CryptoController : ControllerBase
    {
        private readonly ICryptoService _cryptoService;
        private readonly ILogger<CryptoController> _logger;

        public CryptoController(ICryptoService cryptoService, ILogger<CryptoController> logger)
        {
            _cryptoService = cryptoService;
            _logger = logger;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLatestQuotes(string cryptocurrencyCode, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cryptocurrencyCode))
                {
                    _logger.LogError("CryptocurrencyCode parameter is null or empty.");
                    return BadRequest();
                }

                var latestquotes = await _cryptoService.GetLatesQuotes(cryptocurrencyCode.ToUpper(), cancellationToken);

                return Ok(new CryptoQuoteDto(cryptocurrencyCode.ToUpper())
                        { ExchangeRate = latestquotes });
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving latest quotes for Cryptocurrency Code: {cryptocurrencyCode}";
                _logger.LogError($"{errorMessage} {ex?.ToString()}");
                
                if (ex is UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                else if (ex is SecurityAccessDeniedException)
                {
                    return Forbid();
                }
                return BadRequest(errorMessage);
            }
        }
    }
}

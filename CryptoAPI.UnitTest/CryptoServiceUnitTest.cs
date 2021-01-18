using CryptoAPI.DTOModels;
using CryptoAPI.ExternalClients;
using CryptoAPI.Options;
using CryptoAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CryptoAPI.UnitTest
{
    [Trait("Category", "UnitTests")]
    public class CryptoServiceUnitTest: TestData
    {
        private Mock<ICryptoClient> cryptoClientMock = null;
        private Mock<IOptionsMonitor<CurrenciesOptions>> mockOptions = null;
        private CryptoService cryptoService = null;
        private CancellationTokenSource cancellationTokenSource = null;
        private Mock<ILogger<CryptoService>> mockLogger = null;

        public CryptoServiceUnitTest()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cryptoClientMock = new Mock<ICryptoClient>();
            mockOptions = new Mock<IOptionsMonitor<CurrenciesOptions>>();
            mockOptions.Setup(x => x.CurrentValue).Returns(new CurrenciesOptions { Currencies = CurrencyList });
            mockLogger = new Mock<ILogger<CryptoService>>();
            mockOptions.Setup(x => x.Get(CurrenciesOptions.CryptoApiCurrencies))
                      .Returns(new CurrenciesOptions { Currencies = CurrencyList });
            cryptoService = new CryptoService(cryptoClientMock.Object, mockOptions.Object, mockLogger.Object);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenCryptocurrencyCodeIsExisting_ShouldSuccessullyRetrieveQuotesForAllCurrencies()
        {
            //Arrange
            cryptoClientMock.Setup(x => x.VerifyCryptocurrencyCode(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            cryptoClientMock.Setup(x => x.GetLatestQuotes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                    .Returns(GetStreamCryptoPrice());

            //Act
            var result = await cryptoService.GetLatesQuotes(BTCsymbol, cancellationTokenSource.Token);

            //Assert
            cryptoClientMock.Verify(x => x.VerifyCryptocurrencyCode(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            cryptoClientMock.Verify(x => x.GetLatestQuotes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(CurrencyList.Count, result.Count);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenCryptocurrencyCodeIsNotExisting_ShouldThrowException()
        {
            //Arrange
            cryptoClientMock.Setup(x => x.VerifyCryptocurrencyCode(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            cryptoClientMock.Setup(x => x.GetLatestQuotes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                    .Returns(GetStreamCryptoPrice());

            //Act
            Func<Task> sut = () => cryptoService.GetLatesQuotes(BTCsymbol, cancellationTokenSource.Token);

            //Assert
            var exception = await Assert.ThrowsAsync<Exception>(sut);
            cryptoClientMock.Verify(x => x.VerifyCryptocurrencyCode(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            cryptoClientMock.Verify(x => x.GetLatestQuotes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenClientThrowsException_ShouldReThrowException()
        {
            //Arrange
            var rnd = new Random();
            async IAsyncEnumerable<CryptocurrencyPrice> StreamData()
            {
                yield return new CryptocurrencyPrice(CurrencyList[1]) { Price = rnd.NextDouble() };
                await Task.FromException(new UnauthorizedAccessException());
                yield break;
            }
            cryptoClientMock.Setup(x => x.VerifyCryptocurrencyCode(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            cryptoClientMock.Setup(x => x.GetLatestQuotes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                    .Returns(StreamData());

            //Act
            Func<Task> sut = () => cryptoService.GetLatesQuotes(BTCsymbol, cancellationTokenSource.Token);

            //Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(sut);
            cryptoClientMock.Verify(x => x.VerifyCryptocurrencyCode(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            cryptoClientMock.Verify(x => x.GetLatestQuotes(It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

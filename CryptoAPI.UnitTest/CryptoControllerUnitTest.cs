using CryptoAPI.Controllers;
using CryptoAPI.DTOModels;
using CryptoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CryptoAPI.UnitTest
{
    [Trait("Category", "UnitTests")]
    public class CryptoControllerUnitTest: TestData
    {
        private Mock<ICryptoService> cryptoServiceMock = null;
        private Mock<ILogger<CryptoController>> mockLogger = null;
        private CryptoController cryptoController = null;
        private CancellationTokenSource cancellationTokenSource = null;

        public  CryptoControllerUnitTest()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cryptoServiceMock = new Mock<ICryptoService>();
            mockLogger = new Mock<ILogger<CryptoController>>();
            cryptoController = new CryptoController(cryptoServiceMock.Object, mockLogger.Object);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenInputIsValid_ShouldSuccessfullyReturnLatestQuotes()
        {
            //Arrange
            var expectedResult = new Dictionary<string, double> { { "EUR", 123.1 } };

            cryptoServiceMock.Setup(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            //Act
            var result = await cryptoController.GetLatestQuotes(BTCsymbol, cancellationTokenSource.Token);
            var resultCrypto = ((ObjectResult)result);

            //Assert
            cryptoServiceMock.Verify(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal((int)HttpStatusCode.OK, resultCrypto.StatusCode);
            Assert.Equal(expectedResult.FirstOrDefault().Key, ((CryptoQuoteDto)resultCrypto.Value).ExchangeRate.FirstOrDefault().Key);
            Assert.Equal(expectedResult.FirstOrDefault().Value, ((CryptoQuoteDto)resultCrypto.Value).ExchangeRate.FirstOrDefault().Value);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenUnauthorizedAccessExceptionIsThrown_ShouldReturnUnauthorized()
        {
            //Arrange
            var expectedResult = new Dictionary<string, double>();
            cryptoServiceMock.Setup(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws<UnauthorizedAccessException>();

            //Act
            var result = await cryptoController.GetLatestQuotes(BTCsymbol, cancellationTokenSource.Token);
            var resultCrypto = ((StatusCodeResult)result);

            //Assert
            cryptoServiceMock.Verify(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultCrypto.StatusCode);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenOperationCanceledExceptionIsThrown_ShouldReturnNoContent()
        {
            //Arrange
            var expectedResult = new Dictionary<string, double>();
            cryptoServiceMock.Setup(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>();

            //Act
            var result = await cryptoController.GetLatestQuotes(BTCsymbol, cancellationTokenSource.Token);
            var resultCrypto = ((ObjectResult)result);

            //Assert
            cryptoServiceMock.Verify(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal((int)HttpStatusCode.BadRequest, resultCrypto.StatusCode);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenArgumentNullExceptionIsThrown_ShouldReturnBadRequest()
        {
            //Arrange
            var expectedResult = new Dictionary<string, double>();
            cryptoServiceMock.Setup(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws<ArgumentNullException>();

            //Act
            var result = await cryptoController.GetLatestQuotes(BTCsymbol, cancellationTokenSource.Token);
            var resultCrypto = ((ObjectResult)result);

            //Assert
            cryptoServiceMock.Verify(x => x.GetLatesQuotes(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal((int)HttpStatusCode.BadRequest, resultCrypto.StatusCode);
        }
    }
}

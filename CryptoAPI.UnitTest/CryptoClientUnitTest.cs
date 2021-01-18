using CryptoAPI.DTOModels;
using CryptoAPI.ExternalClients;
using CryptoAPI.Models;
using CryptoAPI.Options;
using Dasync.Collections;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CryptoAPI.UnitTest
{
    [Trait("Category", "UnitTests")]
    public class CryptoClientUnitTest: TestData
    { 
        private Mock<HttpMessageHandler> httpMessageHandlerMock = null;
        private Mock<IOptionsMonitor<ExternalApiOptions>> mockOptions = null;
        private Mock<ILogger<CryptoClient>> mockLogger = null;
        private IList<Cryptocurrency> cryptoModels = null;
        private HttpClient _httpClient = null;
        private CancellationTokenSource cancellationTokenSource = null;
        private CryptoClient cryptoClient = null;

        public CryptoClientUnitTest()
        {
            cancellationTokenSource = new CancellationTokenSource();
            cryptoModels = CryptoQuoteList(BTCsymbol, CurrencyList);
            httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            mockOptions = new Mock<IOptionsMonitor<ExternalApiOptions>>();
            mockOptions.Setup(x => x.CurrentValue)
                       .Returns(new ExternalApiOptions { ApiKey = "test", Uri = "https://pro-api.coinmarketcap.com/" });
            mockLogger = new Mock<ILogger<CryptoClient>>();
            httpMessageHandlerMock.Protected()
               .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
             ).ReturnsAsync(new HttpResponseMessage()
             {
                 StatusCode = HttpStatusCode.OK,
                 Content = new StreamContent(GetStream(cryptoModels[0]))
             })
             .ReturnsAsync(new HttpResponseMessage()
             {
                 StatusCode = HttpStatusCode.OK,
                 Content = new StreamContent(GetStream(cryptoModels[1]))
             })
             .ReturnsAsync(new HttpResponseMessage()
             {
                 StatusCode = HttpStatusCode.OK,
                 Content = new StreamContent(GetStream(cryptoModels[2]))
             })
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(GetStream(cryptoModels[3]))
            });
            _httpClient = new HttpClient(httpMessageHandlerMock.Object);
            cryptoClient = new CryptoClient(_httpClient, mockOptions.Object, mockLogger.Object);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenInputIsValid_ShouldSuccessfullyReturnLatestQuotes()
        {
            //Arrange
            var result = new List<CryptocurrencyPrice>();

            //Act
            await cryptoClient.GetLatestQuotes("BTC", CurrencyList, cancellationTokenSource.Token).ForEachAsync(x => result.Add(x));

            //Assert
            Assert.Equal(CurrencyList.Count, result.Count);
            httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Exactly(CurrencyList.Count),
                                   ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetLatestQuotes_WhenResponseHttpStatusCodeIsUnauthorized_ShouldThrowUnauthorizedAccessException()
        {
            //Arrange
            var result = new List<CryptocurrencyPrice>();
            httpMessageHandlerMock.Protected()
                 .SetupSequence<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               ).ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StreamContent(GetStream(cryptoModels[0]))
               })
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.Unauthorized,
                   Content = new StreamContent(GetStream(cryptoModels[1]))
               })
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StreamContent(GetStream(cryptoModels[2]))
               })
              .ReturnsAsync(new HttpResponseMessage()
              {
                  StatusCode = HttpStatusCode.OK,
                  Content = new StreamContent(GetStream(cryptoModels[3]))
              });

            //Act
            Func<Task> sut = () => cryptoClient.GetLatestQuotes("BTC", CurrencyList, cancellationTokenSource.Token)
                                    .ForEachAsync(x => result.Add(x));

            //Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(sut);
        }

        [Fact]
        public async Task GetLatestQuotes_WhenCancellationIsRequested_ShouldThrowOperationCanceledException()
        {
            //Arrange
            var result = new List<CryptocurrencyPrice>();

            //Act
            var enumerator = cryptoClient.GetLatestQuotes("BTC", CurrencyList, cancellationTokenSource.Token)
                                  .GetAsyncEnumerator(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            Func<Task> sut = async () =>  await enumerator.MoveNextAsync();

           //Assert
            await Assert.ThrowsAsync<OperationCanceledException>(sut);
        }
    }
}

using CryptoAPI.Handlers;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CryptoAPI.UnitTest
{
    public class RetryPolicyHandlerUnitTest
    {
        private RetryPolicyHandler _handler;
        private const int MaximumRetries = 3;
        private readonly HttpRequestMessage _httpRequest;
        private Mock<DelegatingHandler> _retryPolicyHandler;
        private HttpClient _httpClient;

        public RetryPolicyHandlerUnitTest()
        {
            _handler = new RetryPolicyHandler(MaximumRetries);
            _httpRequest = new HttpRequestMessage(HttpMethod.Get, "/test");
            _retryPolicyHandler = new Mock<DelegatingHandler>();
            _handler = new RetryPolicyHandler(3)
            {
                InnerHandler = _retryPolicyHandler.Object
            };
            _httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://localhost")
            };
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.Unauthorized)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Forbidden)]
        public async Task SendAsync_WhenStatuscodeIsNotTimeoutOrServiceUnavailable_ShouldExecuteSendAsyncOnlyOnce(HttpStatusCode httpStatusCode)
        {
            //Arrange
            var mockedResult = new HttpResponseMessage(httpStatusCode);
            _retryPolicyHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequest, ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockedResult);

            //Act
            await _httpClient.SendAsync(_httpRequest);

            //Assert
            _retryPolicyHandler.Protected().Verify("SendAsync", Times.Exactly(1),
                                    ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }

        [Theory]
        [InlineData(HttpStatusCode.GatewayTimeout)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        public async Task SendAsync_WhenStatuscodeIsTimeoutOrServiceUnavailable_ShouldExecuteSendAsyncWithMaximumRetries(HttpStatusCode httpStatusCode)
        {
            //Arrange
            var mockedResult = new HttpResponseMessage(httpStatusCode);
            _retryPolicyHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", _httpRequest, ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockedResult);

            //Act
            await _httpClient.SendAsync(_httpRequest);

            //Assert
            _retryPolicyHandler.Protected().Verify("SendAsync", Times.Exactly(MaximumRetries),
                                      ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
        }
    }
}

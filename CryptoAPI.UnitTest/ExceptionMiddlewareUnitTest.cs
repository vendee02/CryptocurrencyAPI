using CryptoAPI.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CryptoAPI.UnitTest
{
    [Trait("Category", "UnitTests")]
    public class ExceptionMiddlewareUnitTest
    {
        private Mock<ILogger<ExceptionMiddleware>> _mockLogger = null;

        public ExceptionMiddlewareUnitTest()
        {
            _mockLogger = new Mock<ILogger<ExceptionMiddleware>>();
        }

        [Fact]
        public async Task InvokeAsync_WhenExceptionIsThrown_ShouldBeHandledWithACustomerErrorResponse()
        {
            // Arrange
            var exceptionMiddleware = new ExceptionMiddleware(next: (httpcontext) =>
            {
                throw new Exception("Error");
            }, _mockLogger.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //Act
            await exceptionMiddleware.InvokeAsync(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var contextResponse = new StreamReader(context.Response.Body).ReadToEnd();
            var response = JsonConvert.DeserializeObject<ErrorDetailsDto>(contextResponse);

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal("Error", response.Message);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoExceptionIsThrown_ShouldNotReturnAnErrorResponse()
        {
            // Arrange
            var exceptionMiddleware = new ExceptionMiddleware(next: async (httpcontext) =>
            {
                await httpcontext.Response.WriteAsync("Successful");
            },_mockLogger.Object);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            //Act
            await exceptionMiddleware.InvokeAsync(context);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var contextResponse = new StreamReader(context.Response.Body).ReadToEnd();

            //Assert
            Assert.Equal("Successful", contextResponse);
        }
    }
}

using CryptoAPI.DTOModels;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Security;

namespace CryptoAPI.Utilities
{
    public static class HttpResponseExtensions
    {
        public static void ValidateClientErrorStatusCodes(this HttpResponseMessage httpResponseMessage, Stream contentSteam)
        {
            if (httpResponseMessage == null)
            {
                throw new ArgumentNullException(nameof(httpResponseMessage));
            }

            var errorModel = contentSteam.ReadAndDeserializeFromJson<ErrorModel>();
            var errorMessage = errorModel.ErrorStatus?.ErrorMessage;

            if (httpResponseMessage.StatusCode is HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException(errorMessage);
            }
            else if (httpResponseMessage.StatusCode is HttpStatusCode.Forbidden)
            {
                throw new SecurityAccessDeniedException(errorMessage);
            }
            else
            {
                throw new Exception(errorMessage);
            }
        }
    }
}

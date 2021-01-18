﻿using Newtonsoft.Json;

namespace CryptoAPI
{
    public class ErrorDetailsDto
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

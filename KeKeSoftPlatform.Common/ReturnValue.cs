using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeKeSoftPlatform.Common
{
    public class ReturnValue
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        public ReturnValue()
        {
            IsSuccess = false;
        }
    }

    public class ReturnValue<T>
    {
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
        public ReturnValue()
        {
            IsSuccess = false;
        }
    }
}

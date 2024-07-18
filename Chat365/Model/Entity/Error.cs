using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.Entity
{
    public class Error
    {
        [JsonProperty("code")]
        public int code { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
    }
}

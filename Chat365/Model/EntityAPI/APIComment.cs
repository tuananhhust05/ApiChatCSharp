using APIChat365.Model.Entity;
using APIChat365.Model.MongoEntity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace APIChat365.Model.EntityAPI
{
    public class APIComment
    {
        [JsonProperty("data")]
        public DataComment data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataComment
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("listComment")]
        public List<CommentDB> listComment { get; set; }
    }
}

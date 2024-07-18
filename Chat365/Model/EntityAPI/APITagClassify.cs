using APIChat365.Model.Entity;
using APIChat365.Model.MongoEntity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace APIChat365.Model.EntityAPI
{
    
    public class APITagClassify
    {
        [JsonProperty("data")]
        public DataTagClassify data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataTagClassify
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("listComment")]
        public List<TagClassifyDB> listTag { get; set; }
    }
}

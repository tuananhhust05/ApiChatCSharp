using APIChat365.Model.Entity;
using Chat365.Server.Model.Entity;
using Newtonsoft.Json;

namespace APIChat365.Model.EntityAPI
{
    public class APIQR
    {
        [JsonProperty("data")]
        public DataQR data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataQR
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("listNameFile")]
        public string[] listNameFile { get; set; }
        [JsonProperty("conversationId")]
        public int conversationId { get; set; }
        [JsonProperty("conversationInfo")]
        public Conversation conversationInfo { get; set; }
    }

}

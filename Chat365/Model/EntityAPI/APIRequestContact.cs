using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat365.Server.Model.Entity;
using APIChat365.Model.Entity;

namespace Chat365.Server.Model.EntityAPI
{
    public class APIRequestContact
    {
        [JsonProperty("data")]
        public DataRequest data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataRequest
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("ListRequestContact")]
        public List<RequestContact> ListRequestContact { get; set; }
        [JsonProperty("RequestListContact")]
        public List<RequestContact1> RequestListContact { get; set; }
        [JsonProperty("ListUserSendRequest")]
        public List<RequestContact1> ListUserSendRequest { get; set; }
        [JsonProperty("conversationId")]
        public int conversationId { get; set; }
        [JsonProperty("id")]
        public int id { get; set; }
    }

    public class APIRequestFriend
    {
        [JsonProperty("data")]
        public DataRequestFriend data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataRequestFriend
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("ListRequestFriend")]
        public List<RequestFriend> ListRequestFriend { get; set; }
    }

}

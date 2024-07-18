using APIChat365.Model.Entity;
using Chat365.Model.Entity;
using Chat365.Server.Model.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace APIChat365.Model.EntityAPI
{
    public class NotificationAPI
    {
        [JsonProperty("data")]
        public DataNotification data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }

    public class DataNotification
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("listNotification")]
        public List<Notifications> listNotification { get; set; }
    }
}

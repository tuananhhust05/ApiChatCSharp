using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat365.Server.Model.Entity;
using APIChat365.Model.Entity;

namespace Chat365.Server.Model.EntityAPI
{
    public class APIConversation
    {
        [JsonProperty("data")]
        public DataConversation data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataConversation
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("conversation")]
        public Conversation conversation { get; set; }
        [JsonProperty("countConversation")]
        public int countConversation { get; set; }
        [JsonProperty("conversation_info")]
        public Conversation conversation_info { get; set; }
        [JsonProperty("user_list")]
        public List<MemberConversation> user_list { get; set; }
        [JsonProperty("listCoversation")]
        public List<Conversation> listCoversation { get; set; }
        [JsonProperty("ConversationInSearch")]
        public List<ConversationInSearch> listCoversationInSearch { get; set; }
    }
    public class APIConversationForward
    {
        [JsonProperty("data")]
        public DataConversationForward data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataConversationForward
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("listCoversation")]
        public List<ConversationForward> listCoversation { get; set; }
    }
    public class APINewConversation
    {
        [JsonProperty("conversation")]
        public string conversation { get; set; }
    }

    public class APIConversationUnreader
    {
        [JsonProperty("data")]
        public DataConversationUnreader data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataConversationUnreader
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("countConversation")]
        public int countConversation { get; set; }
        [JsonProperty("listConversation")]
        public List<int> listConversation { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat365.Server.Model.Entity;
using APIChat365.Model.Entity;

namespace Chat365.Server.Model.EntityAPI
{
    public class APIMessage
    {
        [JsonProperty("data")]
        public DataMessage data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataMessage
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("messageId")]
        public string messageId { get; set; }
        [JsonProperty("senderName")]
        public string senderName { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("countMessage")]
        public int countMessage { get; set; }
        [JsonProperty("message_info")]
        public Messages message_info { get; set; }
        [JsonProperty("listMessages")]
        public List<Messages> listMessages { get; set; }
    }
    public class APIMessageTimViec
    {
        public APIMessageTimViec(string username, string message, string files, string send, int uid, int uid_type, string room, string avatar, int userId)
        {
            this.username = username;
            this.message = message;
            this.files = files;
            this.send = send;
            this.uid = uid;
            this.room = room;
            this.uid_type = uid_type;
            if (string.IsNullOrEmpty(avatar))
            {
                this.ava = "https://timviec365.vn/images/ic_ava1.png";
            }
            else
            {
                this.ava = "https://mess.timviec365.vn/avatarUser/" + userId + "/" + avatar;
            }
            if (uid_type == 1)
            {
                canonical = "https://timviec365.vn/cong-ty-timviec365-co" + uid;
            }
            else
            {
                canonical = "https://timviec365.vn/ung-vien/timviec365-uv" + uid + ".html";
            }
        }

        public string username { get; set; }
        public string message { get; set; }
        public string files { get; set; }
        public string send { get; set; }
        public int uid { get; set; }
        public int uid_type { get; set; }
        public string ava { get; set; }
        public string canonical { get; set; }
        public string room { get; set; }
    }


    public class APIMessage_v2
    {
        [JsonProperty("data")]
        public DataMessage_v2 data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class ItemMessage_v2
    {
        [JsonProperty("conversationId")]
        public int conversationID { get; set; }
        [JsonProperty("countMessage")]
        public int countMessage { get; set; }
        [JsonProperty("message_info")]
        public Messages message_info { get; set; }
        [JsonProperty("listMessages")]
        public List<Messages> listMessages { get; set; }
    }
    public class DataMessage_v2
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        public List<ItemMessage_v2> listConversation { get; set; }
    }

    public class APIMessage_LiveChat
    {
        [JsonProperty("data")]
        public DataMessage_LiveChat data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataMessage_LiveChat
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("conversationId")]
        public int conversationId { get; set; }
        public List<int> listMember { get; set; }

        [JsonProperty("countMessage")]
        public int countMessage { get; set; }
        [JsonProperty("unReader")]
        public int unReader { get; set; }
        [JsonProperty("message_info")]
        public Messages message_info { get; set; }
        [JsonProperty("listMessages")]
        public List<Messages_v2> listMessages { get; set; }
        [JsonProperty("messageId")]
        public string messageId { get; set; }
        [JsonProperty("timeLastSeener")]
        public DateTime timeLastSeener { get; set; }
        [JsonProperty("nameLastSeener")]
        public string nameLastSeener { get; set; }
        [JsonProperty("avatarLastSeener")]
        public string avatarLastSeener { get; set; }
    }
}

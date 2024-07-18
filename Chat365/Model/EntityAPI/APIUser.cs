using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat365.Server.Model.Entity;
using APIChat365.Model.Entity;
using APIChat365.Model.MongoEntity;

namespace Chat365.Server.Model.EntityAPI
{
    public class APIUser
    {
        [JsonProperty("data")]
        public DataUser data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataUser
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("userName")]
        public string userName { get; set; }
        [JsonProperty("countConversation")]
        public int countConversation { get; set; }
        [JsonProperty("conversationId")]
        public int conversationId { get; set; }
        [JsonProperty("total")]
        public int total { get; set; }
        [JsonProperty("currentTime")]
        public Int64 currentTime { get; set; }
        [JsonProperty("ListUserOnline")]
        public int[] ListUserOnline { get; set; }
        [JsonProperty("user_info")]
        public User user_info { get; set; }
        [JsonProperty("user_list")]
        public List<Contact> user_list { get; set; }
    }

    public class APIUser1
    {
        [JsonProperty("data")]
        public DataUser1 data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataUser1
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("user_list")]
        public List<Contact1> user_list { get; set; }
    }
    public class Contact1
    {
        public Contact1()
        {
        }

        public Contact1(int id, string userName, string email, string avatarUser, string status, int active, int isOnline, int looker, int statusEmotion, DateTime lastActive, int companyId, int type365,int id365, string friendStatus = "none")
        {
            ID = id;
            UserName = userName;
            AvatarUser = avatarUser;
            Status = status;
            Active = active;
            this.isOnline = isOnline;
            Looker = looker;
            StatusEmotion = statusEmotion;
            LastActive = lastActive;
            CompanyId = companyId;
            Email = email;
            FriendStatus = friendStatus;
            Type365 = type365;
            ID365 = id365;
        }

        public int ID { get; set; }
        public int ID365 { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string AvatarUser { get; set; }
        public string Status { get; set; }
        public int Active { get; set; }
        public int isOnline { get; set; }
        public int Looker { get; set; }
        public int StatusEmotion { get; set; }
        public DateTime LastActive { get; set; }
        public string LinkAvatar { get; set; }
        public int CompanyId { get; set; }
        public string FriendStatus { get; set; }
        public int Type365 { get; set; }
    }

    public class APIUserHHP365
    {
        [JsonProperty("data")]
        public DataUserHHP365 data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataUserHHP365
    {
        public bool result { get; set; }
        public string message { get; set; }
        public int conversationId { get; set; }
        public int userId { get; set; }
        public string secretCode { get; set; }
        public string fromWeb { get; set; }

    }
    public class APIGetFormQLC
    {
        [JsonProperty("data")]
        public DataRequest data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataGetFormQLC
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("type")]
        public int type { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
    }
    public class APIUserCompany
    {
        [JsonProperty("data")]
        public DataUserCompany data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataUserCompany
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("userName")]
        public string userName { get; set; }
        [JsonProperty("countConversation")]
        public int countConversation { get; set; }
        [JsonProperty("conversationId")]
        public int conversationId { get; set; }
        [JsonProperty("total")]
        public int total { get; set; }
        [JsonProperty("currentTime")]
        public Int64 currentTime { get; set; }
        [JsonProperty("ListUserOnline")]
        public int[] ListUserOnline { get; set; }
        [JsonProperty("user_info")]
        public User user_info { get; set; }
        [JsonProperty("user_list")]
        public List<ContactCompany> user_list { get; set; }
    }

    public class APIFriend
    {
        [JsonProperty("data")]
        public DataFriend data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataFriend
    {
        public bool result { get; set; }
        public string message { get; set; }
        public List<FriendRequest> listFriend { get; set; }
    }

    public class APIUserCheck
    {
        [JsonProperty("data")]
        public DataUserCheck data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataUserCheck
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("user_list")]
        public List<UserDB> user_list { get; set; }
    }
}

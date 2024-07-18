using APIChat365.Model.Entity;
using Chat365.Server.Model.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Server.Model.EntityAPI
{
    public class APIOTP
    {
        [JsonProperty("data")]
        public DataOTP data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataOTP
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("listNameFile")]
        public string[] listNameFile { get; set; }
        [JsonProperty("otp")]
        public string otp { get; set; }
    }

    public class APISearch
    {
        [JsonProperty("data")]
        public DataSearch data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class DataSearch
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("listContactInCompany")]
        public List<Contact> listContactInCompany { get; set; }

        [JsonProperty("listGroup")]
        public List<Conversation> listGroup { get; set; }

        [JsonProperty("listEveryone")]
        public List<Contact> listEveryone { get; set; }
    }
    public class OTPByPhone
    {
        public string ApiKey = "CD013D8EF367403A13DB9695679A32";
        public string Content { get; set; }
        public string Phone { get; set; }
        public string SecretKey = "80A7A1845725B74E5766A5BFB0B167";
        public string IsUnicode = "1";
        public string Brandname = "TIMVIEC365";
        public string SmsType = "2";

        public OTPByPhone(string content, string phone)
        {
            Content = content;
            Phone = phone;
        }
    }
}

using APIChat365.Model.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Server.Model.Entity
{
    public class InforFromAPI
    {
        [JsonProperty("data")]
        public Data data { get; set; }
        [JsonProperty("error")]
        public Error error { get; set; }
    }
    public class Data
    {
        [JsonProperty("result")]
        public bool result { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }
        [JsonProperty("user_info")]
        public UserFormAPI user_info { get; set; }
        [JsonProperty("items")]
        public List<UserFormAPI> items { get; set; }
        [JsonProperty("item_rose")]
        public Item_rose item_rose { get; set; }
        [JsonProperty("otp")]
        public int otp { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("access_token")]
        public string access_token { get; set; }
    }

    public class Item_rose
    {
        [JsonProperty("rose_sum")]
        public string rose_sum { get; set; }
        [JsonProperty("rose1")]
        public string rose1 { get; set; }
        [JsonProperty("rose2")]
        public string rose2 { get; set; }
        [JsonProperty("rose3")]
        public string rose3 { get; set; }
        [JsonProperty("rose4")]
        public string rose4 { get; set; }
        [JsonProperty("rose5")]
        public string rose5 { get; set; }
    }

    public class UserFormAPI
    {
        [JsonProperty("ep_id")]
        public int ep_id { get; set; }
        [JsonProperty("ep_email")]
        public string ep_email { get; set; }
        [JsonProperty("ep_name")]
        public string ep_name { get; set; }
        [JsonProperty("ep_pass")]
        public string ep_pass { get; set; }
        [JsonProperty("ep_phone")]
        public string ep_phone { get; set; }
        [JsonProperty("ep_image")]
        public string ep_image { get; set; }
        [JsonProperty("ep_authentic")]
        public int ep_authentic { get; set; }
        [JsonProperty("com_id")]
        public int com_id { get; set; }
        [JsonProperty("com_name")]
        public string com_name { get; set; }
        [JsonProperty("com_email")]
        public string com_email { get; set; }
        [JsonProperty("com_pass")]
        public string com_pass { get; set; }
        [JsonProperty("com_phone")]
        public string com_phone { get; set; }
        [JsonProperty("com_logo")]
        public string com_logo { get; set; }
        [JsonProperty("com_authentic")]
        public int com_authentic { get; set; }
        [JsonProperty("update_time")]
        public string update_time { get; set; }
        [JsonProperty("dep_name")]
        public string dep_name { get; set; }
        [JsonProperty("dep_id")]
        public string dep_id { get; set; }
        [JsonProperty("ep_birth_day")]
        public string ep_birth_day { get; set; }
    }
}

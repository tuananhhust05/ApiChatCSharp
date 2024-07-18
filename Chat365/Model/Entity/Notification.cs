using Chat365.Server.Model.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Model.Entity
{
    public class Notifications
    {
        public Notifications(string iD, int userID, User participant, string title, string message, int isUnreader, string type, string messageId, int conversayionId, DateTime createAt, string link)
        {
            IDNotification = iD;
            UserID = userID;
            Participant = participant;
            Title = title;
            Message = message;
            IsUnreader = isUnreader;
            Type = type;
            ConversationId = conversayionId;
            CreateAt = createAt;
            setDate();
            Link = link;
        }

        private void setDate()
        {
            DateTime DaysMessage = new DateTime(CreateAt.Year, CreateAt.Month, CreateAt.Day, 0, 0, 0);
            if (DateTime.Now.Subtract(DaysMessage).Days < 1)
            {
                Time = "Hôm nay" + " lúc " + CreateAt.ToString("h:mm tt");
            }
            else if (DateTime.Now.Subtract(DaysMessage).Days >= 1 && DateTime.Now.Subtract(DaysMessage).Days < 2)
            {
                Time = "Hôm qua" + " lúc " + CreateAt.ToString("h:mm tt");
            }
            else
            {
                if (DateTime.Now.Subtract(DaysMessage).Days >= 2 && DateTime.Now.Subtract(DaysMessage).Days < 7)
                {
                    Time = CreateAt.DayOfWeek.ToString() + " lúc " + CreateAt.ToString("h:mm tt");
                }
                else
                {
                    Time = CreateAt.ToString("dddd, dd MMMM, yyyy") + " lúc " + CreateAt.ToString("h:mm tt");
                }
                Time = Time.Replace("MonDay", "Thứ 2");
                Time = Time.Replace("Tuesday", "Thứ 3");
                Time = Time.Replace("Wednesday", "Thứ 4");
                Time = Time.Replace("Thursday", "Thứ 5");
                Time = Time.Replace("Friday", "Thứ 6");
                Time = Time.Replace("Saturday", "Thứ 7");
                Time = Time.Replace("Sunday", "Chủ nhật");
                Time = Time.Replace("January", "Tháng 1");
                Time = Time.Replace("February", "Tháng 2");
                Time = Time.Replace("March", "Tháng 3");
                Time = Time.Replace("April", "Tháng 4");
                Time = Time.Replace("May", "Tháng 5");
                Time = Time.Replace("June", "Tháng 6");
                Time = Time.Replace("July", "Tháng 7");
                Time = Time.Replace("August", "Tháng 8");
                Time = Time.Replace("September", "Tháng 9");
                Time = Time.Replace("October", "Tháng 10");
                Time = Time.Replace("November", "Tháng 11");
                Time = Time.Replace("December", "Tháng 12");
            }

        }

        [JsonProperty("IDNotification")]
        public string IDNotification { get; set; }
        [JsonProperty("UserID")]
        public int UserID { get; set; }
        [JsonProperty("Participant")]
        public User Participant { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
        [JsonProperty("IsUnreader")]
        public int IsUnreader { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("MessageId")]
        public string MessageId { get; set; }
        [JsonProperty("ConversationId")]
        public int ConversationId { get; set; }
        [JsonProperty("CreateAt")]
        public DateTime CreateAt { get; set; }
        [JsonProperty("Time")]
        public string Time { get; set; }
        [JsonProperty("Link")]
        public string Link { get; set; }
    }
    public class NotificationNTD
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public int SenderId { get; set; }
        public string Link { get; set; }
    }
}

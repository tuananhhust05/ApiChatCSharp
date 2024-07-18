
using System;

namespace APIChat365.Model.MongoEntity
{
    public class InfoSupportDB
    {
        public InfoSupportDB()
        {
            this.title = "";
            this.message = "";
            this.supportId = "";
            this.haveConversation = 0;
            this.status = 0;
            this.userId = 0;
            this.time = DateTime.MinValue;
        }

        public InfoSupportDB(string title, string message, string supportId, int haveConversation, int userId, int status, DateTime time)
        {
            this.title = title;
            this.message = message;
            this.supportId = supportId;
            this.haveConversation = haveConversation;
            this.userId = userId;
            this.status = status;
            this.time = time;
        }

        public string title { get; set; }
        public string message { get; set; }
        public string supportId { get; set; }
        public int haveConversation { get; set; }
        public int userId { get; set; }
        public int status { get; set; }
        public DateTime time { get; set; }
    }
}

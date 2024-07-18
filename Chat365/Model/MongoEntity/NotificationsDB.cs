using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.MongoEntity
{
    public class NotificationsDB
    {
        public NotificationsDB(string id, int userId, int paticipantId, string title, string message, int isUndeader, DateTime createAt, string type, string mesageId, int conversationId, string link)
        {
            this.id = id;
            this.userId = userId;
            this.paticipantId = paticipantId;
            this.title = title;
            this.message = message;
            this.isUndeader = isUndeader;
            this.createAt = createAt;
            this.type = type;
            this.messageId = mesageId;
            this.conversationId = conversationId;
            this.link = link;
        }

        public string id { get; set; }
        public int userId { get; set; }
        public int paticipantId { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public int isUndeader { get; set; }
        public DateTime createAt { get; set; }
        public string type { get; set; }
        public string messageId { get; set; }
        public int conversationId { get; set; }
        public string link { get; set; }
    }
}

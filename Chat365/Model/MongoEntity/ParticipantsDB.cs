using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.MongoEntity
{
    public class ParticipantsDB
    {
        public ParticipantsDB()
        {
        }

        public ParticipantsDB(int memberId, string conversationName, int unReader, int messageDisplay, int isHidden, int isFavorite, int notification, DateTime timeLastSeener,int deleteTime, int deleteType)
        {
            this.memberId = memberId;
            this.conversationName = conversationName;
            this.unReader = unReader;
            this.messageDisplay = messageDisplay;
            this.isHidden = isHidden;
            this.isFavorite = isFavorite;
            this.notification = notification;
            this.timeLastSeener = timeLastSeener;
            this.deleteTime = deleteTime;
            this.deleteType = deleteType;
        }

        public int memberId { get; set; }
        public string conversationName { get; set; }
        public int unReader { get; set; }
        public long messageDisplay { get; set; }
        public int isHidden { get; set; }
        public int isFavorite { get; set; }
        public int notification { get; set; }
        public DateTime timeLastSeener { get; set; }
        public int deleteTime { get; set; }
        public int deleteType { get; set; }
        public List<string> favoriteMessage { get; set; }
        public LiveChatDB liveChat { get; set; }
    }
}

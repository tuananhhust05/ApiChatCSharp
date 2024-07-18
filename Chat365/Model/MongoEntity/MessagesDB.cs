using APIChat365.MongoEntity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.MongoEntity
{
    public class MessagesDB
    {
        public MessagesDB(string id, int displayMessage, int senderId, string messageType, string message, string quoteMessage, string messageQuote, DateTime createAt, int isEdited, InfoLinkDB infoLink, List<FileSendDB> listFile, EmotionMessageDB emotion, int deleteTime, int deleteType, DateTime deleteDate,InfoSupportDB infoSupport, LiveChatDB liveChat, List<int> listClicked, string from)
        {
            this.id = id;
            this.displayMessage = displayMessage;
            this.senderId = senderId;
            this.messageType = messageType;
            this.message = message;
            this.quoteMessage = quoteMessage;
            this.messageQuote = messageQuote;
            this.createAt = createAt;
            this.isEdited = isEdited;
            this.infoLink = infoLink;
            this.listFile = listFile;
            this.emotion = emotion;
            this.deleteTime = deleteTime;
            this.deleteType = deleteType;
            this.deleteDate = deleteDate;
            this.infoSupport = infoSupport;
            this.liveChat = liveChat;
            this.notiClicked = listClicked;
            this.from = from;
        }

        public string id { get; set; }
        public int displayMessage { get; set; }
        public int senderId { get; set; }
        public string messageType { get; set; }
        public string message { get; set; }
        public string quoteMessage { get; set; }
        public string messageQuote { get; set; }
        public DateTime createAt { get; set; }
        public int isEdited { get; set; }
        public InfoLinkDB infoLink { get; set; }
        public List<FileSendDB> listFile { get; set; }
        public EmotionMessageDB emotion { get; set; }
        public int deleteTime { get; set; }
        public int deleteType { get; set; }
        public DateTime deleteDate { get; set; }
        public List<int> notiClicked { get; set; }
        public InfoSupportDB infoSupport { get; set; }
        public LiveChatDB liveChat { get; set; }
        public string from { get; set; }
    }
}

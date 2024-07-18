using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace APIChat365.Model.MongoEntity
{
    public class ConversationsDB
    {
        public ConversationsDB()
        {
        }

        public ConversationsDB(int conversationId, int isGroup, string typeGroup, string avatarConversation, int adminId, int shareGroupFromLinkOption, int browseMemberOption, string pinMessage, List<ParticipantsDB> memberList, List<MessagesDB> messageList, List<BrowseMembersDB> browseMemberList, DateTime timeLastMessage,DateTime updatedAt,DateTime createdAt)
        {
            this.id = conversationId;
            this.isGroup = isGroup;
            this.typeGroup = typeGroup;
            this.avatarConversation = avatarConversation;
            this.adminId = adminId;
            this.shareGroupFromLinkOption = shareGroupFromLinkOption;
            this.browseMemberOption = browseMemberOption;
            this.pinMessage = pinMessage;
            this.memberList = memberList;
            this.messageList = messageList;
            this.browseMemberList = browseMemberList;
            this.timeLastMessage = timeLastMessage;
            this.updatedAt = updatedAt;
            this.createdAt = createdAt;
        }
        [BsonElement("_id")]
        public int id { get; set; }
        public int isGroup { get; set; }
        public string typeGroup { get; set; }
        public string avatarConversation { get; set; }
        public int adminId { get; set; }
        public int shareGroupFromLinkOption { get; set; }
        public int browseMemberOption { get; set; }
        public string pinMessage { get; set; }
        public List<ParticipantsDB> memberList { get; set; }
        public List<MessagesDB> messageList { get; set; }
        public List<BrowseMembersDB> browseMemberList { get; set; }
        public DateTime timeLastMessage { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime createdAt { get; set; }
    }
}

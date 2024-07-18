using APIChat365.Model.MongoEntity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chat365.Server.Model.Entity
{
    public class Conversation
    {

        public Conversation()
        { }
        public Conversation(int conversationId, string conversationName, string avatarConversation, int unreader, int isGroup, int senderId, string message, string messageType, DateTime createAt, Int64 messageDisplay, string typegroup, int shareLinkGroup, int browseMember, int notification, int isFavorite, int isHidden, string pinMessageId, int deleteTime,int deleteType, int adminId, string messageId)
        {
            this.conversationId = conversationId;
            this.conversationName = conversationName;
            this.avatarConversation = avatarConversation;
            this.unReader = unreader;
            this.isGroup = isGroup;
            this.senderId = senderId;
            this.message = message;
            this.messageType = messageType;
            this.createAt = createAt;
            this.countMessage = countMessage;
            this.messageDisplay = messageDisplay;
            this.typeGroup = typegroup;
            this.adminId = adminId;
            this.shareGroupFromLink = shareLinkGroup;
            this.browseMember = browseMember;
            this.isFavorite = isFavorite;
            this.isHidden = isHidden;
            this.notification = notification;
            this.pinMessageId = pinMessageId;
            this.DeleteTime = deleteTime;
            this.DeleteType = deleteType;
            this.messageId = messageId;
        }
        public Conversation(ConversationsDB c, int userId)
        {
            this.conversationId = c.id;
            if (c.memberList.Count>0)
            {
                this.conversationName = c.memberList[0].conversationName;
                this.unReader = c.memberList[0].unReader;
                this.messageDisplay = c.memberList[0].messageDisplay;
            }
            if (c.messageList.Count>0)
            {
                this.senderId = c.messageList[0].senderId;
                this.message = c.messageList[0].message;
                this.messageType = c.messageList[0].messageType;
                this.createAt = c.messageList[0].createAt;
            }

            this.avatarConversation = c.avatarConversation;
            this.isGroup = c.isGroup;
            this.countMessage = c.messageList.Count;
            this.typeGroup = c.typeGroup;
            this.adminId = c.adminId;
            this.shareGroupFromLink = c.shareGroupFromLinkOption;
            this.browseMember = c.browseMemberOption;
            this.isFavorite = c.memberList.Where(x => x.memberId == userId).ToList()[0].isFavorite;
            this.isHidden = c.memberList.Where(x => x.memberId == userId).ToList()[0].isHidden;
            this.notification = c.memberList.Where(x => x.memberId == userId).ToList()[0].notification;
            this.pinMessageId = c.pinMessage;
            this.DeleteTime= c.memberList.Where(x => x.memberId == userId).ToList()[0].deleteTime;
            this.DeleteType= c.memberList.Where(x => x.memberId == userId).ToList()[0].deleteTime;
        }

        public int conversationId { get; set; }
        public int companyId { get; set; }
        public String conversationName { get; set; }
        public String avatarConversation { get; set; }
        public int unReader { get; set; }
        public int isGroup { get; set; }
        public int senderId { get; set; }
        public String pinMessageId { get; set; }
        public String messageId { get; set; }
        public String message { get; set; }
        public String messageType { get; set; }
        public DateTime createAt { get; set; }
        public int countMessage { get; set; }
        public Int64 messageDisplay { get; set; }
        public string typeGroup { get; set; }
        public int adminId { get; set; }
        public int shareGroupFromLink { get; set; }
        public string memberList { get; set; }
        public int browseMember { get; set; }
        public int isFavorite { get; set; }
        public int notification { get; set; }
        public int isHidden { get; set; }
        public int DeleteTime { get; set; }
        public int DeleteType { get; set; }
        public int listMess { get; set; }
        public String LinkAvatar { get; set; }
        public List<BrowseMember> listBrowerMember { get; set; }
        public List<MemberConversation> listMember { get; set; }
        public List<Messages> listMessage { get; set; }
    }

    public class ConversationInSearch
    {
        public ConversationInSearch(int conversationId, string conversationName, string avatarConversation, int isGroup, string status)
        {
            this.conversationId = conversationId;
            this.conversationName = conversationName;
            this.avatarConversation = avatarConversation;
            this.status = status;
            this.isGroup = isGroup;
        }
        //public ConversationInSearch(ConversationsDB c)
        //{
        //    this.conversationId = c.id;
        //    this.conversationName = c.memberList[0].conversationName;
        //    this.avatarConversation = c.avatarConversation;
        //    this.status = u.status;
        //    this.isGroup = c.isGroup;
        //}
        public int conversationId { get; set; }
        public String conversationName { get; set; }
        public String avatarConversation { get; set; }
        public int isGroup { get; set; }
        public String status { get; set; }
        public String LinkAvatar { get; set; }
    }

    class CompareByTimeLastMessage : IComparer<Conversation>
    {
        public int Compare(Conversation x, Conversation y)
        {
            return y.createAt.CompareTo(x.createAt);
        }
    }

    public class MemberConversation
    {
        public MemberConversation(int id, string userName, string avatarUser, string status, int statusEmotion, string lastActive, int active, int isOnline, int unReader, int companyId, DateTime timeLastSeener, int idTimViec, int type365, string friendStatus)
        {
            ID = id;
            UserName = userName;
            AvatarUser = avatarUser;
            Status = status;
            Active = active;
            this.isOnline = isOnline;
            UnReader = unReader;
            StatusEmotion = statusEmotion;
            LastActive = lastActive;
            CompanyId = companyId;
            TimeLastSeener = timeLastSeener;
            IdTimViec = idTimViec;
            Type365 = type365;
            FriendStatus = friendStatus;
        }

        public int ID { get; set; }
        public string UserName { get; set; }
        public string AvatarUser { get; set; }
        public string Status { get; set; }
        public int Active { get; set; }
        public int isOnline { get; set; }
        public int StatusEmotion { get; set; }
        public string LastActive { get; set; }
        public int UnReader { get; set; }
        public string LinkAvatar { get; set; }
        public int CompanyId { get; set; }
        public DateTime TimeLastSeener { get; set; }
        public int IdTimViec { get; set; }
        public int Type365 { get; set; }
        public string FriendStatus { get; set; }
        public InfoLiveChat LiveChat { get; set; }
    }

    public class ConversationForward
    {
        public ConversationForward() { }
        public ConversationForward(int conversationId, string conversationName, string avatarConversation, int isGroup, string status)
        {
            this.conversationId = conversationId;
            this.conversationName = conversationName;
            this.avatarConversation = avatarConversation;
            this.isGroup = isGroup;
            this.status = status;
        }
        public int conversationId { get; set; }
        public string avatarConversation { get; set; }
        public string conversationName { get; set; }
        public int isGroup { get; set; }
        public string status { get; set; }
        public int contactId { get; set; }
    }
    public class BrowseMember
    {
        public BrowseMember()
        {
        }

        public BrowseMember(User userMember, int memberAddId)
        {
            UserMember = userMember;
            MemberAddId = memberAddId;
        }

        public User UserMember { get; set; }
        public int MemberAddId { get; set; }
    }

    public class infoConversation
    {
        public infoConversation()
        {
        }

        public infoConversation(int memberId, string email, int type365, string userName, int isOnline, string conversationName, int isGroup)
        {
            this.memberId = memberId;
            this.email = email;
            this.type365 = type365;
            this.userName = userName;
            this.isOnline = isOnline;
            this.conversationName = conversationName;
            this.isGroup = isGroup;
        }

        public int memberId { get; set; }
        public string email { get; set; }
        public int type365 { get; set; }
        public string userName { get; set; }
        public int isOnline { get; set; }
        public string conversationName { get; set; }
        public int isGroup { get; set; }
    }

}


using System;
using System.Collections.Generic;
using System.Data;
using Chat365.Server.Model.Entity;
using Chat365.Server.Model.DAO;

namespace Chat365.Server.Model.Entity

{
    public class ContactCompany
    {
        public ContactCompany()
        {
        }

        public ContactCompany(int id, string userName, string email, string avatarUser, string status, int active, int isOnline, int looker, int statusEmotion, DateTime lastActive, int companyId, string friendStatus = "none")
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
        }

        public int ID { get; set; }
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
    }
    public class Contact
    {
        public Contact()
        {
        }

        public Contact(int id, string userName, string email, string avatarUser, string status, int active, int isOnline, int looker, int statusEmotion, DateTime lastActive, int companyId,int type365, string friendStatus = "none")
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
        }

        public int ID { get; set; }
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
    public class User : Contact
    {
        public User() : base()
        {
        }

        public User(int id, int id365, int idTimViec, int type365, string email, string password, string phone, string userName, string avatarUser, string status, int statusEmotion, DateTime lastActive, int active, int isOnline, int looker, int companyId, string companyName, int notificationPayoff, int notificationCalendar, int notificationReport, int notificationOffer, int notificationPersonnelChange, int notificationRewardDiscipline, int notificationNewPersonnel, int notificationTransferAsset, int notificationChangeProfile, int notificationMissMessage, int notificationCommentFromTimViec, int notificationCommentFromRaoNhanh, int notificationTag, int notificationSendCandidate, int notificationChangeSalary, int notificationAllocationRecall, int notificationAcceptOffer, int notificationDecilineOffer, int notificationNTDPoint, int notificationNTDExpiredPin, int notificationNTDExpiredRecruit, string fromWeb, int notificationNTDApplying) : base(id, email, userName, avatarUser, status, active, isOnline, looker, statusEmotion, lastActive, companyId, type365)
        {
            ID = id;
            Email = email;
            Password = password;
            Phone = phone;
            UserName = userName;
            AvatarUser = avatarUser;
            Status = status;
            Active = active;
            this.isOnline = isOnline;
            Looker = looker;
            StatusEmotion = statusEmotion;
            LastActive = lastActive;
            CompanyId = companyId;
            NotificationCalendar = notificationCalendar;
            NotificationPayoff = notificationPayoff;
            NotificationReport = notificationReport;
            NotificationOffer = notificationOffer;
            NotificationPersonnelChange = notificationPersonnelChange;
            NotificationRewardDiscipline = notificationRewardDiscipline;
            NotificationNewPersonnel = notificationNewPersonnel;
            NotificationChangeProfile = notificationChangeProfile;
            NotificationTransferAsset = notificationTransferAsset;
            CompanyName = companyName;
            ID365 = id365;
            Type365 = type365;
            IDTimViec = idTimViec;
            NotificationMissMessage = notificationMissMessage;
            NotificationCommentFromTimViec = notificationCommentFromTimViec;
            NotificationCommentFromRaoNhanh = notificationCommentFromRaoNhanh;
            NotificationTag = notificationTag;
            NotificationSendCandidate = notificationSendCandidate;
            NotificationChangeSalary = notificationChangeSalary;
            NotificationAllocationRecall = notificationAllocationRecall;
            NotificationAcceptOffer = notificationAcceptOffer;
            NotificationDecilineOffer = notificationDecilineOffer;
            NotificationNTDPoint = notificationNTDPoint;
            NotificationNTDExpiredPin = notificationNTDExpiredPin;
            NotificationNTDExpiredRecruit = notificationNTDExpiredRecruit;
            FromWeb = fromWeb;
            NotificationNTDApplying = notificationNTDApplying;
        }

        public User(int id, int id365, int idTimViec, int type365, string email, string password, string phone, string userName, string avatarUser, string status, int statusEmotion, DateTime lastActive, int active, int isOnline, int looker, int companyId, string companyName, string fromWeb) : base(id, email, userName, avatarUser, status, active, isOnline, looker, statusEmotion, lastActive, companyId, type365)
        {
            ID = id;
            Email = email;
            Password = password;
            Phone = phone;
            UserName = userName;
            AvatarUser = avatarUser;
            Status = status;
            Active = active;
            this.isOnline = isOnline;
            Looker = looker;
            StatusEmotion = statusEmotion;
            LastActive = lastActive;
            CompanyId = companyId;
            CompanyName = companyName;
            ID365 = id365;
            Type365 = type365;
            IDTimViec = idTimViec;
            FromWeb = fromWeb;
        }

        public int ID365 { get; set; }
        public int IDTimViec { get; set; }
        public int Type365 { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int NotificationPayoff { get; set; }
        public int NotificationCalendar { get; set; }
        public int NotificationReport { get; set; }
        public int NotificationOffer { get; set; }
        public int NotificationPersonnelChange { get; set; }
        public int NotificationRewardDiscipline { get; set; }
        public int NotificationNewPersonnel { get; set; }
        public int NotificationChangeProfile { get; set; }
        public int NotificationTransferAsset { get; set; }
        public int AcceptMessStranger { get; set; }
        public int Type_Pass { get; set; }
        public string CompanyName { get; set; }
        public string secretCode { get; set; }
        public int NotificationMissMessage { get; set; }
        public int NotificationCommentFromTimViec { get; set; }
        public int NotificationCommentFromRaoNhanh { get; set; }
        public int NotificationTag { get; set; }
        public int NotificationSendCandidate { get; set; }
        public int NotificationChangeSalary { get; set; }
        public int NotificationAllocationRecall { get; set; }
        public int NotificationAcceptOffer { get; set; }
        public int NotificationDecilineOffer { get; set; }
        public int NotificationNTDPoint { get; set; }
        public int NotificationNTDExpiredPin { get; set; }
        public int NotificationNTDExpiredRecruit { get; set; }
        public string FromWeb { get; set; }
        public int NotificationNTDApplying { get; set; }
        public string userQr { get; set; }
    }

    public class RequestContact
    {
        public RequestContact()
        {
        }

        public RequestContact(int userId, int contactId, string status, int type365)
        {
            this.userId = userId;
            this.contactId = contactId;
            this.status = status;
            this.type365 = type365;
        }

        public int userId { get; set; }
        public int contactId { get; set; }
        public string status { get; set; }
        public int type365 { get; set; }
    }

    public class RequestContact1
    {
        public RequestContact1()
        {
        }

        public RequestContact1(int contactId, string contactName, string contactAvatar, int type365)
        {
            this.contactId = contactId;
            this.contactName = contactName;
            this.contactAvatar = contactAvatar;
            this.type365 = type365;
        }

        public int contactId { get; set; }
        public string contactName { get; set; }
        public string contactAvatar { get; set; }
        public int type365 { get; set; }
    }

    public class RequestFriend
    {
        public RequestFriend()
        {
        }

        public RequestFriend(int id, string userName, string avatar, string status, int type365)
        {
            this.id = id;
            this.userName = userName;
            this.avatar = avatar;
            this.status = status;
            this.type365 = type365;
        }

        public int id { get; set; }
        public string userName { get; set; }
        public string avatar { get; set; }
        public string status { get; set; }
        public int type365 { get; set; }
    }

    public class FriendRequest
    {
        public FriendRequest(int id, int id365, string userName, string avatarUser, int isOnline, int type365)
        {
            this.id = id;
            this.id365 = id365;
            this.userName = userName;
            this.avatarUser = avatarUser;
            this.isOnline = isOnline;
            this.type365 = type365;
        }
        public int id { get; set; }
        public int id365 { get; set; }
        public string userName { get; set; }
        public string avatarUser { get; set; }
        public int isOnline { get; set; }
        public int type365 { get; set; }
    }

    public class CompareUserByName : IComparer<User>
    {
        public int Compare(User x, User y)
        {
            return x.UserName.CompareTo(y.UserName);
        }
    }

    public class CompareContactByName : IComparer<Contact>
    {
        public int Compare(Contact x, Contact y)
        {
            return x.UserName.CompareTo(y.UserName);
        }
    }
}

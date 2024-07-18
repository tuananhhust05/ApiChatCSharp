using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIChat365.Model.DAO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Linq;
using VisioForge.Libs.DirectShowLib;

namespace APIChat365.Model.MongoEntity
{
    public class UserDB
    {
        public UserDB() { }

        public UserDB(int id, int id365, int type365, string email, string password, string phone, string userName, string avatarUser, string status, int statusEmotion, DateTime lastActive, int active, int isOnline, int looker, int companyId, string companyName, int notificationPayoff, int notificationCalendar, int notificationReport, int notificationOffer, int notificationPersonnelChange, int notificationRewardDiscipline, int notificationNewPersonnel, int notificationChangeProfile, int notificationTransferAsset, int acceptMessStranger, int idTimViec,int notificationMissMessage, int notificationCommentFromTimViec, int notificationCommentFromRaoNhanh, int notificationTag, int notificationSendCandidate, int notificationChangeSalary, int notificationAllocationRecall, int notificationAcceptOffer, int notificationDecilineOffer, int notificationNTDPoint, int notificationNTDExpiredPin, int notificationNTDExpiredRecruit, List<HistoryAccessDB> historyAccess, double latitude, double longtitude, List<int> removeSugges, int notificationNTDApplying, string userNameNoVn)
        {
            this.id = id;
            this.id365 = id365;
            this.type365 = type365;
            this.email = email;
            this.password = password;
            this.phone = phone;
            this.userName = userName;
            this.avatarUser = avatarUser;
            this.status = status;
            this.statusEmotion = statusEmotion;
            this.lastActive = lastActive;
            this.active = active;
            this.isOnline = isOnline;
            this.looker = looker;
            this.companyId = companyId;
            this.companyName = companyName;
            this.notificationPayoff = notificationPayoff;
            this.notificationCalendar = notificationCalendar;
            this.notificationReport = notificationReport;
            this.notificationOffer = notificationOffer;
            this.notificationPersonnelChange = notificationPersonnelChange;
            this.notificationRewardDiscipline = notificationRewardDiscipline;
            this.notificationNewPersonnel = notificationNewPersonnel;
            this.notificationChangeProfile = notificationChangeProfile;
            this.notificationTransferAsset = notificationTransferAsset;
            this.acceptMessStranger = acceptMessStranger;
            this.notificationMissMessage = notificationMissMessage;
            this.notificationCommentFromTimViec = notificationCommentFromTimViec;
            this.notificationCommentFromRaoNhanh = notificationCommentFromRaoNhanh;
            this.notificationTag = notificationTag;
            this.notificationSendCandidate = notificationSendCandidate;
            this.notificationChangeSalary = notificationChangeSalary;
            this.notificationAllocationRecall = notificationAllocationRecall;
            this.notificationAcceptOffer = notificationAcceptOffer;
            this.notificationDecilineOffer = notificationDecilineOffer;
            this.notificationNTDPoint = notificationNTDPoint;
            this.notificationNTDExpiredPin = notificationNTDExpiredPin;
            this.notificationNTDExpiredRecruit = notificationNTDExpiredRecruit;
            this.idTimViec = idTimViec;
            HistoryAccess = historyAccess;
            this.latitude = latitude;
            this.longtitude = longtitude;
            this.removeSugges = removeSugges;
            this.notificationNTDApplying = notificationNTDApplying;
            this.userNameNoVn = userNameNoVn;
        }

        [BsonElement("_id")]
        public int id { get; set; }
        [BsonElement("id365")]
        public int id365 { get; set; }
        [BsonElement("type365")]
        public int type365 { get; set; }
        [BsonElement("email")]
        public string email { get; set; }
        [BsonElement("password")]
        public string password { get; set; }
        [BsonElement("phone")]
        public string phone { get; set; }
        [BsonElement("userName")]
        public string userName { get; set; }
        [BsonElement("avatarUser")]
        public string avatarUser { get; set; }
        [BsonElement("status")]
        public string status { get; set; }
        [BsonElement("statusEmotion")]
        public int statusEmotion { get; set; }
        [BsonElement("lastActive")]
        public DateTime lastActive { get; set; }
        [BsonElement("active")]
        public int active { get; set; }
        [BsonElement("isOnline")]
        public int isOnline { get; set; }
        [BsonElement("looker")]
        public int looker { get; set; }
        [BsonElement("companyId")]
        public int companyId { get; set; }
        [BsonElement("companyName")]
        public string companyName { get; set; }
        [BsonElement("notificationPayoff")]
        public int notificationPayoff { get; set; }
        [BsonElement("notificationCalendar")]
        public int notificationCalendar { get; set; }
        [BsonElement("notificationReport")]
        public int notificationReport { get; set; }
        [BsonElement("notificationOffer")]
        public int notificationOffer { get; set; }
        [BsonElement("notificationPersonnelChange")]
        public int notificationPersonnelChange { get; set; }
        [BsonElement("notificationRewardDiscipline")]
        public int notificationRewardDiscipline { get; set; }
        [BsonElement("notificationNewPersonnel")]
        public int notificationNewPersonnel { get; set; }
        [BsonElement("notificationChangeProfile")]
        public int notificationChangeProfile { get; set; }
        [BsonElement("notificationTransferAsset")]
        public int notificationTransferAsset { get; set; }
        [BsonElement("acceptMessStranger")]
        public int acceptMessStranger { get; set; }
        [BsonElement("fromWeb")]
        public string fromWeb { get; set; }
        [BsonElement("secretCode")]
        public string secretCode { get; set; }
        public int idTimViec { get; set; }
        public int notificationMissMessage { get; set; }
        public int notificationCommentFromTimViec { get; set; }
        public int notificationCommentFromRaoNhanh { get; set; }
        public int notificationTag { get; set; }
        public int notificationSendCandidate { get; set; }
        public int notificationChangeSalary { get; set; }
        public int notificationAllocationRecall { get; set; }
        public int notificationAcceptOffer { get; set; }
        public int notificationDecilineOffer { get; set; }
        public int notificationNTDPoint { get; set; }
        public int notificationNTDExpiredPin { get; set; }
        public int notificationNTDExpiredRecruit { get; set; }
        public List<HistoryAccessDB> HistoryAccess { get; set; }
        public double latitude { get; set; }
        public double longtitude { get; set; }
        public List<int> removeSugges { get; set; }
        public int notificationNTDApplying { get; set; }
        public string userNameNoVn { get; set; }
    }
}

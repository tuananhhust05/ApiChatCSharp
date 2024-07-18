using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using static APIChat365.Controllers.RunToolController;

namespace APIChat365.Model.DAO
{
    public class DaoTool
    {
        public static int RunLinkNoti()
        {
            //FilterDefinition<NotificationsDB> filter = Builders<NotificationsDB>.Filter.Where(x => true);
            //UpdateDefinition<NotificationsDB> update = Builders<NotificationsDB>.Update.Set("link", "");
            //var check = ConnectDB.database.GetCollection<NotificationsDB>("Notifications").UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RunAddNoti()
        {
            //FilterDefinition<UserDB> filter = Builders<UserDB>.Filter.Where(x => true);
            //UpdateDefinition<UserDB> update = Builders<UserDB>.Update.Set("notificationMissMessage",1).Set("notificationCommentFromTimViec", 1).Set("notificationCommentFromRaoNhanh", 1).Set("notificationTag", 1).Set("notificationSendCandidate", 1).Set("notificationChangeSalary", 1).Set("notificationAllocationRecall", 1).Set("notificationAcceptOffer", 1).Set("notificationDecilineOffer", 1).Set("notificationNTDPoint", 1).Set("notificationNTDExpiredPin", 1).Set("notificationNTDExpiredRecruit", 1);
            //var check = ConnectDB.database.GetCollection<UserDB>("Users").UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RunDeleteTime()
        {
            var c = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => true).ToList();
            foreach (var item in c)
            {
                //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == item.id);
                //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("messageList.$[].deleteTime", item.typeGroup == "Secret" ? 10 : 0);
                //var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            }
            return c.Count;
        }
        public static int RunDeleteTimeMember()
        {
            var c = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => true).ToList();
            //foreach (var item in c)
            //{
            //    //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == item.id);
            //    //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("memberList.$[].deleteTime", item.typeGroup == "Secret" ? 10 : 0);
            //    //var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            //}
            return c.Count;
        }
        public static int RunDeleteType()
        {
            var c = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => true).ToList();
            foreach (var item in c)
            {
                //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == item.id);
                //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("messageList.$[].deleteType", item.typeGroup == "Secret" ? 1 : 0);
                //var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            }
            return c.Count;
        }
        public static int RunDeleteTypeMember()
        {
            var c = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => true).ToList();
            foreach (var item in c)
            {
                //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == item.id);
                //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("memberList.$[].deleteType", item.typeGroup == "Secret" ? 1 : 0);
                //var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            }
            return c.Count;
        }
        public static int RunDeleteDate()
        {
            //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => true);
            //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("messageList.$[].deleteDate", DateTime.MinValue);
            //var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        ////thêm trường gán thẻ vào cuộc trò chuyện
        public static int RunTagClassify()
        {
            //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => true);
            //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("messageList.$[].deleteDate", DateTime.MinValue);
            //var check = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RunfavoriteMessage()
        {
            //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => true);
            //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("memberList.$[].favoriteMessage", new List<string>());
            //var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RunAddUserNode()
        {
            //FilterDefinition<UserDB> filter = Builders<UserDB>.Filter.Where(x => true);
            //UpdateDefinition<UserDB> update = Builders<UserDB>.Update.Set("HistoryAccess",new List<HistoryAccessDB>()).Set("latitude",0).Set("longtitude", 0).Set("removeSugges",new List<int>());
            //var check = ConnectDB.database.GetCollection<UserDB>("Users").UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RunNotiApplying()
        {
            //FilterDefinition<UserDB> filter = Builders<UserDB>.Filter.Where(x => true);
            //UpdateDefinition<UserDB> update = Builders<UserDB>.Update.Set("notificationNTDApplying",1);
            //var check = ConnectDB.database.GetCollection<UserDB>("Users").UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }

        public static int RunLiveChat()
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => true);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("memberList.$[].liveChat", BsonNull.Value);
            var check = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RunInfoSupport()
        {
            //FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => true);
            //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("messageList.$[].infoSupport", BsonNull.Value);
            //.Set("messageList.$[].liveChat", BsonNull.Value)
            //UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Unset("messageList.$[].infoSupport");
            //var check = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int OldInfoSupport()
        {
            //var filter = Builders<ConversationsDB>.Filter.Where(x => true);
            //var update = Builders<ConversationsDB>.Update.PullFilter(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.messageType, "support"));
            //var ck = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateMany(filter, update);
            //if (ck.ModifiedCount > 0) return 1;
            //else
            return 0;
        }

        public static int RunFixLiveChat()
        {
            //ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.typeGroup == "liveChat").ToList().ForEach(c =>
            //{
            //    foreach (var item in c.messageList)
            //    {
            //        int index = c.memberList.FindIndex(m => m.memberId == item.senderId);
            //        if (index > -1)
            //        {
            //            var member = c.memberList[index];
            //            if (member.liveChat != null && !string.IsNullOrEmpty(member.liveChat.clientId))
            //            {
            //                var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, item.id));
            //                UpdateDefinition<ConversationsDB> update;
            //                update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].liveChat, new LiveChatDB(member.liveChat.clientId, member.liveChat.clientName, member.liveChat.fromWeb));
            //                var ck = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateOne(filter, update);
            //            }
            //        }
            //    }
            //});
            return 1;
        }

        public static int RunAddNonUnicodeName()
        {
            //FilterDefinition<UserDB> filter = Builders<UserDB>.Filter.Where(x => true);
            //UpdateDefinition<UserDB> update = Builders<UserDB>.Update.Set("userNameNoVn", "");
            //var check = ConnectDB.database.GetCollection<UserDB>("Users").UpdateMany(filter, update);
            //if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int[] RunDupMember()
        {
            List<int> result = new List<int>();
            var convers = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.isGroup == 1 && x.memberList != null && x.memberList.Count > 2).ToList();
            foreach (ConversationsDB c in convers)
            {
                if (c.memberList.GroupBy(x => x.memberId).Any(g => g.Count() > 1))
                {
                    if (!result.Contains(c.id)) result.Add(c.id);
                    List<int> member = new List<int>();
                    foreach (var item in c.memberList)
                    {
                        if (!member.Contains(item.memberId)) member.Add(item.memberId);
                        else DAOConversation.OutGroup(c.id, item.memberId);
                    }
                }
            }
            return result.ToArray();
        }
        public static int RunClicked()
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => true);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("messageList.$[].notiClicked", new List<int>());
            var check = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RunFrom()
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => true);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("messageList.$[].from", "");
            var check = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
    }
}

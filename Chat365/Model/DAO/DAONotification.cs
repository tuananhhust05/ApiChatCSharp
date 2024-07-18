using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using Chat365.Server.Model.EntityAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Model.DAO
{
    public class DAONotification
    {
        public static string tableNotifications = "Notifications";
        //nhập vào thông báo
        public static int InsertNotification(string id, int userId, int paticipantId, string title, string message, string type, string mesageId, int conversationId, DateTime createAt, string link)
        {

            var tg = ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).Find(x => x.userId == userId).ToList().OrderByDescending(x => x.createAt).ToList();
            if (tg.Count >= 20)
            {
                ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).DeleteOne(x => x.userId == userId);
            }
            try
            {
                var document = new NotificationsDB(id, userId, paticipantId, title, message, 1, createAt, type, mesageId, conversationId,link);
                ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).InsertOne(document);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //lấy danh sách thông báo 
        public static List<NotificationsDB> GetListNotification(int userId)
        {
            return ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).Find(x => x.userId == userId).SortByDescending(x => x.createAt).ToList();
        }

        public static bool CheckSpamNotification(int userId, int paticipantId, string title, string message, DateTime createAt)
        {
            createAt = createAt.AddMinutes(-10);
            var tg = ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).Find(x => x.userId == userId && x.paticipantId == paticipantId && x.title == title && x.message == message && x.createAt > createAt).ToList();
            if (tg.Count == 0)
            {
                return true;
            }
            else return false;
        }
        //xoá thông báo theo userid
        public static int DeleteAllNotification(int userId)
        {
            try
            {
                DeleteResult check = ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).DeleteMany(x => x.userId == userId);
                if (check.DeletedCount > 0) return 1;
                else return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //xoá thông báo theo id
        public static int DeleteNotification(string id)
        {
            try
            {
                DeleteResult check = ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).DeleteOne(x => x.id == id);
                if (check.DeletedCount > 0) return 1;
                else return 0;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        //cập nhật đã xem cho thông báo theo id
        public static int ReadNotification(string id)
        {
            var filter = Builders<NotificationsDB>.Filter.Eq("_id", id);
            var update = Builders<NotificationsDB>.Update.Set("isUndeader", 0);
            var check = ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).UpdateOne(filter, update);
            if (check.ModifiedCount > 0)
            {
                return 1;
            }
            else return 0;
        }
        //cập nhật đã xem cho thông báo theo userid
        public static int ReadAllNotification(int userId)
        {
            var filter = Builders<NotificationsDB>.Filter.Eq("userId", userId);
            var update = Builders<NotificationsDB>.Update.Set("isUndeader", 0);
            var check = ConnectDB.database.GetCollection<NotificationsDB>(tableNotifications).UpdateMany(filter, update);
            if (check.ModifiedCount > 0)
            {
                return 1;
            }
            else return 0;
        }
    }
}

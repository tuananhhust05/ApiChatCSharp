using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace APIChat365.Model.DAO
{
    public class DAOComment
    {

        public static string tableComment = "Comment";
        public static int InsertCmt(string url, int id_user_url, int parent_id, string comment, int id_user_comment, int ip_comment, int type, string id_user_like)
        {
            try
            {
                ConnectDB.database.GetCollection<CommentDB>(tableComment).InsertOne(new CommentDB() { id = DAOCounter.getNextID("CommentID"), url = url, id_user_url = id_user_url, parent_id = parent_id, comment = comment, id_user_comment = id_user_comment, ip_comment = ip_comment, time_comment = DateTime.Now, type = type, id_user_like = id_user_like }); ;
                DAOCounter.updateID("CommentID");
                return 1;
            }
            catch 
            {
                return 0;
            }
        }
        public static int DeleteCmt(int id)
        {
            ConnectDB.database.GetCollection<CommentDB>(tableComment).DeleteOne(x => x.id == id);
            return 1;
        }
        public static int UpdateCmt(int id, string url, int id_user_url, int parent_id, string comment, int id_user_comment, int ip_comment, int type, string id_user_like)
        {
            List<CommentDB> list = getTestById(id);
            if (list.Count>0)
            {
                CommentDB cmt = list[0];
                FilterDefinition<CommentDB> filter = Builders<CommentDB>.Filter.Eq("_id", id);
                UpdateDefinition<CommentDB> update = Builders<CommentDB>.Update.Set("time_comment", DateTime.Now).Set("url", !string.IsNullOrEmpty(url) ? url : cmt.url).Set("id_user_url", id_user_url > 0 ? id_user_url : cmt.id_user_url)
                    .Set("parent_id", parent_id > 0 ? parent_id : cmt.parent_id).Set("comment", !string.IsNullOrEmpty(comment) ? comment : cmt.comment).Set("id_user_comment", id_user_comment > 0 ? id_user_comment : cmt.id_user_comment)
                    .Set("id_user_comment", id_user_comment > 0 ? id_user_comment : cmt.id_user_comment).Set("ip_comment", ip_comment > 0 ? ip_comment : cmt.ip_comment).Set("type", type > 0 ? type : cmt.type)
                    .Set("id_user_like", !string.IsNullOrEmpty(id_user_like) ? id_user_like : cmt.id_user_like);
                var check = ConnectDB.database.GetCollection<CommentDB>(tableComment).UpdateMany(filter, update);
                if (check.ModifiedCount > 0) return 1;
            }
            return 0;
        }
        public static List<CommentDB> getTestById(int id)
        {
            return ConnectDB.database.GetCollection<CommentDB>(tableComment).Find(x => x.id == id).ToList();
        }
        public static List<CommentDB> getListCommentByUrl(string url, int skip, int limit, string order)
        {
            List<CommentDB> list = new List<CommentDB>();
            if (order == "DES")
            {
                list = ConnectDB.database.GetCollection<CommentDB>(tableComment).Find(x => x.url == url).Skip(skip).Limit(limit).SortByDescending(x => x.time_comment).ToList();
            }
            else if (order == "ASC")
            {
                list = ConnectDB.database.GetCollection<CommentDB>(tableComment).Find(x => x.url == url).Skip(skip).Limit(limit).SortBy(x => x.time_comment).ToList();
            }
            list.ForEach(x=>x.time_comment=x.time_comment.ToLocalTime());
            return list;
        }
        public static List<CommentDB> getListCommentByParent_id(int parent_id, int skip, int limit, string order)
        {
            List<CommentDB> list = new List<CommentDB>();
            if (order == "DES")
            {
                list = ConnectDB.database.GetCollection<CommentDB>(tableComment).Find(x => x.parent_id == parent_id).Skip(skip).Limit(limit).SortByDescending(x => x.time_comment).ToList();
            }
            else if (order == "ASC")
            {
                list = ConnectDB.database.GetCollection<CommentDB>(tableComment).Find(x => x.parent_id == parent_id).Skip(skip).Limit(limit).SortBy(x => x.time_comment).ToList();
            }
            list.ForEach(x => x.time_comment = x.time_comment.ToLocalTime());
            return list;
        }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
namespace APIChat365.Model.MongoEntity
{
    public class CommentDB
    {
        public CommentDB()
        {
        }

        public CommentDB(int id, string url, int id_user_url, int parent_id, string comment, int id_user_comment, int ip_comment, DateTime time_comment, int type, string id_user_like)
        {
            this.id = id;
            this.url = url;
            this.id_user_url = id_user_url;
            this.parent_id = parent_id;
            this.comment = comment;
            this.id_user_comment = id_user_comment;
            this.ip_comment = ip_comment;
            this.time_comment = time_comment;
            this.type = type;
            this.id_user_like = id_user_like;
        }

        [BsonElement("_id")]
        public int id { get; set; }
        public string url { get; set; }
        public int id_user_url { get; set; }
        public int parent_id { get; set; }
        public string comment { get; set; }
        public int id_user_comment { get; set; }
        public int ip_comment { get; set; }
        public DateTime time_comment { get; set; }
        public int type { get; set; }
        public string id_user_like { get; set; }
    }

}

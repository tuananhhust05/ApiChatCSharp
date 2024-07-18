using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.MongoEntity
{
    public class ReportDB
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public int id { get; set; }
        public int userId { get; set; }
        public int reportId { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }
}

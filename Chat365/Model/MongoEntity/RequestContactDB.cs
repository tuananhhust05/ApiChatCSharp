using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.MongoEntity
{
    public class RequestContactDB
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public int userId { get; set; }
        public int contactId { get; set; }
        public string status { get; set; }
        public int type365 { get; set; }
    }
}

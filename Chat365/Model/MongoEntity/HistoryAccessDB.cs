using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace APIChat365.Model.MongoEntity
{
    public class HistoryAccessDB
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public string IdDevice { get; set; }
        public string IpAddress { get; set; }
        public string NameDevice { get; set; }
        public DateTime Time { get; set; }
        public bool AccessPermision { get; set; }
    }
}

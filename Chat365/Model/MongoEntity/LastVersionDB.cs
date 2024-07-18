using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace APIChat365.MongoEntity
{
    public class LastVersionDB
    {
        [BsonElement("from")]
        public string from { get; set; }

        [BsonElement("lastVersion")]
        public string lastVersion { get; set; }
    }
}

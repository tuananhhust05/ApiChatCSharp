using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APIChat365.Model.MongoEntity
{
    public class ContactsDB
    {

        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        public int userFist { get; set; }
        public int userSecond { get; set; }
    }
}

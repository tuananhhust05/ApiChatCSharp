using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APIChat365.Model.MongoEntity
{
    public class BlockListDB
    {
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int userId { get; set; }
        public int blockId { get; set; }
    }
}

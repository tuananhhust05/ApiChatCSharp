using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace APIChat365.Model.MongoEntity
{
    public class ErrorListDB
    {
        public ErrorListDB()
        {
        }

        public ErrorListDB(string messagesError, string createAt, string errorFrom)
        {
            this.messagesError = messagesError;
            this.createAt = createAt;
            this.errorFrom = errorFrom;
        }

        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("messagesError")]
        public string messagesError { get; set; }

        [BsonElement("createAt")]
        public string createAt { get; set; }

        [BsonElement("errorFrom")]
        public string errorFrom { get; set; }
    }
}

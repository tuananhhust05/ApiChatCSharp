using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace APIChat365.MongoEntity
{
    public class InfoLinkDB
    {
        public InfoLinkDB()
        {

        }
        public InfoLinkDB(string title, string description, string linkHome, string image, int isNotification)
        {
            this.title = title;
            this.description = description;
            this.linkHome = linkHome;
            this.image = image;
            this.isNotification = isNotification;
        }

        

        [BsonElement("title")]
        public string title { get; set; }

        [BsonElement("description")]
        public string description { get; set; }

        [BsonElement("linkHome")]
        public string linkHome { get; set; }

        [BsonElement("image")]
        public string image { get; set; }

        [BsonElement("isNotification")]
        public int isNotification { get; set; }
    }
}

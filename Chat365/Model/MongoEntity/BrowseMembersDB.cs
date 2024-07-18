using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APIChat365.Model.MongoEntity
{
    public class BrowseMembersDB
    {
        public int memberAddId { get; set; }
        public int memberBrowserId { get; set; }
    }
}

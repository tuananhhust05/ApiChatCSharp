using MongoDB.Bson.Serialization.Attributes;
using System;

namespace APIChat365.Model.MongoEntity
{
    public class TagClassifyDB
    {
        public TagClassifyDB()
        {
        }

        public TagClassifyDB(int id,string tagName, string tagColor, DateTime createTime)
        {
            this.id = id;
            TagName = tagName;
            TagColor = tagColor;
            CreateTime = createTime;
        }

        [BsonElement("_id")]
        public int id { get; set; }
        public string TagName { get; set; }
        public string TagColor { get; set; }
        public DateTime CreateTime { get; set; }

    }
}

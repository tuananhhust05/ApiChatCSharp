using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace APIChat365.Model.DAO
{
    public class DAOTagClassify
    {
        public static string tableTagClassify = "TagClassify";

        public static int InsertTagClassify(string TagName, string TagColor)
        {
            try
            {
                ConnectDB.database.GetCollection<TagClassifyDB>(tableTagClassify).InsertOne(new TagClassifyDB() { id = DAOCounter.getNextID("TagClassifyID"), TagName = TagName, TagColor = TagColor, CreateTime = DateTime.Now}); ;
                DAOCounter.updateID("TagClassifyID");
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public static int DeleteTagClassify(int id)
        {
            ConnectDB.database.GetCollection<TagClassifyDB>(tableTagClassify).DeleteOne(x => x.id == id);
            return 1;
        }
        public static int UpdateTagClassify(int id, string TagName, string TagColor)
        {
            List<TagClassifyDB> list = getTagById(id);
            if (list.Count > 0)
            {
                TagClassifyDB TagClassify = list[0];
                FilterDefinition<TagClassifyDB> filter = Builders<TagClassifyDB>.Filter.Eq("_id", id);
                UpdateDefinition<TagClassifyDB> update = Builders<TagClassifyDB>.Update.Set("CreateTime", DateTime.Now).Set("TagName", !string.IsNullOrEmpty(TagName) ? TagName : TagClassify.TagName).Set("TagColor", !string.IsNullOrEmpty(TagColor) ? TagColor : TagClassify.TagColor);
                var check = ConnectDB.database.GetCollection<TagClassifyDB>(tableTagClassify).UpdateMany(filter, update);
                if (check.ModifiedCount > 0) return 1;
            }
            return 0;
        }
        public static List<TagClassifyDB> getTagById(int id)
        {
            return ConnectDB.database.GetCollection<TagClassifyDB>(tableTagClassify).Find(x => x.id == id).ToList();
        }
    }
}

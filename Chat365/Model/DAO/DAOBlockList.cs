using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.DAO
{
    public class DAOBlockList
    {
        private static string tblBlockList = "blockList";

        public static List<BlockListDB> getList()
        {
            return ConnectDB.database.GetCollection<BlockListDB>(tblBlockList).Find(_ => true).ToList();
        }
        public static List<BlockListDB> getTestById(string id)
        {
            return ConnectDB.database.GetCollection<BlockListDB>(tblBlockList).Find(x => x.Id == id).ToList();
        }
        public static int Insert(int userID,int blockID)
        {
            ConnectDB.database.GetCollection<BlockListDB>(tblBlockList).InsertOne(new BlockListDB() { userId = userID, blockId = blockID });;
            return 0;
        }
        public static int Update(string id, int userId, int blockId)
        {
            var filter = Builders<BlockListDB>.Filter.Eq("id", id);
            var update = Builders<BlockListDB>.Update.Set("userId", userId.ToString());
            update.Set("blockId", blockId.ToString());
            ConnectDB.database.GetCollection<BlockListDB>(tblBlockList).UpdateOne(filter, update);
            return 0;
        }
        public static int Delete(string id)
        {
            ConnectDB.database.GetCollection<BlockListDB>(tblBlockList).DeleteOne(x => x.Id == id);
            return 0;
        }
    }
}

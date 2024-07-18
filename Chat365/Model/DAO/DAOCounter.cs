using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Chat365.Server.Model.DAO
{
    public class DAOCounter
    {
        private static readonly string table = "Counter";
        public static int getNextID(string name)
        {
            int id = -1;
            var z = ConnectDB.database.GetCollection<CounterDB>(table).Find(x => x.name == name).ToList();
            if (z.Count>0)
            {
                id = z[0].countID;
                if (id >= 0) id++;
            }
            return id;
        }
        public static int updateID(string name)
        {
            int id = getNextID(name);
            var filter = Builders<CounterDB>.Filter.Eq("name", name);
            var update = Builders<CounterDB>.Update.Set("countID", id);
            ConnectDB.database.GetCollection<CounterDB>(table).UpdateOne(filter, update);
            return id++;
        }
    }
}

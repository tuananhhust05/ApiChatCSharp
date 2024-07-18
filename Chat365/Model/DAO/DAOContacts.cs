using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIChat365.Model.DAO
{
    public class DAOContacts
    {
        private static string tblContacts = "contacts";

        public static List<ContactsDB> getList()
        {
            return ConnectDB.database.GetCollection<ContactsDB>(tblContacts).Find(_ => true).ToList();
        }
        public static List<ContactsDB> getTestById(string id)
        {
            return ConnectDB.database.GetCollection<ContactsDB>(tblContacts).Find(x => x.id == id).ToList();
        }
        public static int Insert(int userFist, int userSecond)
        {
            ConnectDB.database.GetCollection<ContactsDB>(tblContacts).InsertOne(new ContactsDB() { userFist = userFist, userSecond = userSecond }); ;
            return 0;
        }
        public static int Update(string id, int userFist, int userSecond)
        {
            var filter = Builders<ContactsDB>.Filter.Eq("id", id);
            var update = Builders<ContactsDB>.Update.Set("userFist", userFist.ToString());
            update.Set("userSecond", userSecond.ToString());
            ConnectDB.database.GetCollection<ContactsDB>(tblContacts).UpdateOne(filter, update);
            return 0;
        }
        public static int Delete(string id)
        {
            ConnectDB.database.GetCollection<ContactsDB>(tblContacts).DeleteOne(x => x.id == id);
            return 0;
        }
    }
}

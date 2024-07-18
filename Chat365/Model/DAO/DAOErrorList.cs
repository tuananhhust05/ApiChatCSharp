using APIChat365.Model.MongoEntity;
using APIChat365.MongoEntity;
using Chat365.Server.Model.DAO;
using System;

namespace APIChat365.Model.DAO
{
    public class DAOErrorList
    {
        private static string table = "ErrorList";
        public static int InsertError(string message, string errorFrom)
        {
            ConnectDB.database.GetCollection<ErrorListDB>(table).InsertOne(new ErrorListDB(){ messagesError = message,createAt=DateTime.Now.ToString(), errorFrom = errorFrom });
            return 0;
        }
        

    }
}

using FastMember;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Server.Model.DAO
{
    internal class ConnectDB
    {
        public static IMongoDatabase database = new MongoClient(MongoClientSettings.FromConnectionString("mongodb://localhost:27017?retryWrites=true&w=majority")).GetDatabase("Chat365");

        public static readonly string tblBlockList = "BlockList";
        public static readonly string tblContacts = "Contacts";
        public static readonly string tblCounter = "Counter";
        public static readonly string tblMeeting = "Meeting";
        public static readonly string tblConversations = "Conversations";
        public static readonly string tblNotifications = "Notifications";
        public static readonly string tblReport = "Report";
        public static readonly string tblRequestContact = "RequestContact";
        public static readonly string tblUsers = "Users";
        public static readonly string tbllastVersion = "lastVersion";
        public static readonly string tblLoginHistory = "LoginHistory";

        public static DataTable toDataTable<T>(IEnumerable<T> list)
        {
            DataTable data = new DataTable();
            using (var reader = ObjectReader.Create(list))
            {
                data.Load(reader);
            }
            return data;
        }
        public static DataTable toDataTable(MongoCursor cursor)
        {
            if (cursor != null && cursor.Count() > 0)
            {

                DataTable dt = new DataTable(cursor.ToString());
                foreach (BsonDocument doc in cursor)
                {

                    foreach (BsonElement elm in doc.Elements)
                    {
                        if (!dt.Columns.Contains(elm.Name))
                        {
                            dt.Columns.Add(new DataColumn(elm.Name));
                        }

                    }
                    DataRow dr = dt.NewRow();
                    foreach (BsonElement elm in doc.Elements)
                    {
                        dr[elm.Name] = elm.Value;

                    }
                    dt.Rows.Add(dr);
                }
                return dt;

            }
            return null;
        }
        public static SqlConnection GetConnection()
        {
            return new SqlConnection("server=LAPTOP-HRMGDFVO;database=ChatWinForm;Max Pool Size=32767; user=sa; password=123123;Integrated Security=False");
            //return new SqlConnection("server=43.239.223.156; database =ChatWinForm;  Max Pool Size=32767 ; user=sa; password=v65GQp5mqezK1");
        }

        public static DataTable GetDataBySQL(String sql)
        {
            SqlCommand command = new SqlCommand(sql, GetConnection());
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];
        }

        public static DataTable GetDataBySQLInfo(String sql, params SqlParameter[] parameters)
        {

            SqlCommand command = new SqlCommand(sql, GetConnection());
            command.Parameters.AddRange(parameters);
            command.Connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];

        }

        public static DataTable GetDataByStoreProceduresInfo(String sql, params SqlParameter[] parameters)
        {

            SqlCommand command = new SqlCommand(sql, GetConnection());
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddRange(parameters);
            command.Connection.Open();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            return ds.Tables[0];

        }

        public static int ExecuteSQL(string sql, params SqlParameter[] parameters)
        {

            SqlCommand command = new SqlCommand(sql, GetConnection());
            command.Parameters.AddRange(parameters);
            command.Connection.Open();
            int count = command.ExecuteNonQuery();
            command.Connection.Close();
            return count;
        }
    }
}

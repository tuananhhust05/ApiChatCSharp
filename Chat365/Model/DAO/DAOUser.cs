using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APIChat365.Model.DAO;
using APIChat365.Model.MongoEntity;
using Chat365.Server.Controllers;
using Chat365.Server.Model.Entity;
using Chat365.Server.Model.EntityAPI;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using VisioForge.MediaFramework.FFMPEGEXE;

namespace Chat365.Server.Model.DAO
{
    public class DAOUsers
    {
        private static readonly string table = "Users";
        public static List<UserDB> getErrorUser()
        {
            List<UserDB> list = new List<UserDB>();
            list = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.email.ToLower().Contains(@"'")).ToList();
            return list;
        }
        public static int deleteErrorUser()
        {
            FilterDefinition<UserDB> filter = Builders<UserDB>.Filter.Where(x => x.email.ToLower().Contains(@"'"));
            var z = ConnectDB.database.GetCollection<UserDB>("Users").DeleteMany(filter);
            if (z.DeletedCount > 0) return 1;
            return 0;
        }
        public static List<UserDB> getNonIdTv()
        {
            List<UserDB> list = new List<UserDB>();
            list = ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.fromWeb == "timviec365" && x.idTimViec == 0).SortByDescending(x => x.id).ToList();
            return list;
        }
        public static int CheckContact(int id, int contactId)
        {
            List<ContactsDB> c = ConnectDB.database.GetCollection<ContactsDB>(ConnectDB.tblContacts).Find(x => (x.userFist == id && x.userSecond == contactId) || (x.userFist == contactId && x.userSecond == id)).ToList();
            if (c.Count > 0) return 1;
            return 0;
        }
        public static int DeleteAccount(int id)
        {
            FilterDefinition<UserDB> filter = Builders<UserDB>.Filter.Where(x => x.id == id);
            UpdateDefinition<UserDB> update = Builders<UserDB>.Update.Set(x => x.email, "").Set(x => x.password, "").Set(x => x.companyId, 0);
            var check = ConnectDB.database.GetCollection<UserDB>("Users").UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        //lấy tất cả user
        public static List<UserDB> GetAllUsers()
        {
            List<UserDB> list = new List<UserDB>();
            list = ConnectDB.database.GetCollection<UserDB>(table).Find(_ => true).ToList();
            return list;

        }
        //lấy thông tin user từ id365(1/8/22)
        public static List<UserDB> GetUserByID365(int id365, int type365)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).FindSync(x => x.id365 == id365 && x.type365 == type365).ToList();
        }
        //lấy user đã có avatar(1/8/22)
        public static DataTable SetupNewAvatar()
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.avatarUser != " ").ToList());
        }
        //không dùng
        public static DataTable GetUserBySchedul(string typeSchedul)
        {
            return null;
        }
        //lấy userName từ id(1/8/22)
        public static List<UserDB> GetUserById(int id)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == id).ToList();
        }
        //lấy type365 từ id(1/8/22)
        public static DataTable GetType365ById(int id)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == id).ToList());
        }
        //lấy email từ id(1/8/22)
        public static DataTable GetEmailById(int id)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == id).ToList());
        }
        //lấy danh sách id365 của nhân viên công ty(1/8/22)
        public static DataTable GetListIdUserCompany(int companyId)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.companyId == companyId && x.type365 == 2).ToList());
        }
        //lấy thông tin user từ email, password và type365(1/8/22)
        public static DataTable GetUsersByEmailAndPassword(string email, string pass, int type365)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == email.ToLower() && x.password == pass && x.type365 == type365).ToList());
        }
        public static List<UserDB> GetListUsersByEmailAndPassword(string email, string pass, int type365)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == email.ToLower() && x.password == pass && x.type365 == type365).ToList();
        }
        public static List<UserDB> CheckUsersByEmailAndPassword(string email, string pass)
        {
            List<UserDB> list = new List<UserDB>();
            list = ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == email.ToLower() && x.password == pass).ToList();
            return list;
        }

        public static List<UserDB> GetUsersByEmail(string email)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == email.ToLower()).ToList();
        }
        //lấy thông tin user từ email, password và id(1/8/22)
        public static List<UserDB> GetUsersByEmailAndPasswordAndId(string email, string pass, int id)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == email.ToLower() && x.password == pass && x.id == id).ToList();
        }
        public static List<UserDB> GetUsersByPasswordAndId(string pass, int id)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.password == pass && x.id == id).ToList();
        }
        public static List<UserDB> GetUsersBySecretCodeAndId(string secretCode, int id)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.secretCode == secretCode && x.id == id).ToList();
        }
        //lấy thông tin user từ email và type365(1/8/22)
        public static List<UserDB> GetUsersByEmailAndType365(string email, int type365)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == email.ToLower() && x.type365 == type365).ToList();
        }
        //lấy id công ty của nhân viên(1/8/22)
        public static int GetCompanyId(int userId)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == userId).SingleOrDefault().companyId;
        }
        //lấy tên công ty(1/8/22)
        public static string GetCompanyName(int companyId)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id365 == companyId && x.type365 == 1).SingleOrDefault().companyName;
        }
        //lấy id công ty từ id365 và type365(1/8/22)
        public static DataTable GetCompanyIdById365(int id365, int type365)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id365 == id365 && x.type365 == type365).ToList());
        }
        //lấy id chat từ id365, type365(1/8/22)
        public static DataTable GetUserIdById365(int id365, int type365)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id365 == id365 && x.type365 == type365).ToList());
        }
        public static DataTable GetUserIdById365(int id365)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id365 == id365).ToList());
        }
        //check mail tồn tại chưa(1/8/22)
        public static List<UserDB> checkMailEmpty(string Email)
        {
            return ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.email == Email).ToList();
        }
        //
        public static List<UserDB> checkIdMailAndType365(string Email, int id, int type365)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == id && x.email.ToLower() == Email.ToLower() && x.type365 == type365).ToList();
        }
        //check user đã tồn tại băng email và type365(1/8/22)
        public static List<UserDB> checkMailEmpty365(string Email, int type365)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == Email.ToLower() && x.type365 == type365).ToList();
        }
        //lấy cài đặt chặn người lạ(1/8/22)
        public static List<UserDB> getAcceptMessStranger(int userId)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == userId).ToList();
        }
        //check user tồn tại bằng email, type365 và id365(1/8/22)
        public static List<UserDB> checkMaiAndIdlEmpty365(string Email, int type365, int id365)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == Email.ToLower() && x.type365 == type365 && x.id365 == id365).ToList();
        }
        public static List<UserDB> checkMailIdAndId365(string Email, int id, int id365)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.email.ToLower() == Email.ToLower() && x.id == id && (x.id365 == id365 || x.id365 == 0) && (x.type365 == 2 || x.type365 == 0)).ToList();
        }
        //lấy userName từ id chat(1/8/22)
        public static List<UserDB> GetUsersById(int id)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == id).ToList();
        }
        //lấy thông tin user gửi tin nhắn(1/8/22)
        public static List<UserDB> GetUserSendMessById(int id)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == id).ToList();
        }
        //lấy version mới nhất của app wpf
        public static DataTable GetLastVersion(string from)
        {
            string sql = "SELECT * FROM dbo.LastVersion WHERE [from] = @from";
            SqlParameter[] param = { new SqlParameter("@from", SqlDbType.VarChar) };
            param[0].Value = from;
            return ConnectDB.GetDataBySQLInfo(sql, param);
        }
        public static List<UserDB> GetListFriend(int userId, int companyId,int offset, int limit)
        {
            List<UserDB> list = new List<UserDB>();
            if (companyId > 0)
            {
                list = GetListContactCompany(companyId, 0, limit+offset);
            }
            else
            {
                List<UserDB> ck = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == userId).ToList();
                if (ck.Count > 0 && !list.Any(x => x.id == ck[0].id) && list.Count<limit)
                {
                    list.Add(ck[0]);
                }
            }
            list = list.Skip(offset).ToList();
            List<int> listContact = new List<int>();

            var contacts = ConnectDB.database.GetCollection<ContactsDB>("Contacts").Find(x => x.userFist == userId || x.userSecond == userId).Limit(limit+offset).ToList();
            foreach (ContactsDB item in contacts)
            {
                if (list.Count >= limit) break;
                if (listContact.Count >= limit) break;
                if (item.userFist != userId) listContact.Add(item.userFist);
                else if (item.userSecond != userId) listContact.Add(item.userSecond);
            }
            List<UserDB> listf = ConnectDB.database.GetCollection<UserDB>(table).Find(x => listContact.Contains(x.id)).ToList();
            listf.RemoveAll(x => list.Any(u => u.id == x.id));
            list.AddRange(listf);
            return list.OrderBy(x => x.id).OrderByDescending(x => x.isOnline).Skip(offset).Take(limit).ToList();
        }

        public static List<UserDB> GetListFriend365(int userId, int companyId,int offset, int limit)
        {
            List<UserDB> list = new List<UserDB>();
            offset = offset + 1;
            if (companyId > 0)
            {
                list = GetListContactCompany(companyId, offset, limit);
            }
            else
            {
                List<UserDB> ck = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == userId).ToList();
                if (ck.Count > 0 && !list.Any(x => x.id == ck[0].id) && list.Count<limit)
                {
                    list.Add(ck[0]);
                }
            }

            List<int> listContact = new List<int>();
            int skipContact = offset - list.Count > 0 ? offset - list.Count : 0;
            var contacts = ConnectDB.database.GetCollection<ContactsDB>("Contacts").Find(x => x.userFist == userId || x.userSecond == userId).Skip(skipContact).Limit(limit).ToList();
            contacts.Reverse();
            foreach (ContactsDB item in contacts)
            {
                if (list.Count >= limit) break;
                if (item.userFist != userId) listContact.Add(item.userFist);
                else if (item.userSecond != userId) listContact.Add(item.userSecond);
            }
            List<UserDB> listf = ConnectDB.database.GetCollection<UserDB>(table).Find(x => listContact.Contains(x.id)).ToList();
            listf.RemoveAll(x => list.Any(u => u.id == x.id));
            list.AddRange(listf);
            return list.ToList();
        }
        //lấy danh sách liên hệ(1/8/22)
        public static List<UserDB> GetListContact(int id, int countContact, int countLoad)
        {
            List<int> listContact = new List<int>();

            ConnectDB.database.GetCollection<ContactsDB>("Contacts").Find(x => x.userFist == id || x.userSecond == id).Skip(countContact).Limit(countLoad).ToList().ForEach(contact =>
            {
                if (contact.userFist != id) listContact.Add(contact.userFist);
                else if (contact.userSecond != id) listContact.Add(contact.userSecond);
            });
            if (listContact.Count > 0)
            {
                return ConnectDB.database.GetCollection<UserDB>(table).Find(x => listContact.Contains(x.id)).ToList();
            }
            else
            {
                return new List<UserDB>();
            }

        }
        //lấy danh sách liên hệ cá nhân(1/8/22)
        public static List<UserDB> GetListContactPrivate(int id, int countContact, int countLoad)
        {
            List<int> listContact = new List<int>();

            ConnectDB.database.GetCollection<ContactsDB>("Contacts").Find(x => x.userFist == id || x.userSecond == id).ToList().ForEach(contact =>
            {
                if (contact.userFist != id) listContact.Add(contact.userFist);
                else if (contact.userSecond != id) listContact.Add(contact.userSecond);
            });
            listContact.Sort();
            if (listContact.Count > 0)
            {
                return ConnectDB.database.GetCollection<UserDB>(table).Find(x => listContact.Contains(x.id) && x.type365 == 0).Skip(countContact).Limit(countLoad).SortBy(x => x.id).ToList();
            }
            else
            {
                return new List<UserDB>();
            }

        }
        //xóa liên hệ(1/8/22)
        public static int DeleteContact(int userId, int contactId)
        {
            var z = ConnectDB.database.GetCollection<ContactsDB>("Contacts").DeleteOne(x => (x.userFist == userId && x.userSecond == contactId) || (x.userFist == contactId && x.userSecond == userId));
            if (z.DeletedCount > 0) return 1;
            else return 0;
        }
        //lấy danh sách user đang online(1/8/22)
        public static List<UserDB> GetListAllUserOnline(int id)
        {
            List<UserDB> list = new List<UserDB>();
            UserDB u = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == id).FirstOrDefault();
            ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.companyId == u.companyId && x.isOnline == 1 && x.id != u.id).ToList().ForEach(x =>
            {
                if (!list.Contains(x)) list.Add(x);
            });
            ConnectDB.database.GetCollection<ContactsDB>("Contacts").Find(x => x.userFist == id || x.userSecond == id).ToList().ForEach(x =>
            {
                UserDB user = new UserDB();
                if (x.userFist != id) user.id = x.userFist;
                else if (x.userSecond != id) user.id = x.userSecond;
                if (!list.Any(ck => ck.id == x.userFist || ck.id == x.userSecond))
                {
                    UserDB uu = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == user.id).SingleOrDefault();
                    if (uu.isOnline == 1)
                    {
                        list.Add(uu);
                    }
                }
            });

            return list.OrderBy(x => x.id).ToList();
        }
        //lấy danh sách liên hệ trong công ty(1/8/22)
        public static List<UserDB> GetListContactCompany(int companyId, int countContact, int countLoad)
        {
            return ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.companyId == companyId).Skip(countContact).Limit(countLoad).ToList();
        }
        //cập nhật avatar user(1/8/22)
        public static int UpdateAvatarUser(string avatar, int id)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("avatarUser", avatar);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateFromWeb(int id, string fromWeb)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("fromWeb", fromWeb);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật trang thái user(1/8/22)
        public static int UpdateActiveUser(int id, int active)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("active", active);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //check user có online hay không(1/8/22)
        public static DataTable GetIsOnlineUser(int id)
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.id == id).ToList());
        }
        //cập nhật trang thái online cho user(1/8/22)
        public static int UpdateIsOnlineUser(int id, int isOnline)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("isOnline", isOnline);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //lấy danh sách lời mới kế bạn(1/8/22)
        public static List<RequestContact> getRequestContact(int userId)
        {
            DataTable listRequest = ConnectDB.toDataTable(ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.userId == userId).ToList());
            List<RequestContact> listContact = new List<RequestContact>();
            foreach (DataRow member in listRequest.Rows)
            {
                if (member["status"].ToString().Equals("send"))
                {
                    listContact.Add(new RequestContact(userId, Convert.ToInt32(member["contactId"]), "send", Convert.ToInt32(member["type365"])));
                }
                else if (member["status"].ToString().Equals("accept"))
                {
                    listContact.Add(new RequestContact(userId, Convert.ToInt32(member["contactId"]), "accept", Convert.ToInt32(member["type365"])));
                }
                else if (member["status"].ToString().Equals("deciline"))
                {
                    listContact.Add(new RequestContact(userId, Convert.ToInt32(member["contactId"]), "deciline", Convert.ToInt32(member["type365"])));
                }
            }
            listRequest = ConnectDB.toDataTable(ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.contactId == userId).ToList());
            foreach (DataRow member in listRequest.Rows)
            {
                if (member["status"].ToString().Equals("send"))
                {
                    listContact.Add(new RequestContact(userId, Convert.ToInt32(member["userId"]), "request", Convert.ToInt32(member["type365"])));
                }
            }
            return listContact;
        }

        public static List<RequestFriend> getRequestFriend(int userId, int contactId)
        {
            DataTable listRequest = ConnectDB.toDataTable(ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.userId == userId).ToList());
            if (contactId > 0) listRequest = ConnectDB.toDataTable(ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.userId == userId && x.contactId == contactId).ToList());
            List<RequestFriend> listContact = new List<RequestFriend>();
            foreach (DataRow member in listRequest.Rows)
            {
                int contact = Convert.ToInt32(member["contactId"]);
                List<UserDB> us = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == contact).ToList();
                if (us.Count > 0)
                {
                    var request = new RequestFriend(us[0].id, us[0].userName, us[0].avatarUser, "", us[0].type365);
                    if (String.IsNullOrWhiteSpace(request.avatar.Trim()))
                    {
                        string letter = request.userName.Substring(0, 1).ToLower().RemoveUnicode().ToUpper();
                        try
                        {
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                            }
                            else
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        catch (Exception ex)
                        {
                            request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                        }
                    }
                    else
                    {
                        request.avatar = "https://mess.timviec365.vn/avatarUser/" + request.id + "/" + request.avatar;
                    }
                    if (member["status"].ToString().Equals("send"))
                    {
                        request.status = "send";
                    }
                    else if (member["status"].ToString().Equals("accept"))
                    {
                        request.status = "accept";
                    }
                    else if (member["status"].ToString().Equals("deciline"))
                    {
                        request.status = "deciline";
                    }
                    listContact.Add(request);
                }
            }
            listRequest = ConnectDB.toDataTable(ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.contactId == userId).ToList());
            if (contactId > 0) listRequest = ConnectDB.toDataTable(ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.contactId == userId && x.userId == contactId).ToList());
            foreach (DataRow member in listRequest.Rows)
            {
                int contact = Convert.ToInt32(member["userId"]);
                List<UserDB> us = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == contact).ToList();
                if (us.Count > 0)
                {
                    var request = new RequestFriend(us[0].id, us[0].userName, us[0].avatarUser, "", us[0].type365);
                    if (String.IsNullOrWhiteSpace(request.avatar.Trim()))
                    {
                        string letter = request.userName.Substring(0, 1).ToLower().RemoveUnicode().ToUpper();
                        try
                        {
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                            }
                            else
                            {
                                request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        catch (Exception ex)
                        {
                            request.avatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                        }
                    }
                    else
                    {
                        request.avatar = "https://mess.timviec365.vn/avatarUser/" + request.id + "/" + request.avatar;
                    }
                    if (member["status"].ToString().Equals("send"))
                    {
                        request.status = "request";
                    }
                    listContact.Add(request);
                }
            }
            return listContact;
        }

        public static List<RequestContact1> getRequestContact1(int userId, int contactId)
        {
            List<RequestContactDB> listRequest = new List<RequestContactDB>();
            if (contactId != 0) listRequest = ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.contactId == userId && x.userId == contactId).ToList();
            else listRequest = ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.contactId == userId).ToList();
            List<RequestContact1> listContact = new List<RequestContact1>();
            foreach (var member in listRequest)
            {
                if (member.status == "send")
                {
                    var request = new RequestContact1(member.userId, "", "", member.type365);
                    List<UserDB> us = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == member.userId).ToList();
                    if (us.Count > 0)
                    {
                        UserDB u = us[0];
                        request.contactName = u.userName;
                        request.contactAvatar = u.avatarUser;

                        if (String.IsNullOrWhiteSpace(request.contactAvatar.Trim()))
                        {
                            string letter = request.contactName.Substring(0, 1).ToLower().RemoveUnicode().ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            request.contactAvatar = "https://mess.timviec365.vn/avatarUser/" + request.contactId + "/" + request.contactAvatar;
                        }

                    }
                    listContact.Add(request);
                }
            }
            return listContact;
        }

        public static List<RequestContact1> getUserRequestContact1(int userId, int contactId)
        {
            List<RequestContactDB> listRequest = new List<RequestContactDB>();
            if (contactId != 0) listRequest = ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.userId == userId && x.contactId == contactId).ToList();
            else listRequest = ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => x.userId == userId).ToList();
            List<RequestContact1> listContact = new List<RequestContact1>();
            foreach (var member in listRequest)
            {
                if (member.status == "send")
                {
                    var request = new RequestContact1(member.contactId, "", "", member.type365);
                    List<UserDB> us = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == member.contactId).ToList();
                    if (us.Count > 0)
                    {
                        UserDB u = us[0];
                        request.contactName = u.userName;
                        request.contactAvatar = u.avatarUser;

                        if (String.IsNullOrWhiteSpace(request.contactAvatar.Trim()))
                        {
                            string letter = request.contactName.Substring(0, 1).ToLower().RemoveUnicode().ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                request.contactAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            request.contactAvatar = "https://mess.timviec365.vn/avatarUser/" + request.contactId + "/" + request.contactAvatar;
                        }

                    }
                    listContact.Add(request);
                }
            }
            return listContact;
        }

        //kết bạn(1/8/22)
        public static int AddFriend(int userId, int contactId, int type365)
        {
            try
            {
                if (ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").Find(x => (x.userId == userId && x.contactId == contactId) || (x.userId == contactId && x.contactId == userId)).ToList().Count > 0) return 0;
                else
                {
                    ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").InsertOne(new RequestContactDB()
                    {
                        userId = userId,
                        contactId = contactId,
                        status = "send",
                        type365 = type365
                    });
                    return 1;
                }
            }
            catch (Exception)
            {

                return 0;
            }
        }
        //thêm liên hệ(1/8/22)
        public static int AddNewContact(int userId, int contactId)
        {
            try
            {
                if (ConnectDB.database.GetCollection<ContactsDB>("Contacts").Find(x => (x.userFist == userId && x.userSecond == contactId) || (x.userFist == userId && x.userSecond == contactId)).ToList().Count > 0) return 0;
                else
                {
                    ConnectDB.database.GetCollection<ContactsDB>("Contacts").InsertOne(new ContactsDB { userFist = userId, userSecond = contactId });
                    return 1;
                }
            }
            catch (Exception)
            {

                return 0;
            }
        }
        //xóa lời mời kết bạn(1/8/22)
        public static int DeleteRequestAddFriend(int userId, int contactId)
        {
            var ck = ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").DeleteOne(x => x.userId == userId && x.contactId == contactId);
            if (ck.DeletedCount > 0) return 1;
            else return 0;
        }
        //đồng ý kết bạn(1/8/22)
        public static int AcceptRequestAddFriend(int userId, int contactId)
        {
            var filter = Builders<RequestContactDB>.Filter.Where(x => x.contactId == userId && x.userId == contactId);
            var update = Builders<RequestContactDB>.Update.Set("status", "accept");
            var ck = ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //từ chối kết bạn(1/8/22)
        public static int DecilineRequestAddFriend(int userId, int contactId)
        {
            var filter = Builders<RequestContactDB>.Filter.Where(x => x.contactId == userId && x.userId == contactId);
            var update = Builders<RequestContactDB>.Update.Set("status", "deciline");
            var ck = ConnectDB.database.GetCollection<RequestContactDB>("RequestContact").UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //thêm user mới(1/8/22)
        public static int InsertNewUser(string userName, int id365, int idTimViec, int type365, string email, string password, int companyId, string companyName, string fromWeb)
        {
            try
            {
                int userId = DAOCounter.getNextID("UserID");
                UserDB document = new UserDB(userId, id365, type365, email, password, "", userName, "", "", 0, DateTime.MinValue, 1, 0, 0, companyId, companyName, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, idTimViec, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, new List<HistoryAccessDB>(), 0, 0, new List<int>(), 1,userName.RemoveUnicode());
                document.fromWeb = fromWeb;
                document.secretCode = RandomString(10);
                ConnectDB.database.GetCollection<UserDB>(table).InsertOne(document);
                DAO.DAOCounter.updateID("UserID");

                if (AddFriend(56387, userId, 0) == 1) AcceptRequestAddFriend(userId, 56387);
                if (AddNewContact(56387, userId) == 1) UserController.WIO.EmitAsync("AcceptRequestAddFriend", userId, 56387);

                return userId;
            }
            catch (Exception)
            {

                return 0;
            }

        }
        //cập nhật userName(1/8/22)
        public static int UpdateNameUser(int id, string userName)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("userName", userName);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật mật khẩu(1/8/22)
        public static int UpdatePassword(int id, string password)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("password", password);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật mật khẩu băng email, type365(1/8/22)
        public static int UpdatePasswordByEmail(string email, string password, int type365)
        {
            var filter = Builders<UserDB>.Filter.Where(x => x.type365 == type365 && x.email == email);
            var update = Builders<UserDB>.Update.Set("password", password);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //lấy danh sách liên hệ(1/8/22)
        public static List<UserDB> searchContactInHomePage(string txtSearch, int userId, int companyId, int countSearch)
        {
            List<UserDB> list = new List<UserDB>();

            if (list.Count < countSearch)
            {
                List<RequestContact> request = DAOUsers.getRequestContact(userId);
                foreach (RequestContact r in request)
                {
                    if (r.status == "request")
                    {
                        List<UserDB> users = ConnectDB.database.GetCollection<UserDB>(ConnectDB.tblUsers).Find(x => x.id == r.contactId).ToList();
                        if (users.Count > 0)
                        {
                            UserDB user = users[0];
                            if (user.userName.ToLower().RemoveUnicode().EndsWith(txtSearch.ToLower().RemoveUnicode()) && (user.companyId != companyId || user.companyId == 0) && !list.Any(u => u.id == user.id))
                            {
                                list.Add(user);
                            }
                            if (user.userName.ToLower().RemoveUnicode().Contains(txtSearch.ToLower().RemoveUnicode()) && (user.companyId != companyId || user.companyId == 0) && !list.Any(u => u.id == user.id))
                            {
                                list.Add(user);
                            }
                            if (user.userName.ToLower().RemoveUnicode().StartsWith(txtSearch.ToLower().RemoveUnicode()) && (user.companyId != companyId || user.companyId == 0) && !list.Any(u => u.id == user.id))
                            {
                                list.Add(user);
                            }
                            if (list.Count >= countSearch)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            List<ConversationsDB> conversations = new List<ConversationsDB>();
            conversations = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.memberList.Count > 1 && x.memberList.Any(x => x.memberId == userId) && x.isGroup == 0 && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).ToList();
            foreach (ConversationsDB c in conversations)
            {
                int index = c.memberList.FindIndex(x => x.memberId != userId);
                if (index != -1)
                {
                    List<UserDB> u = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == c.memberList[index].memberId && (x.companyId != companyId || x.companyId == 0) && (x.userName.ToLower().Contains(txtSearch.ToLower()) || x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicodeP1()) || x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicode()))).ToList();
                    if (u.Count > 0 && !list.Any(x => x.id == u[0].id))
                    {
                        list.Add(u[0]);
                    }
                }
                if (list.Count >= countSearch)
                {
                    break;
                }
            }

            if (list.Count < countSearch)
            {
                List<UserDB> contac = DAOUsers.GetListContact(userId, 0, 10000);
                foreach (UserDB user in contac)
                {
                    if (user.userName.ToLower().RemoveUnicode().EndsWith(txtSearch.ToLower().RemoveUnicode()) && (user.companyId != companyId || user.companyId == 0) && !list.Any(u => u.id == user.id))
                    {
                        list.Add(user);
                    }
                    if (user.userName.ToLower().RemoveUnicode().Contains(txtSearch.ToLower().RemoveUnicode()) && (user.companyId != companyId || user.companyId == 0) && !list.Any(u => u.id == user.id))
                    {
                        list.Add(user);
                    }
                    if (user.userName.ToLower().RemoveUnicode().StartsWith(txtSearch.ToLower().RemoveUnicode()) && (user.companyId != companyId || user.companyId == 0) && !list.Any(u => u.id == user.id))
                    {
                        list.Add(user);
                    }
                    if (list.Count >= countSearch)
                    {
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(txtSearch))
            {
                if (list.Count < countSearch)
                {
                    ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && (x.companyId != companyId || x.companyId == 0) && x.userName.ToLower() == (txtSearch.ToLower())).SortByDescending(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                    {
                        if (!list.Any(u => u.id == x.id))
                            list.Add(x);
                    });
                }
                if (list.Count < countSearch)
                {
                    ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && (x.companyId != companyId || x.companyId == 0) && x.userName.ToLower().Contains(txtSearch.ToLower())).SortByDescending(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                    {
                        if (!list.Any(u => u.id == x.id))
                            list.Add(x);
                    });
                }
                if (list.Count < countSearch)
                {
                    ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && (x.companyId != companyId || x.companyId == 0) && x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicodeP1())).SortByDescending(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                    {
                        if (!list.Any(u => u.id == x.id))
                            list.Add(x);
                    });
                }
                if (list.Count < countSearch)
                {
                    ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && (x.companyId != companyId || x.companyId == 0) && x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicode())).SortBy(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                    {
                        if (!list.Any(u => u.id == x.id))
                            list.Add(x);
                    });
                }
                if (list.Count < countSearch)
                {
                    ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && (x.companyId != companyId || x.companyId == 0) && x.email.ToLower().Contains(txtSearch.ToLower().RemoveUnicode())).SortBy(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                    {
                        if (!list.Any(u => u.id == x.id))
                            list.Add(x);
                    });
                }
            }

            return list;
        }
        //lấy danh sách liên hệ trong công ty(1/8/22)
        public static List<UserDB> searchByCompanyContactInHomePage(string txtSearch, int userId, int companyId, int countSearch)
        {
            List<UserDB> list = new List<UserDB>();

            List<ConversationsDB> conversations = new List<ConversationsDB>();
            conversations = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.memberList.Count > 1 && x.memberList.Any(x => x.memberId == userId) && x.isGroup == 0 && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).ToList();
            foreach (ConversationsDB c in conversations)
            {
                int index = c.memberList.FindIndex(x => x.memberId != userId);
                if (index != -1)
                {
                    List<UserDB> u = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == c.memberList[index].memberId && x.companyId == companyId && (x.userName.ToLower().Contains(txtSearch.ToLower()) || x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicodeP1()) || x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicode()))).ToList();
                    if (u.Count > 0 && !list.Any(x => x.id == u[0].id))
                    {
                        list.Add(u[0]);
                    }
                }
                if (list.Count >= countSearch)
                {
                    break;
                }
            }

            if (list.Count < countSearch)
            {
                ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && x.companyId == companyId && x.userName.ToLower() == (txtSearch.ToLower())).SortByDescending(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                {
                    if (!list.Any(u => u.id == x.id))
                        list.Add(x);
                });
            }
            if (list.Count < countSearch)
            {
                ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && x.companyId == companyId && x.userName.ToLower().Contains(txtSearch.ToLower())).SortByDescending(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                {
                    if (!list.Any(u => u.id == x.id))
                        list.Add(x);
                });
            }
            if (list.Count < countSearch)
            {
                ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && x.companyId == companyId && x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicodeP1())).SortByDescending(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                {
                    if (!list.Any(u => u.id == x.id))
                        list.Add(x);
                });
            }

            if (list.Count < countSearch)
            {
                ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && x.companyId == companyId && x.userName.ToLower().Contains(txtSearch.ToLower().RemoveUnicode())).SortBy(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                {
                    if (!list.Any(u => u.id == x.id))
                        list.Add(x);
                });
            }

            if (list.Count < countSearch)
            {
                ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && x.companyId == companyId && x.email.ToLower().Contains(txtSearch.ToLower().RemoveUnicode())).SortBy(x => x.id).Limit(countSearch - list.Count).ToList().ForEach(x =>
                {
                    if (!list.Any(u => u.id == x.id))
                        list.Add(x);
                });
            }
            return list;
        }
        //cập nhật id, tên công ty cho user(1/8/22)
        public static int UpdateCompany(int id, int companyId, string companyName, int id365)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("companyId", companyId).Set("companyName", companyName).Set("id365", id365);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //nghỉ việc(1/8/22)
        public static int UserQuitJob(int userId)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", userId);
            var update = Builders<UserDB>.Update.Set("companyId", 0).Set("companyName", "").Set("id365", 0).Set("type365", 0);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int MultiQuitJob(int userId)
        {
            var filter = Builders<UserDB>.Filter.Eq("id365", userId);
            var update = Builders<UserDB>.Update.Set("companyId", 0).Set("companyName", "").Set("id365", 0).Set("type365", 0);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật thông báo thưởng phạt(1/8/22)
        public static int UpdateNotificationPayoff(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationPayoff", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật thông báo lịch làm việc(1/8/22)
        public static int UpdateNotificationCalendar(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationCalendar", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //(1/8/22)
        public static int UpdateNotificationOffer(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationOffer", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //(1/8/22)
        public static int UpdateNotificationReport(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationReport", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật thông báo nhân sự mới(1/8/22)
        public static int UpdateNotificationNewPersonnel(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationNewPersonnel", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật chăng người lạ(1/8/22)
        public static int UpdateStatusAcceptMessStranger(int id, int StatusAcceptMessStranger)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("acceptMessStranger", StatusAcceptMessStranger);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật thông báo khen thưởng(1/8/22)
        public static int UpdateNotificationRewardDiscipline(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationRewardDiscipline", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật thông báo thay đổi thông tin cá nhân(1/8/22)
        public static int UpdateNotificationPersionalChange(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationPersonnelChange", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //(1/8/22)
        public static int UpdateNotificationChangeProfile(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationChangeProfile", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật thông báo điều hướng tài sản(1/8/22)
        public static int UpdateNotificationTransferAsset(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationTransferAsset", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //(1/8/22)
        public static DataTable getAllEmailAndType()
        {
            return ConnectDB.toDataTable(ConnectDB.database.GetCollection<UserDB>(table).Find(x => x.idTimViec == 0).ToList());
        }
        //cập nhật idTimViec(1/8/22)
        public static int UpdateIdTimViec(string email, int type, int idTimViec)
        {
            var filter = Builders<UserDB>.Filter.Where(x => x.email == email && x.type365 == type);
            var update = Builders<UserDB>.Update.Set("idTimViec", idTimViec);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateIdTimViec1(int id, int idTimViec)
        {
            var filter = Builders<UserDB>.Filter.Where(x => x.id == id);
            var update = Builders<UserDB>.Update.Set("idTimViec", idTimViec);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //mới
        //lấy thông tin user từ idtimviec và type365(1/8/22)
        public static List<UserDB> GetUsersByIDTimViecAndType365(int idTimViec, int type365)
        {
            return ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.idTimViec == idTimViec && x.type365 == type365).ToList();
        }
        //lấy tất cả email và type365(1/8/22)
        public static List<UserDB> getAllEmailAndTypeQLC()
        {
            return ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.type365 == 1 || x.type365 == 2).ToList();
        }
        //cập nhật id365(1/8/22)
        public static int UpdateIdQLC(string email, int type, int id365)
        {
            var filter = Builders<UserDB>.Filter.Where(x => x.email == email && x.type365 == type);
            var update = Builders<UserDB>.Update.Set("id365", id365);
            var ck = ConnectDB.database.GetCollection<UserDB>("Users").UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //lấy thông tin user từ id chat(1/8/22)
        public static List<UserDB> GetInforUserById(int id)
        {
            return ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == id).ToList();
        }
        //lấy idtimviec từ email, type365(1/8/22)
        public static List<UserDB> GetIdTimViec(string Email, int type365)
        {
            return ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.email.ToLower() == Email.ToLower() && x.type365 == type365).ToList();
        }
        //cập nhật thông tin tin User(1/8/22)
        public static int UpdateInfoUser(int id, int id365, int type365, string userName, string avatar, string password, int companyId, string companyName, int idTimViec)
        {
            var filter = Builders<UserDB>.Filter.Where(x => x.id == id);
            var update = Builders<UserDB>.Update.Set("id365", id365).Set("idTimViec", idTimViec).Set("type365", type365).Set("userName", userName).Set("avatarUser", avatar).Set("password", password).Set("companyId", companyId).Set("companyName", companyName);
            var ck = ConnectDB.database.GetCollection<UserDB>("Users").UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //gợi ý kết bạn
        public static List<UserDB> getListSuggesContact(int userId, int companyId, int countSearch, int countLoad)
        {
            List<UserDB> listSugges = new List<UserDB>();
            List<UserDB> listContact = GetListContact(userId, 0, 10000);
            List<RequestContact> listRequest = getRequestContact(userId);
            List<ConversationsDB> conversations = new List<ConversationsDB>();
            List<ConversationsDB> conversationsContact = new List<ConversationsDB>();

            List<UserDB> getUser = GetInforUserById(userId);
            if (getUser.Count > 0)
            {
                List<int> skipIds = new List<int>();
                if (getUser[0].removeSugges != null) skipIds = getUser[0].removeSugges;
                conversations = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.memberList.Count > 1 && x.memberList.Any(x => x.memberId == userId) && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).Skip(0).Limit(10).ToList();
                foreach (ConversationsDB c in conversations)
                {
                    int index = c.memberList.FindIndex(x => x.memberId != userId);
                    if (index != -1)
                    {
                        List<UserDB> u = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == c.memberList[index].memberId && x.companyId != companyId).ToList();
                        u.RemoveAll(x => listContact.Any(c => c.id == x.id));
                        u.RemoveAll(x => listRequest.Any(c => c.contactId == x.id && (c.status == "send" || c.status == "request")));
                        if (u.Count > 0 && !listSugges.Any(x => x.id == u[0].id))
                        {
                            if (listSugges.Count < countSearch && !skipIds.Contains(u[0].id))
                            {
                                listSugges.Add(u[0]);
                                countLoad++;
                            }

                        }
                    }

                }
                if (listSugges.Count < countSearch)
                {
                    foreach (UserDB u in listContact)
                    {
                        if (u.id != userId && u.companyId != companyId)
                        {
                            conversationsContact = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.memberList.Count > 1 && x.memberList.Any(x => x.memberId == u.id) && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).Limit(5).ToList();
                            foreach (ConversationsDB c in conversationsContact)
                            {
                                int index = c.memberList.FindIndex(x => x.memberId != userId);
                                if (index != -1)
                                {
                                    List<UserDB> list = GetListContact(c.memberList[index].memberId, 0, 10000);
                                    list.RemoveAll(x => listSugges.Any(s => s.id == x.id));
                                    list.RemoveAll(x => listContact.Any(s => s.id == x.id));
                                    list.RemoveAll(x => listRequest.Any(s => s.userId == x.id && (s.status == "send" || s.status == "request")));
                                    if (list.Count > 0 && !skipIds.Contains(list[0].id)) listSugges.Add(list[0]);
                                }
                            }

                        }
                    }
                }
                if (listSugges.Count < countSearch)
                {
                    foreach (UserDB u in listContact)
                    {
                        if (u.id != userId && u.companyId != companyId)
                        {
                            List<UserDB> list = GetListContact(u.id, 0, 10000);
                            list.RemoveAll(x => listSugges.Any(s => s.id == x.id));
                            list.RemoveAll(x => listContact.Any(s => s.id == x.id));
                            list.RemoveAll(x => listRequest.Any(s => s.userId == x.id && (s.status == "send" || s.status == "request")));
                            if (list.Count > 0 && !skipIds.Contains(list[0].id)) listSugges.Add(list[0]);
                        }
                    }
                }
                listSugges.RemoveAll(x => x.companyId == companyId);
            }

            return listSugges;
        }
        public static List<UserDB> GetListOfferContactByPhone(int userId, int companyId, string[] phones)
        {
            List<UserDB> list = new List<UserDB>();
            //List<UserDB> contact = GetListContact(userId, 0, 10000);
            list = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id != userId && (x.companyId != companyId || (x.companyId == 0 && companyId == 0)) && phones.Contains(x.email)).ToList();
            //list.RemoveAll(x => contact.Any(con => con.id == x.id));
            return list;
        }

        public static int updateRemoveSugges(int userId, int contactId)
        {
            var filter = Builders<UserDB>.Filter.Where(x => x.id == userId);
            var update = Builders<UserDB>.Update.Push("removeSugges", contactId);
            var ck = ConnectDB.database.GetCollection<UserDB>("Users").UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }

        //thêm cột vào cuộc trò chuyện
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static int AddNewFieldUser()
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => true);
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].deleteTime, 0);
            var options = new UpdateOptions { IsUpsert = true };
            var check = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").UpdateMany(filter, update, options);
            if (check.ModifiedCount != 0) return Convert.ToInt32(check.ModifiedCount);
            return 0;
        }

        public static List<UserDB> getErrorPassTV(int type)
        {
            List<UserDB> list = new List<UserDB>();
            list = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.fromWeb == "timviec365" && x.password == "" && !string.IsNullOrEmpty(x.email)).ToList();
            return list;
        }
        public static int UpdateNotificationMissMessage(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationMissMessage", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationCommentFromTimViec(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationCommentFromTimViec", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationCommentFromRaoNhanh(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationCommentFromRaoNhanh", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationTag(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationTag", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationChangeSalary(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationChangeSalary", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationAllocationRecall(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationAllocationRecall", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationAcceptOffer(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationAcceptOffer", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationDecilineOffer(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationDecilineOffer", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationNTDPoint(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationNTDPoint", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationNTDExpiredPin(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationNTDExpiredPin", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationNTDExpiredRecruit(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationNTDExpiredRecruit", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateNotificationSendCandidate(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationSendCandidate", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }

        public static int UpdateNotificationNTDApplying(int id, int status)
        {
            var filter = Builders<UserDB>.Filter.Eq("_id", id);
            var update = Builders<UserDB>.Update.Set("notificationNTDApplying", status);
            var ck = ConnectDB.database.GetCollection<UserDB>(table).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
    }
}

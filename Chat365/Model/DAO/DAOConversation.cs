using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIChat365.Model.DAO;
using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.Entity;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Chat365.Server.Model.DAO
{
    public class DAOConversation
    {
        public static string tableConversations = "Conversations";
        public static string tableBrowseMembers = "BrowseMembers";
        public static string tableparticipants = "participants";
        public static string tableUsers = "Users";
        public static string tableMessages = "Messages";

        public static string RemoveUnicode(string text)
        {
            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ", };
            string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y", };
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }
            return text;
        }

        //lấy danh sách cuộc trò chuyện(3/8/22)
        public static List<ConversationsDB> GetListConversation(int userId, int countLoad, int countConversation)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.memberList.Count > 0 && x.memberList.Any(u => u.memberId == userId) && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).Skip(countConversation).Limit(countLoad).ToList();
        }
        public static List<ConversationsDB> GetListConversationUnreader(int userId)
        {
            List<ConversationsDB> conver = new List<ConversationsDB>();
            conver = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.memberList.Count > 0 && x.memberList.Any(u => u.memberId == userId && u.unReader > 0) && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).ToList();
            List<int> remove = new List<int>();
            for (int i = 0; i < conver.Count; i++)
            {
                int index = conver[i].memberList.FindIndex(x => x.memberId == userId);
                if (index > -1 && conver[i].memberList[index].isHidden == 1 && !remove.Contains(conver[i].id))
                {
                    remove.Add(conver[i].id);
                }
            }
            if (remove.Count > 0) conver.RemoveAll(x => remove.Contains(x.id));
            return conver;
        }
        ////thêm cột vào cuộc trò chuyện
        //public static int AddNewFieldConversation()
        //{
        //    var cc = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => true).ToList();
        //    foreach(ConversationsDB conversation in cc)
        //    {
        //        var filter = Builders<ConversationsDB>.Filter.Eq(x => x.id, conversation.id);
        //        var update = Builders<ConversationsDB>.Update.Set(x => x.timeLastMessage, conversation.messageList.Count > 0 ? conversation.messageList[conversation.messageList.Count - 1].createAt: DateTime.Now);
        //        var options = new UpdateOptions { IsUpsert = true }; 
        //        ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update, options);
        //    }
        //    return cc.Count;
        //}
        //kết nối lại cuộc trò chuyên(2/8/22)
        public static List<ConversationsDB> CheckReconnectInternet(int userId, DateTime lastMessage)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.memberList.Count > 0 && x.memberList.Any(p => p.memberId == userId) && x.messageList.Count > 0 && x.messageList[0].createAt > lastMessage).ToList();
        }
        //lấy danh sách cuộc trò chuyện dạng call
        public static DataTable GetListCall(int userId)
        {

            return null;
        }
        //lấy thông tin cuộc trò chuyện(2/8/22)
        public static ConversationsDB GetConversation(int conversationId, int userId)
        {
            List<ConversationsDB> listConversation = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId).ToList();
            if (listConversation.Count > 0) return listConversation[0];
            return null;
        }
        public static ConversationsDB CheckIsMemeber(int conversationId, int userId)
        {
            List<ConversationsDB> listConversation = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId && x.memberList.Count > 0 && x.memberList.Any(m => m.memberId == userId)).ToList();
            if (listConversation.Count > 0) return listConversation[0];
            return null;
        }
        //tìm cuộc trò chuyện nhóm theo userid và tên cuộc trò chuyện(2/8/22)
        public static List<Conversation> SearchGroupByName(int userId, string messSearch, int countSearch)
        {
            var listConversationDB = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.isGroup == 1 && x.memberList.Count > 0 && x.memberList.Any(u => u.memberId == userId) && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).ToList();
            List<Conversation> listConversation = new List<Conversation>();
            if (listConversationDB.Count > 0)
            {
                var listRequest = DAOUsers.getRequestContact(userId);
                var listFriend = DAOUsers.GetListContact(userId, 0, 10000);
                foreach (ConversationsDB dr in listConversationDB)
                {
                    ParticipantsDB userCurrent = new ParticipantsDB();
                    List<MemberConversation> listMember = new List<MemberConversation>();
                    string conversationName = "";
                    foreach (ParticipantsDB member in dr.memberList)
                    {
                        try
                        {
                            UserDB memberDB = DAOUsers.GetUserById(member.memberId)[0];
                            if (member.memberId == userId)
                            {
                                userCurrent = member;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(member.conversationName)) conversationName += memberDB.userName + ",";
                                else if (!string.IsNullOrEmpty(member.conversationName) && dr.isGroup == 1) conversationName = member.conversationName + " ";
                            }
                            MemberConversation userInfo = new MemberConversation(member.memberId, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive.ToLocalTime().ToString(), memberDB.active, memberDB.isOnline, member.unReader, memberDB.companyId, member.timeLastSeener.ToLocalTime(), memberDB.idTimViec, memberDB.type365, "none");
                            if (String.IsNullOrWhiteSpace(userInfo.AvatarUser.Trim()))
                            {
                                string letter = RemoveUnicode(userInfo.UserName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            else
                            {
                                userInfo.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + userInfo.ID + "/" + userInfo.AvatarUser;
                                userInfo.LinkAvatar = userInfo.AvatarUser;
                            }
                            if (listFriend.Any(x => x.id == member.memberId)) userInfo.FriendStatus = "friend";
                            else
                            {
                                int requestIndex = listRequest.FindIndex(x => x.contactId == member.memberId);
                                if (requestIndex > -1)
                                {
                                    userInfo.FriendStatus = listRequest[requestIndex].status;
                                }
                            }
                            listMember.Add(userInfo);
                        }
                        catch
                        {

                        }
                    }
                    if (String.IsNullOrEmpty(userCurrent.conversationName.Trim()) && !string.IsNullOrEmpty(conversationName))
                    {
                        userCurrent.conversationName = conversationName.Remove(conversationName.Length - 1);
                    }
                    if (dr.memberList.Count == 1)
                    {
                        userCurrent.conversationName = "Chỉ mình bạn";
                    }
                    if (RemoveUnicode(userCurrent.conversationName.Trim().ToLower()).Contains(RemoveUnicode(messSearch.Trim().ToLower())))
                    {
                        Conversation conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, dr.messageList[dr.messageList.Count - 1].senderId, dr.messageList[dr.messageList.Count - 1].message, dr.messageList[dr.messageList.Count - 1].messageType, dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime(), userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                        conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";
                        conversation.listMember = listMember;
                        conversation.countMessage = Convert.ToInt32(DAOConversation.GetCountMessageOfConversation(conversation.conversationId)[0].messageList.Count);

                        conversation.listBrowerMember = new List<BrowseMember>();
                        if (conversation.isGroup == 1 && conversation.Equals("Moderate"))
                        {
                            foreach (BrowseMembersDB member in dr.browseMemberList)
                            {
                                try
                                {
                                    UserDB memberDB = DAOUsers.GetUserById(member.memberBrowserId)[0];
                                    BrowseMember userInfo = new BrowseMember(new User(memberDB.id, 0, 0, 0, "", "", "", memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, 0, 0, "", memberDB.fromWeb), member.memberAddId);
                                    if (String.IsNullOrWhiteSpace(userInfo.UserMember.AvatarUser.Trim()))
                                    {
                                        string letter = RemoveUnicode(userInfo.UserMember.UserName.Substring(0, 1).ToLower()).ToUpper();
                                        try
                                        {
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                userInfo.UserMember.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                            }
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                userInfo.UserMember.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                            }
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                userInfo.UserMember.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                            }
                                            else
                                            {
                                                userInfo.UserMember.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            userInfo.UserMember.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    else
                                    {
                                        userInfo.UserMember.LinkAvatar = "https://mess.timviec365.vn/avatarUser/" + userInfo.UserMember.ID + "/" + userInfo.UserMember.AvatarUser;
                                        userInfo.UserMember.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + userInfo.UserMember.ID + "/" + userInfo.UserMember.AvatarUser;
                                    }
                                    conversation.listBrowerMember.Add(userInfo);
                                }
                                catch
                                {
                                }
                            }
                        }

                        if (String.IsNullOrWhiteSpace(conversation.avatarConversation.Trim()) && conversation.isGroup == 1)
                        {
                            string letter = RemoveUnicode(!String.IsNullOrWhiteSpace(conversation.conversationName.Trim()) ? conversation.conversationName.Substring(0, 1).ToLower() : conversation.listMember[0].UserName.Substring(0, 1).ToLower()).ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    conversation.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    conversation.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    conversation.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    conversation.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                conversation.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            conversation.avatarConversation = "https://mess.timviec365.vn/avatarGroup/" + conversation.conversationId + "/" + conversation.avatarConversation;
                            conversation.LinkAvatar = conversation.avatarConversation;
                        }

                        listConversation.Add(conversation);
                    }
                    if (listConversation.Count >= countSearch) break;
                }
            }
            return listConversation;
        }
        //lấy danh sách cuộc trò chuyện gần đây(3/8/22)
        public static List<ConversationForward> GetListConversationForward(int userId, int companyId, string mess, int countLoad, int countConversation)
        {
            List<int> has = new List<int>();
            List<ConversationForward> list = new List<ConversationForward>();
            var listConversationDB = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.memberList.Count > 0 && x.memberList.Any(u => u.memberId == userId) && x.messageList.Count > 0).SortByDescending(x => x.timeLastMessage).ToList();
            int count = 0;
            foreach (ConversationsDB c in listConversationDB)
            {
                ConversationForward con = new ConversationForward(c.id, "", c.avatarConversation, c.isGroup, "");
                if (c.memberList.Count == 1)
                {
                    con.avatarConversation = "https://mess.timviec365.vn/avatar/C_4.png";
                    con.conversationName = "Chỉ mình bạn";
                    con.status = "1 thành viên";
                }
                else
                {
                    var uu = c.memberList.Where(x => x.memberId != userId).ToList();
                    if (uu.Count > 0)
                    {
                        con.conversationName = uu[0].conversationName;
                        if (string.IsNullOrEmpty(con.conversationName) && c.isGroup == 0 && c.memberList.Count == 2)
                        {
                            var mem = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == uu[0].memberId).ToList();
                            if (mem.Count > 0)
                            {
                                con.conversationName = mem[0].userName;
                                con.status = mem[0].status;
                                if (string.IsNullOrEmpty(mem[0].avatarUser.Trim()))
                                {
                                    string letter = RemoveUnicode(mem[0].userName.Substring(0, 1).ToLower()).ToUpper();
                                    try
                                    {
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                        }
                                        else
                                        {
                                            con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                else
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatarUser/" + mem[0].id.ToString() + "/" + mem[0].avatarUser;
                                }
                            }
                        }
                        if (con.isGroup == 1 && c.memberList.Count > 0) con.status = $"{c.memberList.Count} thành viên";
                    }
                    if (string.IsNullOrEmpty(con.conversationName))
                    {
                        c.memberList.ForEach(m =>
                        {
                            var mem = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == m.memberId).ToList();
                            if (mem.Count > 0)
                            {
                                con.conversationName += mem[0].userName + ",";
                            }
                        });
                        if (con.conversationName.Length > 0) con.conversationName = con.conversationName.Remove(con.conversationName.Length - 1);
                    }
                    if (string.IsNullOrEmpty(con.avatarConversation.Trim()))
                    {
                        string letter = RemoveUnicode(con.conversationName.Substring(0, 1).ToLower()).ToUpper();
                        try
                        {
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                            }
                            else
                            {
                                con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        catch (Exception ex)
                        {
                            con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                        }
                    }
                    else if (!con.avatarConversation.ToLower().Contains("https://mess.timviec365.vn/"))
                    {
                        con.avatarConversation = "https://mess.timviec365.vn/avatarGroup/" + con.conversationId + "/" + con.avatarConversation;
                    }
                }
                if (RemoveUnicode(con.conversationName).ToLower().Contains(RemoveUnicode(mess.ToLower())))
                {
                    count++;
                    if (count > countConversation && list.Count < countLoad)
                    {
                        list.Add(con);
                    }
                    if (c.isGroup == 0 && c.memberList.Count > 1)
                    {
                        int index = c.memberList.FindIndex(x => x.memberId != userId);
                        if (index > -1 && !has.Contains(c.memberList[index].memberId))
                        {
                            has.Add(c.memberList[index].memberId);
                        }
                    }
                }
                if (list.Count >= countLoad) break;
            }

            if (list.Count < countLoad)
            {
                List<UserDB> contac = DAOUsers.GetListContact(userId, 0, 10000);
                if (contac.Count > 0)
                {
                    foreach (UserDB item in contac)
                    {
                        ConversationForward con = new ConversationForward(-1, item.userName, item.avatarUser, 0, item.status);
                        con.contactId = item.id;
                        if (string.IsNullOrEmpty(con.avatarConversation.Trim()))
                        {
                            string letter = RemoveUnicode(con.conversationName.Substring(0, 1).ToLower()).ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            con.avatarConversation = "https://mess.timviec365.vn/avatarUser/" + item.id + "/" + con.avatarConversation;
                        }
                        if (RemoveUnicode(con.conversationName).ToLower().Contains(RemoveUnicode(mess.ToLower())))
                        {
                            count++;
                            if (count > countConversation && list.Count < countLoad)
                            {
                                if (!has.Contains(item.id)) list.Add(con);
                            }
                            if (!has.Contains(item.id)) has.Add(item.id);
                        }
                        if (list.Count >= countLoad) break;
                    }
                }
            }

            if (list.Count < countLoad)
            {
                List<UserDB> listCompany = DAOUsers.searchByCompanyContactInHomePage(mess, userId, companyId, 20 - count);
                if (listCompany.Count > 0)
                {
                    foreach (UserDB item in listCompany)
                    {
                        ConversationForward con = new ConversationForward(-1, item.userName, item.avatarUser, 0, item.status);
                        con.contactId = item.id;
                        if (string.IsNullOrEmpty(con.avatarConversation.Trim()))
                        {
                            string letter = RemoveUnicode(con.conversationName.Substring(0, 1).ToLower()).ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                con.avatarConversation = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            con.avatarConversation = "https://mess.timviec365.vn/avatarUser/" + item.id + "/" + con.avatarConversation;
                        }
                        if (RemoveUnicode(con.conversationName).ToLower().Contains(RemoveUnicode(mess.ToLower())))
                        {
                            count++;
                            if (count > countConversation && list.Count < countLoad)
                            {
                                if (!has.Contains(item.id)) list.Add(con);
                            }
                            if (!has.Contains(item.id)) has.Add(item.id);
                        }
                        if (list.Count >= countLoad) break;
                    }
                }
            }


            return list;
        }
        //lấy info conversation(2/8/22)
        public static DataTable GetInfoConversation(int conversationId)
        {
            List<infoConversation> info = new List<infoConversation>();
            var infoFromDB = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId).ToList();
            if (infoFromDB.Count > 0)
            {
                foreach (ParticipantsDB p in infoFromDB[0].memberList)
                {
                    List<UserDB> us = ConnectDB.database.GetCollection<UserDB>(tableUsers).Find(x => x.id == p.memberId).ToList();
                    if (us.Count > 0)
                    {
                        info.Add(new infoConversation(p.memberId, us[0].email, us[0].type365, us[0].userName, us[0].isOnline, p.conversationName, infoFromDB[0].isGroup));
                    }
                }
            }
            return ConnectDB.toDataTable(info);
        }
        //lấy thông tin member trong cuộc trò chuyện(1/8/22)
        public static List<MemberConversation> getListMemberOfConversation(int conversationId)
        {
            List<MemberConversation> listMember = new List<MemberConversation>();
            List<ConversationsDB> conver = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId).ToList();
            if (conver.Count > 0)
                foreach (var mem in conver[0].memberList)
                {
                    try
                    {
                        var member = ConnectDB.database.GetCollection<UserDB>(tableUsers).Find(x => x.id == mem.memberId).ToList()[0];
                        MemberConversation userInfo = new MemberConversation(member.id, member.userName, member.avatarUser, member.status, member.statusEmotion, member.lastActive.ToString(), member.active, member.isOnline, mem.unReader, member.companyId, mem.timeLastSeener.ToLocalTime(), member.idTimViec, member.type365, "none");

                        if (String.IsNullOrWhiteSpace(userInfo.AvatarUser.Trim()))
                        {
                            string letter = RemoveUnicode(userInfo.UserName.Substring(0, 1).ToLower()).ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            userInfo.LinkAvatar = "https://mess.timviec365.vn/avatarUser/" + userInfo.ID + "/" + userInfo.AvatarUser;
                            userInfo.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + userInfo.ID + "/" + userInfo.AvatarUser;
                        }
                        listMember.Add(userInfo);
                    }
                    catch
                    {

                    }
                }
            return listMember;

        }
        //tính tổng số tin nhắn trong cuộc trò chuyện(1/8/22)
        public static List<ConversationsDB> GetCountMessageOfConversation(int conversationId)
        {
            List<ConversationsDB> cm = new List<ConversationsDB>();
            List<ConversationsDB> c = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.id == conversationId).ToList();
            if (c.Count > 0)
            {
                cm = c;
            }
            return cm;
        }
        //đếm cuộc trò chuyện theo memberid(2/8/22)
        public static int GetCountConversation(int memberId)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.memberList.Count > 0 && x.memberList.Any(p => p.memberId == memberId) && x.messageList.Count > 0).ToList().Count;
        }
        //lấy id admin từ cuộc trò chuyện(1/8/22)
        public static List<ConversationsDB> GetAdminIdOfConversation(int conversationId)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.id == conversationId).ToList();
        }
        //lay info tu nguoi tham gia
        public static List<ConversationsDB> SetupNewAvatarGroup()
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.avatarConversation != "").ToList();
        }
        //lấy id member trong cuộc trò chuyện(2/8/22)
        public static List<ParticipantsDB> GetMemberIdOfConversation(int conversationId)
        {
            List<ParticipantsDB> list = new List<ParticipantsDB>();

            var c = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId).ToList();
            if (c.Count > 0 && c[0].memberList.Count > 0)
            {
                foreach (ParticipantsDB p in c[0].memberList)
                {
                    list.Add(new ParticipantsDB() { memberId = p.memberId });
                }
            }
            return list;

        }
        //tìm người nhắn tin cho bạn trong cuộc trò truyện 2 người(3/8/22)
        public static List<ParticipantsDB> GetContactIdOfConversationSingle(int conversationId, int userId)
        {
            List<ParticipantsDB> listP = new List<ParticipantsDB>();
            var c = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId && x.isGroup == 0 && x.memberList.Count > 0 && x.memberList.Any(p => p.memberId == userId)).ToList();
            if (c.Count > 0 && c[0].memberList.Count > 1)
            {
                listP.Add(c[0].memberList.Where(x => x.memberId != userId).Single());
            }
            return listP;
        }
        //check cuộc trò chuyện rỗng(2/8/22)
        public static List<ConversationsDB> checkEmptyConversation(int userId, int contactId)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.isGroup == 0 && x.typeGroup != "Secret" && x.memberList.Count > 0 && x.memberList.Any(p => p.memberId == userId) && x.memberList.Any(p => p.memberId == contactId)).ToList();
        }
        //check cuộc trò chuyện w247
        public static List<ConversationsDB> checkEmptyConversationLiveChat(int userId, int contactId, string clientId, string typeConversation)
        {
            //string typeG = $"{typeConversation}_{clientId}";
            //return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.typeGroup != "Secret" && x.typeGroup.Contains(typeG) && x.memberList.Count > 0 && x.memberList.Any(p => p.memberId == userId) && x.memberList.Any(p => p.memberId == contactId)).ToList();
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.typeGroup == "liveChat" && x.memberList.Count == 2 && x.memberList.Any(p => p.memberId == userId) && x.memberList.Any(p => p.memberId == contactId) && x.memberList.Any(m => m.liveChat != null && m.liveChat.fromWeb == typeConversation && m.liveChat.clientId == clientId && m.memberId != contactId)).ToList();
        }
        //check cuộc trò chuyện w247
        public static List<ConversationsDB> checkClientConversationLiveChat(string clientId, int contactId, string fromWeb)
        {
            //return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.typeGroup == "liveChat" && x.isGroup==1 && x.memberList.Count==2 && x.messageList.Count > 0 && x.memberList.Any(m => m.memberId==contactId) && x.memberList.Any(m => m.liveChat != null && m.liveChat.fromWeb == fromWeb && m.liveChat.clientId == clientId && m.memberId!=contactId)).ToList();
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.typeGroup == "liveChat" && x.isGroup == 1 && x.messageList.Count > 0 && x.memberList.Any(m => m.liveChat != null && m.liveChat.fromWeb == fromWeb && m.liveChat.clientId == clientId)).ToList();
        }
        //check cuộc trò chuyện bí mật rống
        public static List<ConversationsDB> checkEmptySecretConversation(int userId, int contactId)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.isGroup == 0 && x.typeGroup == "Secret" && x.memberList.Count > 0 && x.memberList.Any(p => p.memberId == userId) && x.memberList.Any(p => p.memberId == contactId)).ToList();
        }
        //thêm cuộc trò chuyện mới(1/8/22)
        public static int insertNewConversation(int isGroup, string TypeGroup, int adminId)
        {
            try
            {
                int conversationId = DAOCounter.getNextID("ConversationID");
                var document = new ConversationsDB(conversationId, isGroup, TypeGroup, "", adminId, 1, 1, "", new List<ParticipantsDB>(), new List<MessagesDB>(), new List<BrowseMembersDB>(), DateTime.Now,DateTime.Now,DateTime.Now);
                ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).InsertOne(document);
                DAOCounter.updateID("ConversationID");
                return conversationId;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //thêm thành viên mới(1/8/22)
        public static int insertNewParticipant(int conversationId, string conversationName, int isGroup, int[] listmember, int adminId, string typeGroup)
        {
            try
            {
                int countMember = 0;
                var c = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId).ToList();
                List<int> memberList = listmember.ToList();
                if (c.Count > 0)
                {
                    memberList.RemoveAll(x => c[0].memberList.Any(m=>m.memberId==x));
                }
                for (int i = 0; i < memberList.Count; i++)
                {
                    int haveReader = 0;
                    if (memberList[i] != adminId)
                    {
                        if (isGroup == 1)
                        {
                            haveReader = 1 + memberList.Count;
                        }
                        else
                        {
                            haveReader = 0;
                        }
                    }


                    if (c.Count > 0)
                    {
                        int del = typeGroup == "Secret" ? 10 : 0;
                        int type = typeGroup == "Secret" ? 1 : 0;
                        if (c[0].memberList.Count > 0)
                        {
                            del = typeGroup == "Secret" ? 10 : c[0].memberList[0].deleteTime;
                            type = typeGroup == "Secret" ? 1 : c[0].memberList[0].deleteType;
                        }


                        if (isGroup == 1 && string.IsNullOrEmpty(conversationName))
                        {
                            foreach (var mem in c[0].memberList)
                            {
                                conversationName = mem.conversationName;
                                if (!string.IsNullOrEmpty(conversationName)) break;
                            }
                        }

                        var filter = Builders<BsonDocument>.Filter.Eq("_id", conversationId);
                        ParticipantsDB member = new ParticipantsDB(memberList[i], conversationName, haveReader, 0, 0, 0, 1, DateTime.Now, del, type);
                        var update = Builders<BsonDocument>.Update.Push("memberList", member);
                        ConnectDB.database.GetCollection<BsonDocument>(tableConversations).UpdateOne(filter, update);
                    }

                    countMember++;
                }
                return countMember;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        //update live chat
        public static int UpdateLiveChat(string clientId, string clientName, string fromWeb, int conversationId, int userId)
        {
            var filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Where(x => x.memberId == userId));
            var update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].liveChat, new LiveChatDB(clientId, clientName, fromWeb));
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        //lấy tt các cuộc trò chuyện(2/8/22)
        public static List<ConversationsDB> getAllGroup(int memberId)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.memberList.Count > 0 && x.memberList.Any(p => p.memberId == memberId) && x.isGroup==1).ToList();
        }
        //nhập mới member
        public static int InsertNewBrowserMember(int conversationId, int memberBrowserId, int memberAddId)
        {
            try
            {
                var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId);
                BrowseMembersDB BrowseMember = new BrowseMembersDB() { memberBrowserId = memberBrowserId, memberAddId = memberAddId };
                var update = Builders<ConversationsDB>.Update.Push("browseMemberList", BrowseMember);
                ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //check empty Browser menber
        public static bool CheckEmptyBrowserMember(int conversationId, int memberBrowserId)
        {
            var tg = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId && x.browseMemberList.Any(y => y.memberBrowserId == memberBrowserId)).ToList();
            if (tg.Count > 0) return false;
            else return true;
        }
        //sửa ngày 3/8
        //kiểm tra empty thanh vien
        public static bool CheckEmptyMember(int conversationId, int memberId)
        {
            var tg = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId && x.memberList.Any(y => y.memberId == memberId)).ToList();
            if (tg.Count > 0) return true;
            return false;
        }
        //lâm sửa ngày 3/8
        //xoá thành viên chờ duyệt
        public static int DeleteBrowse(int conversationId, int[] listmember)
        {
            int countMember = 0;
            foreach (int member in listmember)
            {
                var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId && x.browseMemberList.Count > 0 && x.browseMemberList.Any(m => m.memberBrowserId == member));
                var update = Builders<ConversationsDB>.Update.PullFilter(x => x.browseMemberList, Builders<BrowseMembersDB>.Filter.Eq(x => x.memberBrowserId, member));
                var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                countMember++;
            }
            return countMember;

        }
        //cập nhật lại trạng thái đã đọc hay chưa(3/8/22)
        public static int ReadMessage(int conversationId, int userId)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].unReader, 0).Set(x => x.memberList[-1].timeLastSeener, DateTime.Now);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;

        }
        //cập nhật lại avatar của cuộc trò chuyện(3/8/22)
        public static int ChangeAvatarGroup(int conversationId, string avatar)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set(x => x.avatarConversation, avatar);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        // cập nhật lại thây đổi chia se link của nhóm(3/8/22)
        public static int ChangeShareLinkOfGroup(int conversationId, int shareLink)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("shareGroupFromLinkOption", shareLink);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;

        }
        //cập nhật lại trang thái ghim tin nhắn(3/8/22)
        public static int PinMessage(int conversationId, string pinMessage)
        {
            var filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId);
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set("pinMessage", pinMessage);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        // cập nhật lại trang thái của thông báo(3/8/22)
        public static int ChangeNotificationConversation(int conversationId, int notification, int userId)
        {
            var filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Where(x => x.memberId == userId));
            var update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].notification, notification);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        //cập nhật lại trang thái của browse member(2/8/22)
        public static int ChangeBowserMemberOfGroup(int conversationId, int browseMember)
        {
            var filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId);
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set("browseMemberOption", browseMember);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        //cập nhật lại tên cuộc trò chuyện
        public static int ChangeNameGroup(int conversationId, string namegroup)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("memberList.$[].conversationName", namegroup);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;

            return 0;
        }
        //thay đổi ních name(2/8/22)
        public static int ChangeNickName(int conversationId, string namegroup, int userId)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].conversationName, namegroup);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        //hiển thị số tin tắt chưa đọc
        public static int MarkUnreader(int conversationId, int userId)
        {

            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Inc(x => x.memberList[-1].unReader, 1);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        //chưa tối ưu
        //03/08
        //tắt hiển thị số tin nhắn đã đọc
        public static int MarkUnreaderMessage(int conversationId, int userId, int[] listMember)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Inc("memberList.$[f].unReader", 1);
            var arrayFilters = new[] { new BsonDocumentArrayFilterDefinition<BsonDocument>(new BsonDocument("f.memberId", new BsonDocument("$ne", new BsonArray(new[] { userId })))), };
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update, new UpdateOptions { ArrayFilters = arrayFilters });

            FilterDefinition<ConversationsDB> filterPaticipant = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            UpdateDefinition<ConversationsDB> updatePaticipant = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].unReader, 0).Set(x => x.memberList[-1].timeLastSeener, DateTime.Now);
            var checkPaticipant = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filterPaticipant, updatePaticipant);

            if (checkPaticipant.ModifiedCount > 0)
            {
                return 1;
            }
            else return 0;

        }
        //lấy tin nhắn cuối cùng trong cuộc trò chuyện(2/8/22)
        public static Int64 GetLastMessageConversation(int conversationId)
        {
            var c = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId).ToList();
            if (c.Count > 0 && c[0].messageList.Count > 0)
            {
                List<MessagesDB> mess = c[0].messageList.OrderByDescending(x => x.displayMessage).ToList();
                return mess[0].displayMessage;
            }
            else return 0;
        }
        //thoát nhóm(2/8/22)
        public static int OutGroup(int conversationId, int userId)
        {
            var filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId);
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.PullFilter(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //cập nhật admin cho nhóm(2/8/22)
        public static int UpdateAdminOfgroup(int conversationId, int userId)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId);
            var update = Builders<ConversationsDB>.Update.Set("adminId", userId);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            else return 0;
        }
        //xoá cuộc trò chuyện(2/8/22)
        public static int DeleteConversation(int conversationId, int userId, Int64 messageDisplay)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].messageDisplay, messageDisplay);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            else return 0;

        }
        //xoá cuộc trò chuyện(2/8/22)
        public static int RemoveConversation(int conversationId)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).DeleteOne(filter);
            if (check.DeletedCount > 0) return 1;
            else return 0;

        }
        //ẩn cuộc trò chuyện(2/8/22)
        public static int HiddenConversation(int conversationId, int userId, int isHidden)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            var update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].isHidden, isHidden);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            else return 0;

        }
        //thêm cuộc trò chuyện yêu thích(2/8/22)
        public static int AddToFavoriteConversation(int conversationId, int userId, int isFavorite)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(x => x.memberId, userId));
            var update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].isFavorite, isFavorite);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            else return 0;
        }
        //mới 
        public static int GetCountConversationUnreader(int userId)
        {
            return ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.memberList.Any(y => y.memberId == userId && y.unReader != 0)).ToList().Count;
        }

        public static int UpdateDeleteTime(int userId, int conversationId, int deleteTime)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(y => y.memberId, userId));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].deleteTime, deleteTime).Set(x => x.memberList[-1].deleteType, 1);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            else return 0;
        }

        public static int SetupDeleteTime(int userId, int conversationId, int deleteTime, int deleteType)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId);
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set("memberList.$[].deleteTime", deleteTime).Set("memberList.$[].deleteType", deleteType);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            else return 0;
        }

        public static List<ParticipantsDB> getListMemberIdOfConversation(int conversationId)
        {
            List<ParticipantsDB> listMember = new List<ParticipantsDB>();
            List<ConversationsDB> conver = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId).ToList();
            if (conver.Count > 0) listMember = conver[0].memberList;
            return listMember;
        }
    }
}
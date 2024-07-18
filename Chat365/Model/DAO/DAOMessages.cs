using APIChat365.MongoEntity;
using APIChat365.Model.MongoEntity;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Chat365.Server.Model.Entity;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using MongoDB.Driver.Builders;
using Chat365.Model.Entity;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.Mail;
using System.Net;
using VisioForge.Libs.MediaFoundation.OPM;

namespace Chat365.Server.Model.DAO
{
    public class DAOMessages
    {
        //public static string tableMess = "Messages";
        //public static string tableFileSend = "FileSend";
        //public static string tableEmotionMess = "EmotionMessage";
        //public static string tableInfoLink = "InfoLink";
        //public static string tableParticipants = "participants";
        public static string tableConversations = "Conversations";

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
        public static int UpdateMap(string messageId, string message, string image)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].message, message).Set(x => x.messageList[-1].infoLink.image, image);
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static List<Messages> GetListFavoriteMessage(int userId, int countMess, int countLoad)
        {
            List<Messages> list = new List<Messages>();
            List<ConversationsDB> conver = ConnectDB.database.GetCollection<ConversationsDB>(ConnectDB.tblConversations).Find(x => x.memberList.Count > 0 && x.memberList.Any(m => m.memberId == userId && m.favoriteMessage.Count > 0)).SortByDescending(x => x.timeLastMessage).ToList();
            int count = 0;
            foreach (var c in conver)
            {
                int memIndex = c.memberList.FindIndex(m => m.memberId == userId);
                if (memIndex > -1)
                {
                    List<string> listFavo = c.memberList[memIndex].favoriteMessage.OrderByDescending(x => x).ToList();
                    List<MessagesDB> listMess = c.messageList.Where(x => listFavo.Contains(x.id)).ToList();
                    foreach (var mess in listMess)
                    {
                        try
                        {
                            Messages messages = new Messages(mess.id, c.id, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime, mess.deleteType);
                            if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                            {
                                messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image, mess.infoLink.isNotification);
                                if (messages.InfoLink.HaveImage == "True") messages.InfoLink.Image.Replace("amp;", "");
                            }
                            if (mess.listFile != null && mess.listFile.Count != 0)
                            {
                                messages.ListFile = new List<InfoFile>();
                                foreach (var file in mess.listFile)
                                {
                                    messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                                }
                            }
                            if (mess.emotion != null)
                            {
                                messages.EmotionMessage = new List<Chat365.Model.Entity.Emotion>();
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion1.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(1, mess.emotion.Emotion1.Split(","), "https://mess.timviec365.vn/Emotion/Emotion1.png"));
                                }
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion2.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(2, mess.emotion.Emotion2.Split(","), "https://mess.timviec365.vn/Emotion/Emotion2.png"));
                                }
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion3.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(3, mess.emotion.Emotion3.Split(","), "https://mess.timviec365.vn/Emotion/Emotion3.png"));
                                }
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion4.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(4, mess.emotion.Emotion4.Split(","), "https://mess.timviec365.vn/Emotion/Emotion4.png"));
                                }
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion5.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(5, mess.emotion.Emotion5.Split(","), "https://mess.timviec365.vn/Emotion/Emotion5.png"));
                                }
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion6.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(6, mess.emotion.Emotion6.Split(","), "https://mess.timviec365.vn/Emotion/Emotion6.png"));
                                }
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion7.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(7, mess.emotion.Emotion7.Split(","), "https://mess.timviec365.vn/Emotion/Emotion7.png"));
                                }
                                if (!String.IsNullOrEmpty(mess.emotion.Emotion8.Trim()))
                                {
                                    messages.EmotionMessage.Add(new Emotion(8, mess.emotion.Emotion8.Split(","), "https://mess.timviec365.vn/Emotion/Emotion8.png"));
                                }
                            }
                            if (!string.IsNullOrEmpty(mess.quoteMessage))
                            {
                                Messages quote = DAOMessages.GetMessageById(mess.quoteMessage);
                                List<UserDB> sender = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == quote.SenderID).Limit(1).ToList();
                                if (sender.Count > 0)
                                {
                                    messages.QuoteMessage = new MessageQuote(quote.MessageID, sender[0].userName, quote.SenderID, quote.MessageType, quote.Message, quote.CreateAt.ToLocalTime());
                                }
                            }
                            if (mess.messageType.Equals("sendProfile"))
                            {
                                UserDB memberDB = DAOUsers.GetUserById(Convert.ToInt32(mess.message))[0];
                                messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                                if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                                {
                                    string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                    try
                                    {
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                        }
                                        else
                                        {
                                            messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                else
                                {
                                    messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                    messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                                }

                            }
                            if (mess.listFile != null && mess.listFile.Count != 0 && mess.isEdited == 1 && mess.senderId != userId) continue;

                            if (!list.Any(x => x.MessageID == messages.MessageID) && list.Count < countLoad)
                            {
                                count++;
                                if (count > countMess)
                                {
                                    messages.IsFavorite = 1;
                                    list.Add(messages);
                                }
                            }
                            if (list.Count > countLoad) break;
                        }
                        catch { }
                    }
                }
                if (list.Count == countLoad) break;
            }

            return list.OrderBy(x => x.CreateAt).ToList();
        }
        public static int CheckFavoriteMessage(int conversationId, int userId, string messageId)
        {
            List<ConversationsDB> conver = ConnectDB.database.GetCollection<ConversationsDB>(ConnectDB.tblConversations).Find(x => x.id == conversationId && x.memberList.Count > 0 && x.memberList.Any(m => m.memberId == userId)).ToList();
            if (conver.Count > 0)
            {
                ConversationsDB c = conver[0];
                int index = c.memberList.FindIndex(x => x.memberId == userId);
                if (index > -1 && c.memberList[index].favoriteMessage != null && c.memberList[index].favoriteMessage.Contains(messageId))
                {
                    return 1;
                }
                if (c.memberList[index].favoriteMessage == null)
                {
                    FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId && x.memberList.Count > 0 && x.memberList.Any(m => m.memberId == userId)) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(y => y.memberId, userId));
                    UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set(x => x.memberList[-1].favoriteMessage, new List<string>());
                    var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
                }
            }
            return 0;
        }
        public static int SetClicked(int conversationId, int userId, string messageId)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId && x.memberList.Count > 0 && x.memberList.Any(m => m.memberId == userId)) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(y => y.id, messageId));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Push(x => x.messageList[-1].notiClicked, userId);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int SetFavoriteMessage(int conversationId, int userId, string messageId)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId && x.memberList.Count > 0 && x.memberList.Any(m => m.memberId == userId)) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(y => y.memberId, userId));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Push(x => x.memberList[-1].favoriteMessage, messageId);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static int RemoveFavoriteMessage(int conversationId, int userId, string messageId)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId && x.memberList.Count > 0 && x.memberList.Any(m => m.memberId == userId)) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.memberList, Builders<ParticipantsDB>.Filter.Eq(y => y.memberId, userId));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Pull(x => x.memberList[-1].favoriteMessage, messageId);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateMany(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        public static UserDB CheckFirstMessageInDay(int conversationId, DateTime createAt, int userId)
        {
            List<ConversationsDB> conver = ConnectDB.database.GetCollection<ConversationsDB>(ConnectDB.tblConversations).Find(x => x.id == conversationId && x.isGroup == 0 && x.typeGroup != "Secret" && x.memberList.Count > 1 && x.memberList.Any(m => m.memberId == userId)).ToList();
            if (conver.Count > 0)
            {
                int flag = 0;
                if (conver[0].messageList.Count == 0) flag = 1;
                else if (createAt.Date > conver[0].timeLastMessage.Date) flag = 1;

                if (flag == 1)
                {
                    int index = conver[0].memberList.FindIndex(x => x.memberId != userId);
                    if (index > -1)
                    {
                        List<UserDB> users = ConnectDB.database.GetCollection<UserDB>(ConnectDB.tblUsers).Find(x => x.id == conver[0].memberList[index].memberId).Limit(1).ToList();
                        if (users.Count > 0 && users[0].email.Contains("@") && users[0].isOnline == 0)
                        {
                            return users[0];
                        }
                    }
                }
            }
            return null;
        }
        //đã sửa 02/08
        //lấy tin nhắn theo id cuộc trò chuyện
        public static int GetCountMessage(int conversationId, int userId, Int64 messDisplay)
        {
            int count = 0;
            ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId && x.messageList.Count > 0).ToList().ForEach(c =>
            {
                var lismessageDB = c.messageList.Where(x => x.displayMessage > messDisplay).ToList();
                for (int i = 0; i < lismessageDB.Count; i++)
                {
                    if (lismessageDB[i].listFile != null && lismessageDB[i].listFile.Count != 0 && lismessageDB[i].isEdited == 1 && lismessageDB[i].senderId != userId) continue;
                    count++;
                }
            });
            return count;
        }
        public static List<Messages> GetMessageByConversatinId(int conversationId, int countMess, int countLoad, Int64 messDisplay, int userId, Int64 loadTo)
        {
            List<Messages> list = new List<Messages>();
            List<string> componentMessType = new List<string>() { "applying", "document", "OfferReceive", "newCandidate" };
            ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId && x.messageList.Count > 0).ToList().ForEach(c =>
            {
                List<MessagesDB> l = new List<MessagesDB>();
                var lismessageDB = c.messageList.Where(x => x.displayMessage > messDisplay).ToList();
                if (loadTo > 0 && loadTo > messDisplay)
                {
                    lismessageDB = lismessageDB.Where(x => x.displayMessage < loadTo).ToList();
                    l = lismessageDB.Skip(0).Take(20).ToList();
                }
                else l = lismessageDB.Skip(lismessageDB.Count - countMess - countLoad).Take(countLoad).ToList();
                List<string> favo = new List<string>();
                int index = c.memberList.FindIndex(x => x.memberId == userId);
                if (index > -1 && c.memberList[index].favoriteMessage != null) favo = c.memberList[index].favoriteMessage;
                foreach (var mess in l)
                {
                    try
                    {
                        Messages messages = new Messages(mess.id, conversationId, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime, mess.deleteType);
                        int indeLink = c.messageList.FindIndex(x => x.id == mess.id);
                        if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                        {
                            messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image, mess.infoLink.isNotification);
                            if (messages.InfoLink.HaveImage == "True") messages.InfoLink.Image.Replace("amp;", "");
                            if (messages.InfoLink.LinkHome == null) messages.InfoLink.LinkHome = "";
                            if (indeLink - 1 >= 0 && componentMessType.Contains(c.messageList[indeLink - 1].messageType)) messages.InfoLink.TypeLink = c.messageList[indeLink - 1].messageType;
                        }
                        if (indeLink > -1 && indeLink + 1 < c.messageList.Count && c.messageList[indeLink + 1].messageType == "link" && componentMessType.Contains(c.messageList[indeLink].messageType))
                        {
                            MessagesDB t = c.messageList[indeLink + 1];
                            messages.InfoLink = new InfoLink(t.id, t.infoLink.title, t.infoLink.description, t.infoLink.linkHome, t.infoLink.image, t.infoLink.isNotification);
                            messages.InfoLink.TypeLink = c.messageList[indeLink].messageType;
                            messages.LinkNotification = t.message;
                        }
                        if (mess.listFile != null && mess.listFile.Count != 0)
                        {
                            messages.ListFile = new List<InfoFile>();
                            foreach (var file in mess.listFile)
                            {
                                messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                            }
                        }
                        if (mess.emotion != null)
                        {
                            messages.EmotionMessage = new List<Chat365.Model.Entity.Emotion>();
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion1.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(1, mess.emotion.Emotion1.Split(","), "http://43.239.223.142:3005/Emotion/Emotion1.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion2.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(2, mess.emotion.Emotion2.Split(","), "http://43.239.223.142:3005/Emotion/Emotion2.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion3.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(3, mess.emotion.Emotion3.Split(","), "http://43.239.223.142:3005/Emotion/Emotion3.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion4.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(4, mess.emotion.Emotion4.Split(","), "http://43.239.223.142:3005/Emotion/Emotion4.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion5.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(5, mess.emotion.Emotion5.Split(","), "http://43.239.223.142:3005/Emotion/Emotion5.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion6.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(6, mess.emotion.Emotion6.Split(","), "http://43.239.223.142:3005/Emotion/Emotion6.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion7.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(7, mess.emotion.Emotion7.Split(","), "http://43.239.223.142:3005/Emotion/Emotion7.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion8.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(8, mess.emotion.Emotion8.Split(","), "http://43.239.223.142:3005/Emotion/Emotion8.png"));
                            }
                        }
                        if (!string.IsNullOrEmpty(mess.quoteMessage))
                        {
                            Messages quote = GetMessageById(mess.quoteMessage);
                            List<UserDB> sender = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == quote.SenderID).Limit(1).ToList();
                            if (sender.Count > 0)
                            {
                                messages.QuoteMessage = new MessageQuote(quote.MessageID, sender[0].userName, quote.SenderID, quote.MessageType, quote.Message, quote.CreateAt.ToLocalTime());
                            }
                        }
                        if (mess.messageType.Equals("sendProfile"))
                        {
                            UserDB memberDB = DAOUsers.GetUserById(Convert.ToInt32(mess.message))[0];
                            messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                            if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                            {
                                string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            else
                            {
                                messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                            }

                        }
                        if (mess.listFile != null && mess.listFile.Count != 0 && mess.isEdited == 1 && mess.senderId != userId) continue;
                        if (favo.Contains(mess.id)) messages.IsFavorite = 1;
                        if (mess.liveChat != null && !string.IsNullOrEmpty(mess.liveChat.clientId))
                        {
                            messages.LiveChat = new InfoLiveChat(mess.liveChat.clientId, mess.liveChat.clientName, "", mess.liveChat.fromWeb);
                            if (string.IsNullOrEmpty(messages.LiveChat.ClientName)) messages.LiveChat.ClientName = messages.LiveChat.ClientId;
                            if (String.IsNullOrWhiteSpace(messages.LiveChat.ClientAvatar.Trim()))
                            {
                                string letter = RemoveUnicode(messages.LiveChat.ClientName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                        }
                        if (mess.messageType == "newCandidate" && mess.message.Contains("&candidateId"))
                        {
                            List<string> list = (mess.message.Split('&')).ToList();
                            messages.Message = list.Count > 0 ? list[0] : messages.Message;
                            if (list.Count > 1)
                            {
                                list.RemoveAt(0);
                                int find = list.FindIndex(x => x.Contains("candidateId"));
                                if (find > -1)
                                {
                                    int idc;
                                    string ck = list[find].Substring(12);
                                    if (!string.IsNullOrEmpty(ck) && int.TryParse(ck, out idc))
                                    {
                                        var getuser = DAOUsers.GetInforUserById(idc);
                                        if (getuser.Count > 0)
                                        {
                                            var memberDB = getuser[0];
                                            messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                                            if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                                            {
                                                string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                                try
                                                {
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                                    }
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                                    }
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                                    }
                                                    else
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                                }
                                            }
                                            else
                                            {
                                                messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                                messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (mess.infoSupport != null)
                        {
                            messages.InfoSupport = new InfoSupport();
                            messages.InfoSupport.Title = mess.infoSupport.title;
                            messages.InfoSupport.Message = mess.infoSupport.message;
                            if (mess.infoSupport.userId != 0)
                            {
                                var users = DAOUsers.GetInforUserById(mess.infoSupport.userId);
                                if (users.Count > 0)
                                {
                                    messages.InfoSupport.UserId = users[0].id;
                                    messages.InfoSupport.userName = users[0].userName;
                                }
                            }
                            messages.InfoSupport.SupportId = mess.infoSupport.supportId;
                            messages.InfoSupport.Status = mess.infoSupport.status;
                            messages.InfoSupport.Time = mess.infoSupport.time.ToLocalTime();
                            messages.InfoSupport.HaveConversation = mess.infoSupport.haveConversation;
                        }
                        if (componentMessType.Contains(mess.messageType) && mess.notiClicked != null && mess.notiClicked.Contains(userId)) messages.IsClicked = 1;
                        list.Add(messages);
                    }
                    catch { }
                }
            });
            return list;
        }
        public static List<Messages_v2> GetMessageForLiveChat(string clientId, string fromWeb, int countMess, int countLoad)
        {
            List<Messages_v2> list = new List<Messages_v2>();
            List<string> componentMessType = new List<string>() { "applying", "document", "OfferReceive" };
            List<ConversationsDB> conversations = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.messageList.Count > 0 && x.memberList.Any(m => m.liveChat != null && m.liveChat.fromWeb == fromWeb && m.liveChat.clientId == clientId)).ToList();
            if (conversations.Count > 0)
            {
                ConversationsDB c = conversations[0];
                List<MessagesDB> l = new List<MessagesDB>();
                l = c.messageList.OrderByDescending(x => x.displayMessage).Skip(countMess).Take(countLoad).ToList();
                List<string> favo = new List<string>();
                foreach (var mess in l)
                {
                    try
                    {
                        Messages_v2 messages = new Messages_v2(mess.id, c.id, mess.senderId, mess.displayMessage, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm"), mess.deleteDate.ToLocalTime(), mess.deleteTime, mess.deleteType);
                        if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                        {
                            messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image, mess.infoLink.isNotification);
                            if (messages.InfoLink.HaveImage == "True") messages.InfoLink.Image.Replace("amp;", "");
                            if (messages.InfoLink.LinkHome == null) messages.InfoLink.LinkHome = "";
                        }
                        int indeLink = c.messageList.FindIndex(x => x.id == mess.id);
                        if (indeLink > -1 && indeLink + 1 < c.messageList.Count && c.messageList[indeLink + 1].messageType == "link" && componentMessType.Contains(c.messageList[indeLink].messageType))
                        {
                            MessagesDB t = c.messageList[indeLink + 1];
                            messages.InfoLink = new InfoLink(t.id, t.infoLink.title, t.infoLink.description, t.infoLink.linkHome, t.infoLink.image, t.infoLink.isNotification);
                            messages.LinkNotification = t.message;
                        }
                        if (mess.listFile != null && mess.listFile.Count != 0)
                        {
                            messages.ListFile = new List<InfoFile>();
                            foreach (var file in mess.listFile)
                            {
                                messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                            }
                        }
                        if (mess.emotion != null)
                        {
                            messages.EmotionMessage = new List<Chat365.Model.Entity.Emotion>();
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion1.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(1, mess.emotion.Emotion1.Split(","), "https://mess.timviec365.vn/Emotion/Emotion1.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion2.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(2, mess.emotion.Emotion2.Split(","), "https://mess.timviec365.vn/Emotion/Emotion2.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion3.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(3, mess.emotion.Emotion3.Split(","), "https://mess.timviec365.vn/Emotion/Emotion3.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion4.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(4, mess.emotion.Emotion4.Split(","), "https://mess.timviec365.vn/Emotion/Emotion4.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion5.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(5, mess.emotion.Emotion5.Split(","), "https://mess.timviec365.vn/Emotion/Emotion5.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion6.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(6, mess.emotion.Emotion6.Split(","), "https://mess.timviec365.vn/Emotion/Emotion6.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion7.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(7, mess.emotion.Emotion7.Split(","), "https://mess.timviec365.vn/Emotion/Emotion7.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion8.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(8, mess.emotion.Emotion8.Split(","), "https://mess.timviec365.vn/Emotion/Emotion8.png"));
                            }
                        }
                        if (!string.IsNullOrEmpty(mess.quoteMessage))
                        {
                            Messages quote = GetMessageById(mess.quoteMessage);
                            List<UserDB> sender = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == quote.SenderID).Limit(1).ToList();
                            if (sender.Count > 0)
                            {
                                messages.QuoteMessage = new MessageQuote(quote.MessageID, sender[0].userName, quote.SenderID, quote.MessageType, quote.Message, quote.CreateAt.ToLocalTime());
                            }
                        }
                        if (mess.messageType.Equals("sendProfile"))
                        {
                            UserDB memberDB = DAOUsers.GetUserById(Convert.ToInt32(mess.message))[0];
                            messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                            if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                            {
                                string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            else
                            {
                                messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                            }

                        }
                        if (mess.liveChat != null && !string.IsNullOrEmpty(mess.liveChat.clientId))
                        {
                            messages.LiveChat = new InfoLiveChat(mess.liveChat.clientId, mess.liveChat.clientName, "", mess.liveChat.fromWeb);
                            if (string.IsNullOrEmpty(messages.LiveChat.ClientName)) messages.LiveChat.ClientName = messages.LiveChat.ClientId;
                            if (String.IsNullOrWhiteSpace(messages.LiveChat.ClientAvatar.Trim()))
                            {
                                string letter = RemoveUnicode(messages.LiveChat.ClientName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                        }
                        if (mess.messageType == "newCandidate" && mess.message.Contains("&candidateId"))
                        {
                            List<string> listSplit = (mess.message.Split('&')).ToList();
                            messages.Message = listSplit.Count > 0 ? listSplit[0] : messages.Message;
                            if (listSplit.Count > 1)
                            {
                                listSplit.RemoveAt(0);
                                int find = listSplit.FindIndex(x => x.Contains("candidateId"));
                                if (find > -1)
                                {
                                    int idc;
                                    string ck = listSplit[find].Substring(12);
                                    if (!string.IsNullOrEmpty(ck) && int.TryParse(ck, out idc))
                                    {
                                        var getuser = DAOUsers.GetInforUserById(idc);
                                        if (getuser.Count > 0)
                                        {
                                            var memberDB = getuser[0];
                                            messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                                            if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                                            {
                                                string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                                try
                                                {
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                                    }
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                                    }
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                                    }
                                                    else
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                                }
                                            }
                                            else
                                            {
                                                messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                                messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (mess.infoSupport != null)
                        {
                            messages.InfoSupport = new InfoSupport();
                            messages.InfoSupport.Title = mess.infoSupport.title;
                            messages.InfoSupport.Message = mess.infoSupport.message;
                            if (mess.infoSupport.userId != 0)
                            {
                                var users = DAOUsers.GetInforUserById(mess.infoSupport.userId);
                                if (users.Count > 0)
                                {
                                    messages.InfoSupport.UserId = users[0].id;
                                    messages.InfoSupport.userName = users[0].userName;
                                }
                            }
                            messages.InfoSupport.SupportId = mess.infoSupport.supportId;
                            messages.InfoSupport.Status = mess.infoSupport.status;
                            messages.InfoSupport.Time = mess.infoSupport.time.ToLocalTime();
                            messages.InfoSupport.HaveConversation = mess.infoSupport.haveConversation;
                        }
                        if (favo.Contains(mess.id)) messages.IsFavorite = 1;
                        list.Add(messages);
                    }
                    catch { }
                }
            }
            return list.OrderBy(x => x.MessageID).ToList();
        }
        public static List<Messages_v2> GetMessageForLiveChat_v2(ConversationsDB conversation, int countMess, int countLoad, string clientId)
        {
            List<Messages_v2> list = new List<Messages_v2>();
            List<string> componentMessType = new List<string>() { "applying", "document", "OfferReceive" };
            if (conversation.messageList.Count > 0)
            {
                List<MessagesDB> l = new List<MessagesDB>();
                l = conversation.messageList.OrderByDescending(x => x.displayMessage).Skip(countMess).Take(countLoad).ToList();
                List<string> favo = new List<string>();
                ParticipantsDB currentMember = null;
                int indexUser = conversation.memberList.FindIndex(x => x.liveChat != null && !string.IsNullOrEmpty(x.liveChat.clientId) && x.liveChat.clientId == clientId);
                if (indexUser > -1) currentMember = conversation.memberList[indexUser];
                foreach (var mess in l)
                {
                    try
                    {
                        Messages_v2 messages = new Messages_v2(mess.id, conversation.id, mess.senderId, mess.displayMessage, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), mess.deleteDate.ToLocalTime(), mess.deleteTime, mess.deleteType);
                        if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                        {
                            messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image, mess.infoLink.isNotification);
                            if (messages.InfoLink.HaveImage == "True") messages.InfoLink.Image.Replace("amp;", "");
                            if (messages.InfoLink.LinkHome == null) messages.InfoLink.LinkHome = "";
                        }
                        int indeLink = conversation.messageList.FindIndex(x => x.id == mess.id);
                        if (indeLink > -1 && indeLink + 1 < conversation.messageList.Count && conversation.messageList[indeLink + 1].messageType == "link" && componentMessType.Contains(conversation.messageList[indeLink].messageType))
                        {
                            MessagesDB t = conversation.messageList[indeLink + 1];
                            messages.InfoLink = new InfoLink(t.id, t.infoLink.title, t.infoLink.description, t.infoLink.linkHome, t.infoLink.image, t.infoLink.isNotification);
                            messages.LinkNotification = t.message;
                        }
                        messages.ListFile = new List<InfoFile>();
                        if (mess.listFile != null && mess.listFile.Count != 0)
                        {
                            foreach (var file in mess.listFile)
                            {
                                messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                            }
                        }
                        if (mess.emotion != null)
                        {
                            messages.EmotionMessage = new List<Chat365.Model.Entity.Emotion>();
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion1.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(1, mess.emotion.Emotion1.Split(","), "https://mess.timviec365.vn/Emotion/Emotion1.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion2.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(2, mess.emotion.Emotion2.Split(","), "https://mess.timviec365.vn/Emotion/Emotion2.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion3.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(3, mess.emotion.Emotion3.Split(","), "https://mess.timviec365.vn/Emotion/Emotion3.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion4.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(4, mess.emotion.Emotion4.Split(","), "https://mess.timviec365.vn/Emotion/Emotion4.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion5.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(5, mess.emotion.Emotion5.Split(","), "https://mess.timviec365.vn/Emotion/Emotion5.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion6.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(6, mess.emotion.Emotion6.Split(","), "https://mess.timviec365.vn/Emotion/Emotion6.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion7.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(7, mess.emotion.Emotion7.Split(","), "https://mess.timviec365.vn/Emotion/Emotion7.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion8.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(8, mess.emotion.Emotion8.Split(","), "https://mess.timviec365.vn/Emotion/Emotion8.png"));
                            }
                        }
                        if (!string.IsNullOrEmpty(mess.quoteMessage))
                        {
                            Messages quote = GetMessageById(mess.quoteMessage);
                            List<UserDB> sender = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == quote.SenderID).Limit(1).ToList();
                            if (sender.Count > 0)
                            {
                                messages.QuoteMessage = new MessageQuote(quote.MessageID, sender[0].userName, quote.SenderID, quote.MessageType, quote.Message, quote.CreateAt.ToLocalTime());
                            }
                        }
                        if (mess.messageType.Equals("sendProfile"))
                        {
                            UserDB memberDB = DAOUsers.GetUserById(Convert.ToInt32(mess.message))[0];
                            messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                            if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                            {
                                string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            else
                            {
                                messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                            }

                        }
                        if (mess.liveChat != null && !string.IsNullOrEmpty(mess.liveChat.clientId))
                        {
                            messages.LiveChat = new InfoLiveChat(mess.liveChat.clientId, mess.liveChat.clientName, "", mess.liveChat.fromWeb);
                            if (string.IsNullOrEmpty(messages.LiveChat.ClientName)) messages.LiveChat.ClientName = messages.LiveChat.ClientId;
                            if (String.IsNullOrWhiteSpace(messages.LiveChat.ClientAvatar.Trim()))
                            {
                                string letter = RemoveUnicode(messages.LiveChat.ClientName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                        }
                        if (favo.Contains(mess.id)) messages.IsFavorite = 1;
                        if (mess.liveChat != null && !string.IsNullOrEmpty(mess.liveChat.clientId))
                        {
                            messages.LiveChat = new InfoLiveChat(mess.liveChat.clientId, mess.liveChat.clientName, "", mess.liveChat.fromWeb);
                            if (string.IsNullOrEmpty(messages.LiveChat.ClientName)) messages.LiveChat.ClientName = messages.LiveChat.ClientId;
                            if (String.IsNullOrWhiteSpace(messages.LiveChat.ClientAvatar.Trim()))
                            {
                                string letter = RemoveUnicode(messages.LiveChat.ClientName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                        }
                        if (mess.messageType == "newCandidate" && mess.message.Contains("&candidateId"))
                        {
                            List<string> listSplit = (mess.message.Split('&')).ToList();
                            messages.Message = listSplit.Count > 0 ? listSplit[0] : messages.Message;
                            if (listSplit.Count > 1)
                            {
                                listSplit.RemoveAt(0);
                                int find = listSplit.FindIndex(x => x.Contains("candidateId"));
                                if (find > -1)
                                {
                                    int idc;
                                    string ck = listSplit[find].Substring(12);
                                    if (!string.IsNullOrEmpty(ck) && int.TryParse(ck, out idc))
                                    {
                                        var getuser = DAOUsers.GetInforUserById(idc);
                                        if (getuser.Count > 0)
                                        {
                                            var memberDB = getuser[0];
                                            messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                                            if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                                            {
                                                string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                                try
                                                {
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                                    }
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                                    }
                                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                                    }
                                                    else
                                                    {
                                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                                }
                                            }
                                            else
                                            {
                                                messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                                messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (mess.infoSupport != null)
                        {
                            messages.InfoSupport = new InfoSupport();
                            messages.InfoSupport.Title = mess.infoSupport.title;
                            messages.InfoSupport.Message = mess.infoSupport.message;
                            if (mess.infoSupport.userId != 0)
                            {
                                var users = DAOUsers.GetInforUserById(mess.infoSupport.userId);
                                if (users.Count > 0)
                                {
                                    messages.InfoSupport.UserId = users[0].id;
                                    messages.InfoSupport.userName = users[0].userName;
                                }
                            }
                            messages.InfoSupport.SupportId = mess.infoSupport.supportId;
                            messages.InfoSupport.Status = mess.infoSupport.status;
                            messages.InfoSupport.Time = mess.infoSupport.time.ToLocalTime();
                            messages.InfoSupport.HaveConversation = mess.infoSupport.haveConversation;
                        }
                        if (currentMember != null && currentMember.timeLastSeener >= mess.createAt)
                        {
                            messages.IsSeen = 1;
                        }
                        var userSender = DAOUsers.GetInforUserById(mess.senderId);
                        if (userSender.Count > 0)
                        {
                            messages.SenderName = userSender[0].userName;
                        }
                        list.Add(messages);
                    }
                    catch { }
                }
            }
            return list.OrderBy(x => x.DisplayMessage).ToList();
        }
        public static List<MessagesDB> getOldMessageFromLiveChat(int conversationId, string clientId, string fromWeb)
        {
            List<MessagesDB> list = new List<MessagesDB>();
            var z = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.id == conversationId).ToList();
            if (z.Count > 0)
            {
                ConversationsDB conversation = z[0];
                //int display = 0;
                //var mess = conversation.messageList.Where(x => x.deleteType == 2).OrderBy(x => x.createAt).ToList();
                //if (mess.Count > 0)
                //{
                //    display = mess[mess.Count - 1].displayMessage;
                //}
                list = conversation.messageList.Where(x => x.liveChat != null && x.infoSupport != null && !string.IsNullOrEmpty(x.liveChat.clientId) && !string.IsNullOrEmpty(x.infoSupport.supportId) && x.infoSupport.status == 0 && x.infoSupport.userId == 0 && x.liveChat.clientId == clientId && x.liveChat.fromWeb == fromWeb).ToList();
            }
            return list;
        }
        public static List<Messages> GetInfoFileByConversationId(int conversationId, int countMess, int countLoad, Int64 messDisplay, int Type)
        {

            List<Messages> list = new List<Messages>();
            ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.id == conversationId && x.messageList.Count > 0).ToList().ForEach(c =>
            {
                if (Type == 0)
                {
                    var lismessageDB = c.messageList.Where(x => x.displayMessage > messDisplay && (x.messageType == "sendPhoto" || x.messageType == "sendFile" || x.messageType == "link"));
                    countLoad = lismessageDB.Count() >= (countMess + countLoad) ? countLoad : (lismessageDB.Count() - countMess > countLoad ? 0 : lismessageDB.Count() - countMess);
                    countMess = lismessageDB.Count() - countMess - countLoad;
                    List<MessagesDB> l = lismessageDB.Skip(countMess).Take(countLoad).ToList();
                    foreach (var mess in l)
                    {
                        try
                        {
                            Messages messages = new Messages(mess.id, conversationId, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime, mess.deleteTime);
                            if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                            {
                                messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image, mess.infoLink.isNotification);
                            }
                            if (mess.listFile != null && mess.listFile.Count != 0)
                            {
                                messages.ListFile = new List<InfoFile>();
                                foreach (var file in mess.listFile)
                                {
                                    messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                                }
                            }
                            list.Add(messages);

                        }
                        catch
                        {
                        }
                    }
                }
                else if (Type == 1)
                {
                    var lismessageDB = c.messageList.Where(x => x.displayMessage > messDisplay && (x.messageType == "sendPhoto"));
                    countLoad = lismessageDB.Count() >= (countMess + countLoad) ? countLoad : (lismessageDB.Count() - countMess > countLoad ? 0 : lismessageDB.Count() - countMess);
                    countMess = lismessageDB.Count() - countMess - countLoad;
                    List<MessagesDB> l = lismessageDB.Skip(countMess).Take(countLoad).ToList();
                    foreach (var mess in l)
                    {
                        try
                        {
                            Messages messages = new Messages(mess.id, conversationId, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime);
                            if (mess.listFile != null && mess.listFile.Count != 0)
                            {
                                messages.ListFile = new List<InfoFile>();
                                foreach (var file in mess.listFile)
                                {
                                    messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                                }
                            }
                            list.Add(messages);

                        }
                        catch
                        {
                        }
                    }
                }
                else if (Type == 2)
                {
                    var lismessageDB = c.messageList.Where(x => x.displayMessage > messDisplay && (x.messageType == "sendFile"));
                    countLoad = lismessageDB.Count() >= (countMess + countLoad) ? countLoad : (lismessageDB.Count() - countMess > countLoad ? 0 : lismessageDB.Count() - countMess);
                    countMess = lismessageDB.Count() - countMess - countLoad;
                    List<MessagesDB> l = lismessageDB.Skip(countMess).Take(countLoad).ToList();
                    foreach (var mess in l)
                    {
                        try
                        {
                            Messages messages = new Messages(mess.id, conversationId, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime);
                            if (mess.listFile != null && mess.listFile.Count != 0)
                            {
                                messages.ListFile = new List<InfoFile>();
                                foreach (var file in mess.listFile)
                                {
                                    messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                                }
                            }
                            list.Add(messages);

                        }
                        catch
                        {
                        }
                    }
                }
                else if (Type == 3)
                {
                    var lismessageDB = c.messageList.Where(x => x.displayMessage > messDisplay && (x.messageType == "link"));
                    countLoad = lismessageDB.Count() >= (countMess + countLoad) ? countLoad : (lismessageDB.Count() - countMess > countLoad ? 0 : lismessageDB.Count() - countMess);
                    countMess = lismessageDB.Count() - countMess - countLoad;
                    List<MessagesDB> l = lismessageDB.Skip(countMess).Take(countLoad).ToList();
                    foreach (var mess in l)
                    {
                        try
                        {
                            Messages messages = new Messages(mess.id, conversationId, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime);
                            if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                            {
                                messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image, mess.infoLink.isNotification);
                            }
                            list.Add(messages);
                        }
                        catch
                        {
                        }
                    }
                }



            });
            return list;
        }
        //đã sửa 02/08
        //lấy tin nhắn theo id tin nhắn
        public static Messages GetMessageById(string messageId)
        {
            try
            {
                var messageFilter = Builders<ConversationsDB>.Filter.ElemMatch(z => z.messageList, a => a.id == messageId);
                var conversation = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(messageFilter).FirstOrDefault();
                var mess = conversation.messageList.FirstOrDefault(a => a.id == messageId);

                Messages messages = new Messages(mess.id, conversation.id, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime, mess.deleteType);
                if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                {
                    messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image.Replace("amp;", ""), mess.infoLink.isNotification);
                }
                if (mess.listFile != null && mess.listFile.Count != 0)
                {
                    messages.ListFile = new List<InfoFile>();
                    foreach (var file in mess.listFile)
                    {
                        messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                    }
                }
                if (mess.emotion != null)
                {
                    messages.EmotionMessage = new List<Chat365.Model.Entity.Emotion>();
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion1.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(1, mess.emotion.Emotion1.Split(","), "https://mess.timviec365.vn/Emotion/Emotion1.png"));
                    }
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion2.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(2, mess.emotion.Emotion2.Split(","), "https://mess.timviec365.vn/Emotion/Emotion2.png"));
                    }
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion3.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(3, mess.emotion.Emotion3.Split(","), "https://mess.timviec365.vn/Emotion/Emotion3.png"));
                    }
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion4.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(4, mess.emotion.Emotion4.Split(","), "https://mess.timviec365.vn/Emotion/Emotion4.png"));
                    }
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion5.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(5, mess.emotion.Emotion5.Split(","), "https://mess.timviec365.vn/Emotion/Emotion5.png"));
                    }
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion6.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(6, mess.emotion.Emotion6.Split(","), "https://mess.timviec365.vn/Emotion/Emotion6.png"));
                    }
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion7.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(7, mess.emotion.Emotion7.Split(","), "https://mess.timviec365.vn/Emotion/Emotion7.png"));
                    }
                    if (!String.IsNullOrEmpty(mess.emotion.Emotion8.Trim()))
                    {
                        messages.EmotionMessage.Add(new Emotion(8, mess.emotion.Emotion8.Split(","), "https://mess.timviec365.vn/Emotion/Emotion8.png"));
                    }
                }
                if (!string.IsNullOrEmpty(mess.quoteMessage))
                {
                    Messages quote = GetMessageById(mess.quoteMessage);
                    List<UserDB> sender = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == quote.SenderID).Limit(1).ToList();
                    if (sender.Count > 0)
                    {
                        messages.QuoteMessage = new MessageQuote(quote.MessageID, sender[0].userName, quote.SenderID, quote.MessageType, quote.Message, quote.CreateAt.ToLocalTime());
                    }
                }
                if (mess.messageType.Equals("sendProfile"))
                {
                    UserDB memberDB = DAOUsers.GetUserById(Convert.ToInt32(mess.message))[0];
                    messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                    if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                    {
                        string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                        try
                        {
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                            }
                            else
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        catch (Exception ex)
                        {
                            messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                        }
                    }
                    else
                    {
                        messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                        messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                    }

                }
                if (mess.messageType.Equals("map"))
                {
                    UserDB memberDB = DAOUsers.GetUserById(Convert.ToInt32(mess.senderId))[0];
                    messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                    if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                    {
                        string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                        try
                        {
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                            }
                            else
                            {
                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        catch (Exception ex)
                        {
                            messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                        }
                        if (!String.IsNullOrWhiteSpace(messages.UserProfile.LinkAvatar.Trim())) messages.UserProfile.AvatarUser = messages.UserProfile.LinkAvatar;
                    }
                    else
                    {
                        messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                        messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                    }

                }
                if (mess.liveChat != null && !string.IsNullOrEmpty(mess.liveChat.clientId))
                {
                    messages.LiveChat = new InfoLiveChat(mess.liveChat.clientId, mess.liveChat.clientName, "", mess.liveChat.fromWeb);
                    if (string.IsNullOrEmpty(messages.LiveChat.ClientName)) messages.LiveChat.ClientName = messages.LiveChat.ClientId;
                    if (String.IsNullOrWhiteSpace(messages.LiveChat.ClientAvatar.Trim()))
                    {
                        string letter = RemoveUnicode(messages.LiveChat.ClientName.Substring(0, 1).ToLower()).ToUpper();
                        try
                        {
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                            }
                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                            {
                                messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                            }
                            else
                            {
                                messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        catch (Exception ex)
                        {
                            messages.LiveChat.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                        }
                    }
                }
                if (mess.messageType == "newCandidate" && mess.message.Contains("&candidateId"))
                {
                    List<string> listSplit = (mess.message.Split('&')).ToList();
                    messages.Message = listSplit.Count > 0 ? listSplit[0] : messages.Message;
                    if (listSplit.Count > 1)
                    {
                        listSplit.RemoveAt(0);
                        int find = listSplit.FindIndex(x => x.Contains("candidateId"));
                        if (find > -1)
                        {
                            int idc;
                            string ck = listSplit[find].Substring(12);
                            if (!string.IsNullOrEmpty(ck) && int.TryParse(ck, out idc))
                            {
                                var getuser = DAOUsers.GetInforUserById(idc);
                                if (getuser.Count > 0)
                                {
                                    var memberDB = getuser[0];
                                    messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                                    if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                                    {
                                        string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                        try
                                        {
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                            }
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                            }
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                            }
                                            else
                                            {
                                                messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    else
                                    {
                                        messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                        messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                                    }
                                }
                            }
                        }
                    }
                }
                if (mess.infoSupport != null)
                {
                    messages.InfoSupport = new InfoSupport();
                    messages.InfoSupport.Title = mess.infoSupport.title;
                    messages.InfoSupport.Message = mess.infoSupport.message;
                    if (mess.infoSupport.userId != 0)
                    {
                        var users = DAOUsers.GetInforUserById(mess.infoSupport.userId);
                        if (users.Count > 0)
                        {
                            messages.InfoSupport.UserId = users[0].id;
                            messages.InfoSupport.userName = users[0].userName;
                        }
                    }
                    messages.InfoSupport.SupportId = mess.infoSupport.supportId;
                    messages.InfoSupport.Status = mess.infoSupport.status;
                    messages.InfoSupport.Time = mess.infoSupport.time.ToLocalTime();
                    messages.InfoSupport.HaveConversation = mess.infoSupport.haveConversation;
                }
                return messages;
            }
            catch
            {
                return null;
            }
        }
        //đã sửa 02/08
        //sửa icon trong tin nhắn
        public static int UpdateInforEmotion(string messageId, string emotion, int typeEmotion)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            switch (typeEmotion)
            {
                case 1:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion1, emotion); break;
                case 2:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion2, emotion); break;
                case 3:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion3, emotion); break;
                case 4:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion4, emotion); break;
                case 5:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion5, emotion); break;
                case 6:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion6, emotion); break;
                case 7:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion7, emotion); break;
                case 8:
                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].emotion.Emotion8, emotion); break;
                default:
                    return 0;
            }
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }
        //xong05/08
        //đã sửa 02/08
        //xoá tin nhắn, link, file, icon
        public static int DeleteMessage(string messageId)
        {
            var filter = Builders<ConversationsDB>.Filter.Where(x => x.messageList.Count > 0 && x.messageList.Any(m => m.id == messageId));
            var update = Builders<ConversationsDB>.Update.PullFilter(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //xong 05/08
        //sửa ngày 02/08
        //sửa tin nhắn theo id của tin nhắn
        public static int EditMessage(string messageId, string message)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].message, message).Set(x => x.messageList[-1].isEdited, 1);
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int SetOutLivechat(int conversationId, string clientId, string fromWeb)
        {
            var z = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.id == conversationId).ToList();
            if (z.Count > 0)
            {
                ConversationsDB c = z[0];
                if (c.messageList != null && c.messageList.Count > 0)
                {
                    var mess = c.messageList.Where(x => x.infoSupport != null && !string.IsNullOrEmpty(x.infoSupport.supportId) && x.liveChat != null && !string.IsNullOrEmpty(x.liveChat.clientId) && x.liveChat.clientId == clientId && x.liveChat.fromWeb == fromWeb).OrderByDescending(x => x.createAt).ToList();
                    if (mess.Count > 0)
                    {
                        var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, mess[0].id));
                        UpdateDefinition<ConversationsDB> update;
                        update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].deleteType, 2);
                        var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                        if (ck.ModifiedCount > 0) return 1;
                        else return 0;
                    }
                }
            }
            return 0;

        }
        public static int EditDeleteType(string messageId, int type)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].deleteType, type);
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateSupportStatus(string messageId, int userId, int status, int conversationId)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].infoSupport.userId, userId).Set(x => x.messageList[-1].infoSupport.status, status).Set(x => x.messageList[-1].infoSupport.time, DateTime.Now).Set(x => x.messageList[-1].infoSupport.haveConversation, conversationId);
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int UpdateAllSupportStatus(int supportGroup, string messageId, int userId, int status, int conversationId)
        {
            var z = ConnectDB.database.GetCollection<ConversationsDB>("Conversations").Find(x => x.id == supportGroup).ToList();
            if (z.Count > 0)
            {
                ConversationsDB c = z[0];
                int index = c.messageList.FindIndex(x => x.id == messageId);
                if (index > -1)
                {
                    LiveChatDB lc = c.messageList[index].liveChat;
                    if (lc != null)
                    {
                        //var sp = c.messageList.Where(x => x.infoSupport != null && !string.IsNullOrEmpty(x.infoSupport.supportId) && (x.infoSupport.status == 0 || x.infoSupport.status == 2) && x.liveChat != null && !string.IsNullOrEmpty(x.liveChat.clientId) && x.liveChat.clientId == lc.clientId && x.liveChat.fromWeb == lc.fromWeb).ToList();
                        //if (sp.Count > 0)
                        //{
                        //    foreach (var item in sp)
                        //    {
                        //        var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, item.id));
                        //        UpdateDefinition<ConversationsDB> update;
                        //        update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].infoSupport.userId, userId).Set(x => x.messageList[-1].infoSupport.status, status).Set(x => x.messageList[-1].infoSupport.time, DateTime.Now).Set(x => x.messageList[-1].infoSupport.haveConversation, conversationId);
                        //        var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                        //    }
                        //}
                        foreach (MessagesDB item in c.messageList)
                        {
                            if (item.infoSupport != null && !string.IsNullOrEmpty(item.infoSupport.supportId) && (item.infoSupport.status == 0 || item.infoSupport.status == 2))
                            {
                                if (item.liveChat != null && !string.IsNullOrEmpty(item.liveChat.clientId) && item.liveChat.clientId == lc.clientId && item.liveChat.fromWeb == lc.fromWeb)
                                {
                                    var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, item.id));
                                    UpdateDefinition<ConversationsDB> update;
                                    update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].infoSupport.userId, userId).Set(x => x.messageList[-1].infoSupport.status, status).Set(x => x.messageList[-1].infoSupport.time, DateTime.Now).Set(x => x.messageList[-1].infoSupport.haveConversation, conversationId);
                                    var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                                }
                            }

                        }
                    }
                }
            }
            return 0;
        }
        public static int UpdateStatusMessage(string messageId)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].isEdited, 0);
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        //bỏ
        public static int InsertFile(string messageId, string fileName, string sizeFile, double height, double width)
        {
            try
            {
                var filter = Builders<ConversationsDB>.Filter.Where(x => x.messageList.Any(y => y.id == messageId)) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(y => y.id, messageId));
                FileSendDB fileSend = new FileSendDB() { nameFile = fileName, height = height, sizeFile = Convert.ToInt32(sizeFile), width = width };
                List<FileSendDB> list = new List<FileSendDB>();
                list.Add(fileSend);
                var update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].listFile, list);
                ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);

                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //bỏ
        public static int InsertLink(string messageId, string Title, string Description, string LinkHome, string Image, int isNotification)
        {
            if (String.IsNullOrWhiteSpace(Title))
            {
                return 0;
            }
            if (String.IsNullOrWhiteSpace(Description))
            {
                Description = "";
            }
            if (String.IsNullOrWhiteSpace(Image))
            {
                Image = "";
            }
            if (String.IsNullOrWhiteSpace(LinkHome))
            {
                LinkHome = "";
            }
            try
            {
                var filter = Builders<ConversationsDB>.Filter.Where(x => x.messageList.Any(y => y.id == messageId)) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
                InfoLinkDB info = new InfoLinkDB(Title, Description, LinkHome, Image, isNotification);
                var update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].infoLink, info);
                ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //đã sửa ngày 01/08
        //nhập info tin nhắn
        public static int InsertMessage(string messageId, int conversationId, int senderId, string typeMessage, string message, string quoteMessage, string messgeQuote, DateTime CreateAt, InfoLinkDB link, List<FileSendDB> listFile, IWebHostEnvironment environment, int deleteTime, int deleteType, DateTime deletedate, int isEdited, string from = "")
        {
            if (message == null)
            {
                message = "";
            }
            if (quoteMessage == null)
            {
                quoteMessage = "";
            }
            if (messgeQuote == null)
            {
                messgeQuote = "";
            }
            try
            {
                if (typeMessage.Equals("sendPhoto"))
                {
                    foreach (var item in listFile)
                    {
                        if (item.height <= 0 || item.width <= 0)
                        {
                            item.UpdateWidthHeightPhoto(environment);
                        }
                    }
                }
                int displayMessage = DAOCounter.getNextID("MessageId");
                var filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId);
                MessagesDB mess = new MessagesDB(messageId, displayMessage, senderId, typeMessage, message, quoteMessage, messgeQuote, CreateAt, isEdited, link, listFile, new EmotionMessageDB(), deleteTime, deleteType, deletedate, null, null, new List<int>(), from);
                var update = Builders<ConversationsDB>.Update.Push("messageList", mess).Set("timeLastMessage", CreateAt);
                ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                DAOCounter.updateID("MessageId");
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static int InsertMessage(string messageId, int conversationId, int senderId, string typeMessage, string message, string quoteMessage, string messgeQuote, DateTime CreateAt, InfoLinkDB link, List<FileSendDB> listFile, IWebHostEnvironment environment, int deleteTime, int deleteType, DateTime deletedate, int isEdited, LiveChatDB livechat, string from = "")
        {
            if (message == null)
            {
                message = "";
            }
            if (quoteMessage == null)
            {
                quoteMessage = "";
            }
            if (messgeQuote == null)
            {
                messgeQuote = "";
            }
            try
            {
                if (typeMessage.Equals("sendPhoto"))
                {
                    foreach (var item in listFile)
                    {
                        if (item.height <= 0 || item.width <= 0)
                        {
                            item.UpdateWidthHeightPhoto(environment);
                        }
                    }
                }
                int displayMessage = DAOCounter.getNextID("MessageId");
                var filter = Builders<ConversationsDB>.Filter.Eq("_id", conversationId);
                MessagesDB mess = new MessagesDB(messageId, displayMessage, senderId, typeMessage, message, quoteMessage, messgeQuote, CreateAt, isEdited, link, listFile, new EmotionMessageDB(), deleteTime, deleteType, deletedate, null, livechat, new List<int>(), from);
                var update = Builders<ConversationsDB>.Update.Push("messageList", mess).Set("timeLastMessage", CreateAt);
                ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
                DAOCounter.updateID("MessageId");
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }
        public static int EditInfoLiveChat(string messageId, LiveChatDB info)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].liveChat, info);
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int EditInfoSupport(string messageId, InfoSupportDB info)
        {
            var filter = Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Eq(x => x.id, messageId));
            UpdateDefinition<ConversationsDB> update;
            update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].infoSupport, info);
            var ck = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (ck.ModifiedCount > 0) return 1;
            else return 0;
        }
        public static int CheckMessageEmpty(string messageId)
        {
            var con = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.messageList.Count > 0 && x.messageList.Any(i => i.id == messageId)).ToList();
            if (con.Count > 0) return con[0].id;
            else return 0;
        }

        public static string GetDeleteTimeLeft(string messageId)
        {
            string left = "";
            var con = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => x.messageList.Count > 0 && x.messageList.Any(i => i.id == messageId && i.deleteTime > 0)).ToList();
            if (con.Count > 0)
            {
                int index = con[0].messageList.FindIndex(x => x.id == messageId);
                if (index > -1)
                {
                    MessagesDB mess = con[0].messageList[index];
                    var par = con[0].memberList.Where(x => x.memberId != int.Parse(messageId.Split('_')[0])).ToList();
                    if (par.Count > 0)
                    {

                    }
                    left = $"Tin nhắn tự động xóa sau khi xem ({mess.deleteTime})";
                }
            }
            return left;
        }

        public static int SetDeleteDate(int conversationId, string messId, DateTime date)
        {
            FilterDefinition<ConversationsDB> filter = Builders<ConversationsDB>.Filter.Where(x => x.id == conversationId) & Builders<ConversationsDB>.Filter.ElemMatch(x => x.messageList, Builders<MessagesDB>.Filter.Where(y => y.id == messId && y.deleteTime > 0 && y.deleteType == 1));
            UpdateDefinition<ConversationsDB> update = Builders<ConversationsDB>.Update.Set(x => x.messageList[-1].deleteDate, date);
            var check = ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).UpdateOne(filter, update);
            if (check.ModifiedCount > 0) return 1;
            return 0;
        }

        public static List<Messages> CheckMessageError()
        {
            List<Messages> list = new List<Messages>();
            ConnectDB.database.GetCollection<ConversationsDB>(tableConversations).Find(x => (x.id == 170 || x.id == 4935) && x.messageList.Count > 0).ToList().ForEach(c =>
            {
                var lismessageDB = c.messageList.Where(x => x.messageType == "notification").ToList();
                List<MessagesDB> l = lismessageDB.ToList();
                foreach (var mess in l)
                {
                    try
                    {
                        Messages messages = new Messages(mess.id, c.id, mess.senderId, mess.messageType, mess.message, mess.isEdited, mess.createAt.ToLocalTime(), mess.deleteDate.ToLocalTime(), mess.deleteTime, mess.deleteType);
                        if (mess.infoLink != null && !String.IsNullOrWhiteSpace(mess.infoLink.title))
                        {
                            messages.InfoLink = new InfoLink(mess.id, mess.infoLink.title, mess.infoLink.description, mess.infoLink.linkHome, mess.infoLink.image, mess.infoLink.isNotification);
                            if (messages.InfoLink.HaveImage == "True") messages.InfoLink.Image.Replace("amp;", "");
                        }
                        if (mess.listFile != null && mess.listFile.Count != 0)
                        {
                            messages.ListFile = new List<InfoFile>();
                            foreach (var file in mess.listFile)
                            {
                                messages.ListFile.Add(new InfoFile(mess.messageType, file.nameFile, file.sizeFile, file.height, file.width));
                            }
                        }
                        if (mess.emotion != null)
                        {
                            messages.EmotionMessage = new List<Chat365.Model.Entity.Emotion>();
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion1.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(1, mess.emotion.Emotion1.Split(","), "https://mess.timviec365.vn/Emotion/Emotion1.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion2.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(2, mess.emotion.Emotion2.Split(","), "https://mess.timviec365.vn/Emotion/Emotion2.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion3.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(3, mess.emotion.Emotion3.Split(","), "https://mess.timviec365.vn/Emotion/Emotion3.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion4.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(4, mess.emotion.Emotion4.Split(","), "https://mess.timviec365.vn/Emotion/Emotion4.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion5.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(5, mess.emotion.Emotion5.Split(","), "https://mess.timviec365.vn/Emotion/Emotion5.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion6.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(6, mess.emotion.Emotion6.Split(","), "https://mess.timviec365.vn/Emotion/Emotion6.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion7.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(7, mess.emotion.Emotion7.Split(","), "https://mess.timviec365.vn/Emotion/Emotion7.png"));
                            }
                            if (!String.IsNullOrEmpty(mess.emotion.Emotion8.Trim()))
                            {
                                messages.EmotionMessage.Add(new Emotion(8, mess.emotion.Emotion8.Split(","), "https://mess.timviec365.vn/Emotion/Emotion8.png"));
                            }
                        }
                        if (!string.IsNullOrEmpty(mess.quoteMessage))
                        {
                            Messages quote = GetMessageById(mess.quoteMessage);
                            List<UserDB> sender = ConnectDB.database.GetCollection<UserDB>("Users").Find(x => x.id == quote.SenderID).Limit(1).ToList();
                            if (sender.Count > 0)
                            {
                                messages.QuoteMessage = new MessageQuote(quote.MessageID, sender[0].userName, quote.SenderID, quote.MessageType, quote.Message, quote.CreateAt.ToLocalTime());
                            }
                        }
                        if (mess.messageType.Equals("sendProfile"))
                        {
                            UserDB memberDB = DAOUsers.GetUserById(Convert.ToInt32(mess.message))[0];
                            messages.UserProfile = new User(memberDB.id, memberDB.id365, memberDB.idTimViec, memberDB.type365, memberDB.email, memberDB.password, memberDB.phone, memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive, memberDB.active, memberDB.isOnline, memberDB.looker, memberDB.companyId, memberDB.companyName, memberDB.fromWeb);
                            if (String.IsNullOrWhiteSpace(messages.UserProfile.AvatarUser.Trim()))
                            {
                                string letter = RemoveUnicode(messages.UserProfile.UserName.Substring(0, 1).ToLower()).ToUpper();
                                try
                                {
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                    }
                                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                    }
                                    else
                                    {
                                        messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    messages.UserProfile.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            else
                            {
                                messages.UserProfile.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + messages.UserProfile.ID + "/" + messages.UserProfile.AvatarUser;
                                messages.UserProfile.LinkAvatar = messages.UserProfile.AvatarUser;
                            }

                        }
                        list.Add(messages);
                    }
                    catch { }
                }
            });
            return list;
        }
    }
}

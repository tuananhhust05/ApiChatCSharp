using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Channels;
using System.Security.Cryptography;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Chat365.Server.Model.EntityAPI;
using APIChat365.Model.Entity;
using Newtonsoft.Json;
using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using APIChat365.Model.EntityAPI;
using System.ServiceModel.Security.Tokens;
using Chat365.Server.Model.Entity;
using VisioForge.Libs.DirectShowLib;
using System.Linq;

namespace APIChat365.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QRController : ControllerBase
    {
        private readonly ILogger<QRController> _logger;
        private readonly IWebHostEnvironment _environment;
        public static SocketIO WIO = new SocketIO(new Uri("http://43.239.223.142:3000/"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });
        public static SocketIO WIO2 = new SocketIO(new Uri("https://chat.timviec365.vn"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });
        public QRController(ILogger<QRController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            if (WIO.Disconnected)
            {
                WIO.ConnectAsync();
            }
            if (WIO2.Disconnected)
            {
                WIO2.ConnectAsync();
            }
        }
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

        private int GetConversationId(int idUser, int idContact)
        {
            int conversationId = 0;
            List<ConversationsDB> conversations = DAOConversation.checkEmptyConversation(idUser, idContact);
            if (conversations.Count == 0)
            {
                conversationId = DAOConversation.insertNewConversation(0, "Normal", idUser);
                if (conversationId > 0)
                {
                    DAOConversation.insertNewParticipant(conversationId, "", 0, new int[2] { idUser, idContact }, idUser, "Normal");
                }
            }
            else
            {
                conversationId = Convert.ToInt32(conversations[0].id);
            }
            return conversationId;
        }
        private Conversation GetConversationInfo(int conversationId, int senderId)
        {
            Conversation conversation = new Conversation();
            ConversationsDB dr = DAOConversation.GetConversation(conversationId, senderId);
            if (dr != null)
            {
                ParticipantsDB userCurrent = new ParticipantsDB();
                List<MemberConversation> listMember = new List<MemberConversation>();
                string conversationName = "";
                var listRequest = DAOUsers.getRequestContact(senderId);
                var listFriend = DAOUsers.GetListContact(senderId, 0, 10000);
                foreach (ParticipantsDB member in dr.memberList)
                {
                    try
                    {
                        if (member.memberId == senderId)
                        {
                            userCurrent = member;
                        }
                        if (string.IsNullOrEmpty(conversationName) && !string.IsNullOrEmpty(member.conversationName))
                        {
                            conversationName = member.conversationName;
                        }
                        UserDB memberDB = DAOUsers.GetUserById(member.memberId)[0];
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
                if (dr.isGroup == 1 && string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, 0, "", "", DateTime.MinValue, userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                conversation.message = dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay ? dr.messageList[dr.messageList.Count - 1].message : "";
                conversation.messageType = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].messageType : "";
                conversation.senderId = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].senderId : 0;
                conversation.createAt = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime() : DateTime.MinValue;
                conversation.listMember = listMember;
                conversation.countMessage = Convert.ToInt32(DAOConversation.GetCountMessageOfConversation(conversation.conversationId)[0].messageList.Count);
                conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";

                conversation.listBrowerMember = new List<BrowseMember>();
                if (conversation.isGroup == 1 && conversation.Equals("Moderate"))
                {
                    foreach (BrowseMembersDB member in dr.browseMemberList)
                    {
                        try
                        {
                            UserDB memberDB = DAOUsers.GetUserById(member.memberBrowserId)[0];
                            BrowseMember userInfo = new BrowseMember(new User(memberDB.id, 0, 0, 0, "", "", "", memberDB.userName, memberDB.avatarUser, memberDB.status, memberDB.statusEmotion, memberDB.lastActive.ToLocalTime(), memberDB.active, memberDB.isOnline, 0, 0, "", memberDB.fromWeb), member.memberAddId);
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
            }
            return conversation;
        }
        public static string Decode(string txt)
        {
            try
            {
                string ourText = txt;
                string x = null;
                string _key = "HHP889@@";
                string privatekey = "hgfedcba";
                byte[] privatekeyByte = { };
                privatekeyByte = Encoding.UTF8.GetBytes(privatekey);
                byte[] _keybyte = { };
                _keybyte = Encoding.UTF8.GetBytes(_key);
                byte[] inputtextbyteArray = new byte[ourText.Replace(" ", "+").Length];
                //This technique reverses base64 encoding when it is received over the Internet.
                inputtextbyteArray = Convert.FromBase64String(ourText.Replace(" ", "+"));
                using (DESCryptoServiceProvider dEsp = new DESCryptoServiceProvider())
                {
                    var memstr = new MemoryStream();
                    var crystr = new CryptoStream(memstr, dEsp.CreateDecryptor(_keybyte, privatekeyByte), CryptoStreamMode.Write);
                    crystr.Write(inputtextbyteArray, 0, inputtextbyteArray.Length);
                    crystr.FlushFinalBlock();
                    return Encoding.UTF8.GetString(memstr.ToArray());
                }
                return x;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost("QR365")]
        [AllowAnonymous]
        public APIQR GetQRInfo()
        {
            APIQR api = new APIQR();
            try
            {
                var http = HttpContext.Request;
                string data = http.Form["data"];
                int userId = Convert.ToInt32(http.Form["id"]);
                if (!string.IsNullOrEmpty(data) && userId != 0)
                {
                    List<UserDB> getUser = DAOUsers.GetInforUserById(userId);
                    if (getUser.Count > 0)
                    {
                        string decode = Decode(data);
                        QRInfo info = JsonConvert.DeserializeObject<QRInfo>(decode);
                        if (info.QRType == "QRAddFriend")
                        {
                            QRFriend contact = JsonConvert.DeserializeObject<QRFriend>(info.data.ToString());
                            if (userId != contact.userId)
                            {
                                var requestList = DAOUsers.getRequestContact(userId);
                                var contactList = DAOUsers.GetListContact(userId, 0, 10000);
                                if (requestList.Any(x => x.contactId == contact.userId && (x.status == "send")))
                                {
                                    api.data = new DataQR();
                                    api.data.result = false;
                                    api.data.message = "Gửi lời mời kết bạn thành công";
                                    api.data.conversationId = GetConversationId(userId, contact.userId);
                                    api.data.conversationInfo = GetConversationInfo(api.data.conversationId, userId);
                                }
                                else if (contactList.Any(x => x.id == contact.userId))
                                {
                                    api.data = new DataQR();
                                    api.data.result = false;
                                    api.data.message = "Gửi lời mời kết bạn thành công";
                                    api.data.conversationId = GetConversationId(userId, contact.userId);
                                    api.data.conversationInfo = GetConversationInfo(api.data.conversationId, userId);
                                }
                                else
                                {
                                    DAOUsers.DeleteRequestAddFriend(userId, contact.userId);
                                    if (DAOUsers.AddFriend(userId, contact.userId, contact.type365) > 0)
                                    {
                                        api.data = new DataQR();
                                        api.data.result = true;
                                        api.data.message = "Gửi lời mời kết bạn thành công";
                                        api.data.conversationId = GetConversationId(userId, contact.userId);
                                        api.data.conversationInfo = GetConversationInfo(api.data.conversationId, userId);
                                    }
                                    else
                                    {
                                        api.error = new Error();
                                        api.error.code = 200;
                                        api.error.message = "Gửi lời mời kết bạn thất bại";
                                    }

                                }
                            }
                            else
                            {
                                api.error = new Error();
                                api.error.code = 200;
                                api.error.message = "Bạn không thể quét mã QR của chính mính";
                            }
                        }
                        else if (info.Time > DateTime.Now)
                        {
                            if (info.QRType == "QRJoinGroup")
                            {
                                QRJoinGroup group = JsonConvert.DeserializeObject<QRJoinGroup>(info.data.ToString());
                                var conversation = DAOConversation.GetConversation(group.ConversationId, userId);
                                if (conversation != null)
                                {
                                    if (DAOConversation.CheckIsMemeber(group.ConversationId, userId) == null)
                                    {
                                        List<int> memberList = new List<int> { userId };
                                        if (DAOConversation.insertNewParticipant(group.ConversationId, group.ConversationName, 1, memberList.ToArray(), 0, conversation.typeGroup) > 0)
                                        {
                                            api.data = new DataQR();
                                            api.data.result = true;
                                            api.data.message = "Tham gia nhóm thành công";
                                            api.data.conversationId = group.ConversationId;
                                            api.data.conversationInfo = GetConversationInfo(group.ConversationId, userId);
                                        }
                                        else
                                        {
                                            api.error = new Error();
                                            api.error.code = 200;
                                            api.error.message = "Tham gia nhóm thất bại";
                                        }
                                    }
                                    else
                                    {
                                        api.error = new Error();
                                        api.error.code = 200;
                                        api.error.message = "User đã là thành viên của nhóm";
                                    }
                                }
                                else
                                {
                                    api.error = new Error();
                                    api.error.code = 200;
                                    api.error.message = "Cuộc trò chuyện không tồn tại";
                                }

                            }
                            else
                            {
                                api.error = new Error();
                                api.error.code = 200;
                                api.error.message = "lấy thông tin thất bại";
                            }
                        }
                        else
                        {
                            api.error = new Error();
                            api.error.code = 200;
                            api.error.message = "lấy thông tin thất bại";
                        }
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "Không có user này";
                    }
                }
                else
                {
                    api.error = new Error();
                    api.error.code = 200;
                    api.error.message = "thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                api.error = new Error();
                api.error.code = 200;
                api.error.message = ex.ToString();
            }
            return api;
        }
    }
}

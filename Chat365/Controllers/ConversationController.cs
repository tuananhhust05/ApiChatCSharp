using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Chat365.Server.Model.Entity;
using Chat365.Server.Model.DAO;
using Chat365.Server.Model.EntityAPI;
using Newtonsoft.Json;
using System.Text;
using APIChat365.Model.MongoEntity;
using APIChat365.MongoEntity;
using APIChat365.Model.Entity;
using Chat365.Model.Entity;
using MongoDB.Driver;
using SocketIOClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Chat365.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ILogger<ConversationController> _logger;
        private readonly IWebHostEnvironment _environment;
        public static SocketIO WIO = new SocketIO(new Uri("http://43.239.223.142:3000/"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });
        public ConversationController(ILogger<ConversationController> logger,
    IWebHostEnvironment environment)
        {
            if (WIO.Disconnected)
            {
                WIO.ConnectAsync();
            }
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
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
        public User getInforUser(List<UserDB> getUser)
        {
            User userInfo = new User(getUser[0].id, getUser[0].id365, getUser[0].idTimViec, getUser[0].type365, getUser[0].email, getUser[0].password, getUser[0].phone, getUser[0].userName, getUser[0].avatarUser, getUser[0].status, getUser[0].statusEmotion, getUser[0].lastActive, getUser[0].active, getUser[0].isOnline, getUser[0].looker, getUser[0].companyId, getUser[0].companyName, getUser[0].notificationPayoff, getUser[0].notificationCalendar, getUser[0].notificationReport, getUser[0].notificationOffer, getUser[0].notificationPersonnelChange, getUser[0].notificationRewardDiscipline, getUser[0].notificationNewPersonnel, getUser[0].notificationTransferAsset, getUser[0].notificationChangeProfile, getUser[0].notificationMissMessage, getUser[0].notificationCommentFromTimViec, getUser[0].notificationCommentFromRaoNhanh, getUser[0].notificationTag, getUser[0].notificationSendCandidate, getUser[0].notificationChangeSalary, getUser[0].notificationAllocationRecall, getUser[0].notificationAcceptOffer, getUser[0].notificationDecilineOffer, getUser[0].notificationNTDPoint, getUser[0].notificationNTDExpiredPin, getUser[0].notificationNTDExpiredRecruit, getUser[0].fromWeb, getUser[0].notificationNTDApplying);
            if (string.IsNullOrEmpty(userInfo.AvatarUser) || string.IsNullOrWhiteSpace(userInfo.AvatarUser))
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
                catch
                {
                    userInfo.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                }
            }
            else
            {
                userInfo.LinkAvatar = "https://mess.timviec365.vn/avatarUser/" + userInfo.ID + "/" + userInfo.AvatarUser;
                userInfo.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + userInfo.ID + "/" + userInfo.AvatarUser;
            }
            return userInfo;
        }
        public string getDefaultAvata(string userName)
        {
            string linkAvata = "";
            if (!string.IsNullOrEmpty(userName))
            {
                string letter = RemoveUnicode(userName.Substring(0, 1).ToLower()).ToUpper();
                try
                {
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        linkAvata = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        linkAvata = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        linkAvata = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                    }
                    else
                    {
                        linkAvata = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                    }
                }
                catch
                {
                    linkAvata = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                }
            }
            return linkAvata;
        }
        public InfoLiveChat getClientInfo(LiveChatDB info)
        {
            InfoLiveChat client = new InfoLiveChat();
            client.ClientId = info.clientId;
            client.ClientName = string.IsNullOrEmpty(info.clientName) ? info.clientId : info.clientName;
            client.FromWeb = info.fromWeb;
            if (!string.IsNullOrEmpty(client.ClientName))
            {
                string letter = RemoveUnicode(client.ClientName.Substring(0, 1).ToLower()).ToUpper();
                try
                {
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        client.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        client.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        client.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                    }
                    else
                    {
                        client.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                    }
                }
                catch
                {
                    client.ClientAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                }
            }
            return client;
        }
        //(5/8/22)
        [HttpPost("GetListConversation")]
        [AllowAnonymous]
        public APIConversation GetListConversation([FromForm] APILoadListConversation infoLoad)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (infoLoad.userId != 0 && infoLoad.countConversation != 0)
                {
                    List<ConversationsDB> listConversationDB = new List<ConversationsDB>();
                    if (infoLoad.countConversation - infoLoad.countConversationLoad >= 20)
                    {
                        listConversationDB = DAOConversation.GetListConversation(infoLoad.userId, 20, infoLoad.countConversationLoad);
                    }
                    else
                    {
                        listConversationDB = DAOConversation.GetListConversation(infoLoad.userId, infoLoad.countConversation - infoLoad.countConversationLoad, infoLoad.countConversationLoad);
                    }
                    var listRequest = DAOUsers.getRequestContact(infoLoad.userId);
                    var listFriend = DAOUsers.GetListContact(infoLoad.userId, 0, 10000);
                    List<Conversation> listConversation = new List<Conversation>();
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
                                if (member.memberId == infoLoad.userId)
                                {
                                    userCurrent = member;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(member.conversationName)) conversationName += memberDB.userName + ",";
                                    else if (!string.IsNullOrEmpty(member.conversationName) && dr.isGroup == 1) conversationName = member.conversationName;
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

                                if (dr.typeGroup == "liveChat" && member.liveChat != null) userInfo.LiveChat = getClientInfo(member.liveChat);

                                listMember.Add(userInfo);
                            }
                            catch
                            {

                            }
                        }
                        if (!String.IsNullOrEmpty(conversationName) && conversationName.Contains(",")) conversationName = conversationName.Remove(conversationName.LastIndexOf(","));
                        if (string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                        Conversation conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, 0, "", "", DateTime.MinValue, userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                        conversation.message = dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay ? dr.messageList[dr.messageList.Count - 1].message : "";
                        conversation.messageType = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].messageType : "";
                        conversation.senderId = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].senderId : 0;
                        conversation.createAt = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime() : DateTime.MinValue;
                        conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";
                        conversation.listMember = listMember;
                        conversation.countMessage = dr.messageList.Where(x => x.displayMessage > userCurrent.messageDisplay).Count();

                        conversation.listBrowerMember = new List<BrowseMember>();
                        if (conversation.isGroup == 1 && conversation.typeGroup.Equals("Moderate"))
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

                        if (dr.typeGroup.Contains("w27")) conversation.typeGroup = "Normal";

                        listConversation.Add(conversation);
                    }
                    if (listConversation.Count > 0)
                    {
                        listConversation.Sort(new CompareByTimeLastMessage());
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy danh sách cuộc trò chuyện thành công";
                        APIConversation.data.listCoversation = listConversation;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        //(5/8/22)
        [HttpPost("GetListMemberOfGroup")]
        [AllowAnonymous]
        public APIConversation GetListMemberOfGroup([FromForm] Conversation searchInfo)
        {
            APIConversation apiconversation = new APIConversation();
            try
            {
                if (searchInfo.conversationId != 0)
                {
                    List<MemberConversation> listMember = DAOConversation.getListMemberOfConversation(searchInfo.conversationId);
                    if (listMember.Count > 0)
                    {
                        apiconversation.data = new DataConversation();
                        apiconversation.data.result = true;
                        apiconversation.data.user_list = listMember;
                        apiconversation.data.message = "Lấy danh sách thành viên thành công";
                    }
                    else
                    {
                        apiconversation.error = new Error();
                        apiconversation.error.code = 200;
                        apiconversation.error.message = "Không tìm thấy kết quả";
                    }
                }
                else
                {
                    apiconversation.error = new Error();
                    apiconversation.error.code = 200;
                    apiconversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                apiconversation.error = new Error();
                apiconversation.error.code = 200;
                apiconversation.error.message = ex.ToString();
            }
            return apiconversation;
        }

        [HttpPost("GetListConversationUnreader")]
        [AllowAnonymous]
        public APIConversation GetListConversationUnreader([FromForm] APILoadListConversation infoLoad)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (infoLoad.userId != 0)
                {
                    List<ConversationsDB> listConversationDB = new List<ConversationsDB>();
                    listConversationDB = DAOConversation.GetListConversationUnreader(infoLoad.userId);
                    if (listConversationDB.Count > 0)
                    {
                        ConversationsDB dr = listConversationDB[0];
                        ParticipantsDB userCurrent = new ParticipantsDB();
                        List<MemberConversation> listMember = new List<MemberConversation>();
                        string conversationName = "";
                        foreach (ParticipantsDB member in dr.memberList)
                        {
                            try
                            {
                                UserDB memberDB = DAOUsers.GetUserById(member.memberId)[0];
                                if (member.memberId == infoLoad.userId)
                                {
                                    userCurrent = member;
                                }
                                else if (string.IsNullOrEmpty(member.conversationName))
                                {
                                    conversationName += memberDB.userName + ",";
                                }
                            }
                            catch
                            {

                            }
                        }
                        if (!String.IsNullOrEmpty(conversationName)) conversationName = conversationName.Remove(conversationName.LastIndexOf(","));
                        if (string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                        Conversation conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, 0, "", "", DateTime.MinValue, userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                        conversation.message = dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay ? dr.messageList[dr.messageList.Count - 1].message : "";
                        conversation.messageType = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].messageType : "";
                        conversation.senderId = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].senderId : 0;
                        conversation.createAt = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime() : DateTime.MinValue;
                        conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";

                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy cuộc trò chuyện thành công";
                        APIConversation.data.countConversation = listConversationDB.Count;
                        APIConversation.data.conversation = conversation;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện chưa đọc nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        [HttpPost("GetListUnreaderConversation")]
        [AllowAnonymous]
        public APIConversationUnreader GetListUnreaderConversation([FromForm] APILoadListConversation infoLoad)
        {
            APIConversationUnreader APIConversation = new APIConversationUnreader();
            try
            {
                if (infoLoad.userId != 0)
                {
                    List<ConversationsDB> listConversationDB = new List<ConversationsDB>();
                    listConversationDB = DAOConversation.GetListConversationUnreader(infoLoad.userId);
                    if (listConversationDB.Count > 0)
                    {
                        APIConversation.data = new DataConversationUnreader();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy cuộc trò chuyện thành công";
                        APIConversation.data.countConversation = listConversationDB.Count;
                        APIConversation.data.listConversation = listConversationDB.Select(x => x.id).ToList();
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện chưa đọc nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        [HttpPost("ReadAllMessage")]
        [AllowAnonymous]
        public APIConversation ReadAllMessage([FromForm] APILoadListConversation infoLoad)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (infoLoad.userId != 0)
                {
                    List<ConversationsDB> listConversationDB = new List<ConversationsDB>();
                    listConversationDB = DAOConversation.GetListConversationUnreader(infoLoad.userId);
                    if (listConversationDB.Count > 0)
                    {
                        foreach (ConversationsDB dr in listConversationDB)
                        {
                            DAOConversation.ReadMessage(dr.id, infoLoad.userId);
                            WIO.EmitAsync("ReadMessage", infoLoad.userId, dr.id, dr.memberList.Select(x => x.memberId).ToArray());
                        }
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "đọc tất cả tin nhắn thành công";
                        APIConversation.data.countConversation = listConversationDB.Count;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện chưa đọc nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }


        //(5/8/22)
        [HttpPost("SearchAll")]
        [AllowAnonymous]
        public APISearch SearchAll([FromForm] Conversation searchInfo)
        {
            APISearch api = new APISearch();
            try
            {
                if (searchInfo.senderId != 0)
                {
                    if (searchInfo.message == null)
                    {
                        searchInfo.message = "";
                    }
                    var listRequest = DAOUsers.getRequestContact(searchInfo.senderId);
                    var listFriend = DAOUsers.GetListContact(searchInfo.senderId, 0, 10000);
                    List<Contact> listContactInCom = new List<Contact>();
                    if (searchInfo.companyId != 0)
                    {
                        List<UserDB> datatCompany = DAOUsers.searchByCompanyContactInHomePage(searchInfo.message, searchInfo.senderId, searchInfo.companyId, 5);
                        foreach (UserDB member in datatCompany)
                        {
                            Contact userInfo = new Contact(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive, member.companyId, member.type365);
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

                            if (listFriend.Any(x => x.id == member.id)) userInfo.FriendStatus = "friend";
                            else
                            {
                                int requestIndex = listRequest.FindIndex(x => x.contactId == member.id);
                                if (requestIndex > -1)
                                {
                                    userInfo.FriendStatus = listRequest[requestIndex].status;
                                }
                            }


                            listContactInCom.Add(userInfo);
                        }
                    }

                    List<Conversation> listGroup = DAOConversation.SearchGroupByName(searchInfo.senderId, searchInfo.message, 5);

                    List<UserDB> datatContact = DAOUsers.searchContactInHomePage(searchInfo.message, searchInfo.senderId, searchInfo.companyId, 5);
                    List<Contact> listContact = new List<Contact>();
                    foreach (UserDB member in datatContact)
                    {
                        Contact userInfo = new Contact(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive, member.companyId, member.type365);
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

                        if (listFriend.Any(x => x.id == member.id)) userInfo.FriendStatus = "friend";
                        else
                        {
                            int requestIndex = listRequest.FindIndex(x => x.contactId == member.id);
                            if (requestIndex > -1)
                            {
                                userInfo.FriendStatus = listRequest[requestIndex].status;
                            }
                        }


                        listContact.Add(userInfo);
                    }

                    if (listContactInCom.Count > 0 || listContact.Count > 0 || listGroup.Count > 0)
                    {
                        api.data = new DataSearch();
                        api.data.result = true;
                        api.data.listContactInCompany = listContactInCom;
                        api.data.listGroup = listGroup;
                        api.data.listEveryone = listContact;
                        api.data.message = "Lấy danh sách thành công";
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "Không tìm thấy kết quả";
                    }
                }
                else
                {
                    api.error = new Error();
                    api.error.code = 200;
                    api.error.message = "Thiếu thông tin truyền lên";
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

        //
        [HttpPost("GetListGroup")]
        [AllowAnonymous]
        public APIConversationForward GetListGroup()
        {
            APIConversationForward api = new APIConversationForward();
            try
            {
                var http = HttpContext.Request;
                int userID = Convert.ToInt32(http.Form["userId"]);
                if (userID != 0)
                {
                    List<ConversationForward> list = new List<ConversationForward>();
                    List<Conversation> listGroup = DAOConversation.SearchGroupByName(userID, "", 10000);
                    if (listGroup.Count > 0)
                    {
                        foreach (var item in listGroup)
                        {
                            ConversationForward conver = new ConversationForward(item.conversationId, item.conversationName, !string.IsNullOrEmpty(item.avatarConversation) ? item.avatarConversation : item.LinkAvatar, item.isGroup, $"{item.listMember.Count} thành viên");
                            List<Conversation> listConversation = new List<Conversation>();
                            list.Add(conver);
                        }
                        api.data = new DataConversationForward();
                        api.data.result = true;
                        api.data.message = "lấy danh sách nhóm thành công";
                        api.data.listCoversation = list;
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "user này không thuộc nhóm nào";
                    }
                }
                else
                {
                    api.error = new Error();
                    api.error.code = 200;
                    api.error.message = "Thiếu thông tin truyền lên";
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

        //(5/8/22)
        [HttpPost("SearchListConversation")]
        [AllowAnonymous]
        public APIConversation SearchListConversation([FromForm] Conversation searchInfo)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (searchInfo.senderId != 0)
                {
                    if (searchInfo.message == null)
                    {
                        searchInfo.message = "";
                    }
                    List<Conversation> listConversation = DAOConversation.SearchGroupByName(searchInfo.senderId, searchInfo.message, searchInfo.countMessage == 0 ? 20 : searchInfo.countMessage);
                    if (listConversation.Count > 0)
                    {
                        listConversation.Sort(new CompareByTimeLastMessage());
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy danh sách cuộc trò chuyện thành công";
                        APIConversation.data.listCoversation = listConversation;
                        APIConversation.data.countConversation = listConversation.Count;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        //(5/8/22)
        [HttpPost("CheckReconnectInternet")]
        [AllowAnonymous]
        public APIConversation CheckReconnectInternet([FromForm] APILoadListConversation infoLoad)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (infoLoad.userId != 0)
                {
                    List<ConversationsDB> listConversationDB = DAOConversation.CheckReconnectInternet(infoLoad.userId, infoLoad.lastTimeMess);
                    List<Conversation> listConversation = new List<Conversation>();
                    var listRequest = DAOUsers.getRequestContact(infoLoad.userId);
                    var listFriend = DAOUsers.GetListContact(infoLoad.userId, 0, 10000);
                    foreach (ConversationsDB dr in listConversationDB)
                    {
                        ParticipantsDB userCurrent = dr.memberList.Where(x => x.memberId == infoLoad.userId).Single();
                        Conversation conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, dr.messageList[dr.messageList.Count - 1].senderId, dr.messageList[dr.messageList.Count - 1].message, dr.messageList[dr.messageList.Count - 1].messageType, dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime(), userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                        conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";
                        conversation.listMember = new List<MemberConversation>();
                        foreach (ParticipantsDB member in dr.memberList)
                        {
                            try
                            {
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
                                conversation.listMember.Add(userInfo);
                            }
                            catch
                            {

                            }
                        }

                        conversation.countMessage = Convert.ToInt32(DAOConversation.GetCountMessageOfConversation(conversation.conversationId)[0].messageList.Count);

                        conversation.listBrowerMember = new List<BrowseMember>();
                        if (conversation.isGroup == 1 && conversation.typeGroup.Equals("Moderate"))
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
                        if (listConversation.Count == infoLoad.countConversation - infoLoad.countConversationLoad)
                        {
                            break;
                        }
                    }
                    if (listConversation.Count > 0)
                    {
                        listConversation.Sort(new CompareByTimeLastMessage());
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy danh sách cuộc trò chuyện thành công";
                        APIConversation.data.listCoversation = listConversation;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        //(5/8/22)
        [HttpPost("GetListCall")]
        [AllowAnonymous]
        public APIConversation GetListCall([FromForm] User user)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (user.ID != 0)
                {

                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        //(5/8/22)
        [HttpPost("GetConversationList")]
        [AllowAnonymous]
        public APIConversation GetConversationList([FromForm] APILoadListConversation infoLoad)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (infoLoad.userId != 0)
                {
                    List<ConversationsDB> listConversationDB = new List<ConversationsDB>();
                    listConversationDB = DAOConversation.GetListConversation(infoLoad.userId, 10, 0);
                    List<Conversation> listConversation = new List<Conversation>();
                    var listRequest = DAOUsers.getRequestContact(infoLoad.userId);
                    var listFriend = DAOUsers.GetListContact(infoLoad.userId, 0, 10000);
                    foreach (ConversationsDB dr in listConversationDB)
                    {
                        ParticipantsDB userCurrent = new ParticipantsDB();
                        List<MemberConversation> listMember = new List<MemberConversation>();
                        string conversationName = "";
                        foreach (ParticipantsDB member in dr.memberList)
                        {
                            try
                            {
                                if (member.memberId == infoLoad.userId)
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
                                if (member.liveChat != null && !string.IsNullOrEmpty(member.liveChat.clientId)) userInfo.LiveChat = getClientInfo(member.liveChat);
                                listMember.Add(userInfo);
                            }
                            catch
                            {

                            }
                        }
                        if (dr.isGroup == 1 && string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                        Conversation conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, 0, "", "", DateTime.MinValue, userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                        conversation.message = dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay ? dr.messageList[dr.messageList.Count - 1].message : "";
                        conversation.messageType = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].messageType : "";
                        conversation.senderId = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].senderId : 0;
                        conversation.createAt = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime() : DateTime.MinValue;
                        conversation.listMember = listMember;
                        conversation.countMessage = dr.messageList.Where(x => x.displayMessage > userCurrent.messageDisplay).Count();
                        conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";

                        conversation.listBrowerMember = new List<BrowseMember>();
                        if (conversation.isGroup == 1 && conversation.typeGroup.Equals("Moderate"))
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
                    if (listConversation.Count > 0)
                    {
                        listConversation.Sort(new CompareByTimeLastMessage());
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy danh sách cuộc trò chuyện thành công";
                        APIConversation.data.listCoversation = listConversation;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //(5/8/22)
        [HttpPost("GetConversation")]
        [AllowAnonymous]
        public APIConversation GetConversation([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.senderId != 0)
                {
                    ConversationsDB dr = DAOConversation.GetConversation(con.conversationId, con.senderId);
                    if (dr != null)
                    {
                        Conversation conversation = new Conversation();
                        ParticipantsDB userCurrent = new ParticipantsDB();
                        List<MemberConversation> listMember = new List<MemberConversation>();
                        string conversationName = "";
                        var listRequest = DAOUsers.getRequestContact(con.senderId);
                        var listFriend = DAOUsers.GetListContact(con.senderId, 0, 10000);
                        foreach (ParticipantsDB member in dr.memberList)
                        {
                            try
                            {
                                UserDB memberDB = DAOUsers.GetUserById(member.memberId)[0];
                                if (member.memberId == con.senderId)
                                {
                                    userCurrent = member;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(member.conversationName)) conversationName += memberDB.userName + ",";
                                    else if (!string.IsNullOrEmpty(member.conversationName) && dr.isGroup == 1) conversationName = member.conversationName;
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
                                if (member.liveChat != null && !string.IsNullOrEmpty(member.liveChat.clientId)) userInfo.LiveChat = getClientInfo(member.liveChat);
                                listMember.Add(userInfo);
                            }
                            catch
                            {

                            }
                        }
                        if (!String.IsNullOrEmpty(conversationName) && conversationName.Contains(",")) conversationName = conversationName.Remove(conversationName.LastIndexOf(","));
                        if (string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                        conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, 0, "", "", DateTime.MinValue, userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                        conversation.message = dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay ? dr.messageList[dr.messageList.Count - 1].message : "";
                        conversation.messageType = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].messageType : "";
                        conversation.senderId = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].senderId : 0;
                        if (dr.messageList.Count <= 0) conversation.senderId = con.senderId;
                        conversation.createAt = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime() : DateTime.MinValue;
                        conversation.listMember = listMember;
                        conversation.countMessage = dr.messageList.Where(x => x.displayMessage > userCurrent.messageDisplay).Count();
                        conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";

                        conversation.listBrowerMember = new List<BrowseMember>();
                        if (conversation.isGroup == 1 && conversation.typeGroup.Equals("Moderate"))
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


                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy thông tin cuộc trò chuyện thành công";
                        APIConversation.data.conversation_info = conversation;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Cuộc trò chuyện không tồn tại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        [HttpPost("Get_Conversation")]
        [AllowAnonymous]
        public APIConversation Get_Conversation([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.senderId != 0)
                {
                    ConversationsDB dr = DAOConversation.GetConversation(con.conversationId, con.senderId);
                    if (dr != null)
                    {
                        Conversation conversation = new Conversation();
                        ParticipantsDB userCurrent = new ParticipantsDB();
                        List<MemberConversation> listMember = new List<MemberConversation>();
                        var listRequest = DAOUsers.getRequestContact(con.senderId);
                        var listFriend = DAOUsers.GetListContact(con.senderId, 0, 10000);
                        string conversationName = "";
                        foreach (ParticipantsDB member in dr.memberList)
                        {
                            try
                            {
                                UserDB memberDB = DAOUsers.GetUserById(member.memberId)[0];
                                if (member.memberId == con.senderId)
                                {
                                    userCurrent = member;
                                }
                                if (string.IsNullOrEmpty(conversationName) && !string.IsNullOrEmpty(member.conversationName))
                                {
                                    if (string.IsNullOrEmpty(member.conversationName)) conversationName += memberDB.userName + ",";
                                    else if (!string.IsNullOrEmpty(member.conversationName) && dr.isGroup == 1) conversationName = member.conversationName;
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
                        //if (dr.isGroup == 1 && string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                        if (!String.IsNullOrEmpty(conversationName) && conversationName.Contains(",")) conversationName = conversationName.Remove(conversationName.LastIndexOf(","));
                        if (string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                        conversation = new Conversation(dr.id, userCurrent.conversationName, dr.avatarConversation, userCurrent.unReader, dr.isGroup, 0, "", "", DateTime.MinValue, userCurrent.messageDisplay, dr.typeGroup, dr.shareGroupFromLinkOption, dr.browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, dr.pinMessage, userCurrent.deleteTime, userCurrent.deleteType, dr.adminId, "");
                        conversation.message = dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay ? dr.messageList[dr.messageList.Count - 1].message : "";
                        conversation.messageType = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].messageType : "";
                        conversation.senderId = (dr.messageList.Count > 0 && dr.messageList[dr.messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? dr.messageList[dr.messageList.Count - 1].senderId : 0;
                        conversation.createAt = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].createAt.ToLocalTime() : DateTime.MinValue;
                        conversation.listMember = listMember;
                        conversation.countMessage = Convert.ToInt32(DAOConversation.GetCountMessageOfConversation(conversation.conversationId)[0].messageList.Count);
                        conversation.messageId = dr.messageList.Count > 0 ? dr.messageList[dr.messageList.Count - 1].id : "";

                        conversation.listBrowerMember = new List<BrowseMember>();
                        if (conversation.isGroup == 1 && conversation.typeGroup.Equals("Moderate"))
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

                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy thông tin cuộc trò chuyện thành công";
                        APIConversation.data.conversation_info = conversation;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Cuộc trò chuyện không tồn tại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong05/08
        [HttpPost("AddNewConversation")]
        [AllowAnonymous]
        public APIConversation AddNewConversation([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.memberList != null && con.senderId != 0 && con.typeGroup != null)
                {
                    int conversationId = DAOConversation.insertNewConversation(1, con.typeGroup, con.senderId);
                    if (conversationId != 0)
                    {
                        if (con.conversationName == null)
                        {
                            con.conversationName = "";
                        }
                        int[] memberList = JsonConvert.DeserializeObject<int[]>(con.memberList);
                        int countMember = DAOConversation.insertNewParticipant(conversationId, con.conversationName, 1, memberList, con.senderId, con.typeGroup);
                        for (int i = 0; i < memberList.Length; i++)
                        {
                            if (memberList[i] == con.senderId)
                            {
                                DAOMessages.InsertMessage(DateTime.Now.Ticks + "_" + con.senderId, conversationId, 0, "notification", con.senderId + "  joined this consersation", "", "", DateTime.Now, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0);
                                break;
                            }
                        }
                        for (int i = 0; i < memberList.Length;)
                        {
                            if (DAOMessages.InsertMessage(DateTime.Now.Ticks + "_" + con.senderId, conversationId, 0, "notification", con.senderId + " added " + memberList[i] + " to this consersation", "", "", DateTime.Now, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0) != 0)
                            {
                                i++;
                            }
                        }
                        if (countMember != 0)
                        {
                            APIConversation.data = new DataConversation();
                            APIConversation.data.conversation_info = new Conversation();
                            APIConversation.data.conversation_info.conversationId = conversationId;
                            APIConversation.data.result = true;
                            APIConversation.data.message = "Tạo nhóm thành công";
                        }
                        else
                        {
                            APIConversation.error = new Error();
                            APIConversation.error.code = 200;
                            APIConversation.error.message = "Tạo nhóm thất bạia";
                        }
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Tạo nhóm thất bạib";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //tạo cuộc trò chuyện w247
        [HttpPost("CreateNewLiveChat")]
        [AllowAnonymous]
        public APIConversation CreateNewLiveChat()
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                var http = HttpContext.Request;
                int senderId = Convert.ToInt32(http.Form["senderId"]);
                string clientId = http.Form["clientId"];
                string clientName = http.Form["clientName"];
                int contactId = Convert.ToInt32(http.Form["contactId"]);
                string conversationName = http.Form["conversationName"];
                string fromWeb = http.Form["fromWeb"];
                int fromConversation = Convert.ToInt32(http.Form["fromConversation"]);
                if (senderId != 0 && !string.IsNullOrEmpty(clientId) && contactId != 0 && !string.IsNullOrEmpty(fromWeb))
                {
                    List<ConversationsDB> ckConversation = DAOConversation.checkEmptyConversationLiveChat(senderId, contactId, clientId, fromWeb);
                    if (ckConversation.Count > 0)
                    {
                        Conversation conver = new Conversation(ckConversation[0], senderId);
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.conversation_info = conver;
                        if (conver.countMessage > 0)
                        {
                            APIConversation.data.conversation_info.listMessage = DAOMessages.GetMessageByConversatinId(conver.conversationId, 0, 20, conver.messageDisplay, senderId, 0);
                        }
                        APIConversation.data.conversation_info.listMember = new List<MemberConversation>();
                        foreach (ParticipantsDB memberDB in ckConversation[0].memberList)
                        {
                            List<UserDB> getuser = DAOUsers.GetInforUserById(memberDB.memberId);
                            if (getuser.Count > 0)
                            {
                                User u = getInforUser(getuser);
                                if (memberDB.liveChat != null && !string.IsNullOrEmpty(memberDB.liveChat.clientId))
                                {
                                    int index = ckConversation[0].memberList.FindIndex(x => x.memberId == senderId && x.liveChat.clientId == memberDB.liveChat.clientId);
                                    u.UserName = ckConversation[0].memberList[index].liveChat != null && !string.IsNullOrEmpty(ckConversation[0].memberList[index].liveChat.clientName) ? ckConversation[0].memberList[index].liveChat.clientName : ckConversation[0].memberList[index].liveChat.clientId;
                                    u.AvatarUser = getDefaultAvata(u.UserName);
                                }
                                MemberConversation member = new MemberConversation(u.ID, u.UserName, u.AvatarUser, u.Status, u.StatusEmotion, u.LastActive.ToLocalTime().ToString(), u.Active, u.isOnline, memberDB.unReader, u.CompanyId, memberDB.timeLastSeener.ToLocalTime(), u.IDTimViec, u.Type365, "");
                                APIConversation.data.conversation_info.listMember.Add(member);
                            }
                        }
                        APIConversation.data.message = "Tạo nhóm thành công";
                    }
                    else
                    {
                        int conversationId = DAOConversation.insertNewConversation(1, "liveChat", senderId);
                        if (conversationId != 0)
                        {
                            if (conversationName == null)
                            {
                                conversationName = "";
                            }
                            int[] listMember = new int[2] { senderId, contactId };
                            int countMember = DAOConversation.insertNewParticipant(conversationId, conversationName, 1, listMember, -1, "Normal");
                            for (int i = 0; i < listMember.Length; i++)
                            {
                                if (listMember[i] == senderId)
                                {
                                    string messageId = DateTime.Now.Ticks + "_" + senderId;
                                    DAOMessages.InsertMessage(messageId, conversationId, 0, "notification", senderId + "  joined this consersation", "", "", DateTime.Now, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0);
                                    DAOMessages.EditInfoSupport(messageId, new InfoSupportDB("", "", "", fromConversation, 0, 0, DateTime.MinValue));
                                    break;
                                }
                            }
                            for (int i = 0; i < listMember.Length;)
                            {
                                if (DAOMessages.InsertMessage(DateTime.Now.Ticks + "_" + senderId, conversationId, 0, "notification", senderId + " added " + listMember[i] + " to this consersation", "", "", DateTime.Now, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0) != 0)
                                {
                                    i++;
                                }
                            }
                            if (fromConversation != 0)
                            {
                                List<MessagesDB> listOldMessage = DAOMessages.getOldMessageFromLiveChat(fromConversation, clientId, fromWeb);
                                //System.IO.File.WriteAllText("check_createlivechat.txt",JsonConvert.SerializeObject(listOldMessage));
                                for (int i = 0; i < listOldMessage.Count; i++)
                                {
                                    var item = listOldMessage[i];
                                    string messageId = $"{DateTime.Now.Ticks}_{item.senderId}";
                                    DAOMessages.InsertMessage(messageId, conversationId, item.senderId, item.messageType, item.message, item.quoteMessage, item.messageQuote, item.createAt, item.infoLink, item.listFile, null, item.deleteTime, item.deleteType, item.deleteDate, item.isEdited, item.liveChat);
                                }
                            }
                            if (countMember != 0)
                            {
                                DAOConversation.UpdateLiveChat(clientId, clientName, fromWeb, conversationId, senderId);
                                APIConversation.data = new DataConversation();
                                APIConversation.data.conversation_info = new Conversation();
                                APIConversation.data.conversation_info.conversationId = conversationId;
                                APIConversation.data.result = true;
                                APIConversation.data.message = "Tạo nhóm thành công";

                                ConversationsDB c = DAOConversation.GetConversation(conversationId, senderId);
                                Conversation conver = new Conversation(c, senderId);
                                APIConversation.data.conversation_info = conver;
                                if (conver.countMessage > 0)
                                {
                                    APIConversation.data.conversation_info.listMessage = DAOMessages.GetMessageByConversatinId(conver.conversationId, 0, 20, conver.messageDisplay, senderId, 0);
                                }
                                APIConversation.data.conversation_info.listMember = new List<MemberConversation>();
                                foreach (ParticipantsDB memberDB in c.memberList)
                                {
                                    List<UserDB> getuser = DAOUsers.GetInforUserById(memberDB.memberId);
                                    if (getuser.Count > 0)
                                    {
                                        User u = getInforUser(getuser);
                                        if (memberDB.liveChat != null && !string.IsNullOrEmpty(memberDB.liveChat.clientId))
                                        {
                                            int index = c.memberList.FindIndex(x => x.memberId == senderId && x.liveChat.clientId == memberDB.liveChat.clientId);
                                            u.UserName = c.memberList[index].liveChat != null && !string.IsNullOrEmpty(c.memberList[index].liveChat.clientName) ? c.memberList[index].liveChat.clientName : c.memberList[index].liveChat.clientId;
                                            u.AvatarUser = getDefaultAvata(u.UserName);
                                        }
                                        MemberConversation member = new MemberConversation(u.ID, u.UserName, u.AvatarUser, u.Status, u.StatusEmotion, u.LastActive.ToLocalTime().ToString(), u.Active, u.isOnline, memberDB.unReader, u.CompanyId, memberDB.timeLastSeener.ToLocalTime(), u.IDTimViec, u.Type365, "");
                                        APIConversation.data.conversation_info.listMember.Add(member);
                                    }
                                }
                            }
                            else
                            {
                                APIConversation.error = new Error();
                                APIConversation.error.code = 200;
                                APIConversation.error.message = "Tạo nhóm thất bại";
                            }
                        }
                        else
                        {
                            APIConversation.error = new Error();
                            APIConversation.error.code = 200;
                            APIConversation.error.message = "Tạo nhóm thất bại";
                        }
                    }

                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //đã dược
        [HttpPost("AddBrowseMember")]
        [AllowAnonymous]
        public APIConversation AddBrowseMember([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.memberList != null && con.senderId != 0 && con.conversationId != 0)
                {
                    int[] memberList = JsonConvert.DeserializeObject<int[]>(con.memberList);
                    int count = -1;
                    if (DAOConversation.CheckEmptyMember(con.conversationId, con.senderId))
                    {
                        for (int i = 0; i < memberList.Length; i++)
                        {
                            if (DAOConversation.CheckEmptyBrowserMember(con.conversationId, memberList[i]) && !DAOConversation.CheckEmptyMember(con.conversationId, memberList[i]))
                            {
                                DAOConversation.InsertNewBrowserMember(con.conversationId, memberList[i], con.senderId);
                                count = i;
                            }
                        }
                        if (count != -1)
                        {
                            APIConversation.data = new DataConversation();
                            APIConversation.data.result = true;
                            APIConversation.data.message = "Thêm người chờ duyệt thành công";
                        }
                        else
                        {
                            APIConversation.error = new Error();
                            APIConversation.error.code = 200;
                            APIConversation.error.message = "Tất cả những người này đã nằm trong danh sách chờ duyệt hoặc đã là thành viên của nhóm";
                        }
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Người thêm vào phải là thành viên của nhóm";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong05/08
        [HttpPost("ChangeAvatarGroup")]
        [AllowAnonymous]
        public APIConversation ChangeAvatarGroup([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.avatarConversation != null && con.conversationId != 0)
                {
                    if (con.avatarConversation.EndsWith(".jpg"))
                    {
                        int count = DAOConversation.ChangeAvatarGroup(con.conversationId, con.avatarConversation);
                        if (count != 0)
                        {
                            APIConversation.data = new DataConversation();
                            APIConversation.data.result = true;
                            APIConversation.data.message = "Thay đổi ảnh nhóm thành công";
                        }
                        else
                        {
                            APIConversation.error = new Error();
                            APIConversation.error.code = 200;
                            APIConversation.error.message = "Thay đổi ảnh nhóm thất bại";
                        }
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Định dạng ảnh không đúng";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong 05/08
        [HttpPost("ChangeNameGroup")]
        [AllowAnonymous]
        public APIConversation ChangeNameGroup([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0)
                {
                    if (con.conversationName == null)
                    {
                        con.conversationName = "";
                    }
                    int count = DAOConversation.ChangeNameGroup(con.conversationId, con.conversationName);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Thay đổi tên nhóm thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Thay đổi tên nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong 06/08
        [HttpPost("ChangeNickName")]
        [AllowAnonymous]
        public APIConversation ChangeNickName([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.adminId != 0)
                {
                    int count = DAOConversation.ChangeNickName(con.conversationId, con.conversationName, con.adminId);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Thay đổi biệt hiệu thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Thay đổi biệt hiệu thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong 05/08
        [HttpPost("ChangeShareLinkOfGroup")]
        [AllowAnonymous]
        public APIConversation ChangeShareLinkOfGroup([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0)
                {
                    int count = DAOConversation.ChangeShareLinkOfGroup(con.conversationId, con.shareGroupFromLink);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Thay đổi chia sẻ link nhóm thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Thay đổi chia sẻ link nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong
        [HttpPost("PinMessage")]
        [AllowAnonymous]
        public APIConversation PinMessage([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.pinMessageId != null)
                {
                    int count = DAOConversation.PinMessage(con.conversationId, con.pinMessageId);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Ghim tin nhắn thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Ghim tin nhắn thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        [HttpPost("GetInfoLiveChat")]
        [AllowAnonymous]
        public APIConversation GetInfoLiveChat()
        {
            APIConversation api = new APIConversation();
            try
            {
                var http = HttpContext.Request;
                int senderId = Convert.ToInt32(http.Form["senderId"]);
                string clientId = http.Form["clientId"];
                string fromWeb = http.Form["fromWeb"];
                string contact = http.Form["contactId"];
                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(fromWeb))
                {
                    int contactId = !string.IsNullOrEmpty(contact) ? Convert.ToInt32(contact) : 56387;
                    var ck = DAOConversation.checkClientConversationLiveChat(clientId, contactId, fromWeb);
                    if (ck.Count > 0)
                    {
                        Conversation conversation = new Conversation();
                        ParticipantsDB userCurrent = new ParticipantsDB();
                        List<MemberConversation> listMember = new List<MemberConversation>();
                        string conversationName = "";
                        var listRequest = DAOUsers.getRequestContact(senderId);
                        var listFriend = DAOUsers.GetListContact(senderId, 0, 10000);
                        foreach (ParticipantsDB member in ck[0].memberList)
                        {
                            try
                            {
                                UserDB memberDB = DAOUsers.GetUserById(member.memberId)[0];
                                if (member.memberId == senderId)
                                {
                                    userCurrent = member;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(member.conversationName)) conversationName += memberDB.userName + ",";
                                    else if (!string.IsNullOrEmpty(member.conversationName) && ck[0].isGroup == 1) conversationName = member.conversationName;
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

                                if (member.liveChat != null && !string.IsNullOrEmpty(member.liveChat.clientId))
                                {
                                    userInfo.LiveChat = getClientInfo(member.liveChat);
                                }

                                listMember.Add(userInfo);
                            }
                            catch
                            {

                            }
                        }
                        if (!String.IsNullOrEmpty(conversationName) && conversationName.Contains(",")) conversationName = conversationName.Remove(conversationName.LastIndexOf(","));
                        if (string.IsNullOrEmpty(userCurrent.conversationName)) userCurrent.conversationName = conversationName;
                        conversation = new Conversation(ck[0].id, userCurrent.conversationName, ck[0].avatarConversation, userCurrent.unReader, ck[0].isGroup, 0, "", "", DateTime.MinValue, userCurrent.messageDisplay, ck[0].typeGroup, ck[0].shareGroupFromLinkOption, ck[0].browseMemberOption, userCurrent.notification, userCurrent.isFavorite, userCurrent.isHidden, ck[0].pinMessage, userCurrent.deleteTime, userCurrent.deleteType, ck[0].adminId, "");
                        conversation.message = ck[0].messageList.Count > 0 && ck[0].messageList[ck[0].messageList.Count - 1].displayMessage > userCurrent.messageDisplay ? ck[0].messageList[ck[0].messageList.Count - 1].message : "";
                        conversation.messageType = (ck[0].messageList.Count > 0 && ck[0].messageList[ck[0].messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? ck[0].messageList[ck[0].messageList.Count - 1].messageType : "";
                        conversation.senderId = (ck[0].messageList.Count > 0 && ck[0].messageList[ck[0].messageList.Count - 1].displayMessage > userCurrent.messageDisplay) ? ck[0].messageList[ck[0].messageList.Count - 1].senderId : 0;
                        conversation.createAt = ck[0].messageList.Count > 0 ? ck[0].messageList[ck[0].messageList.Count - 1].createAt.ToLocalTime() : DateTime.MinValue;
                        conversation.listMember = listMember;
                        conversation.countMessage = ck[0].messageList.Where(x => x.displayMessage > userCurrent.messageDisplay).Count();
                        conversation.messageId = ck[0].messageList.Count > 0 ? ck[0].messageList[ck[0].messageList.Count - 1].id : "";

                        conversation.listBrowerMember = new List<BrowseMember>();
                        if (conversation.isGroup == 1 && conversation.typeGroup.Equals("Moderate"))
                        {
                            foreach (BrowseMembersDB member in ck[0].browseMemberList)
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


                        api.data = new DataConversation();
                        api.data.result = true;
                        api.data.message = "Lấy thông tin cuộc trò chuyện thành công";
                        api.data.conversation_info = conversation;
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "Người dùng chưa có cuộc trò chuyện";
                    }

                }
                else
                {
                    api.error = new Error();
                    api.error.code = 200;
                    api.error.message = "Thiêu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                api.error = new Error();
                api.error.message = ex.ToString();
            }
            return api;
        }

        //xong 05/08
        [HttpPost("UnPinMessage")]
        [AllowAnonymous]
        public APIConversation UnPinMessage([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0)
                {
                    int count = DAOConversation.PinMessage(con.conversationId, "");
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Gỡ ghim tin nhắn thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Gỡ ghim tin nhắn thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong 05/08
        [HttpPost("ChangeNotificationConversation")]
        [AllowAnonymous]
        public APIConversation ChangeNotificationConversation([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0)
                {
                    int count = DAOConversation.ChangeNotificationConversation(con.conversationId, con.notification, con.adminId);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Thay đổi thông báo nhóm thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Thay đổi thông báo nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong 05/08
        [HttpPost("ChangeBrowseMemberOfGroup")]
        [AllowAnonymous]
        public APIConversation ChangeBrowseMemberOfGroup([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0)
                {
                    int count = DAOConversation.ChangeBowserMemberOfGroup(con.conversationId, con.browseMember);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Thay đổi chia sẻ link nhóm thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Thay đổi chia sẻ link nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        // xong 05/08
        [HttpPost("OutGroup")]
        [AllowAnonymous]
        public APIConversation OutGroup([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.senderId != 0)
                {
                    //var ck = DAOConversation.GetConversation(con.conversationId, con.senderId);
                    int count = DAOConversation.OutGroup(con.conversationId, con.senderId);
                    if (count != 0)
                    {
                        //Task t = new Task(() =>
                        //{
                        //    if (ck != null)
                        //    {
                        //        if (ck.typeGroup.ToLower() == "livechat")
                        //        {
                        //            if (ck.messageList != null && ck.messageList.Count > 0)
                        //            {
                        //                int conId = 0;
                        //                LiveChatDB lv = null;
                        //                if (ck.messageList[0].infoSupport != null && ck.messageList[0].infoSupport.haveConversation != 0)
                        //                {
                        //                    conId = ck.messageList[0].infoSupport.haveConversation;
                        //                }
                        //                var mem = ck.memberList.Where(x => x.liveChat != null && !string.IsNullOrEmpty(x.liveChat.clientId)).ToList();
                        //                if (mem.Count > 0)
                        //                {
                        //                    lv = mem[0].liveChat;
                        //                }
                        //                if (conId != 0 && lv != null) DAOMessages.SetOutLivechat(conId, lv.clientId, lv.fromWeb);
                        //            }
                        //        }
                        //    }
                        //});
                        //t.ContinueWith(p =>
                        //{
                        //    t.Dispose();
                        //});
                        //t.Start();
                        if (con.adminId != 0)
                        {
                            if (DAOConversation.UpdateAdminOfgroup(con.conversationId, con.adminId) != 0)
                            {
                                APIConversation.data = new DataConversation();
                                APIConversation.data.result = true;
                                APIConversation.data.message = "Trao quyền quản trị viên nhóm thành công";
                            }
                            else
                            {
                                APIConversation.error = new Error();
                                APIConversation.error.code = 200;
                                APIConversation.error.message = "Trao quyền quản trị viên nhóm thất bại";
                            }
                        }
                        else
                        {
                            APIConversation.data = new DataConversation();
                            APIConversation.data.result = true;
                            APIConversation.data.message = "Rời nhóm thành công";
                        }
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Rời nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong 05/08
        [HttpPost("DeleteConversation")]
        [AllowAnonymous]
        public APIConversation DeleteConversation([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.senderId != 0)
                {
                    DAOConversation.HiddenConversation(con.conversationId, con.senderId, 1);
                    DAOConversation.DeleteConversation(con.conversationId, con.senderId, DAOConversation.GetLastMessageConversation(con.conversationId));
                    APIConversation.data = new DataConversation();
                    APIConversation.data.result = true;
                    APIConversation.data.message = "Xóa cuộc trò chuyện thành công";
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }

        [HttpPost("RemoveConversation")]
        [AllowAnonymous]
        public APIConversation RemoveConversation()
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                var http = HttpContext.Request;
                int conversationId = Convert.ToInt32(http.Form["conversationId"]);
                string clientId = http.Form["clientId"];
                string fromWeb = http.Form["fromWeb"];
                if (conversationId != 0)
                {
                    ConversationsDB getConversation = DAOConversation.GetConversation(conversationId, 0);
                    if (getConversation != null)
                    {
                        if (getConversation.messageList.Count > 0 && getConversation.messageList[0].infoSupport != null && getConversation.messageList[0].infoSupport.haveConversation != 0)
                        {
                            Task t = new Task(() =>
                            {
                                int fromConversation = getConversation.messageList[0].infoSupport.haveConversation;
                                var listOld = DAOMessages.getOldMessageFromLiveChat(fromConversation, clientId, fromWeb);
                                if (listOld.Count > 0)
                                {
                                    foreach (var item in listOld)
                                    {
                                        DAOMessages.EditDeleteType(item.id, 2);
                                    }
                                }
                            });
                            t.Start();
                            t.ContinueWith(p =>
                            {
                                t.Dispose();
                            });
                        }
                        if (DAOConversation.RemoveConversation(conversationId) > 0)
                        {
                            if (getConversation.memberList != null && getConversation.memberList.Count > 0)
                            {
                                foreach (ParticipantsDB memberDB in getConversation.memberList)
                                {
                                    WIO.EmitAsync("DeleteConversation", memberDB.memberId, conversationId);
                                }
                            }
                            APIConversation.data = new DataConversation();
                            APIConversation.data.result = true;
                            APIConversation.data.message = "Xóa cuộc trò chuyện thành công";
                        }
                        else
                        {
                            APIConversation.error = new Error();
                            APIConversation.error.code = 200;
                            APIConversation.error.message = "Xóa cuộc trò chuyện thất bại";
                        }
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Cuộc trò chuyện không tồn tại";
                    }

                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong 05/08
        [HttpPost("AddToFavoriteConversation")]
        [AllowAnonymous]
        public APIConversation AddToFavoriteConversation([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.senderId != 0)
                {
                    int count = DAOConversation.AddToFavoriteConversation(con.conversationId, con.senderId, con.isFavorite);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Sửa Trạng thái yêu thích nhóm thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Sửa Trạng thái yêu thích nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //[HttpPost("AddNewFieldConversation")]
        //[AllowAnonymous]
        //public APIConversation AddNewFieldConversation()
        //{ 
        //    APIConversation APIConversation = new APIConversation();
        //    int count = DAOConversation.AddNewFieldConversation();
        //    if (count != 0)
        //    {
        //        APIConversation.data = new DataConversation();
        //        APIConversation.data.result = true;
        //        APIConversation.data.message = "Sửa Trạng thái ẩn nhóm thành công :" + count;
        //    }
        //    else
        //    {
        //        APIConversation.error = new Error();
        //        APIConversation.error.code = 200;
        //        APIConversation.error.message = "Sửa Trạng thái ẩn nhóm thất bại";
        //    }
        //    return APIConversation;
        //}
        //xong 05/08
        [HttpPost("HiddenConversation")]
        [AllowAnonymous]
        public APIConversation HiddenConversation([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.conversationId != 0 && con.senderId != 0)
                {
                    int count = DAOConversation.HiddenConversation(con.conversationId, con.senderId, con.isHidden);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Sửa Trạng thái ẩn nhóm thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Sửa Trạng thái ẩn nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        // xong
        [HttpPost("ReadMessage")]
        [AllowAnonymous]
        public APIConversation ReadMessage([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.senderId != 0 && con.conversationId != 0)
                {
                    int count = DAOConversation.ReadMessage(con.conversationId, con.senderId);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Đánh dấu tin nhắn đã đọc thành công thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Không tồn tại tin nhắn chưa đọc";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong
        [HttpPost("MarkUnreader")]
        [AllowAnonymous]
        public APIConversation MarkUnreader([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.senderId != 0 && con.conversationId != 0)
                {
                    int count = DAOConversation.MarkUnreader(con.conversationId, con.senderId);
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Đánh dấu tin nhắn chưa đọc thành công thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Người dùng chưa đọc hết tin nhắn";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong -5/-8
        [HttpPost("AddNewMemberToGroup")]
        [AllowAnonymous]
        public APIConversation AddNewMemberToGroup([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.memberList != null && con.senderId != 0 && con.typeGroup != null && con.conversationId != 0)
                {
                    if (DAOConversation.CheckEmptyMember(con.conversationId, con.senderId))
                    {
                        if (con.conversationName == null)
                        {
                            con.conversationName = "";
                        }
                        int[] memberList = JsonConvert.DeserializeObject<int[]>(con.memberList);
                        int countMember = DAOConversation.insertNewParticipant(con.conversationId, con.conversationName, 1, memberList, con.senderId, con.typeGroup);
                        if (countMember != 0)
                        {
                            APIConversation.data = new DataConversation();
                            APIConversation.data.conversation_info = new Conversation();
                            APIConversation.data.conversation_info.conversationId = con.conversationId;
                            APIConversation.data.result = true;
                            APIConversation.data.message = "Thêm thành viên vào nhóm thành công";
                        }
                        else
                        {
                            APIConversation.error = new Error();
                            APIConversation.error.code = 200;
                            APIConversation.error.message = "Thêm thành viên vào nhóm thất bại";
                        }
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Người thêm vào phải là thành viên của nhóm";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //(5/8/22)
        [HttpPost("GetListConversationForward")]
        [AllowAnonymous]
        public APIConversationForward GetListConversationForward([FromForm] APILoadListConversation infoLoad)
        {
            APIConversationForward APIConversation = new APIConversationForward();
            try
            {
                if (string.IsNullOrEmpty(infoLoad.message))
                {
                    infoLoad.message = "";
                }
                if (infoLoad.userId != 0)
                {
                    List<ConversationForward> listConversation = new List<ConversationForward>();
                    if (infoLoad.countConversation - infoLoad.countConversationLoad >= 20)
                    {
                        listConversation = DAOConversation.GetListConversationForward(infoLoad.userId, infoLoad.companyId, infoLoad.message, 20, infoLoad.countConversationLoad);
                    }
                    else
                    {
                        listConversation = DAOConversation.GetListConversationForward(infoLoad.userId, infoLoad.companyId, infoLoad.message, infoLoad.countConversation - infoLoad.countConversationLoad, infoLoad.countConversationLoad);
                    }
                    if (listConversation.Count > 0)
                    {
                        APIConversation.data = new DataConversationForward();
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Lấy danh sách cuộc trò chuyện thành công";
                        APIConversation.data.listCoversation = listConversation;
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "User không có cuộc trò chuyện nào";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //đã sửa
        [HttpPost("CreateNewConversation")]
        [AllowAnonymous]
        public APIRequestContact CreateNewConversation([FromForm] RequestContact requestContact)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (requestContact.userId != 0 && requestContact.contactId != 0)
                {
                    if (DAOConversation.checkEmptyConversation(requestContact.userId, requestContact.contactId).Count == 0)
                    {
                        int conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                        if (conversationId > 0)
                        {
                            int[] users = new int[2];
                            users[0] = requestContact.userId;
                            users[1] = requestContact.contactId;
                            DAOConversation.insertNewParticipant(conversationId, "", 0, users, requestContact.userId, "Normal");
                        }
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.conversationId = conversationId;
                    }
                    else
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(requestContact.userId, requestContact.contactId)[0].id);
                    }
                }
                else
                {
                    requestAPI.error = new Error();
                    requestAPI.error.code = 200;
                    requestAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                requestAPI.error = new Error();
                requestAPI.error.code = 200;
                requestAPI.error.message = ex.ToString();
            }
            return requestAPI;
        }

        [HttpPost("CreateNewSecretConversation")]
        [AllowAnonymous]
        public APIRequestContact CreateNewSecretConversation([FromForm] RequestContact requestContact)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (requestContact.userId != 0 && requestContact.contactId != 0)
                {
                    if (DAOConversation.checkEmptySecretConversation(requestContact.userId, requestContact.contactId).Count == 0)
                    {
                        int conversationId = DAOConversation.insertNewConversation(0, "Secret", 0);
                        if (conversationId > 0)
                        {
                            int[] users = new int[2];
                            users[0] = requestContact.userId;
                            users[1] = requestContact.contactId;
                            DAOConversation.insertNewParticipant(conversationId, "", 0, users, requestContact.userId, "Secret");
                        }
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.conversationId = conversationId;
                    }
                    else
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.conversationId = Convert.ToInt32(DAOConversation.checkEmptySecretConversation(requestContact.userId, requestContact.contactId)[0].id);
                    }
                }
                else
                {
                    requestAPI.error = new Error();
                    requestAPI.error.code = 200;
                    requestAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                requestAPI.error = new Error();
                requestAPI.error.code = 200;
                requestAPI.error.message = ex.ToString();
            }
            return requestAPI;
        }
        //xong
        [HttpPost("GetCountConversationUnreader")]
        [AllowAnonymous]
        public int GetCountConversationUnreader([FromForm] User user)
        {
            try
            {
                return DAOConversation.GetCountConversationUnreader(user.ID);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        //xong
        [HttpPost("DeleteBrowse")]
        [AllowAnonymous]
        public APIConversation DeleteBrowse([FromForm] Conversation con)
        {
            APIConversation APIConversation = new APIConversation();
            try
            {
                if (con.memberList != null && con.conversationId != 0)
                {
                    int[] memberList = JsonConvert.DeserializeObject<int[]>(con.memberList);
                    int countMember = DAOConversation.DeleteBrowse(con.conversationId, memberList);

                    if (countMember != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.conversation_info = new Conversation();
                        APIConversation.data.conversation_info.conversationId = con.conversationId;
                        APIConversation.data.result = true;
                        APIConversation.data.message = "Xóa thành viên duyệt vào nhóm thành công";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Xóa thành viên duyệt vào nhóm thất bại";
                    }
                }
                else
                {
                    APIConversation.error = new Error();
                    APIConversation.error.code = 200;
                    APIConversation.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = ex.ToString();
            }
            return APIConversation;
        }
        //xong
        [HttpPost("getAllGroup")]
        [AllowAnonymous]
        public APIConversation getAllGroup([FromForm] Conversation con)
        {
            APIConversation userAPI = new APIConversation();
            try
            {
                if (con.senderId != 0)
                {
                    List<ConversationsDB> getListContact = DAOConversation.getAllGroup(con.senderId);
                    if (getListContact.Count > 0)
                    {
                        List<ConversationInSearch> listGroup = new List<ConversationInSearch>();
                        foreach (ConversationsDB member in getListContact)
                        {
                            listGroup.Add(new ConversationInSearch(
                            Convert.ToInt32(member.id),
                            member.memberList[0].conversationName.ToString(),
                            member.avatarConversation.ToString(),
                            1,
                            member.memberList.Count.ToString()));
                        }
                        userAPI.data = new DataConversation();
                        userAPI.data.listCoversationInSearch = listGroup;
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "Không người dùng trùng khớp";
                    }
                }
                else
                {
                    userAPI.error = new Error();
                    userAPI.error.code = 200;
                    userAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                userAPI.error = new Error();
                userAPI.error.code = 200;
                userAPI.error.message = ex.ToString();
            }
            return userAPI;
        }

        [HttpPost("UpdateDeleteTime")]
        [AllowAnonymous]
        public APIOTP UpdateDeleteTime()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int conversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                int deleteTime = Convert.ToInt32(httpRequest.Form["DeleteTime"]);
                if (userId != 0 && conversationId != 0)
                {
                    if (DAOConversation.UpdateDeleteTime(userId, conversationId, deleteTime) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật thời gian xóa thành công";
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "User không tồn tại";
                    }
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }

        [HttpPost("SetupDeleteTime")]
        [AllowAnonymous]
        public APIOTP SetupDeleteTime()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int conversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                int deleteTime = Convert.ToInt32(httpRequest.Form["DeleteTime"]);
                int deleteType = Convert.ToInt32(httpRequest.Form["DeleteType"]);
                int d = 0;
                if (deleteTime >= 86400) d = deleteTime / 86400;
                if (d > 30)
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "Thời gian xóa tối đa là 30 ngày";
                }
                if (userId != 0 && conversationId != 0 && d <= 30)
                {
                    if (DAOConversation.SetupDeleteTime(userId, conversationId, deleteTime, deleteType) > 0)
                    {
                        string time = "";
                        if (deleteTime == 0)
                        {
                            time = "off";
                        }
                        else if (deleteTime < 60)
                        {
                            time = deleteTime.ToString() + " second";
                        }
                        else if (deleteTime >= 60 && deleteTime < 3600)
                        {
                            time = $"{deleteTime / 60} minute {deleteTime % 60} second";
                        }
                        else if (deleteTime >= 3600 && deleteTime < 86400)
                        {
                            time = $"{deleteTime / 3600} hour {(deleteTime % 3600) / 60} minute {(deleteTime % 3600) % 60} second";
                        }
                        else if (deleteTime >= 86400)
                        {
                            time = $"{deleteTime / 86400} day";
                        }
                        string content = "set delete time";
                        if (deleteType == 1) content += " after reading";
                        string messId = DateTime.Now.Ticks.ToString() + "_" + userId.ToString();
                        Messages mess = new Messages(messId, conversationId, userId, "notification", $"{userId} {content} is {time}", 0, DateTime.Now, DateTime.MinValue, deleteTime, deleteType);
                        List<ParticipantsDB> listMember = DAOConversation.getListMemberIdOfConversation(conversationId);
                        DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, "", "", mess.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0);

                        WIO.EmitAsync("SendMessage", mess, listMember.Select(x => x.memberId).ToArray());

                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật thời gian xóa thành công";
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Cập nhật thời gian xóa thất bại";
                    }
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }
    }
}

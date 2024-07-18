using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Chat365.Server.Model.DAO;
using Chat365.Server.Model.Entity;
using Chat365.Server.Model.EntityAPI;
using Chat365.Model.DAO;
using HtmlAgilityPack;
using SocketIOClient;
using System.Text;
using System.Text.RegularExpressions;
using APIChat365.Model.EntityAPI;
using System.Net.Http;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using APIChat365.MongoEntity;
using APIChat365.Model.MongoEntity;
using APIChat365.Model.Entity;
using VisioForge.GStreamer.API;
using Microsoft.VisualBasic;
using System.Net.Mail;
using VisioForge.MediaFramework.ONVIFDiscovery.Models;
using System.Security.Claims;
using VisioForge.Libs.MediaFoundation.OPM;
using VisioForge.MediaFramework.Helpers;
using System.Diagnostics.Metrics;
using APIChat365.Model.DAO;

namespace Chat365.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
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

        public MessageController(ILogger<MessageController> logger,
            IWebHostEnvironment environment)
        {
            if (WIO.Disconnected)
            {
                WIO.ConnectAsync();
            }
            if (WIO2.Disconnected)
            {
                WIO2.ConnectAsync();
            }
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        public User getInforUser(List<UserDB> getUser)
        {
            User userInfo = new User(getUser[0].id, getUser[0].id365, getUser[0].idTimViec, getUser[0].type365, getUser[0].email, "", getUser[0].phone, getUser[0].userName, getUser[0].avatarUser, getUser[0].status, getUser[0].statusEmotion, getUser[0].lastActive, getUser[0].active, getUser[0].isOnline, getUser[0].looker, getUser[0].companyId, getUser[0].companyName, getUser[0].notificationPayoff, getUser[0].notificationCalendar, getUser[0].notificationReport, getUser[0].notificationOffer, getUser[0].notificationPersonnelChange, getUser[0].notificationRewardDiscipline, getUser[0].notificationNewPersonnel, getUser[0].notificationChangeProfile, getUser[0].notificationTransferAsset, getUser[0].notificationMissMessage, getUser[0].notificationCommentFromTimViec, getUser[0].notificationCommentFromRaoNhanh, getUser[0].notificationTag, getUser[0].notificationSendCandidate, getUser[0].notificationChangeSalary, getUser[0].notificationAllocationRecall, getUser[0].notificationAcceptOffer, getUser[0].notificationDecilineOffer, getUser[0].notificationNTDPoint, getUser[0].notificationNTDExpiredPin, getUser[0].notificationNTDExpiredRecruit, getUser[0].fromWeb, getUser[0].notificationNTDApplying);

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

            return userInfo;
        }

        private InfoLink getInfoLink(string link)
        {
            InfoLink infoLink = new InfoLink();
            try
            {
                infoLink.HaveImage = "False";
                if (link.EndsWith("/"))
                {
                    link = link.Remove(link.Length - 1);
                }
                int index = link.IndexOf('/', 9);
                if (index != -1)
                {
                    infoLink.LinkHome = link.Remove(index);
                }
                else
                {
                    infoLink.LinkHome = link;
                }
                HtmlWeb web = new HtmlWeb();
                var doc = web.LoadFromWebAsync(link);
                try
                {
                    var title = doc.Result.DocumentNode.SelectSingleNode("//title");
                    if (title != null)
                    {
                        infoLink.Title = title.InnerHtml;
                    }
                    else
                    {
                        infoLink.Title = "Không tìm thấy thông tin website";
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    infoLink.Description = doc.Result.DocumentNode.Descendants("meta").Where(meta => meta.GetAttributeValue("name", "null") == "description").First().GetAttributeValue("content", "null");
                }
                catch (Exception)
                {
                }
                try
                {
                    infoLink.Image = doc.Result.DocumentNode.Descendants("meta").Where(meta => meta.GetAttributeValue("property", "null") == "og:image").First().GetAttributeValue("content", "null").Replace("&#47;", "/").Replace("amp;", "");
                    if (infoLink.Image.Equals("null"))
                    {
                        infoLink.HaveImage = "False";
                    }
                    else
                    {
                        infoLink.HaveImage = "True";
                    }
                }
                catch (Exception)
                {
                    infoLink.HaveImage = "False";
                }
            }
            catch (Exception ex)
            {
                infoLink.Title = ex.Message;
            }
            return infoLink;
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
        [HttpPost("RaoNhanhSendMessageToHHP")]
        [AllowAnonymous]
        public APIMessage RaoNhanhSendMessageToHHP()
        {
            APIMessage APIMessages = new APIMessage();
            try
            {
                var httpRequest = HttpContext.Request;
                string MessageID = httpRequest.Form["MessageID"];
                int ConversationID = Convert.ToInt32(httpRequest.Form["ConversationID"]);
                int SenderID = Convert.ToInt32(httpRequest.Form["SenderID"]);
                string MessageType = httpRequest.Form["MessageType"];
                string Message = httpRequest.Form["Message"];
                string Quote = httpRequest.Form["Quote"];
                string Profile = httpRequest.Form["Profile"];
                string ListTag = httpRequest.Form["ListTag"];
                string File = httpRequest.Form["File"];
                string ListMember = httpRequest.Form["ListMember"];
                string IsOnline = httpRequest.Form["IsOnline"];
                string conversationName = httpRequest.Form["ConversationName"];
                int isGroup = Convert.ToInt32(httpRequest.Form["IsGroup"]);
                int deleteTime = Convert.ToInt32(httpRequest.Form["DeleteTime"]);
                int deleteType = Convert.ToInt32(httpRequest.Form["DeleteType"]);
                if (!String.IsNullOrEmpty(MessageType) && (!String.IsNullOrEmpty(File) || !String.IsNullOrEmpty(Message) || !String.IsNullOrEmpty(Quote)))
                {
                    ConversationID = GetConversationId(78007, 56387);
                    SenderID = 78007;
                    Messages mess = new Messages(MessageID, ConversationID, SenderID, MessageType, Message, ListTag, DateTime.MinValue, deleteTime, deleteType);
                    if (String.IsNullOrEmpty(Quote))
                    {
                        mess.QuoteMessage = new MessageQuote();
                        mess.QuoteMessage.MessageID = "";
                        mess.QuoteMessage.Message = "";
                    }
                    else
                    {
                        mess.QuoteMessage = JsonConvert.DeserializeObject<MessageQuote>(Quote);
                    }
                    if (!String.IsNullOrEmpty(File))
                    {
                        mess.ListFile = JsonConvert.DeserializeObject<List<InfoFile>>(File);
                    }
                    else
                    {
                        mess.ListFile = new List<InfoFile>();
                    }
                    if (!String.IsNullOrEmpty(Profile))
                    {
                        mess.UserProfile = JsonConvert.DeserializeObject<User>(Profile);
                    }
                    else
                    {
                        mess.UserProfile = new User();
                    }
                    if (String.IsNullOrEmpty(Message))
                    {
                        mess.Message = "";
                    }
                    mess.CreateAt = DateTime.Now;
                    mess.DeleteDate = DateTime.MinValue;
                    if (mess.DeleteType == 0 && mess.DeleteTime > 0)
                    {
                        mess.DeleteDate = mess.CreateAt.AddSeconds(mess.DeleteTime);
                    }
                    if (String.IsNullOrWhiteSpace(MessageID))
                    {
                        mess.MessageID = mess.CreateAt.Ticks + "_" + SenderID;
                    }
                    int[] listMember = new int[2];
                    int[] isOnline = new int[2];
                    if (!String.IsNullOrEmpty(ListMember))
                    {
                        listMember = JsonConvert.DeserializeObject<int[]>(ListMember);
                        isOnline = JsonConvert.DeserializeObject<int[]>(IsOnline);
                        if (listMember.Length == 0 && isOnline.Length == 0)
                        {
                            DataTable dataMember = DAOConversation.GetInfoConversation(ConversationID);
                            listMember = new int[dataMember.Rows.Count];
                            isOnline = new int[dataMember.Rows.Count];
                            for (int i = 0; i < dataMember.Rows.Count; i++)
                            {
                                listMember[i] = Convert.ToInt32(dataMember.Rows[i]["memberId"]);
                                isOnline[i] = Convert.ToInt32(dataMember.Rows[i]["isOnline"]);
                            }
                        }
                    }
                    else
                    {
                        DataTable dataMember = DAOConversation.GetInfoConversation(ConversationID);
                        listMember = new int[dataMember.Rows.Count];
                        isOnline = new int[dataMember.Rows.Count];
                        for (int i = 0; i < dataMember.Rows.Count; i++)
                        {
                            listMember[i] = Convert.ToInt32(dataMember.Rows[i]["memberId"]);
                            isOnline[i] = Convert.ToInt32(dataMember.Rows[i]["isOnline"]);
                        }
                    }
                    SendMailMissMessage(ConversationID, mess);
                    ConversationsDB conver = DAOConversation.GetConversation(ConversationID, SenderID);
                    if (!((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || (conver != null && conver.typeGroup == "liveChat")))
                    {
                        sendNotificationToTimViec(mess, conversationName, mess.ConversationID, listMember, isOnline, isGroup, true);
                    }
                    if (!mess.MessageType.Equals("link"))
                    {
                        WIO.EmitAsync("SendMessage", mess, listMember);
                        if (mess.MessageType.Equals("sendFile") || mess.MessageType.Equals("sendPhoto"))
                        {
                            List<FileSendDB> fileSend = new List<FileSendDB>();
                            foreach (InfoFile info in mess.ListFile)
                            {
                                fileSend.Add(new FileSendDB(info.SizeFile, info.FullName, info.Height, info.Width));
                            }
                            DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(), fileSend, _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0);
                        }
                        else if (mess.MessageType.Equals("map"))
                        {
                            string[] z = mess.Message.Split(',');
                            string link = String.Format(@"https://www.google.com/maps/search/{0},{1}/@{0},{1},10z?hl=vi", z[0].Trim(), z[1].Trim());
                            mess.InfoLink = getInfoLink(link);
                            DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(mess.InfoLink.Title, mess.InfoLink.Description, mess.InfoLink.LinkHome, mess.InfoLink.Image, mess.InfoLink.IsNotification), new List<FileSendDB>(), _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0);
                            WIO.EmitAsync("SendMessage", mess, listMember);
                        }
                        else
                        {
                            DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0);
                        }

                    }

                    Regex regex = new Regex(@"(http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-]))?");
                    if (mess.MessageType.Equals("link") || (mess.MessageType.Equals("text") && regex.IsMatch(mess.Message) && mess.Message.Length <= 500))
                    {
                        if (mess.MessageType.Equals("link"))
                        {
                            WIO.EmitAsync("SendMessage", mess, listMember);
                            sendLink(mess, listMember);
                        }
                        else
                        {
                            mess.MessageID = mess.CreateAt.Ticks + 12 + "_" + mess.SenderID;
                            mess.MessageType = "link";
                            int index = mess.Message.IndexOf("http", 0);
                            int indexSpace = mess.Message.Trim().IndexOf(" ", index);
                            int indexEnter = mess.Message.IndexOf("\n", index);
                            if ((indexSpace != -1 && indexEnter == -1) || (indexSpace != -1 && indexEnter != -1 && indexSpace < indexEnter))
                            {
                                mess.Message = mess.Message.Substring(index, indexSpace - index);
                                if (regex.IsMatch(mess.Message))
                                {
                                    WIO.EmitAsync("SendMessage", mess, listMember);
                                    sendLink(mess, listMember);
                                }
                            }
                            else if ((indexSpace == -1 && indexEnter != -1) || (indexSpace != -1 && indexEnter != -1 && indexSpace > indexEnter))
                            {
                                mess.Message = mess.Message.Substring(index, indexEnter - index);
                                if (regex.IsMatch(mess.Message))
                                {
                                    WIO.EmitAsync("SendMessage", mess, listMember);
                                    sendLink(mess, listMember);
                                }
                            }
                            else
                            {
                                mess.Message = mess.Message.Substring(index);
                                if (regex.IsMatch(mess.Message))
                                {
                                    WIO.EmitAsync("SendMessage", mess, listMember);
                                    sendLink(mess, listMember);
                                }
                            }
                        }
                    }
                    DAOConversation.MarkUnreaderMessage(ConversationID, SenderID, listMember);

                    APIMessages.data = new DataMessage();
                    APIMessages.data.result = true;
                    APIMessages.data.message = "Gửi tin nhắn thành công" + MessageID;
                }
                else
                {
                    APIMessages.error = new Error();
                    APIMessages.error.code = 200;
                    APIMessages.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessages.error = new Error();
                APIMessages.error.code = 200;
                APIMessages.data.message = ex.ToString();
            }
            return APIMessages;
        }

        [HttpPost("SetClicked")]
        [AllowAnonymous]
        public APIOTP SetClicked()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int conversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                string messageId = httpRequest.Form["MessageId"];
                if (userId != 0 && conversationId != 0 && !string.IsNullOrEmpty(messageId))
                {
                    if (DAOMessages.SetClicked(conversationId, userId, messageId) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Click thành công";
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Click thất bại";
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

        [HttpPost("SetFavoriteMessage")]
        [AllowAnonymous]
        public APIOTP SetFavoriteMessage()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int conversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                string messageId = httpRequest.Form["MessageId"];
                if (userId != 0 && conversationId != 0 && !string.IsNullOrEmpty(messageId))
                {
                    if (DAOMessages.CheckFavoriteMessage(conversationId, userId, messageId) > 0)
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Tin nhắn này đã được đánh dấu";
                    }
                    else
                    {
                        if (DAOMessages.SetFavoriteMessage(conversationId, userId, messageId) > 0)
                        {
                            APIotp.data = new DataOTP();
                            APIotp.data.result = true;
                            APIotp.data.message = "Tin nhắn đã được đánh dấu";
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = "Đánh đấu tin nhắn thất bại";
                        }
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

        [HttpPost("RemoveFavoriteMessage")]
        [AllowAnonymous]
        public APIOTP RemoveFavoriteMessage()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int conversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                string messageId = httpRequest.Form["MessageId"];
                if (userId != 0 && conversationId != 0 && !string.IsNullOrEmpty(messageId))
                {
                    if (DAOMessages.CheckFavoriteMessage(conversationId, userId, messageId) > 0)
                    {
                        if (DAOMessages.RemoveFavoriteMessage(conversationId, userId, messageId) > 0)
                        {
                            APIotp.data = new DataOTP();
                            APIotp.data.result = true;
                            APIotp.data.message = "Bỏ đánh đấu tin nhắn thành công";
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = "Bỏ đánh đấu tin nhắn thất bại";
                        }
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Tin nhắn này không tồn tại";
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
        [HttpPost("GetListFavoriteMessage")]
        [AllowAnonymous]
        public APIMessage GetListFavoriteMessage([FromForm] Conversation conversation)
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                var http = HttpContext.Request;
                int userId = Convert.ToInt32(http.Form["userId"]);
                int countMessage = Convert.ToInt32(http.Form["countMessage"]);
                int listMess = Convert.ToInt32(http.Form["listMess"]);
                if (userId != 0 && countMessage != 0)
                {
                    List<Messages> list = new List<Messages>();
                    if (countMessage - listMess >= (listMess == 0 ? 15 : 30))
                    {
                        list = DAOMessages.GetListFavoriteMessage(userId, listMess, (listMess == 0 ? 15 : 30));
                    }
                    else
                    {
                        list = DAOMessages.GetListFavoriteMessage(userId, listMess, countMessage - listMess);
                    }

                    if (list.Count > 0)
                    {
                        APIMessage.data = new DataMessage();
                        APIMessage.data.result = true;
                        APIMessage.data.message = "Lấy danh sách tin nhắn thành công";
                        APIMessage.data.countMessage = list.Count;
                        APIMessage.data.listMessages = list;
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Cuộc trò chuyện không có tin nhắn nào";
                    }

                }
                else if (conversation.conversationId != 0 && conversation.countMessage == 0)
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Cuộc trò chuyện không có tin nhắn nào";
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiêu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }
        //đã check 5/8
        [HttpPost("GetListMessage")]
        [AllowAnonymous]
        public APIMessage GetListMessage([FromForm] Conversation conversation)
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                var http = HttpContext.Request;
                Int64 loadTo = Convert.ToInt64(http.Form["loadTo"]);
                if (conversation.conversationId != 0 && conversation.countMessage != 0)
                {
                    List<Messages> listMess = new List<Messages>();
                    if (conversation.countMessage - conversation.listMess >= (conversation.listMess == 0 ? 15 : 30))
                    {
                        listMess = DAOMessages.GetMessageByConversatinId(conversation.conversationId, conversation.listMess, (conversation.listMess == 0 ? 15 : 30), conversation.messageDisplay, conversation.adminId, loadTo);
                    }
                    else
                    {
                        listMess = DAOMessages.GetMessageByConversatinId(conversation.conversationId, conversation.listMess, conversation.countMessage - conversation.listMess, conversation.messageDisplay, conversation.adminId, loadTo);
                    }

                    if (listMess.Count > 0)
                    {
                        APIMessage.data = new DataMessage();
                        APIMessage.data.result = true;
                        APIMessage.data.message = "Lấy danh sách tin nhắn thành công";
                        APIMessage.data.listMessages = listMess;
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Cuộc trò chuyện không có tin nhắn nào";
                    }

                }
                else if (conversation.conversationId != 0 && conversation.countMessage == 0)
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Cuộc trò chuyện không có tin nhắn nào";
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiêu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }
        //
        [HttpPost("GetListMessage_LiveChat")]
        [AllowAnonymous]
        public APIMessage_LiveChat GetListMessage_LiveChat()
        {
            APIMessage_LiveChat api = new APIMessage_LiveChat();
            try
            {
                var http = HttpContext.Request;
                string clientId = http.Form["clientId"];
                string fromWeb = http.Form["fromWeb"];
                string contact = http.Form["contactId"];
                int countMess = Convert.ToInt32(http.Form["countMess"]);
                int countLoad = Convert.ToInt32(http.Form["countLoad"]);
                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(fromWeb))
                {
                    int contactId = !string.IsNullOrEmpty(contact) ? Convert.ToInt32(contact) : 56387;
                    var ck = DAOConversation.checkClientConversationLiveChat(clientId, contactId, fromWeb);
                    if (ck.Count > 0)
                    {
                        List<Messages_v2> listMess = new List<Messages_v2>();
                        if (countLoad > 20)
                        {
                            listMess = DAOMessages.GetMessageForLiveChat_v2(ck[0], countMess * 20, 20, clientId);
                        }
                        else
                        {
                            listMess = DAOMessages.GetMessageForLiveChat_v2(ck[0], countMess * countLoad, countLoad, clientId);
                        }

                        api.data = new DataMessage_LiveChat();
                        api.data.result = true;
                        api.data.message = "Lấy danh sách tin nhắn thành công";
                        api.data.listMember = ck[0].memberList.Select(x => x.memberId).ToList();
                        int index = ck[0].memberList.FindIndex(x => x.liveChat != null && x.liveChat.clientId == clientId && x.liveChat.fromWeb == fromWeb);
                        if (index > -1)
                        {
                            api.data.unReader = ck[0].memberList[index].unReader;
                        }
                        api.data.conversationId = ck[0].id;
                        api.data.listMessages = listMess;

                        if (ck[0].memberList != null && ck[0].memberList.Count > 0)
                        {
                            var checkSeener = ck[0].memberList.Where(x => x.liveChat == null).ToList();
                            var seener = checkSeener.Count > 0 ? checkSeener[0] : null;
                            //for (int i = 1; i < ck[0].memberList.Count; i++)
                            //{
                            //    if (seener.timeLastSeener < ck[0].memberList[i].timeLastSeener) seener = ck[0].memberList[i];
                            //}
                            if (seener != null)
                            {
                                api.data.timeLastSeener = seener.timeLastSeener.ToLocalTime();
                                if (ck[0].messageList != null && ck[0].messageList.Count > 0)
                                {
                                    var messages = ck[0].messageList;
                                    messages.Reverse();
                                    var isSeen = messages.Where(m => m.createAt.ToLocalTime() <= api.data.timeLastSeener).ToList();
                                    if (isSeen.Count > 0)
                                    {
                                        api.data.messageId = isSeen[0].id;
                                    }
                                }
                                if (seener.liveChat != null && !string.IsNullOrEmpty(seener.liveChat.clientId))
                                {
                                    api.data.nameLastSeener = !string.IsNullOrEmpty(seener.liveChat.clientName) ? seener.liveChat.clientName : seener.liveChat.clientId;
                                    string letter = RemoveUnicode(api.data.nameLastSeener.Substring(0, 1).ToLower()).ToUpper();
                                    try
                                    {
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                        }
                                        else
                                        {
                                            api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                else
                                {
                                    var userSeener = DAOUsers.GetInforUserById(seener.memberId);
                                    if (userSeener.Count > 0)
                                    {
                                        api.data.nameLastSeener = userSeener[0].userName;
                                        if (String.IsNullOrWhiteSpace(userSeener[0].avatarUser.Trim()))
                                        {
                                            string letter = RemoveUnicode(userSeener[0].userName.Substring(0, 1).ToLower()).ToUpper();
                                            try
                                            {
                                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                {
                                                    api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                                }
                                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                {
                                                    api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                                }
                                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                                {
                                                    api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                                }
                                                else
                                                {
                                                    api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                api.data.avatarLastSeener = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                            }
                                        }
                                        else
                                        {
                                            api.data.avatarLastSeener = "https://mess.timviec365.vn/avatarUser/" + userSeener[0].id + "/" + userSeener[0].avatarUser;
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "Người dùng chưa có cuộc trò chuyện";
                    }

                }
                else if (countLoad == 0)
                {
                    api.error = new Error();
                    api.error.code = 200;
                    api.error.message = "Cuộc trò chuyện không có tin nhắn nào";
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

        //
        //đã check 5/8
        [HttpPost("GetListMessage_v2")]
        [AllowAnonymous]
        public APIMessage_v2 GetListMessage_v2()
        {
            APIMessage_v2 APIMessage = new APIMessage_v2();
            try
            {
                var http = HttpContext.Request;
                int userId = Convert.ToInt32(http.Form["userId"]);
                string conversationList = http.Form["conversationId"];
                string displayMessageList = http.Form["displayMessage"];
                if (!string.IsNullOrEmpty(conversationList) && !string.IsNullOrEmpty(displayMessageList) && userId != 0)
                {
                    int[] conversations = JsonConvert.DeserializeObject<int[]>(conversationList);
                    int[] displayMessages = JsonConvert.DeserializeObject<int[]>(displayMessageList);
                    if (conversations.Length > 0)
                    {
                        APIMessage.data = new DataMessage_v2();
                        APIMessage.data.result = true;
                        APIMessage.data.message = "Lấy thông tin thành công";
                        APIMessage.data.listConversation = new List<ItemMessage_v2>();
                        for (int i = 0; i < conversations.Length; i++)
                        {
                            ItemMessage_v2 item = new ItemMessage_v2();
                            item.conversationID = conversations[i];
                            item.listMessages = DAOMessages.GetMessageByConversatinId(conversations[i], 0, 20, displayMessages[i], userId, 0);
                            APIMessage.data.listConversation.Add(item);
                        }
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Thiêu thông tin truyền lên";
                    }
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiêu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }
        //lấy thư viện anh file
        [HttpPost("GetListLibra")]
        [AllowAnonymous]
        public APIMessage GetListLibra([FromForm] Conversation conversation)
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                var http = HttpContext.Request;
                int type = Convert.ToInt32(http.Form["Type"]);
                if (conversation.conversationId != 0 && conversation.countMessage != 0)
                {
                    List<Messages> listMess = DAOMessages.GetInfoFileByConversationId(conversation.conversationId, conversation.listMess, conversation.countMessage, conversation.messageDisplay, type);

                    if (listMess.Count > 0)
                    {
                        APIMessage.data = new DataMessage();
                        APIMessage.data.result = true;
                        APIMessage.data.message = "Lấy danh sách thư viện thành công";
                        APIMessage.data.listMessages = listMess;
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Cuộc trò chuyện không có ảnh, file, link nào";
                    }

                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiêu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }
        private void sendLink(Messages messageLink, int[] listMember, string[] listDevice = null)
        {
            if (listMember.Length <= 0 && (listDevice == null || listDevice.Length <= 0))
            {
                List<ParticipantsDB> dataMember = DAOConversation.GetMemberIdOfConversation(messageLink.ConversationID);
                for (int i = 0; i < dataMember.Count; i++)
                {
                    listMember[i] = Convert.ToInt32(dataMember[i].memberId);
                }
            }
            messageLink.InfoLink = getInfoLink(messageLink.Message);
            int count = DAOMessages.InsertMessage(messageLink.MessageID, messageLink.ConversationID, messageLink.SenderID, "link", messageLink.Message, "", "", messageLink.CreateAt, new InfoLinkDB(messageLink.InfoLink.Title, messageLink.InfoLink.Description, messageLink.InfoLink.LinkHome, messageLink.InfoLink.Image, 0), new List<FileSendDB>(), null, messageLink.DeleteTime, messageLink.DeleteType, messageLink.DeleteDate, messageLink.IsEdited);
            if (messageLink.LiveChat != null && !string.IsNullOrEmpty(messageLink.LiveChat.ClientId))
            {
                DAOMessages.EditInfoLiveChat(messageLink.MessageID, new LiveChatDB(messageLink.LiveChat.ClientId, messageLink.LiveChat.ClientName, messageLink.LiveChat.FromWeb));
            }
            if (messageLink.InfoSupport != null && !string.IsNullOrEmpty(messageLink.InfoSupport.SupportId))
            {
                DAOMessages.EditInfoSupport(messageLink.MessageID, new InfoSupportDB(messageLink.InfoSupport.Title, messageLink.InfoSupport.Message, messageLink.InfoSupport.SupportId, messageLink.InfoSupport.HaveConversation, messageLink.InfoSupport.UserId, messageLink.InfoSupport.Status, messageLink.InfoSupport.Time));
            }
            if (count > 0)
            {
                DAOConversation.MarkUnreaderMessage(messageLink.ConversationID, messageLink.SenderID, listMember);
                if (listDevice != null && listDevice.Length > 0)
                {
                    string fromWeb = messageLink.LiveChat != null && !string.IsNullOrEmpty(messageLink.LiveChat.ClientId) ? messageLink.LiveChat.FromWeb : "";
                    WIO.EmitAsync("SendMessage", messageLink, listMember, listDevice, "SuppportOtherWeb", fromWeb);
                }
                else WIO.EmitAsync("SendMessage", messageLink, listMember);
            }
        }
        public static void CompressPhoto(Stream sourcePath, string targetPath, String filename)
        {
            try
            {
                using (var image = Image.FromStream(sourcePath))
                {
                    float maxHeight = 250;
                    float maxWidth = 250;
                    int newWidth;
                    int newHeight;
                    string extension;
                    Bitmap originalBMP = new Bitmap(sourcePath);
                    int originalWidth = originalBMP.Width;
                    int originalHeight = originalBMP.Height;

                    if (originalWidth > maxWidth || originalHeight > maxHeight)
                    {

                        // To preserve the aspect ratio  
                        float ratioX = (float)maxWidth / (float)originalWidth;
                        float ratioY = (float)maxHeight / (float)originalHeight;
                        float ratio = Math.Min(ratioX, ratioY);
                        newWidth = (int)(originalWidth * ratio);
                        newHeight = (int)(originalHeight * ratio);
                    }
                    else
                    {
                        newWidth = (int)originalWidth;
                        newHeight = (int)originalHeight;

                    }
                    Bitmap bitMAP1 = new Bitmap(originalBMP, newWidth, newHeight);
                    Graphics imgGraph = Graphics.FromImage(bitMAP1);
                    extension = Path.GetExtension(targetPath);
                    if (extension == ".png" || extension == ".gif")
                    {
                        imgGraph.SmoothingMode = SmoothingMode.AntiAlias;
                        imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);


                        bitMAP1.Save(targetPath, image.RawFormat);

                        bitMAP1.Dispose();
                        imgGraph.Dispose();
                        originalBMP.Dispose();
                    }
                    else
                    {

                        imgGraph.SmoothingMode = SmoothingMode.AntiAlias;
                        imgGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        imgGraph.DrawImage(originalBMP, 0, 0, newWidth, newHeight);
                        ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        EncoderParameters myEncoderParameters = new EncoderParameters(1);
                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        bitMAP1.Save(targetPath, jpgEncoder, myEncoderParameters);

                        bitMAP1.Dispose();
                        imgGraph.Dispose();
                        originalBMP.Dispose();

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static ImageCodecInfo GetEncoder(object jpeg)
        {
            throw new NotImplementedException();
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
        [HttpPost("MessageFromTimViec")]
        [AllowAnonymous]
        public APIOTP MessageFromTimViec()
        {
            APIOTP notificationAPI = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                int IDUser = Convert.ToInt32(httpRequest.Form["IDUser"]);
                int TypeUser = Convert.ToInt32(httpRequest.Form["TypeUser"]);
                int IDContact = Convert.ToInt32(httpRequest.Form["IDContact"]);
                int TypeContact = Convert.ToInt32(httpRequest.Form["TypeContact"]);
                string Message = httpRequest.Form["Message"];
                string MessageType = httpRequest.Form["MessageType"];
                if (IDUser != 0 && TypeUser != 0 && IDContact != 0 && TypeContact != 0 && Message != null)
                {
                    List<UserDB> dataUser = DAOUsers.GetUsersByIDTimViecAndType365(IDUser, TypeUser);
                    List<UserDB> dataContact = DAOUsers.GetUsersByIDTimViecAndType365(IDContact, TypeContact);
                    if (dataUser.Count == 0)
                    {
                        dataUser = DAOUsers.GetUsersByIDTimViecAndType365(IDUser, 0);

                    }
                    if (dataContact.Count == 0)
                    {
                        dataContact = DAOUsers.GetUsersByIDTimViecAndType365(IDContact, 0);
                    }
                    if (dataUser.Count > 0 && dataContact.Count > 0)
                    {
                        User user = getInforUser(dataUser);
                        User contact = getInforUser(dataContact);
                        if (user.CompanyId != contact.CompanyId)
                        {
                            if (DAOUsers.AddNewContact(user.ID, contact.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", user.ID, contact.ID);
                        }

                        int conversationId = 0;
                        int[] users = new int[2];
                        users[0] = user.ID;
                        users[1] = contact.ID;

                        int[] isOnline = new int[2];
                        isOnline[0] = user.isOnline;
                        isOnline[1] = contact.isOnline;

                        if (DAOConversation.checkEmptyConversation(user.ID, contact.ID).Count == 0)
                        {
                            conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                            if (conversationId > 0)
                            {
                                DAOConversation.insertNewParticipant(conversationId, "", 0, users, contact.ID, "Normal");
                            }
                        }
                        else
                        {
                            conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(user.ID, contact.ID)[0].id);
                        }
                        string messageId = DateTime.Now.Ticks + "_" + contact.ID;
                        Messages message = new Messages();
                        if (MessageType.Equals("text"))
                        {
                            message = new Messages(messageId, conversationId, contact.ID, MessageType, Message, 0, DateTime.Now, DateTime.MinValue, 0);
                            ConversationsDB conver = DAOConversation.GetConversation(conversationId, contact.ID);
                            if (!((message.LiveChat != null && !string.IsNullOrEmpty(message.LiveChat.ClientId)) || (conver != null && conver.typeGroup == "liveChat")))
                            {
                                sendNotificationToTimViec(message, user.UserName, conversationId, users, isOnline, 0, false);
                            }
                            int count = DAOMessages.InsertMessage(message.MessageID, message.ConversationID, message.SenderID, MessageType, message.Message, "", "", message.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), _environment, 0, 0, DateTime.MinValue, 0);
                            if (count > 0)
                            {
                                DAOConversation.MarkUnreaderMessage(conversationId, contact.ID, users);
                                WIO.EmitAsync("SendMessage", message, users);
                            }
                        }
                        else
                        {
                            WebClient webClient = new WebClient();
                            string fileName = DateTime.Now.Ticks + "-" + contact.ID + Message.Substring(Message.LastIndexOf("."), Message.Length - Message.LastIndexOf("."));
                            byte[] dataArr = webClient.DownloadData(Message);
                            if (dataArr.Length > 0)
                            {
                                int bytesize = dataArr.Length;
                                var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");
                                if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
                                System.IO.File.WriteAllBytes(Path.Combine(filePath, fileName), dataArr);
                                List<FileSendDB> list = new List<FileSendDB>();
                                if (MessageType.Equals("sendPhoto"))
                                {
                                    message = new Messages(messageId, conversationId, contact.ID, MessageType, "", 0, DateTime.Now, DateTime.MinValue, 0);
                                    message.ListFile = new List<InfoFile>();
                                    try
                                    {
                                        CompressPhoto(new MemoryStream(dataArr), Path.Combine(_environment.ContentRootPath, @"wwwroot\uploadsImageSmall") + @"\" + fileName, fileName);
                                    }
                                    catch
                                    {
                                        System.IO.File.WriteAllBytes(Path.Combine(Path.Combine(_environment.ContentRootPath, @"wwwroot\uploadsImageSmall"), fileName), dataArr);

                                    }
                                    message.ListFile.Add(new InfoFile(message.MessageType, fileName, bytesize, 0, 0));
                                    list.Add(new FileSendDB(bytesize, fileName, 0, 0));
                                }
                                else
                                {
                                    message = new Messages(messageId, conversationId, contact.ID, MessageType, "", 0, DateTime.Now, DateTime.MinValue, 0);
                                    message.ListFile = new List<InfoFile>();
                                    list.Add(new FileSendDB(bytesize, fileName, 0, 0));
                                }
                                int count = DAOMessages.InsertMessage(message.MessageID, message.ConversationID, message.SenderID, message.MessageType, message.Message, "", "", message.CreateAt, new InfoLinkDB(), list, _environment, 0, 0, DateTime.MinValue, 0);
                                if (count > 0)
                                {
                                    DAOConversation.MarkUnreaderMessage(conversationId, contact.ID, users);
                                    //DAOMessages.InsertFile(messageId, fileName, bytesize.ToString(), 0, 0);
                                    WIO.EmitAsync("SendMessage", message, users);
                                }
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Lỗi khi lấy file";
                            }
                        }
                        notificationAPI.data = new DataOTP();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin người dùng hoặc người dùng chưa đăng nhập chat365";
                    }
                }
                else
                {
                    notificationAPI.error = new Error();
                    notificationAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                notificationAPI.error = new Error();
                notificationAPI.error.message = ex.ToString();
            }
            return notificationAPI;
        }
        private string getAvatarUser(string avatar, string userName, int userId)
        {
            if (String.IsNullOrWhiteSpace(avatar.Trim()))
            {
                string letter = RemoveUnicode(userName.Substring(0, 1).ToLower()).ToUpper();
                try
                {
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        return "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        return "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        return "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                    }
                    else
                    {
                        return "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                    }
                }
                catch (Exception ex)
                {
                    return "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                }
            }
            else
            {
                return "https://mess.timviec365.vn/avatarUser/" + userId + "/" + avatar;
            }
        }
        //thông báo và nhắn tin qua timviec(5/8/22)
        public class payloaddata
        {
            public payloaddata(int isGroup)
            {
                this.isGroup = isGroup;
            }

            public int isGroup { get; set; }
        }
        private async void sendNotificationToTimViec(Messages message, string conversationName, int conversationId, int[] listmember, int[] isOnline, int isGroup, bool flag)
        {
            try
            {

                string receiverid = "";
                int indexContact = -1;
                for (int i = 0; i < listmember.Length; i++)
                {
                    if (listmember[i] != message.SenderID)
                    {
                        if (i < isOnline.Length && isOnline[i] == 0)
                        {
                            receiverid += listmember[i] + ",";
                        }
                        indexContact = i;
                    }
                }
                List<UserDB> DataUser = DAOUsers.GetInforUserById(message.SenderID);
                string mess = "";
                if (message.MessageType.Equals("sendFile"))
                {
                    mess = "Tệp";
                }
                else if (message.MessageType.Equals("sendProfile"))
                {
                    mess = "Thẻ Liên Hệ";
                }
                else if (message.MessageType.Equals("sendPhoto"))
                {
                    mess = "Ảnh";
                }
                else
                {
                    mess = message.Message;
                }

                MultipartFormDataContent content = new MultipartFormDataContent();
                if (!String.IsNullOrEmpty(receiverid))
                {
                    receiverid = receiverid.Remove(receiverid.Length - 1, 1);
                    content.Add(new StringContent(conversationName), "title");
                    content.Add(new StringContent(mess), "message");
                    content.Add(new StringContent("chat365"), "from");
                    content.Add(new StringContent(message.SenderID + ""), "sender_id");
                    content.Add(new StringContent(DataUser[0].email), "sender_email");
                    content.Add(new StringContent(DataUser[0].type365.ToString()), "sender_type");
                    content.Add(new StringContent(getAvatarUser(DataUser[0].avatarUser, DataUser[0].userName, message.SenderID)), "sender_avatar");
                    content.Add(new StringContent(DataUser[0].userName), "sender_name");
                    content.Add(new StringContent(message.ConversationID.ToString()), "converstation_id");
                    content.Add(new StringContent(receiverid), "receiver_id");
                    content.Add(new StringContent("Chat"), "not_type");
                }
                WIO.EmitAsync("SendNotificationToHHP", message.Message, DataUser[0].userName, conversationId, conversationName, message.SenderID, listmember.Where(x => x != message.SenderID), receiverid);
                if (isGroup == 0 && indexContact != -1)
                {
                    List<UserDB> DataContact = DAOUsers.GetInforUserById(Convert.ToInt32(listmember[indexContact]));
                    if (flag)
                    {
                        content.Add(new StringContent(DataContact[0].email), "receiver_email");
                        content.Add(new StringContent(DataContact[0].type365 == 0 ? "2" : DataContact[0].type365.ToString()), "type_user");
                    }
                    if (DataContact[0].isOnline == 0)
                    {
                        MultipartFormDataContent content2 = new MultipartFormDataContent();
                        content2.Add(new StringContent(DataUser[0].userName), "name");
                        content2.Add(new StringContent(DataContact[0].email), "email");
                        content2.Add(new StringContent(DataContact[0].type365.ToString()), "type");
                        content2.Add(new StringContent(mess), "mess");
                        content2.Add(new StringContent(message.SenderID + ""), "senderId");
                        HttpClient httpClient2 = new HttpClient();
                        HttpResponseMessage response2 = await httpClient2.PostAsync("https://timviec365.vn/api_app/notification_chat365.php", content2);
                        LogError(response2.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        //WIO.EmitAsync("SendNotificationTimViec", message.Message, DataUser[0].userName, message.SenderID, listmember[indexContact]);
                    }
                    if (flag && DataUser[0].type365 != DataContact[0].type365 && (DataUser[0].type365 == 1 || DataContact[0].type365 == 1))
                    {
                        if (message.MessageType.Equals("text") || message.MessageType.Equals("link"))
                        {
                            int userId = DataUser[0].idTimViec;
                            string room = DataUser[0].type365 == 1 ? userId + "_" + DataContact[0].idTimViec : DataContact[0].idTimViec + "_" + userId;
                            APIMessageTimViec messageTimViec = new APIMessageTimViec(DataUser[0].userName, mess, message.MessageType, "0", userId, DataUser[0].type365 != 1 ? 0 : Convert.ToInt32(DataUser[0].type365), room, DataUser[0].avatarUser.ToString(), Convert.ToInt32(DataUser[0].id));

                            MultipartFormDataContent content2 = new MultipartFormDataContent();
                            content2.Add(new StringContent(messageTimViec.room), "room");
                            content2.Add(new StringContent(messageTimViec.uid + ""), "uid");
                            content2.Add(new StringContent(messageTimViec.uid_type + ""), "type");
                            content2.Add(new StringContent(messageTimViec.username), "username");
                            content2.Add(new StringContent(messageTimViec.ava), "ava");
                            content2.Add(new StringContent(mess), "mess");
                            content2.Add(new StringContent("mess"), "mess_type");
                            HttpClient httpClient2 = new HttpClient();
                            HttpResponseMessage response2 = await httpClient2.PostAsync("https://timviec365.vn/ajax/save_chat.php", content2);
                        }
                        else if (message.MessageType.Equals("sendFile"))
                        {
                            int userId = Convert.ToInt32(DataUser[0].idTimViec);
                            string room = Convert.ToInt32(DataUser[0].type365) == 1 ? userId + "_" + DataContact[0].idTimViec : DataContact[0].idTimViec + "_" + userId;
                            APIMessageTimViec messageTimViec = new APIMessageTimViec(DataUser[0].userName.ToString(), "https://mess.timviec365.vn/uploads/" + message.ListFile[0].FullName, "files", "0", userId, Convert.ToInt32(DataUser[0].type365) != 1 ? 0 : Convert.ToInt32(DataUser[0].type365), room, DataUser[0].avatarUser.ToString(), Convert.ToInt32(DataUser[0].id));

                            MultipartFormDataContent content2 = new MultipartFormDataContent();
                            content2.Add(new StringContent(messageTimViec.room), "room");
                            content2.Add(new StringContent(messageTimViec.uid + ""), "uid");
                            content2.Add(new StringContent(messageTimViec.uid_type + ""), "type");
                            content2.Add(new StringContent(messageTimViec.username), "username");
                            content2.Add(new StringContent(messageTimViec.ava), "ava");
                            content2.Add(new StringContent("https://mess.timviec365.vn/uploads/" + message.ListFile[0].FullName), "mess");
                            content2.Add(new StringContent("files"), "mess_type");
                            HttpClient httpClient2 = new HttpClient();
                            HttpResponseMessage response2 = await httpClient2.PostAsync("https://timviec365.vn/ajax/save_chat.php", content2);

                        }
                        else if (message.MessageType.Equals("sendPhoto"))
                        {
                            foreach (InfoFile file in message.ListFile)
                            {
                                int userId = DataUser[0].idTimViec;
                                string room = DataUser[0].type365 == 1 ? userId + "_" + DataContact[0].idTimViec : DataContact[0].idTimViec + "_" + userId;
                                APIMessageTimViec messageTimViec = new APIMessageTimViec(DataUser[0].userName, "https://mess.timviec365.vn/uploads/" + file.FullName, "images", "0", userId, Convert.ToInt32(DataUser[0].type365) != 1 ? 0 : Convert.ToInt32(DataUser[0].type365), room, DataUser[0].avatarUser.ToString(), Convert.ToInt32(DataUser[0].id));

                                MultipartFormDataContent content2 = new MultipartFormDataContent();
                                content2.Add(new StringContent(messageTimViec.room), "room");
                                content2.Add(new StringContent(messageTimViec.uid + ""), "uid");
                                content2.Add(new StringContent(messageTimViec.uid_type + ""), "type");
                                content2.Add(new StringContent(messageTimViec.username), "username");
                                content2.Add(new StringContent(messageTimViec.ava), "ava");
                                content2.Add(new StringContent("https://mess.timviec365.vn/uploads/" + file.FullName), "mess");
                                content2.Add(new StringContent("images"), "mess_type");
                                HttpClient httpClient2 = new HttpClient();
                                HttpResponseMessage response2 = await httpClient2.PostAsync("https://timviec365.vn/ajax/save_chat.php", content2);
                            }
                        }
                    }
                }
                if (!String.IsNullOrEmpty(receiverid))
                {
                    content.Add(new StringContent(JsonConvert.SerializeObject(new payloaddata(isGroup))), "payload_data");
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.PostAsync("https://timviec365.vn/notification/push_notification_from_chat365_v2.php", content);
                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Content.ReadAsStringAsync().Result);
                    if (receiveInfo.data != null)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex + "");
            }
        }
        private async void sendNotificationToTimViec_v2(Messages message, string conversationName, int conversationId, int[] listmember, int[] isOnline, int isGroup, bool flag)
        {
            Task t = new Task(() =>
            {
                try
                {

                    string receiverid = "";
                    int indexContact = -1;
                    for (int i = 0; i < listmember.Length; i++)
                    {
                        if (listmember[i] != message.SenderID)
                        {
                            if (i < isOnline.Length && isOnline[i] == 0)
                            {
                                receiverid += listmember[i] + ",";
                            }
                            indexContact = i;
                        }
                    }
                    List<UserDB> DataUser = DAOUsers.GetInforUserById(message.SenderID);
                    string mess = "";
                    if (message.MessageType.Equals("sendFile"))
                    {
                        mess = "Tệp";
                    }
                    else if (message.MessageType.Equals("sendProfile"))
                    {
                        mess = "Thẻ Liên Hệ";
                    }
                    else if (message.MessageType.Equals("sendPhoto"))
                    {
                        mess = "Ảnh";
                    }
                    else
                    {
                        mess = message.Message;
                    }

                    MultipartFormDataContent content = new MultipartFormDataContent();
                    if (!String.IsNullOrEmpty(receiverid))
                    {
                        receiverid = receiverid.Remove(receiverid.Length - 1, 1);
                        content.Add(new StringContent(conversationName), "title");
                        content.Add(new StringContent(mess), "message");
                        content.Add(new StringContent("chat365"), "from");
                        content.Add(new StringContent(message.SenderID + ""), "sender_id");
                        content.Add(new StringContent(DataUser[0].email), "sender_email");
                        content.Add(new StringContent(DataUser[0].type365.ToString()), "sender_type");
                        content.Add(new StringContent(getAvatarUser(DataUser[0].avatarUser, DataUser[0].userName, message.SenderID)), "sender_avatar");
                        content.Add(new StringContent(DataUser[0].userName), "sender_name");
                        content.Add(new StringContent(message.ConversationID.ToString()), "converstation_id");
                        content.Add(new StringContent(receiverid), "receiver_id");
                        content.Add(new StringContent("Chat"), "not_type");
                    }
                    WIO.EmitAsync("SendNotificationToHHP", message.Message, DataUser[0].userName, conversationId, conversationName, message.SenderID, listmember.Where(x => x != message.SenderID));
                    if (isGroup == 0 && indexContact != -1)
                    {
                        List<UserDB> DataContact = DAOUsers.GetInforUserById(Convert.ToInt32(listmember[indexContact]));
                        if (flag)
                        {
                            content.Add(new StringContent(DataContact[0].email), "receiver_email");
                            content.Add(new StringContent(DataContact[0].type365 == 0 ? "2" : DataContact[0].type365.ToString()), "type_user");
                        }
                        if (DataContact[0].isOnline == 0)
                        {
                            MultipartFormDataContent content2 = new MultipartFormDataContent();
                            content2.Add(new StringContent(DataUser[0].userName), "name");
                            content2.Add(new StringContent(DataContact[0].email), "email");
                            content2.Add(new StringContent(DataContact[0].type365.ToString()), "type");
                            content2.Add(new StringContent(mess), "mess");
                            content2.Add(new StringContent(message.SenderID + ""), "senderId");
                            HttpClient httpClient2 = new HttpClient();
                            Task<HttpResponseMessage> response2 = httpClient2.PostAsync("https://timviec365.vn/api_app/notification_chat365.php", content2);
                            //LogError(response2.Content.ReadAsStringAsync().Result);
                        }
                        else
                        {
                            //WIO.EmitAsync("SendNotificationTimViec", message.Message, DataUser[0].userName, message.SenderID, listmember[indexContact]);
                        }
                        if (flag && DataUser[0].type365 != DataContact[0].type365 && (DataUser[0].type365 == 1 || DataContact[0].type365 == 1))
                        {
                            if (message.MessageType.Equals("text") || message.MessageType.Equals("link"))
                            {
                                int userId = DataUser[0].idTimViec;
                                string room = DataUser[0].type365 == 1 ? userId + "_" + DataContact[0].idTimViec : DataContact[0].idTimViec + "_" + userId;
                                APIMessageTimViec messageTimViec = new APIMessageTimViec(DataUser[0].userName, mess, message.MessageType, "0", userId, DataUser[0].type365 != 1 ? 0 : Convert.ToInt32(DataUser[0].type365), room, DataUser[0].avatarUser.ToString(), Convert.ToInt32(DataUser[0].id));

                                MultipartFormDataContent content2 = new MultipartFormDataContent();
                                content2.Add(new StringContent(messageTimViec.room), "room");
                                content2.Add(new StringContent(messageTimViec.uid + ""), "uid");
                                content2.Add(new StringContent(messageTimViec.uid_type + ""), "type");
                                content2.Add(new StringContent(messageTimViec.username), "username");
                                content2.Add(new StringContent(messageTimViec.ava), "ava");
                                content2.Add(new StringContent(mess), "mess");
                                content2.Add(new StringContent("mess"), "mess_type");
                                HttpClient httpClient2 = new HttpClient();
                                Task<HttpResponseMessage> response2 = httpClient2.PostAsync("https://timviec365.vn/ajax/save_chat.php", content2);
                            }
                            else if (message.MessageType.Equals("sendFile"))
                            {
                                int userId = Convert.ToInt32(DataUser[0].idTimViec);
                                string room = Convert.ToInt32(DataUser[0].type365) == 1 ? userId + "_" + DataContact[0].idTimViec : DataContact[0].idTimViec + "_" + userId;
                                APIMessageTimViec messageTimViec = new APIMessageTimViec(DataUser[0].userName.ToString(), "https://mess.timviec365.vn/uploads/" + message.ListFile[0].FullName, "files", "0", userId, Convert.ToInt32(DataUser[0].type365) != 1 ? 0 : Convert.ToInt32(DataUser[0].type365), room, DataUser[0].avatarUser.ToString(), Convert.ToInt32(DataUser[0].id));

                                MultipartFormDataContent content2 = new MultipartFormDataContent();
                                content2.Add(new StringContent(messageTimViec.room), "room");
                                content2.Add(new StringContent(messageTimViec.uid + ""), "uid");
                                content2.Add(new StringContent(messageTimViec.uid_type + ""), "type");
                                content2.Add(new StringContent(messageTimViec.username), "username");
                                content2.Add(new StringContent(messageTimViec.ava), "ava");
                                content2.Add(new StringContent("https://mess.timviec365.vn/uploads/" + message.ListFile[0].FullName), "mess");
                                content2.Add(new StringContent("files"), "mess_type");
                                HttpClient httpClient2 = new HttpClient();
                                Task<HttpResponseMessage> response2 = httpClient2.PostAsync("https://timviec365.vn/ajax/save_chat.php", content2);

                            }
                            else if (message.MessageType.Equals("sendPhoto"))
                            {
                                foreach (InfoFile file in message.ListFile)
                                {
                                    int userId = DataUser[0].idTimViec;
                                    string room = DataUser[0].type365 == 1 ? userId + "_" + DataContact[0].idTimViec : DataContact[0].idTimViec + "_" + userId;
                                    APIMessageTimViec messageTimViec = new APIMessageTimViec(DataUser[0].userName, "https://mess.timviec365.vn/uploads/" + file.FullName, "images", "0", userId, Convert.ToInt32(DataUser[0].type365) != 1 ? 0 : Convert.ToInt32(DataUser[0].type365), room, DataUser[0].avatarUser.ToString(), Convert.ToInt32(DataUser[0].id));

                                    MultipartFormDataContent content2 = new MultipartFormDataContent();
                                    content2.Add(new StringContent(messageTimViec.room), "room");
                                    content2.Add(new StringContent(messageTimViec.uid + ""), "uid");
                                    content2.Add(new StringContent(messageTimViec.uid_type + ""), "type");
                                    content2.Add(new StringContent(messageTimViec.username), "username");
                                    content2.Add(new StringContent(messageTimViec.ava), "ava");
                                    content2.Add(new StringContent("https://mess.timviec365.vn/uploads/" + file.FullName), "mess");
                                    content2.Add(new StringContent("images"), "mess_type");
                                    HttpClient httpClient2 = new HttpClient();
                                    Task<HttpResponseMessage> response2 = httpClient2.PostAsync("https://timviec365.vn/ajax/save_chat.php", content2);
                                }
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(receiverid))
                    {
                        content.Add(new StringContent(JsonConvert.SerializeObject(new payloaddata(isGroup))), "payload_data");
                        HttpClient httpClient = new HttpClient();
                        Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/notification/push_notification_from_chat365_v2.php", content);
                        InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                        if (receiveInfo.data != null)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex + "");
                }

            });
            t.Start();
            t.ContinueWith(p =>
            {
                t.Dispose();
            });
        }
        public static async Task LogError(string hihi)
        {
            using StreamWriter file = new("WriteLines2.txt");

            await file.WriteLineAsync(hihi);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        private void SendMailMissMessage(int conversationId, Messages message)
        {
            Task t = new Task(() =>
            {
                UserDB u = DAOMessages.CheckFirstMessageInDay(conversationId, message.CreateAt, message.SenderID);
                if (u != null)
                {
                    List<UserDB> users = DAOUsers.GetInforUserById(message.SenderID);
                    if (users.Count > 0 && u.notificationMissMessage == 1)
                    {
                        UserDB sender = users[0];
                        string mailContent = "";
                        int flag = 0;
                        if (sender.type365 == 1) flag = 1;
                        else if (sender.type365 == 2 && sender.companyId == u.companyId) flag = 2;
                        if (flag > -1)
                        {
                            if (sender.fromWeb == "timviec365" && u.fromWeb == "timviec365")
                            {
                                string[] lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(_environment.ContentRootPath, @"wwwroot\mailForm\mail_timviec.php"));
                                mailContent = string.Join("\n", lines);
                                if (flag == 1)
                                {
                                    string p = "Nhà tuyển dụng";
                                    mailContent = mailContent.Replace("positionEnd", p.ToLower() + " này").Replace("position", p);
                                }
                                else if (flag == 2 || flag == 0)
                                {
                                    string p = "Ứng viên";
                                    mailContent = mailContent.Replace("positionEnd", p.ToLower() + " này").Replace("position", p);
                                }
                            }
                            else if (sender.fromWeb == "raonhanh365" && u.fromWeb == "raonhanh365")
                            {
                                string[] lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(_environment.ContentRootPath, @"wwwroot\mailForm\mail_raonhanh.php"));
                                mailContent = string.Join("\n", lines);
                            }
                            else
                            {
                                string[] lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(_environment.ContentRootPath, @"wwwroot\mailForm\mail_chat.php"));
                                mailContent = string.Join("\n", lines);
                                if (flag == 1)
                                {
                                    string p = "Công ty";
                                    mailContent = mailContent.Replace("positionEnd", p.ToLower() + " này").Replace("position", p);
                                }
                                else if (flag == 2)
                                {
                                    string p = "Nhân viên";
                                    mailContent = mailContent.Replace("positionEnd", p.ToLower() + " này").Replace("position", p);
                                }
                                else if (flag == 0)
                                {
                                    mailContent = mailContent.Replace("positionEnd", "tài khoản này").Replace("position", "Tài khoản");
                                }
                            }
                            string mess = "";
                            if (message.MessageType.Equals("sendFile") || message.MessageType.Equals("sendPhoto"))
                            {
                                if (message.ListFile.Count > 0)
                                {
                                    string FileSizeInByte = "";
                                    if (Convert.ToInt64(message.ListFile[0].SizeFile) / 1024 < 1)
                                    {
                                        FileSizeInByte = Convert.ToInt64(message.ListFile[0].SizeFile) + " bytes";
                                    }
                                    else if (Convert.ToInt64(message.ListFile[0].SizeFile) / 1024 < 1024)
                                    {
                                        FileSizeInByte = ((double)Convert.ToInt64(message.ListFile[0].SizeFile) / 1024).ToString("0.00") + " KB";
                                    }
                                    else
                                    {
                                        FileSizeInByte = ((double)Convert.ToInt64(message.ListFile[0].SizeFile) / (1024 * 1024)).ToString("0.00") + " MB";
                                    }
                                    mess = $"{message.ListFile[0].NameDisplay} ({FileSizeInByte})";
                                }
                            }
                            else if (message.MessageType.Equals("sendProfile"))
                            {
                                mess = "Thẻ liên hệ của: " + message.UserProfile.UserName;
                            }
                            else if (message.MessageType.Equals("link"))
                            {
                                InfoLink link = getInfoLink(message.Message);
                                if (link.Title.Length > 55) link.Title = link.Title.Substring(0, 52) + "...";
                                if (link.Description.Length > 55) link.Description = link.Description.Substring(0, 52) + "...";
                                if (link.LinkHome.Length > 55) link.LinkHome = link.LinkHome.Substring(0, 52) + "...";
                                mess = $"{link.Title}<br>{link.Description}<br>{link.LinkHome}";
                            }
                            else if (!string.IsNullOrEmpty(message.QuoteMessage.MessageID) && !string.IsNullOrEmpty(message.QuoteMessage.Message) && string.IsNullOrEmpty(message.Message))
                            {
                                mess = message.QuoteMessage.Message;
                                if (mess.Length > 162)
                                {
                                    mess = mess.Substring(0, 159) + "...";
                                }
                            }
                            else
                            {
                                mess = message.Message;
                                if (mess.Length > 162)
                                {
                                    mess = mess.Substring(0, 159) + "...";
                                }
                            }

                            string urlChat = "https://chat365.timviec365.vn/";
                            var baseC = Base64Encode(conversationId.ToString());
                            var baseU = Base64Encode(u.id.ToString());
                            if (!string.IsNullOrEmpty(baseC.Trim()) && !string.IsNullOrEmpty(baseU.Trim()))
                            {
                                if (baseC.Contains("=")) baseC = baseC.Replace("=", "");
                                if (baseU.Contains("=")) baseU = baseU.Replace("=", "");
                                urlChat = string.Format(@"https://chat365.timviec365.vn/conversation365-c{0}-u{1}", baseC, baseU);
                            }

                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.mailgun.org";
                            smtp.Port = 587;
                            smtp.EnableSsl = true;
                            smtp.Credentials = new NetworkCredential("postmaster@mailtimviec365.vn", "bcbe2993383b34c696e1d1c5603b1618-100b5c8d-51aa397c");
                            MailMessage msg = new MailMessage("chat365@mailtimviec365.vn", u.email);
                            msg.Subject = "Bạn có một tin nhắn mới tại Chat365";
                            msg.Body = mailContent.Replace("userName", u.userName).Replace("senderName", users[0].userName).Replace("message", mess).Replace("conversationLink", urlChat);
                            msg.IsBodyHtml = true;
                            smtp.Send(msg);
                        }

                    }
                }
            });
            t.ContinueWith((p) =>
            {
                t.Dispose();
            });
            t.Start();
        }

        //check 05/08
        [HttpPost("SendMessage")]
        [AllowAnonymous]
        public APIMessage SendMessage()
        {
            APIMessage APIMessages = new APIMessage();
            try
            {
                var httpRequest = HttpContext.Request;
                string MessageID = httpRequest.Form["MessageID"];
                int ConversationID = Convert.ToInt32(httpRequest.Form["ConversationID"]);
                int SenderID = Convert.ToInt32(httpRequest.Form["SenderID"]);
                string MessageType = httpRequest.Form["MessageType"];
                string Message = httpRequest.Form["Message"];
                string Title = httpRequest.Form["Title"];
                string Quote = httpRequest.Form["Quote"];
                string Profile = httpRequest.Form["Profile"];
                string ListTag = httpRequest.Form["ListTag"];
                string File = httpRequest.Form["File"];
                string ListMember = httpRequest.Form["ListMember"];
                string IsOnline = httpRequest.Form["IsOnline"];
                string conversationName = httpRequest.Form["ConversationName"];
                int isGroup = Convert.ToInt32(httpRequest.Form["IsGroup"]);
                int deleteTime = Convert.ToInt32(httpRequest.Form["DeleteTime"]);
                int deleteType = Convert.ToInt32(httpRequest.Form["DeleteType"]);
                string infoSupport = httpRequest.Form["InfoSupport"];
                string liveChat = httpRequest.Form["LiveChat"];
                string from = httpRequest.Form["from"];
                if (!String.IsNullOrEmpty(MessageType) && (!String.IsNullOrEmpty(File) || !String.IsNullOrEmpty(Message) || !String.IsNullOrEmpty(Quote)))
                {
                    Messages mess = new Messages(MessageID, ConversationID, SenderID, MessageType, Message, ListTag, DateTime.MinValue, deleteTime, deleteType);
                    if (String.IsNullOrEmpty(Quote))
                    {
                        mess.QuoteMessage = new MessageQuote();
                        mess.QuoteMessage.MessageID = "";
                        mess.QuoteMessage.Message = "";
                    }
                    else
                    {
                        mess.QuoteMessage = JsonConvert.DeserializeObject<MessageQuote>(Quote);
                    }
                    //if(mess.SenderID==59721)System.IO.File.WriteAllText("check_livechat_sendFile.txt",$"{mess.SenderID}_{File}");
                    if (!String.IsNullOrEmpty(File))
                    {
                        mess.ListFile = JsonConvert.DeserializeObject<List<InfoFile>>(File);
                        for (int i = 0; i < mess.ListFile.Count; i++)
                        {
                            mess.ListFile[i].FullName = mess.ListFile[i].FullName.RemoveSpecChar();
                            mess.ListFile[i].NameDisplay = mess.ListFile[i].FullName.getDisplayNameFile();
                            if (!string.IsNullOrEmpty(mess.ListFile[i].FileSizeInByte))
                            {
                                long value = mess.ListFile[i].SizeFile;
                                if (Convert.ToInt64(value) / 1024 < 1)
                                {
                                    mess.ListFile[i].FileSizeInByte = Convert.ToInt64(value) + " bytes";
                                }
                                else if (Convert.ToInt64(value) / 1024 < 1024)
                                {
                                    mess.ListFile[i].FileSizeInByte = ((double)Convert.ToInt64(value) / 1024).ToString("0.00") + " KB";
                                }
                                else
                                {
                                    mess.ListFile[i].FileSizeInByte = ((double)Convert.ToInt64(value) / (1024 * 1024)).ToString("0.00") + " MB";
                                }
                            }
                        }
                    }
                    else
                    {
                        mess.ListFile = new List<InfoFile>();
                    }
                    if (!String.IsNullOrEmpty(Profile))
                    {
                        mess.UserProfile = JsonConvert.DeserializeObject<User>(Profile);
                    }
                    else
                    {
                        mess.UserProfile = new User();
                    }
                    if (String.IsNullOrEmpty(Message))
                    {
                        mess.Message = "";
                    }
                    if (!string.IsNullOrEmpty(Title))
                    {
                        mess.Message = $"{Title}\n{mess.Message}";
                    }
                    mess.CreateAt = DateTime.Now;
                    mess.DeleteDate = DateTime.MinValue;
                    if (mess.DeleteType == 0 && mess.DeleteTime > 0)
                    {
                        mess.DeleteDate = mess.CreateAt.AddSeconds(mess.DeleteTime);
                    }
                    if (String.IsNullOrWhiteSpace(MessageID))
                    {
                        mess.MessageID = mess.CreateAt.Ticks + "_" + SenderID;
                    }
                    mess.LiveChat = null;
                    ConversationsDB conver = null;
                    string currentWeb = "";
                    if (!string.IsNullOrEmpty(liveChat))
                    {
                        mess.LiveChat = JsonConvert.DeserializeObject<InfoLiveChat>(liveChat);
                    }
                    else
                    {
                        if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                        if (conver != null && conver.typeGroup == "liveChat")
                        {
                            int index = conver.memberList.FindIndex(x => x.memberId == SenderID);
                            if (index > -1 && conver.memberList[index].liveChat != null && !string.IsNullOrEmpty(conver.memberList[index].liveChat.clientId))
                            {
                                mess.LiveChat = getClientInfo(conver.memberList[index].liveChat);
                                currentWeb = mess.LiveChat.FromWeb;
                            }
                        }
                    }
                    InfoSupportDB infoSupportDB = null;
                    if (!string.IsNullOrEmpty(infoSupport))
                    {
                        mess.InfoSupport = JsonConvert.DeserializeObject<InfoSupport>(infoSupport);
                        if (string.IsNullOrEmpty(mess.InfoSupport.SupportId)) mess.InfoSupport.SupportId = mess.MessageID;
                        infoSupportDB = new InfoSupportDB();
                        infoSupportDB.title = mess.InfoSupport.Title;
                        infoSupportDB.message = mess.InfoSupport.Message;
                        infoSupportDB.supportId = mess.InfoSupport.SupportId;
                        infoSupportDB.userId = mess.InfoSupport.UserId;
                        infoSupportDB.status = mess.InfoSupport.Status;
                        infoSupportDB.haveConversation = mess.InfoSupport.HaveConversation;
                    }
                    List<int> listMember = new List<int>();
                    List<string> listDevices = new List<string>();
                    List<string> listfromWeb = new List<string>();
                    List<int> isOnline = new List<int>();
                    ParticipantsDB userSender = null;
                    if (!String.IsNullOrEmpty(ListMember))
                    {
                        listMember = JsonConvert.DeserializeObject<List<int>>(ListMember);
                        isOnline = JsonConvert.DeserializeObject<List<int>>(IsOnline);
                        if ((listMember.Count == 0 && isOnline.Count == 0) || (mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                        {
                            if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                            if (conver != null)
                                for (int i = 0; i < conver.memberList.Count; i++)
                                {
                                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                    {
                                        if (conver.memberList[i].liveChat != null && !string.IsNullOrEmpty(conver.memberList[i].liveChat.clientId))
                                        {
                                            if (!listDevices.Contains(conver.memberList[i].liveChat.clientId)) listDevices.Add(conver.memberList[i].liveChat.clientId);
                                            if (listMember.Contains(conver.memberList[i].memberId)) listMember.Remove(conver.memberList[i].memberId);
                                            if (conver.memberList[i].memberId == SenderID) currentWeb = conver.memberList[i].liveChat.fromWeb;
                                        }
                                        else
                                        {
                                            if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                        }
                                    }
                                    else
                                    {
                                        if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                    }
                                    if (listMember.Contains(conver.memberList[i].memberId))
                                    {
                                        var uu = DAOUsers.GetUserById(conver.memberList[i].memberId);
                                        if (uu.Count > 0) isOnline.Add(uu[0].isOnline);
                                    }

                                }
                        }
                    }
                    else
                    {
                        if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                        if (conver != null)
                            for (int i = 0; i < conver.memberList.Count; i++)
                            {
                                if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                {
                                    if (conver.memberList[i].liveChat != null && !string.IsNullOrEmpty(conver.memberList[i].liveChat.clientId))
                                    {
                                        if (!listDevices.Contains(conver.memberList[i].liveChat.clientId)) listDevices.Add(conver.memberList[i].liveChat.clientId);
                                        if (listMember.Contains(conver.memberList[i].memberId)) listMember.Remove(conver.memberList[i].memberId);
                                        if (conver.memberList[i].memberId == SenderID) currentWeb = conver.memberList[i].liveChat.fromWeb;
                                    }
                                    else if (mess.InfoSupport != null && !string.IsNullOrEmpty(mess.InfoSupport.SupportId) && mess.SenderID == conver.memberList[i].memberId)
                                    {
                                        if (listMember.Contains(conver.memberList[i].memberId)) listMember.Remove(conver.memberList[i].memberId);
                                    }
                                    else
                                    {
                                        if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                    }
                                }
                                else
                                {
                                    if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                }
                                if (listMember.Contains(conver.memberList[i].memberId))
                                {
                                    var uu = DAOUsers.GetUserById(conver.memberList[i].memberId);
                                    if (uu.Count > 0) isOnline.Add(uu[0].isOnline);
                                }

                            }
                    }
                    if (mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId) && !listDevices.Contains(mess.LiveChat.ClientId))
                    {
                        listDevices.Add(mess.LiveChat.ClientId);
                        listfromWeb.Add(mess.LiveChat.FromWeb);
                        currentWeb = mess.LiveChat.FromWeb;
                    }
                    if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                    {
                        List<UserDB> sender = DAOUsers.GetInforUserById(mess.SenderID);
                        mess.SenderName = sender.Count > 0 ? sender[0].userName : "";
                        if (String.IsNullOrWhiteSpace(sender[0].avatarUser.Trim()))
                        {
                            string letter = RemoveUnicode(sender[0].userName.Substring(0, 1).ToLower()).ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            mess.SenderAvatar = "https://mess.timviec365.vn/avatarUser/" + sender[0].id + "/" + sender[0].avatarUser;
                        }

                    }
                    bool flag = true;
                    if ((mess.MessageType == "sendFile" || mess.MessageType == "sendPhoto") && (mess.ListFile == null || mess.ListFile.Count <= 0)) flag = false;
                    if (flag)
                    {
                        if (!((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || (conver != null && conver.typeGroup == "liveChat")))
                        {
                            SendMailMissMessage(ConversationID, mess);
                            sendNotificationToTimViec(mess, conversationName, mess.ConversationID, listMember.ToArray(), isOnline.ToArray(), isGroup, true);
                        }
                        if (!mess.MessageType.Equals("link"))
                        {
                            if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                            {
                                var date = DateTime.Now;
                                if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || (userSender != null && userSender.liveChat != null && !string.IsNullOrEmpty(userSender.liveChat.clientId)))
                                    mess.CreateAt = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second + 4);
                                WIO.EmitAsync("SendMessage", mess, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                            }
                            else WIO.EmitAsync("SendMessage", mess, listMember);
                            if (mess.MessageType.Equals("sendFile") || mess.MessageType.Equals("sendPhoto"))
                            {
                                List<FileSendDB> fileSend = new List<FileSendDB>();
                                foreach (InfoFile info in mess.ListFile)
                                {
                                    fileSend.Add(new FileSendDB(info.SizeFile, info.FullName, info.Height, info.Width));
                                }
                                DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(), fileSend, _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0, from);
                            }
                            else if (mess.MessageType.Equals("map"))
                            {
                                string[] z = mess.Message.Split(',');
                                string link = String.Format(@"https://www.google.com/maps/search/{0},{1}/@{0},{1},10z?hl=vi", z[0].Trim(), z[1].Trim());
                                mess.InfoLink = getInfoLink(link);
                                DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(mess.InfoLink.Title, mess.InfoLink.Description, mess.InfoLink.LinkHome, mess.InfoLink.Image, mess.InfoLink.IsNotification), new List<FileSendDB>(), _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0, from);
                                if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                {
                                    WIO.EmitAsync("SendMessage", mess, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                }
                                else WIO.EmitAsync("SendMessage", mess, listMember);
                            }
                            else
                            {
                                DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0, from);
                            }
                            if (mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId))
                            {
                                Task t = new Task(() =>
                                {
                                    DAOMessages.EditInfoLiveChat(mess.MessageID, new LiveChatDB(mess.LiveChat.ClientId, mess.LiveChat.ClientName, currentWeb));
                                });
                                t.Start();
                                t.ContinueWith(p =>
                                {
                                    t.Dispose();
                                });
                            }
                            if (!string.IsNullOrEmpty(infoSupport))
                            {
                                Task t = new Task(() =>
                                {
                                    DAOMessages.EditInfoSupport(mess.MessageID, infoSupportDB);
                                });
                                t.Start();
                                t.ContinueWith(p =>
                                {
                                    t.Dispose();
                                });
                            }
                        }

                        Regex regex = new Regex(@"(http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-]))?");
                        if (mess.MessageType.Equals("link") || (mess.MessageType.Equals("text") && regex.IsMatch(mess.Message) && mess.Message.Length <= 500))
                        {
                            if (mess.MessageType.Equals("link"))
                            {
                                if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                {
                                    WIO.EmitAsync("SendMessage", mess, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                }
                                else WIO.EmitAsync("SendMessage", mess, listMember);
                                sendLink(mess, listMember.ToArray(), listDevices.ToArray());
                            }
                            else
                            {
                                mess.MessageID = mess.CreateAt.Ticks + 12 + "_" + mess.SenderID;
                                mess.MessageType = "link";
                                int index = mess.Message.IndexOf("http", 0);
                                int indexSpace = mess.Message.Trim().IndexOf(" ", index);
                                int indexEnter = mess.Message.IndexOf("\n", index);
                                if ((indexSpace != -1 && indexEnter == -1) || (indexSpace != -1 && indexEnter != -1 && indexSpace < indexEnter))
                                {
                                    mess.Message = mess.Message.Substring(index, indexSpace - index);
                                    if (regex.IsMatch(mess.Message))
                                    {
                                        if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                        {
                                            mess.InfoLink = getInfoLink(mess.Message);
                                            WIO.EmitAsync("SendMessage", mess, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                        }
                                        else WIO.EmitAsync("SendMessage", mess, listMember);
                                        sendLink(mess, listMember.ToArray(), listDevices.ToArray());
                                    }
                                }
                                else if ((indexSpace == -1 && indexEnter != -1) || (indexSpace != -1 && indexEnter != -1 && indexSpace > indexEnter))
                                {
                                    mess.Message = mess.Message.Substring(index, indexEnter - index);
                                    if (regex.IsMatch(mess.Message))
                                    {
                                        if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                        {
                                            mess.InfoLink = getInfoLink(mess.Message);
                                            WIO.EmitAsync("SendMessage", mess, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                        }
                                        else WIO.EmitAsync("SendMessage", mess, listMember);
                                        sendLink(mess, listMember.ToArray(), listDevices.ToArray());
                                    }
                                }
                                else
                                {
                                    mess.Message = mess.Message.Substring(index);
                                    if (regex.IsMatch(mess.Message))
                                    {
                                        if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                        {
                                            mess.InfoLink = getInfoLink(mess.Message);
                                            WIO.EmitAsync("SendMessage", mess, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                        }
                                        else WIO.EmitAsync("SendMessage", mess, listMember);
                                        sendLink(mess, listMember.ToArray());
                                    }
                                }
                            }
                        }
                        DAOConversation.MarkUnreaderMessage(ConversationID, SenderID, listMember.ToArray());

                        APIMessages.data = new DataMessage();
                        APIMessages.data.result = true;
                        APIMessages.data.message = "Gửi tin nhắn thành công";
                        APIMessages.data.messageId = mess.MessageID;
                        if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                        if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                        {
                            APIMessages.data.senderName = mess.SenderName;
                        }
                    }
                    else
                    {
                        APIMessages.error = new Error();
                        APIMessages.error.code = 200;
                        APIMessages.error.message = "Có lỗi xảy ra";
                    }
                }
                else
                {
                    APIMessages.error = new Error();
                    APIMessages.error.code = 200;
                    APIMessages.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessages.error = new Error();
                APIMessages.error.code = 200;
                APIMessages.data.message = ex != null ? ex.ToString() : "Có lỗi xảy ra";
            }
            return APIMessages;
        }

        [HttpPost("SendNewMessage")]
        [AllowAnonymous]
        public APIMessage SendNewMessage()
        {
            APIMessage APIMessages = new APIMessage();
            try
            {
                var httpRequest = HttpContext.Request;
                string MessageID = httpRequest.Form["MessageID"];
                int ConversationID = Convert.ToInt32(httpRequest.Form["ConversationID"]);
                int SenderID = Convert.ToInt32(httpRequest.Form["SenderID"]);
                string MessageType = httpRequest.Form["MessageType"];
                string Message = httpRequest.Form["Message"];
                string Quote = httpRequest.Form["Quote"];
                string Profile = httpRequest.Form["Profile"];
                string ListTag = httpRequest.Form["ListTag"];
                string File = httpRequest.Form["File"];
                string ListMember = httpRequest.Form["ListMember"];
                string IsOnline = httpRequest.Form["IsOnline"];
                string conversationName = httpRequest.Form["ConversationName"];
                int isGroup = Convert.ToInt32(httpRequest.Form["IsGroup"]);
                int deleteTime = Convert.ToInt32(httpRequest.Form["DeleteTime"]);
                int deleteType = Convert.ToInt32(httpRequest.Form["DeleteType"]);
                string from = httpRequest.Form["from"];
                if (!String.IsNullOrEmpty(MessageType) && (!String.IsNullOrEmpty(File) || !String.IsNullOrEmpty(Message) || !String.IsNullOrEmpty(Quote)))
                {
                    Messages mess = new Messages(MessageID, ConversationID, SenderID, MessageType, Message, ListTag, DateTime.MinValue, deleteTime, deleteType);
                    mess.CreateAt = DateTime.Now;
                    if (String.IsNullOrEmpty(Quote))
                    {
                        mess.QuoteMessage = new MessageQuote();
                        mess.QuoteMessage.MessageID = "";
                        mess.QuoteMessage.Message = "";
                    }
                    else
                    {
                        mess.QuoteMessage = JsonConvert.DeserializeObject<MessageQuote>(Quote);
                    }
                    if (File != null && !String.IsNullOrEmpty(File))
                    {
                        mess.ListFile = JsonConvert.DeserializeObject<List<InfoFile>>(File);
                        if (mess.ListFile != null && mess.ListFile.Count > 0)
                            for (int i = 0; i < mess.ListFile.Count; i++)
                            {
                                if (mess.ListFile[i] != null && mess.ListFile[i].FullName != null)
                                {
                                    mess.ListFile[i].FullName = mess.ListFile[i].FullName.RemoveSpecChar();
                                    mess.ListFile[i].NameDisplay = mess.ListFile[i].FullName.getDisplayNameFile();
                                }
                            }
                    }
                    else
                    {
                        mess.ListFile = new List<InfoFile>();
                    }
                    if (!String.IsNullOrEmpty(Profile))
                    {
                        mess.UserProfile = JsonConvert.DeserializeObject<User>(Profile);
                    }
                    else
                    {
                        mess.UserProfile = new User();
                    }
                    if (String.IsNullOrEmpty(Message))
                    {
                        mess.Message = "";
                    }
                    if (mess.DeleteType == 0 && mess.DeleteTime > 0)
                    {
                        mess.DeleteDate = mess.CreateAt.AddSeconds(mess.DeleteTime);
                    }
                    if (String.IsNullOrWhiteSpace(MessageID))
                    {
                        mess.MessageID = mess.CreateAt.Ticks + "_" + SenderID;
                    }
                    ConversationsDB conver = DAOConversation.GetConversation(ConversationID, SenderID);
                    List<int> listMember = new List<int>();
                    List<int> isOnline = new List<int>();
                    List<string> listDevices = new List<string>();
                    string currentWeb = "";
                    if (!String.IsNullOrEmpty(ListMember))
                    {
                        listMember = JsonConvert.DeserializeObject<List<int>>(ListMember);
                        isOnline = JsonConvert.DeserializeObject<List<int>>(IsOnline);
                        if ((listMember.Count == 0 && isOnline.Count == 0) || (mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                        {
                            if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                            if (conver != null)
                                for (int i = 0; i < conver.memberList.Count; i++)
                                {
                                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                    {
                                        if (conver.memberList[i].liveChat != null && !string.IsNullOrEmpty(conver.memberList[i].liveChat.clientId))
                                        {
                                            if (!listDevices.Contains(conver.memberList[i].liveChat.clientId)) listDevices.Add(conver.memberList[i].liveChat.clientId);
                                            if (listMember.Contains(conver.memberList[i].memberId)) listMember.Remove(conver.memberList[i].memberId);
                                            if (conver.memberList[i].memberId == SenderID) currentWeb = conver.memberList[i].liveChat.fromWeb;
                                        }
                                        else
                                        {
                                            if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                        }
                                    }
                                    else
                                    {
                                        if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                    }
                                    if (listMember.Contains(conver.memberList[i].memberId))
                                    {
                                        var uu = DAOUsers.GetUserById(conver.memberList[i].memberId);
                                        if (uu.Count > 0) isOnline.Add(uu[0].isOnline);
                                    }

                                }
                        }
                    }
                    else
                    {
                        if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                        if (conver != null)
                            for (int i = 0; i < conver.memberList.Count; i++)
                            {
                                if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                {
                                    if (conver.memberList[i].liveChat != null && !string.IsNullOrEmpty(conver.memberList[i].liveChat.clientId))
                                    {
                                        if (!listDevices.Contains(conver.memberList[i].liveChat.clientId)) listDevices.Add(conver.memberList[i].liveChat.clientId);
                                        if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Remove(conver.memberList[i].memberId);
                                        if (conver.memberList[i].memberId == SenderID) currentWeb = conver.memberList[i].liveChat.fromWeb;
                                    }
                                    else
                                    {
                                        if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                    }
                                }
                                else
                                {
                                    if (!listMember.Contains(conver.memberList[i].memberId)) listMember.Add(conver.memberList[i].memberId);
                                }
                                if (listMember.Contains(conver.memberList[i].memberId))
                                {
                                    var uu = DAOUsers.GetUserById(conver.memberList[i].memberId);
                                    if (uu.Count > 0) isOnline.Add(uu[0].isOnline);
                                }
                            }
                    }
                    if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                    {
                        List<UserDB> sender = DAOUsers.GetInforUserById(mess.SenderID);
                        mess.SenderName = sender.Count > 0 ? sender[0].userName : "";
                        if (String.IsNullOrWhiteSpace(sender[0].avatarUser.Trim()))
                        {
                            string letter = RemoveUnicode(sender[0].userName.Substring(0, 1).ToLower()).ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                mess.SenderAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            mess.SenderAvatar = "https://mess.timviec365.vn/avatarUser/" + sender[0].id + "/" + sender[0].avatarUser;
                        }
                    }
                    if (conver == null) conver = DAOConversation.GetConversation(ConversationID, SenderID);
                    if (!((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || (conver != null && conver.typeGroup == "liveChat")))
                    {
                        SendMailMissMessage(ConversationID, mess);
                        sendNotificationToTimViec(mess, conversationName, mess.ConversationID, listMember.ToArray(), isOnline.ToArray(), isGroup, true);
                    }
                    if (!mess.MessageType.Equals("link"))
                    {
                        if (mess.MessageType.Equals("sendFile") || mess.MessageType.Equals("sendPhoto"))
                        {
                            List<FileSendDB> fileSend = new List<FileSendDB>();
                            var fileNewPath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploadsImageSmall");
                            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");
                            int edit = 1;
                            foreach (InfoFile info in mess.ListFile)
                            {
                                fileSend.Add(new FileSendDB(info.SizeFile, info.FullName, info.Height, info.Width));
                                if (System.IO.File.Exists(Path.Combine(fileNewPath, info.FullName)) || System.IO.File.Exists(Path.Combine(filePath, info.FullName)))
                                {
                                    edit = 0;
                                }
                            }

                            DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(), fileSend, _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, edit, from);
                            if (edit == 0)
                            {
                                if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                {
                                    var messLive = mess;
                                    if (String.IsNullOrEmpty(Quote))
                                    {
                                        messLive.Quote = null;
                                        messLive.QuoteMessage = null;
                                    }
                                    WIO.EmitAsync("SendMessage", messLive, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                }
                                else WIO.EmitAsync("SendMessage", mess, listMember);
                            }
                        }
                        else if (mess.MessageType.Equals("map"))
                        {
                            string[] z = mess.Message.Split(',');
                            string link = String.Format(@"https://www.google.com/maps/search/{0},{1}/@{0},{1},10z?hl=vi", z[0].Trim(), z[1].Trim());
                            mess.InfoLink = getInfoLink(link);
                            DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(mess.InfoLink.Title, mess.InfoLink.Description, mess.InfoLink.LinkHome, mess.InfoLink.Image, mess.InfoLink.IsNotification), new List<FileSendDB>(), _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0, from);
                            if (conver.typeGroup == "liveChat")
                            {
                                var messLive = mess;
                                if (String.IsNullOrEmpty(Quote))
                                {
                                    messLive.Quote = null;
                                    messLive.QuoteMessage = null;
                                }
                                WIO.EmitAsync("SendMessage", messLive, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                            }
                            else WIO.EmitAsync("SendMessage", mess, listMember);
                        }
                        else
                        {
                            DAOMessages.InsertMessage(mess.MessageID, mess.ConversationID, mess.SenderID, mess.MessageType, mess.Message, mess.QuoteMessage.MessageID, mess.QuoteMessage.Message, mess.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), _environment, mess.DeleteTime, mess.DeleteType, mess.DeleteDate, 0, from);
                            if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                            {
                                var messLive = mess;
                                if (String.IsNullOrEmpty(Quote))
                                {
                                    messLive.Quote = null;
                                    messLive.QuoteMessage = null;
                                }
                                WIO.EmitAsync("SendMessage", messLive, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                            }
                            else
                            {
                                WIO.EmitAsync("SendMessage", mess, listMember);
                            }
                        }

                    }
                    Regex regex = new Regex(@"(http|https):\/\/(\w+:{0,1}\w*@)?(\S+)(:[0-9]+)?(\/|\/([\w#!:.?+=&%@!\-]))?");
                    if (mess.MessageType.Equals("link") || (mess.MessageType.Equals("text") && regex.IsMatch(mess.Message) && mess.Message.Length <= 500))
                    {
                        if (mess.MessageType.Equals("link"))
                        {
                            if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                            {
                                var messLive = mess;
                                if (String.IsNullOrEmpty(Quote))
                                {
                                    messLive.Quote = null;
                                    messLive.QuoteMessage = null;
                                }
                                WIO.EmitAsync("SendMessage", messLive, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                            }
                            else WIO.EmitAsync("SendMessage", mess, listMember);
                            sendLink(mess, listMember.ToArray(), listDevices.ToArray());
                        }
                        else
                        {
                            mess.MessageID = mess.CreateAt.Ticks + 12 + "_" + mess.SenderID;
                            mess.MessageType = "link";
                            int index = mess.Message.IndexOf("http", 0);
                            int indexSpace = mess.Message.Trim().IndexOf(" ", index);
                            int indexEnter = mess.Message.IndexOf("\n", index);
                            if ((indexSpace != -1 && indexEnter == -1) || (indexSpace != -1 && indexEnter != -1 && indexSpace < indexEnter))
                            {
                                mess.Message = mess.Message.Substring(index, indexSpace - index);
                                if (regex.IsMatch(mess.Message))
                                {
                                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                    {
                                        var messLive = mess;
                                        if (String.IsNullOrEmpty(Quote))
                                        {
                                            messLive.Quote = null;
                                            messLive.QuoteMessage = null;
                                        }
                                        WIO.EmitAsync("SendMessage", messLive, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                    }
                                    else WIO.EmitAsync("SendMessage", mess, listMember);
                                    sendLink(mess, listMember.ToArray(), listDevices.ToArray());
                                }
                            }
                            else if ((indexSpace == -1 && indexEnter != -1) || (indexSpace != -1 && indexEnter != -1 && indexSpace > indexEnter))
                            {
                                mess.Message = mess.Message.Substring(index, indexEnter - index);
                                if (regex.IsMatch(mess.Message))
                                {
                                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                    {
                                        var messLive = mess;
                                        if (String.IsNullOrEmpty(Quote))
                                        {
                                            messLive.Quote = null;
                                            messLive.QuoteMessage = null;
                                        }
                                        WIO.EmitAsync("SendMessage", messLive, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                    }
                                    else WIO.EmitAsync("SendMessage", mess, listMember);
                                    sendLink(mess, listMember.ToArray(), listDevices.ToArray());
                                }
                            }
                            else
                            {
                                mess.Message = mess.Message.Substring(index);
                                if (regex.IsMatch(mess.Message))
                                {
                                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                                    {
                                        var messLive = mess;
                                        if (String.IsNullOrEmpty(Quote))
                                        {
                                            messLive.Quote = null;
                                            messLive.QuoteMessage = null;
                                        }
                                        WIO.EmitAsync("SendMessage", messLive, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                                    }
                                    else WIO.EmitAsync("SendMessage", mess, listMember);
                                    sendLink(mess, listMember.ToArray(), listDevices.ToArray());
                                }
                            }
                        }
                    }
                    DAOConversation.MarkUnreaderMessage(ConversationID, SenderID, listMember.ToArray());

                    APIMessages.data = new DataMessage();
                    APIMessages.data.result = true;
                    APIMessages.data.message = "Gửi tin nhắn thành công";
                }
                else
                {
                    APIMessages.error = new Error();
                    APIMessages.error.code = 200;
                    APIMessages.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessages.error = new Error();
                APIMessages.error.code = 200;
                APIMessages.data.message = ex != null ? ex.ToString() : "Có lỗi xảy ra";
            }
            return APIMessages;
        }
        //đã sửa xong
        [HttpPost("GetMessage")]
        [AllowAnonymous]
        public APIMessage GetMessage([FromForm] Messages message)
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                if (message.MessageID != null)
                {
                    message = DAOMessages.GetMessageById(message.MessageID);
                    if (message != null)
                    {
                        APIMessage.data = new DataMessage();
                        APIMessage.data.result = true;
                        APIMessage.data.message = "Lấy thông tin tin nhắn thành công";
                        APIMessage.data.message_info = message;
                        return APIMessage;
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Tin nhắn không tồn tại";
                    }
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.data.message = ex != null ? ex.ToString() : "";
            }
            return APIMessage;
        }

        //[HttpPost("UpdateInfoLink")]
        //[AllowAnonymous]
        //public APIMessage UpdateInfoLink([FromForm] InfoLink link)
        //{
        //    APIMessage APIMessage = new APIMessage();
        //    try
        //    {
        //        if (link.MessageID != null && link.Title != null)
        //        {
        //            int count = DAOMessages.InsertLink(link.MessageID, link.Title, link.Description, link.LinkHome, link.Image, 0);
        //            if (count != 0)
        //            {
        //                APIMessage.data = new DataMessage();
        //                APIMessage.data.result = true;
        //                APIMessage.data.message = "Update thông tin link thành công";
        //                return APIMessage;

        //            }
        //            else
        //            {
        //                APIMessage.error = new Error();
        //                APIMessage.error.code = 200;
        //                APIMessage.error.message = "Tin nhắn không tồn tại";
        //            }
        //        }
        //        else
        //        {
        //            APIMessage.error = new Error();
        //            APIMessage.error.code = 200;
        //            APIMessage.error.message = "Thiếu thông tin truyền lên";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        APIMessage.error = new Error();
        //        APIMessage.error.code = 200;
        //        APIMessage.error.message = ex.ToString();
        //    }
        //    return APIMessage;
        //}
        //đã sửa
        [HttpPost("DeleteMessage")]
        [AllowAnonymous]
        public APIMessage DeleteMessage([FromForm] Messages message)
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                if (message.MessageID != null)
                {

                    if (DAOMessages.DeleteMessage(message.MessageID) > 0)
                    {
                        APIMessage.data = new DataMessage();
                        APIMessage.data.result = true;
                        APIMessage.data.message = "Xóa nhắn thành công";
                        return APIMessage;
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Tin nhắn không tồn tại";
                    }
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }
        //đã sửa
        [HttpPost("EditMessage")]
        [AllowAnonymous]
        public APIMessage EditMessage([FromForm] Messages messages)
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                if (messages.Message != null && messages.MessageID != null)
                {
                    if (DAOMessages.EditMessage(messages.MessageID, messages.Message) > 0)
                    {
                        APIMessage.data = new DataMessage();
                        APIMessage.data.result = true;
                        APIMessage.data.message = "Sửa nhắn thành công";
                        return APIMessage;
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Tin nhắn không tồn tại";
                    }
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }

        [HttpPost("UpdateSupportStatusMessage")]
        [AllowAnonymous]
        public APIOTP UpdateSupportStatusMessage()
        {
            APIOTP api = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                string messageId = http.Form["messageId"];
                int userId = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                int conversationId = Convert.ToInt32(http.Form["conversationId"]);
                if (!string.IsNullOrEmpty(messageId))
                {
                    Messages mess = DAOMessages.GetMessageById(messageId);
                    if (mess != null)
                    {
                        Task t = new Task(() =>
                        {
                            DAOMessages.UpdateAllSupportStatus(mess.ConversationID, mess.MessageID, userId, status, conversationId);
                        });
                        t.Start();
                        t.ContinueWith(p =>
                        {
                            t.Dispose();
                        });
                        if (DAOMessages.UpdateSupportStatus(messageId, userId, status, conversationId) > 0)
                        {
                            api.data = new DataOTP();
                            api.data.result = true;
                            api.data.message = "Cập nhật thông tin thành công";
                        }
                        else
                        {
                            api.error = new Error();
                            api.error.message = "Cập nhật thông tin thất bại";
                        }
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.message = "Tin nhắn không tồn tại";
                    }
                }
                else
                {
                    api.error = new Error();
                    api.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.message = ex.ToString();
            }
            return api;
        }

        //[HttpPost("InsertError")]
        //[AllowAnonymous]
        //public APIMessage InsertError([FromForm] Model.EntityAPI.Error NewError)
        //{
        //    APIMessage APIMessage = new APIMessage();
        //    try
        //    {
        //        if (NewError.message != null && NewError.code != 0)
        //        {
        //            APIMessage.data = new DataMessage();
        //            APIMessage.data.result = true;
        //            APIMessage.data.message = "Thêm thông tin lỗi thành công";
        //            //if (DAOError.InsertError(NewError.message, NewError.code.ToString()) > 0)
        //            //{
        //            //    APIMessage.data = new DataMessage();
        //            //    APIMessage.data.result = true;
        //            //    APIMessage.data.message = "Thêm thông tin lỗi thành công";
        //            //    return APIMessage;
        //            //}
        //            //else
        //            //{
        //            //    APIMessage.error = new Error();
        //            //    APIMessage.error.code = 200;
        //            //    APIMessage.error.message = "Thêm thông tin lỗi thất bại";
        //            //}
        //        }
        //        else
        //        {
        //            APIMessage.error = new Error();
        //            APIMessage.error.code = 200;
        //            APIMessage.error.message = "Thiếu thông tin truyền lên";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        APIMessage.error = new Error();
        //        APIMessage.error.code = 200;
        //        APIMessage.error.message = ex.ToString();
        //    }
        //    return APIMessage;
        //}
        //đã sửa
        [HttpPost("SetEmotionMessage")]
        [AllowAnonymous]
        public APIMessage SetEmotionMessage([FromForm] APIEmotion mess)
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                if (mess.MessageID != null && mess.Type != 0)
                {
                    if (mess.ListUserId == null)
                    {
                        mess.ListUserId = "";
                    }
                    DAOMessages.UpdateInforEmotion(mess.MessageID, mess.ListUserId, mess.Type);
                    APIMessage.data = new DataMessage();
                    APIMessage.data.result = true;
                    APIMessage.data.message = "Thả cảm xúc thành công";
                    return APIMessage;
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }

        [HttpPost("SetDeleteDate")]
        [AllowAnonymous]
        public APIOTP SetDeleteDate()
        {
            APIOTP api = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                string id = httpRequest.Form["MessageID"];
                int conversationId = Convert.ToInt32(httpRequest.Form["ConversationID"]);
                string d = httpRequest.Form["DeleteDate"];
                DateTime date;
                if (!string.IsNullOrEmpty(id) && conversationId != 0 && DateTime.TryParse(d, out date))
                {
                    if (DAOMessages.SetDeleteDate(conversationId, id, date) > 0)
                    {
                        api.data = new DataOTP();
                        api.data.result = true;
                        api.data.message = "Cập nhật ngày xóa thành công";
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "Tin nhắn không tồn tại hoặc không đúng dạng tin nhắn";
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

        [HttpPost("UpdateMap")]
        [AllowAnonymous]
        public APIMessage UpdateMap()
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                var http = HttpContext.Request;
                string messId = http.Form["MessageId"];
                int converId = Convert.ToInt32(http.Form["ConversationID"]);
                string mess = http.Form["Message"];
                string mem = http.Form["ListMemeber"];
                if (mess != null && messId != null)
                {

                    string[] temp = mess.Split(";");
                    string location = temp.Length > 0 ? temp[temp.Length - 1] : "";
                    if (!string.IsNullOrEmpty(location))
                    {
                        string[] z = location.Split(',');
                        string link = String.Format(@"https://www.google.com/maps/search/{0},{1}/@{0},{1},10z?hl=vi", z[0].Trim(), z[1].Trim());
                        InfoLink info = getInfoLink(link);
                        if (DAOMessages.UpdateMap(messId, mess, info.Image) > 0)
                        {
                            Messages messageInfo = new Messages();
                            messageInfo.ConversationID = converId;
                            messageInfo.MessageID = messId;
                            messageInfo.Message = mess;
                            messageInfo.InfoLink = info;

                            if (messageInfo.InfoLink.HaveImage == "True")
                            {
                                messageInfo.Message = mess + "&Image:" + messageInfo.InfoLink.Image;
                            }

                            WIO.EmitAsync("EditMessage", messageInfo, JsonConvert.DeserializeObject<int[]>(mem));

                            APIMessage.data = new DataMessage();
                            APIMessage.data.result = true;
                            APIMessage.data.message = "Sửa nhắn thành công";
                            return APIMessage;
                        }
                        else
                        {
                            APIMessage.error = new Error();
                            APIMessage.error.code = 200;
                            APIMessage.error.message = "Sửa nhắn thất bại";
                        }
                    }
                    else
                    {
                        APIMessage.error = new Error();
                        APIMessage.error.code = 200;
                        APIMessage.error.message = "Sửa nhắn thất bại";
                    }
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }

        [HttpPost("CheckMessageError")]
        [AllowAnonymous]
        public APIMessage CheckMessageError()
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                List<Messages> listMess = new List<Messages>();
                listMess = DAOMessages.CheckMessageError();
                if (listMess.Count > 0)
                {
                    APIMessage.data = new DataMessage();
                    APIMessage.data.result = true;
                    APIMessage.data.message = "Lấy danh sách tin nhắn thành công";
                    APIMessage.data.listMessages = listMess;
                }
                else
                {
                    APIMessage.error = new Error();
                    APIMessage.error.code = 200;
                    APIMessage.error.message = "Cuộc trò chuyện không có tin nhắn nào";
                }

            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }


        [HttpPost("testmail")]
        [AllowAnonymous]
        public APIMessage testmail()
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(System.IO.Path.Combine(_environment.ContentRootPath, @"wwwroot\mailForm\mail_timviec.php"));
                string mailContent = string.Join("\n", lines);
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.mailgun.org";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("postmaster@mailtimviec365.vn", "bcbe2993383b34c696e1d1c5603b1618-100b5c8d-51aa397c");
                MailMessage msg = new MailMessage("chat365@mailtimviec365.vn", "quang28320@gmail.com");
                msg.Subject = "Bạn có một tin nhắn mới tại Chat365";
                msg.Body = mailContent;
                msg.IsBodyHtml = true;
                smtp.Send(msg);

                APIMessage.data = new DataMessage();
                APIMessage.data.result = true;
                APIMessage.data.message = "Lấy danh sách tin nhắn thành công";
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }


        [HttpPost("testx")]
        [AllowAnonymous]
        public APIMessage testx()
        {
            APIMessage APIMessage = new APIMessage();
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var remoteIpAddress = HttpContext.Connection.Id;

                APIMessage.data = new DataMessage();
                APIMessage.data.result = true;
                APIMessage.data.message = "Lấy danh sách tin nhắn thành công " + userId;
            }
            catch (Exception ex)
            {
                APIMessage.error = new Error();
                APIMessage.error.code = 200;
                APIMessage.error.message = ex.ToString();
            }
            return APIMessage;
        }
    }
}

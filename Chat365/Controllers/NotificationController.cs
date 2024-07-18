using APIChat365.MongoEntity;
using APIChat365.Model.MongoEntity;
using Chat365.Model.DAO;
using Chat365.Model.Entity;
using Chat365.Server.Model.DAO;
using Chat365.Server.Model.Entity;
using Chat365.Server.Model.EntityAPI;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APIChat365.Model.EntityAPI;
using APIChat365.Model.Entity;
using Chat365.Server.Controllers;
using Quartz.Impl.Triggers;
using VisioForge.MediaFramework.Helpers;
using MongoDB.Driver;
using System.Globalization;
using VisioForge.Libs.TagLib.Riff;
using VisioForge.Libs.Serilog.Sinks.File;

namespace Chat365.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
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
        private readonly ILogger<NotificationController> _logger;
        private readonly IWebHostEnvironment _environment;

        public NotificationController(ILogger<NotificationController> logger,
            IWebHostEnvironment environment)
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

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public User getInforUser(List<UserDB> getUser)
        {
            User userInfo = new User(getUser[0].id, getUser[0].id365, getUser[0].idTimViec, getUser[0].type365, getUser[0].email, getUser[0].password, getUser[0].phone, getUser[0].userName, getUser[0].avatarUser, getUser[0].status, getUser[0].statusEmotion, getUser[0].lastActive, getUser[0].active, getUser[0].isOnline, getUser[0].looker, getUser[0].companyId, getUser[0].companyName, getUser[0].notificationPayoff, getUser[0].notificationCalendar, getUser[0].notificationReport, getUser[0].notificationOffer, getUser[0].notificationPersonnelChange, getUser[0].notificationRewardDiscipline, getUser[0].notificationNewPersonnel, getUser[0].notificationTransferAsset, getUser[0].notificationChangeProfile, getUser[0].notificationMissMessage, getUser[0].notificationCommentFromTimViec, getUser[0].notificationCommentFromRaoNhanh, getUser[0].notificationTag, getUser[0].notificationSendCandidate, getUser[0].notificationChangeSalary, getUser[0].notificationAllocationRecall, getUser[0].notificationAcceptOffer, getUser[0].notificationDecilineOffer, getUser[0].notificationNTDPoint, getUser[0].notificationNTDExpiredPin, getUser[0].notificationNTDExpiredRecruit, getUser[0].fromWeb, getUser[0].notificationNTDApplying);
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

        private User GetEmployeeInfo(int UserId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "id_user", UserId.ToString() } });
                try
                {
                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/get_infor_user.php", infoLogin);
                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                    if (receiveInfo.data != null)
                    {
                        return new User(0, receiveInfo.data.user_info.ep_id, 0, 2, receiveInfo.data.user_info.ep_email, receiveInfo.data.user_info.ep_pass, receiveInfo.data.user_info.ep_phone, receiveInfo.data.user_info.ep_name, receiveInfo.data.user_info.ep_image, "", 1, DateTime.Now, 1, 1, 1, receiveInfo.data.user_info.com_id, receiveInfo.data.user_info.com_name, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "quanlychung365", 1);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        private User GetCompanyInfo(int UserId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    Task<HttpResponseMessage> response2 = httpClient.GetAsync("https://chamcong.24hpay.vn/api_tinhluong/list_com.php?id_com=" + UserId.ToString());
                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response2.Result.Content.ReadAsStringAsync().Result);
                    if (receiveInfo.data != null)
                    {
                        return new User(0, UserId, 0, 1, receiveInfo.data.items[0].com_email, receiveInfo.data.items[0].com_pass, receiveInfo.data.items[0].com_phone, receiveInfo.data.items[0].com_name, receiveInfo.data.items[0].com_logo == null ? "" : receiveInfo.data.items[0].com_logo, "", 1, DateTime.Now, 1, 1, 1, UserId, receiveInfo.data.items[0].com_name, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "quanlychung365", 1);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
        private async void sendNotificationToTimViec(Messages message)
        {
            try
            {
                DataTable infoConversation = DAOConversation.GetInfoConversation(message.ConversationID);
                if (infoConversation.Rows.Count > 0)
                {
                    for (int i = 0; i < infoConversation.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(infoConversation.Rows[i]["memberId"]) != message.SenderID)
                        {
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
                            if (Convert.ToInt32(infoConversation.Rows[i]["isOnline"]) == 0)
                            {
                                MultipartFormDataContent content = new MultipartFormDataContent();
                                content.Add(new StringContent(i == 0 ? infoConversation.Rows[1]["userName"].ToString() : infoConversation.Rows[0]["userName"].ToString()), "name");
                                content.Add(new StringContent(infoConversation.Rows[i]["email"].ToString()), "email");
                                content.Add(new StringContent(infoConversation.Rows[i]["type365"].ToString()), "type");
                                content.Add(new StringContent(mess), "mess");
                                content.Add(new StringContent(message.SenderID + ""), "senderId");
                                HttpClient httpClient = new HttpClient();
                                HttpResponseMessage response = await httpClient.PostAsync("https://timviec365.vn/api_app/notification_chat365.php ", content);
                            }
                            else
                            {
                                WIO.EmitAsync("SendNotificationTimViec", mess, i == 0 ? infoConversation.Rows[1]["userName"].ToString() : infoConversation.Rows[0]["userName"].ToString(), message.SenderID, Convert.ToInt32(infoConversation.Rows[i]["memberId"]));
                            }
                            break;
                        }
                    }
                    User user = getInforUser(DAOUsers.GetUsersById(message.SenderID));
                }
            }
            catch (Exception ex)
            {
            }
        }
        private InfoLink getInfoLink(string link)
        {
            InfoLink infoLink = new InfoLink();
            try
            {
                if (link.EndsWith("/"))
                {
                    link = link.Remove(link.Length - 1);
                }
                int index = link.IndexOf('/', 9);
                if (index != -1)
                {
                    infoLink.LinkHome = link.Remove(index);
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
                        infoLink.Title = "Không tim thấy thông tin website";
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
                    infoLink.Image = doc.Result.DocumentNode.Descendants("meta").Where(meta => meta.GetAttributeValue("property", "null") == "og:image").First().GetAttributeValue("content", "null").Replace("&#47;", "/");
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
            catch (Exception)
            {
                infoLink.Title = "Không tim thấy website";
            }
            infoLink.IsNotification = 1;
            return infoLink;
        }
        private void sendLink(Messages messageLink, int[] listMember)
        {
            messageLink.MessageID = DateTime.Now.Ticks + "_" + messageLink.SenderID;
            messageLink.InfoLink = getInfoLink(messageLink.Message);
            int count = DAOMessages.InsertMessage(messageLink.MessageID, messageLink.ConversationID, messageLink.SenderID, "link", messageLink.Message, "", "", messageLink.CreateAt, new InfoLinkDB(messageLink.InfoLink.Title, messageLink.InfoLink.Description, messageLink.InfoLink.LinkHome, messageLink.InfoLink.Image, messageLink.InfoLink.IsNotification), new List<FileSendDB>(), null, messageLink.DeleteTime, messageLink.DeleteType, messageLink.DeleteDate, 0);
            if (count > 0)
            {
                DAOConversation.MarkUnreaderMessage(messageLink.ConversationID, messageLink.SenderID, listMember);
                WIO.EmitAsync("SendMessage", messageLink, listMember);
            }
        }
        private int getIdTimViec(string email, string type)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(email), "email");
            content.Add(new StringContent(type), "type");
            HttpClient httpClient = new HttpClient();
            Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/get_id_email.php", content);
            APIRequestContact receiveInfo = JsonConvert.DeserializeObject<APIRequestContact>(response.Result.Content.ReadAsStringAsync().Result);
            if (receiveInfo.data != null)
            {
                return receiveInfo.data.id;
            }
            else
            {
                return 0;
            }
        }
        private User InsertNewUser(User user, bool isFullLink)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] dataArr = new byte[1];
                    long bytesize;
                    if (!String.IsNullOrEmpty(user.AvatarUser) && !(user.AvatarUser.Trim().Length == 0))
                    {
                        if (!isFullLink)
                        {
                            if (user.Type365 == 1)
                            {
                                dataArr = webClient.DownloadData("https://chamcong.24hpay.vn/upload/company/logo/" + user.AvatarUser);
                                bytesize = dataArr.Length;
                            }
                            else
                            {
                                dataArr = webClient.DownloadData("https://chamcong.24hpay.vn/upload/employee/" + user.AvatarUser);
                                bytesize = dataArr.Length;
                            }
                        }
                        else
                        {
                            dataArr = webClient.DownloadData(user.AvatarUser);
                            bytesize = dataArr.Length;
                        }
                    }
                    if (user.IDTimViec == 0)
                    {
                        user.IDTimViec = getIdTimViec(user.Email, user.Type365.ToString());
                    }
                    if (DAOUsers.checkMailEmpty365(user.Email, 0).Count > 0)
                    {
                        System.IO.File.WriteAllText("errorX1.txt", user.ID.ToString());
                        user.ID = Convert.ToInt32(DAOUsers.checkMailEmpty365(user.Email, 0)[0].id);
                        var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                        if (dataArr.Length > 1)
                        {
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                            if (!Directory.Exists(filePath + @"\" + user.ID))
                            {
                                Directory.CreateDirectory(filePath + @"\" + user.ID);
                            }
                            System.IO.DirectoryInfo di = new DirectoryInfo(filePath + @"\" + user.ID);
                            if (di.GetFiles().Length > 0)
                            {
                                di.GetFiles()[0].Delete();
                            }
                            string fileName = DateTime.Now.Ticks + "_" + user.ID;
                            System.IO.File.WriteAllBytes(di + @"\" + fileName + ".jpg", dataArr);
                            DAOUsers.UpdateInfoUser(user.ID, user.ID365, user.Type365, user.UserName, fileName + ".jpg", user.Password, user.CompanyId, user.CompanyName, user.IDTimViec);

                        }
                    }
                    else if (DAOUsers.CheckUsersByEmailAndPassword(user.Email, user.Password).Count > 0)
                    {
                        System.IO.File.WriteAllText("errorX2.txt", user.ID.ToString());
                        user.ID = DAOUsers.CheckUsersByEmailAndPassword(user.Email, user.Password)[0].id;
                        var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                        if (dataArr.Length > 1)
                        {
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                            if (!Directory.Exists(filePath + @"\" + user.ID))
                            {
                                Directory.CreateDirectory(filePath + @"\" + user.ID);
                            }
                            System.IO.DirectoryInfo di = new DirectoryInfo(filePath + @"\" + user.ID);
                            if (di.GetFiles().Length > 0)
                            {
                                di.GetFiles()[0].Delete();
                            }
                            string fileName = DateTime.Now.Ticks + "_" + user.ID;
                            System.IO.File.WriteAllBytes(di + @"\" + fileName + ".jpg", dataArr);
                            DAOUsers.UpdateInfoUser(user.ID, user.ID365, user.Type365, user.UserName, fileName + ".jpg", user.Password, user.CompanyId, user.CompanyName, user.IDTimViec);
                        }
                    }
                    else
                    {
                        user.ID = DAOUsers.InsertNewUser(user.UserName, user.ID365, user.IDTimViec, user.Type365, user.Email, user.Password, user.CompanyId, user.CompanyName, "quanlychung365");
                        System.IO.File.WriteAllText("error3.txt", user.ID.ToString());
                        if (dataArr.Length > 1 && user.ID > 0)
                        {
                            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                            if (!Directory.Exists(filePath))
                            {
                                Directory.CreateDirectory(filePath);
                            }
                            if (!Directory.Exists(filePath + @"\" + user.ID))
                            {
                                Directory.CreateDirectory(filePath + @"\" + user.ID);
                            }
                            System.IO.DirectoryInfo di = new DirectoryInfo(filePath + @"\" + user.ID);
                            if (di.GetFiles().Length > 0)
                            {
                                di.GetFiles()[0].Delete();
                            }
                            string fileName = DateTime.Now.Ticks + "_" + user.ID;
                            System.IO.File.WriteAllBytes(di + @"\" + fileName + ".jpg", dataArr);
                            DAOUsers.UpdateAvatarUser(fileName + ".jpg", user.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return user;
        }
        private UserFormAPI GetDepartmentInfo(int dep_id, int companyId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "dpid", dep_id.ToString() }, { "com_id", companyId.ToString() } });
                InforFromAPI receiveInfo = new InforFromAPI();
                try
                {
                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/get_department.php", infoLogin);
                    receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                    if (receiveInfo.data != null)
                    {
                        return receiveInfo.data.user_info;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
        private void sendNewNotificationText(int userId, int contactId, string mess, string link)
        {
            int conversationId = 0;
            int[] users = new int[] { userId, contactId };

            if (DAOConversation.checkEmptyConversation(userId, contactId).Count == 0)
            {
                conversationId = DAOConversation.insertNewConversation(0, "Normal", contactId);
                if (conversationId > 0)
                {
                    DAOConversation.insertNewParticipant(conversationId, "", 0, users, contactId, "Normal");
                }
            }
            else
            {
                conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(userId, contactId)[0].id);
            }
            string messageId = DateTime.Now.Ticks + "_" + contactId;
            Messages message = new Messages(messageId, conversationId, contactId, "text", mess, 0, DateTime.Now, DateTime.MinValue);
            int count = DAOMessages.InsertMessage(message.MessageID, message.ConversationID, message.SenderID, "text", message.Message, "", "", message.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0);
            sendNotificationToTimViec(message);
            if (count > 0)
            {
                DAOConversation.MarkUnreaderMessage(conversationId, contactId, users);
                WIO.EmitAsync("SendMessage", message, users);
                if (!String.IsNullOrWhiteSpace(link.Trim()))
                {
                    sendLink(new Messages(messageId, conversationId, contactId, "link", link, 0, DateTime.Now, DateTime.MinValue), users);
                }
            }
        }
        private void SendNewNotificationX(int userId, int contactId, string mess, string messType, string link)
        {
            int conversationId = 0;
            int[] users = new int[] { userId, contactId };

            if (DAOConversation.checkEmptyConversation(userId, contactId).Count == 0)
            {
                conversationId = DAOConversation.insertNewConversation(0, "Normal", contactId);
                if (conversationId > 0)
                {
                    DAOConversation.insertNewParticipant(conversationId, "", 0, users, contactId, "Normal");
                }
            }
            else
            {
                conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(userId, contactId)[0].id);
            }
            string messageId = DateTime.Now.Ticks + "_" + contactId;
            Messages message = new Messages(messageId, conversationId, contactId, messType, mess, 0, DateTime.Now, DateTime.MinValue);
            int count = DAOMessages.InsertMessage(message.MessageID, message.ConversationID, message.SenderID, messType, message.Message, "", "", message.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0);
            //sendNotificationToTimViec(message);
            if (count > 0)
            {
                if (messType != "OfferReceive" && messType != "applying" && messType != "newCandidate") DAOConversation.MarkUnreaderMessage(conversationId, contactId, users);
                WIO.EmitAsync("SendMessage", message, users);
                if (!String.IsNullOrWhiteSpace(link.Trim()))
                {
                    sendLink(new Messages(messageId, conversationId, contactId, "link", link, 0, DateTime.Now, DateTime.MinValue), users);
                }
            }
        }
        private void SendNewNotificationX1(int senderId, int conversationId, string mess, string messType, string link)
        {
            DataTable dataMember = DAOConversation.GetInfoConversation(conversationId);
            int[] users = new int[dataMember.Rows.Count];
            for (int i = 0; i < dataMember.Rows.Count; i++)
            {
                users[i] = Convert.ToInt32(dataMember.Rows[i]["memberId"]);
            }

            string messageId = DateTime.Now.Ticks + "_" + senderId;
            Messages message = new Messages(messageId, conversationId, senderId, messType, mess, 0, DateTime.Now, DateTime.MinValue);
            int count = DAOMessages.InsertMessage(message.MessageID, message.ConversationID, message.SenderID, messType, message.Message, "", "", message.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0);
            //sendNotificationToTimViec(message);
            if (count > 0)
            {
                if (messType != "OfferReceive" && messType != "applying" && messType != "newCandidate") DAOConversation.MarkUnreaderMessage(conversationId, senderId, users);
                WIO.EmitAsync("SendMessage", message, users);
                if (!String.IsNullOrWhiteSpace(link.Trim()))
                {
                    sendLink(new Messages(messageId, conversationId, senderId, "link", link, 0, DateTime.Now, DateTime.MinValue), users);
                }
            }
        }
        private int insertNewNotification(string id, int userId, User senderId, string title, string message, string type, string messageId, int conversationId, DateTime createAt, string link)
        {
            int result = 0;
            if (DAONotification.InsertNotification(id, userId, senderId.ID, title, message, type, messageId, conversationId, createAt, link) > 0)
            {
                result = 1;
                WIO.EmitAsync("SendNotification", userId, new Notifications(id, userId, senderId, title, message, 1, type, messageId, conversationId, createAt, link));
            }
            return result;
        }
        public static async Task LogError(string hihi)
        {
            using StreamWriter file = new("WriteLines2.txt");

            await file.WriteLineAsync(hihi);
        }

        [HttpPost("TransferPicture")]
        [AllowAnonymous]
        public NotificationAPI TransferPicture()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                string id = httpRequest.Form["id"];
                string picture = httpRequest.Form["picture"];
                string room = httpRequest.Form["room"];
                string time = httpRequest.Form["time"];
                string shift = httpRequest.Form["shift"];
                string name = httpRequest.Form["name"];
                if (id != null && picture != null && room != null && time != null && shift != null && name != null)
                {
                    WIO2.EmitAsync("Send_cc", id, picture, room, time, shift, name);
                    notificationAPI.data = new DataNotification();
                    notificationAPI.data.message = "Gửi ảnh thành công";
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
                notificationAPI.error.code = 200;
                notificationAPI.error.message = ex.ToString();
            }
            return notificationAPI;
        }

        //[HttpPost("ChangeSalary")]
        //[AllowAnonymous]
        //public NotificationAPI ChangeSalary()
        //{
        //    NotificationAPI notificationAPI = new NotificationAPI();
        //    try
        //    {
        //        var httpRequest = HttpContext.Request;
        //        int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
        //        int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
        //        long Salary = Convert.ToInt64(httpRequest.Form["Salary"]);
        //        string ListReceive = httpRequest.Form["ListReceive"];
        //        string CreateAt = httpRequest.Form["CreateAt"];

        //        if (CompanyId != 0 && EmployeeId != 0 && Salary != 0 && ListReceive != null && CreateAt != null)
        //        {
        //            User user = new User();
        //            List<UserDB> dataUser = DAOUsers.GetUserByID365(EmployeeId, 2);
        //            if (dataUser.Count > 0)
        //            {
        //                user = getInforUser(dataUser);
        //            }
        //            else
        //            {
        //                user = GetEmployeeInfo(EmployeeId);
        //            }
        //            if (user != null)
        //            {
        //                if (user.CompanyId == CompanyId)
        //                {
        //                    DataTable dataCompany = DAOUsers.GetUserIdById365(user.CompanyId, 1);
        //                    if (dataCompany.Rows.Count > 0)
        //                    {
        //                        int companyId = Convert.ToInt32(dataCompany.Rows[0]["id"]);
        //                        int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
        //                        for (int i = 0; i < listReceive.Length; i++)
        //                        {
        //                            DataTable dataUserReceive = DAOUsers.GetUserIdById365(listReceive[i], 2);
        //                            if (dataUserReceive.Rows.Count > 0)
        //                            {
        //                                var newSalary = new StringBuilder();
        //                                for (int j = 0; j < Salary.ToString().Length; j++)
        //                                {
        //                                    newSalary.Append(Salary.ToString()[j]);
        //                                    if ((Salary.ToString().Length - 1 - j) % 3 == 0 && (Salary.ToString().Length - 1 - j) != 0)
        //                                    {
        //                                        newSalary.Append(',');
        //                                    }
        //                                }
        //                                sendNewNotificationText(Convert.ToInt32(dataUserReceive.Rows[0]["id"]), companyId, "Lương cơ bản của " + user.UserName + " đã được thay đổi thành:\n" + newSalary + " VNĐ\n" + "Áp dụng từ ngày: " + Convert.ToDateTime(CreateAt).ToString("dd / MM / yyyy"), "");
        //                            }
        //                        }
        //                        notificationAPI.data = new DataNotification();
        //                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
        //                    }
        //                    else
        //                    {
        //                        notificationAPI.error = new Error();
        //                        notificationAPI.error.message = "Sai thông tin công ty";
        //                    }
        //                }
        //                else
        //                {
        //                    notificationAPI.error = new Error();
        //                    notificationAPI.error.message = "Sai thông tin công ty";
        //                }
        //            }
        //        }
        //        else
        //        {
        //            notificationAPI.error = new Error();
        //            notificationAPI.error.message = "Thiếu thông tin truyền lên";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        notificationAPI.error = new Error();
        //        notificationAPI.error.message = ex.ToString();
        //    }
        //    return notificationAPI;
        //}

        [HttpPost("ChangeSalary")]
        [AllowAnonymous]
        public NotificationAPI ChangeSalary()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                long Salary = Convert.ToInt64(httpRequest.Form["Salary"]);
                string ListReceive = httpRequest.Form["ListReceive"];
                string CreateAt = httpRequest.Form["CreateAt"];

                if (CompanyId != 0 && EmployeeId != 0 && Salary != 0 && ListReceive != null && CreateAt != null)
                {
                    User user = new User();
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(EmployeeId, 2);
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        user = GetEmployeeInfo(EmployeeId);
                    }
                    if (user != null)
                    {
                        if (user.NotificationChangeSalary == 1)
                        {
                            if (user.CompanyId == CompanyId)
                            {
                                List<UserDB> dataCompany = DAOUsers.GetUserByID365(user.CompanyId, 1);
                                if (dataCompany.Count > 0)
                                {
                                    User company = getInforUser(dataCompany);
                                    int companyId = company.ID;
                                    int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
                                    for (int i = 0; i < listReceive.Length; i++)
                                    {
                                        List<UserDB> dataUserReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                                        if (dataUserReceive.Count > 0)
                                        {
                                            var newSalary = new StringBuilder();
                                            for (int j = 0; j < Salary.ToString().Length; j++)
                                            {
                                                newSalary.Append(Salary.ToString()[j]);
                                                if ((Salary.ToString().Length - 1 - j) % 3 == 0 && (Salary.ToString().Length - 1 - j) != 0)
                                                {
                                                    newSalary.Append(',');
                                                }
                                            }
                                            DateTime NotiCreateAt = DateTime.Now;
                                            string notificationId = DateTime.Now.Ticks + "_" + dataUserReceive[0].id;
                                            string Status = "";
                                            if (listReceive[i] == EmployeeId) Status = "Mức lương của bạn đã có sự thay đổi";
                                            else Status = "Bạn đã thay đổi mức lương cho nhân viên " + user.UserName;
                                            if (DAONotification.InsertNotification(notificationId, dataUserReceive[0].id, companyId, "", Status, "ChangeSalary", "", 0, NotiCreateAt, "") > 0)
                                            {
                                                WIO.EmitAsync("SendNotification", dataUserReceive[0].id, new Notifications(notificationId, dataUser[0].id, company, "", Status, 1, "ChangeSalary", "", 0, NotiCreateAt, ""));
                                            }

                                        }
                                    }
                                    notificationAPI.data = new DataNotification();
                                    notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                                }
                                else
                                {
                                    notificationAPI.error = new Error();
                                    notificationAPI.error.message = "Sai thông tin công ty";
                                }
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Sai thông tin công ty";
                            }
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "User đã tắt thông báo này";
                        }
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

        //()
        [HttpPost("Payoff")]
        [AllowAnonymous]
        public NotificationAPI Payoff()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                long Salary = Convert.ToInt64(httpRequest.Form["Salary"]);
                string Status = httpRequest.Form["Status"];
                string Message = httpRequest.Form["Message"];
                string ListReceive = httpRequest.Form["ListReceive"];
                string CreateAt = httpRequest.Form["CreateAt"];
                if (CompanyId != 0 && EmployeeId != 0 && Salary != 0 && ListReceive != null && Status != null && CreateAt != null && Message != null)
                {
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(EmployeeId, 2);
                    User user = new User();
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        user = GetEmployeeInfo(EmployeeId);
                    }
                    if (user != null)
                    {
                        if (user.CompanyId == CompanyId)
                        {
                            DataTable dataCompany = DAOUsers.GetUserIdById365(user.CompanyId, 1);
                            if (dataCompany.Rows.Count > 0)
                            {
                                int companyId = Convert.ToInt32(dataCompany.Rows[0]["id"]);
                                int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
                                for (int i = 0; i < listReceive.Length; i++)
                                {
                                    List<UserDB> dataUserReceive = new List<UserDB>();
                                    dataUserReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                                    if (dataUserReceive.Count > 0 && Convert.ToInt32(dataUserReceive[0].notificationPayoff) == 1)
                                    {
                                        var newSalary = new StringBuilder();
                                        for (int j = 0; j < Salary.ToString().Length; j++)
                                        {
                                            newSalary.Append(Salary.ToString()[j]);
                                            if ((Salary.ToString().Length - 1 - j) % 3 == 0 && (Salary.ToString().Length - 1 - j) != 0)
                                            {
                                                newSalary.Append(',');
                                            }
                                        }
                                        string message = "";
                                        if (Status.Equals("bonus"))
                                        {
                                            message = "Thưởng " + user.UserName + ": " + newSalary + " VNĐ\n" + "Vào ngày: " + Convert.ToDateTime(CreateAt).ToString("dd/MM/yyyy") + "\nNội dung: " + Message;
                                        }
                                        else
                                        {
                                            message = "Phạt " + user.UserName + ":" + newSalary + " VNĐ\n" + "Vào ngày:" + Convert.ToDateTime(CreateAt).ToString("dd/MM/yyyy") + "\nNội dung: " + Message;
                                        }
                                        sendNewNotificationText(Convert.ToInt32(dataUserReceive[0].id), companyId, message, "");
                                    }
                                }
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin nhân viên";
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

        //()
        [HttpPost("Calendar")]
        [AllowAnonymous]
        public NotificationAPI Calendar()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                string CreateAt = httpRequest.Form["CreateAt"];
                string ListEmployee = httpRequest.Form["ListEmployee"];
                if (CompanyId != 0 && ListEmployee != null && CreateAt != null)
                {
                    int[] listEmloyee = JsonConvert.DeserializeObject<int[]>(ListEmployee);
                    for (int i = 0; i < listEmloyee.Length; i++)
                    {
                        List<UserDB> dataUser = DAOUsers.GetUserByID365(listEmloyee[i], 2);
                        if (dataUser.Count > 0 && Convert.ToInt32(dataUser[0].notificationCalendar) == 1)
                        {
                            User user = getInforUser(dataUser);
                            if (user.CompanyId == CompanyId)
                            {
                                int companyId = Convert.ToInt32(DAOUsers.GetUserIdById365(user.CompanyId, 1).Rows[0]["id"]);
                                DateTime timeCreate = Convert.ToDateTime(CreateAt);
                                string message = "Lịch làm việc mới tháng " + timeCreate.Month + "/" + timeCreate.Year + " của " + user.UserName + " tại đây :\nhttps://tinhluong.timviec365.vn/lich-lam-viec-nhan-vien.html";
                                sendNewNotificationText(Convert.ToInt32(user.ID), companyId, message, "https://tinhluong.timviec365.vn/lich-lam-viec-nhan-vien.html");
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Sai thông tin công ty";
                            }
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin người dùng";
                        }
                    }
                    notificationAPI.data = new DataNotification();
                    notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
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

        [HttpPost("NotificationSalary")]
        [AllowAnonymous]
        public NotificationAPI NotificationSalary([FromForm] User user)
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                if (user.ID != 0 && user.Password != null && user.ID365 != 0 && user.CompanyId != 0)
                {

                    using (HttpClient httpClient = new HttpClient())
                    {
                        var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "pass", user.Password }, { "ep_id", user.ID365.ToString() }, { "com_id", user.CompanyId.ToString() } });
                        InforFromAPI receiveInfo = new InforFromAPI();
                        try
                        {
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/get_token.php", infoLogin);
                            receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                            if (receiveInfo.data != null)
                            {
                                var infoUser = new FormUrlEncodedContent(new Dictionary<string, string> { { "token", receiveInfo.data.access_token }, { "ep_id", user.ID365.ToString() }, { "com_id", user.CompanyId.ToString() } });
                                Task<HttpResponseMessage> response2 = httpClient.PostAsync("https://tinhluong.timviec365.vn/api_web/api_luong_nv.php", infoUser);

                                if (Convert.ToInt32(response2.Result.Content.ReadAsStringAsync().Result.Replace(",", "")) > 0)
                                {
                                    int companyId = Convert.ToInt32(DAOUsers.GetUserIdById365(user.CompanyId, 1).Rows[0]["id"]);
                                    string message = "Tổng lương hiện tại của bạn trong tháng này là: " + response2.Result.Content.ReadAsStringAsync().Result + " VNĐ";
                                    sendNewNotificationText(Convert.ToInt32(user.ID), companyId, message, "");
                                    notificationAPI.data = new DataNotification();
                                    notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                                }
                                else
                                {
                                    notificationAPI.error = new Error();
                                    notificationAPI.error.message = "Sai thông tin công ty";
                                }
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = receiveInfo.error.message;
                            }
                        }
                        catch
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Có lỗi xảy ra khi xử lý";
                        }
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

        [HttpPost("NotificationRose")]
        [AllowAnonymous]
        public NotificationAPI NotificationRose([FromForm] User user)
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                if (user.ID != 0 && user.ID365 != 0 && user.CompanyId != 0)
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        try
                        {
                            var infoUser = new FormUrlEncodedContent(new Dictionary<string, string> { { "uid", user.ID365.ToString() }, { "cp", user.CompanyId.ToString() } });
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://tinhluong.timviec365.vn/api_web/api_hoa_hong_nv.php", infoUser);
                            InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                            if (receiveInfo.data != null)
                            {
                                int companyId = Convert.ToInt32(DAOUsers.GetUserIdById365(user.CompanyId, 1).Rows[0]["id"]);
                                string message = "Tổng hoa hồng hiện tại của bạn trong tháng này là: " + receiveInfo.data.item_rose.rose_sum + " VNĐ\n" + "Trong đó:\n" + "+ Hoa hồng tiền: " + receiveInfo.data.item_rose.rose1 + " VNĐ\n+ Hoa hồng doanh thu: " + receiveInfo.data.item_rose.rose2 + " VNĐ\n+ Hoa hồng lợi nhuận: " + receiveInfo.data.item_rose.rose3 + " VNĐ\n+ Hoa hồng lệ phí vị trí: " + receiveInfo.data.item_rose.rose4 + " VNĐ\n+ Hoa hồng kế hoạch: " + receiveInfo.data.item_rose.rose5 + " VNĐ";
                                sendNewNotificationText(Convert.ToInt32(user.ID), companyId, message, "");
                                notificationAPI.data = new DataNotification();
                                notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Sai thông tin công ty";
                            }
                        }
                        catch (Exception ex)
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = ex.ToString();
                        }
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

        //()
        [HttpPost("NotificationReport")]
        [AllowAnonymous]
        public NotificationAPI NotificationReport()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                string ListReceive = httpRequest.Form["ListReceive"];
                string ListFollower = httpRequest.Form["ListFollower"];
                string Status = httpRequest.Form["Status"];
                string Message = httpRequest.Form["Message"];
                string Title = httpRequest.Form["Title"];
                string Link = httpRequest.Form["Link"];
                if (CompanyId != 0 && SenderId != 0 && Type != 0 && ListReceive != null && ListFollower != null && Status != null && Message != null && Title != null)
                {
                    string typyUser = "";
                    List<UserDB> dataUser = new List<UserDB>();
                    if (Type == 1 || Type == 2)
                    {
                        dataUser = DAOUsers.GetUserByID365(SenderId, 2);
                        typyUser = "Nhân viên của ";
                    }
                    else if (Type == 3 || Type == 4)
                    {
                        dataUser = DAOUsers.GetUserByID365(SenderId, 1);
                        typyUser = "";
                    }
                    User user = new User();
                    if (dataUser.Count != 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        if (Type == 1 || Type == 2)
                        {
                            user = GetEmployeeInfo(SenderId);
                            if (user != null)
                            {
                                user = InsertNewUser(user, false);
                            }
                        }
                        else if (Type == 3 || Type == 4)
                        {
                            user = GetCompanyInfo(SenderId);
                            if (user != null)
                            {
                                user = InsertNewUser(user, false);
                            }
                        }
                    }
                    if (user != null)
                    {
                        if (CompanyId == user.CompanyId)
                        {
                            string[] typeDocument = new string[] { "Nghị quyết", "Quyết định", "Chỉ thị", "Quy chế", "Quy định", "Thông cáo", "Thông báo", "Hướng dẫn", "Chương trình", "Kế hoạch", "Phương án", "Đề án", "Dự án", "Báo cáo", "Biên bản", "Tờ trình", "Hợp đồng", "Công văn", "Công điện", "Bản ghi nhớ", "Bản thỏa thuận", "Giấy ủy quyền", "Giấy mời", "Giấy giới thiệu", "Giấy nghỉ phép", "Phiếu gửi", "Phiếu chuyển", "Phiếu báo", "Thư công" };
                            int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);

                            for (int i = 0; i < listReceive.Length; i++)
                            {
                                if (listReceive[i] != SenderId)
                                {
                                    List<UserDB> dataReceive = new List<UserDB>();
                                    User userReceive = new User();
                                    if (Type == 1 || Type == 3)
                                    {
                                        dataReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                                    }
                                    else if (Type == 2 || Type == 4)
                                    {
                                        dataReceive = DAOUsers.GetUserByID365(listReceive[i], 1);
                                    }
                                    if (dataReceive.Count > 0)
                                    {
                                        userReceive = getInforUser(dataReceive);
                                    }
                                    else
                                    {
                                        if (Type == 1 || Type == 3)
                                        {
                                            userReceive = GetEmployeeInfo(listReceive[i]);
                                            if (userReceive != null)
                                            {
                                                userReceive = InsertNewUser(userReceive, false);
                                            }
                                        }
                                        else if (Type == 2 || Type == 4)
                                        {
                                            userReceive = GetCompanyInfo(listReceive[i]);
                                            if (userReceive != null)
                                            {
                                                userReceive = InsertNewUser(userReceive, false);
                                            }
                                        }
                                    }
                                    if (userReceive != null && userReceive.NotificationReport == 1)
                                    {
                                        string message = "";
                                        string link = "";
                                        if (String.IsNullOrEmpty(Link))
                                        {
                                            link = "https://vanthu.timviec365.vn/quanly-cong-van.html";
                                        }
                                        else
                                        {
                                            link = Link;
                                        }
                                        if (Status.Equals("1"))
                                        {
                                            message = userReceive.UserName + " có một yêu cầu duyệt " + typeDocument[Convert.ToInt32(Message) - 1] + " mới gửi đi từ " + user.UserName + "\nTiêu đề: " + Title + "\nĐể duyệt bạn vui lòng truy cập tại đây:\n" + link;
                                        }
                                        else
                                        {
                                            if (Type == 1 || Type == 2)
                                            {
                                                message = userReceive.UserName + " vừa được nhận " + typeDocument[Convert.ToInt32(Message)] + " mới chuyển đến từ " + user.UserName + "\nNhân viên của  " + user.CompanyName + "\nTiêu đề: " + Title + "\nĐể biết thêm chi tiết bạn vui lòng truy cập tại đây:\n" + link;
                                            }
                                            else if (Type == 3 || Type == 4)
                                            {
                                                message = userReceive.UserName + " vừa được nhận " + typeDocument[Convert.ToInt32(Message)] + " mới chuyển đến từ " + user.UserName + "\nTiêu đề: " + Title + "\nĐể biết thêm chi tiết bạn vui lòng truy cập tại đây:\n" + link;
                                            }
                                        }
                                        if (user.CompanyId != userReceive.CompanyId)
                                        {
                                            if (DAOUsers.AddNewContact(user.ID, userReceive.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", user.ID, userReceive.ID);
                                        }
                                        sendNewNotificationText(userReceive.ID, user.ID, message, link);
                                    }
                                }
                            }
                            if (!ListFollower.Equals("[0]"))
                            {
                                int[] listFollower = JsonConvert.DeserializeObject<int[]>(ListFollower);
                                for (int i = 0; i < listFollower.Length; i++)
                                {
                                    if (!listReceive.Contains(listFollower[i]) && listFollower[i] != SenderId)
                                    {
                                        List<UserDB> dataReceive = DAOUsers.GetUserByID365(listFollower[i], 2);
                                        User userReceive = new User();
                                        if (dataReceive.Count > 0)
                                        {
                                            userReceive = getInforUser(dataReceive);
                                        }
                                        else
                                        {
                                            userReceive = GetEmployeeInfo(listFollower[i]);
                                            if (userReceive != null)
                                            {
                                                userReceive = InsertNewUser(userReceive, false);
                                            }
                                        }
                                        if (userReceive != null && userReceive.NotificationReport == 1)
                                        {
                                            string message = "";
                                            string link = "";
                                            if (String.IsNullOrEmpty(Link))
                                            {
                                                link = "https://vanthu.timviec365.vn/trang-chu-quan-ly-cong-van.html";
                                            }
                                            else
                                            {
                                                link = Link;
                                            }
                                            if (Status.Equals("1"))
                                            {
                                                if (Type == 1 || Type == 2)
                                                {
                                                    message = userReceive.UserName + " vừa được thêm vào người theo dõi " + typeDocument[Convert.ToInt32(Message)] + " mới gửi đi từ " + user.UserName + "\nTiêu đề: " + Title + "\n" + typyUser + user.CompanyName + "\nĐể xem bạn vui lòng truy cập tại đây:\n" + link;
                                                }
                                                else if (Type == 3 || Type == 4)
                                                {
                                                    message = userReceive.UserName + " vừa được thêm vào người theo dõi " + typeDocument[Convert.ToInt32(Message)] + " mới chuyển từ " + user.UserName + "\nTiêu đề: " + Title + "\nĐể xem bạn vui lòng truy cập tại đây:\n" + link;
                                                }
                                            }
                                            else
                                            {
                                                if (Type == 1 || Type == 2)
                                                {
                                                    message = "Văn bản " + typeDocument[Convert.ToInt32(Message)] + " mới chuyển từ " + user.UserName + "\n" + typyUser + user.CompanyName + "\nTiêu đề: " + Title + "\nĐã được gửi để xem bạn vui lòng truy cập tại đây:\n" + link;
                                                }
                                                else if (Type == 3 || Type == 4)
                                                {
                                                    message = "Văn bản " + typeDocument[Convert.ToInt32(Message)] + " mới chuyển từ " + user.UserName + "\nTiêu đề: " + Title + "\nĐã được gửi để xem bạn vui lòng truy cập tại đây:\n" + link;
                                                }
                                            }
                                            if (user.CompanyId != userReceive.CompanyId)
                                            {
                                                if (DAOUsers.AddNewContact(user.ID, userReceive.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", user.ID, userReceive.ID);
                                            }
                                            sendNewNotificationText(userReceive.ID, user.ID, message, link);
                                        }
                                    }
                                }
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin nhân viên";
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

        [HttpPost("NotificationOfferReceive")]
        [AllowAnonymous]
        public NotificationAPI NotificationOfferReceive()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                string ListReceive = httpRequest.Form["ListReceive"];
                string ListFollower = httpRequest.Form["ListFollower"];
                string Status = httpRequest.Form["Status"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                if (CompanyId != 0 && ListReceive != null && SenderId != 0 && Message != null && Status != null && ListFollower != null)
                {
                    if (!string.IsNullOrEmpty(Status))
                    {
                        if (!string.IsNullOrEmpty(Status))
                        {
                            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                            Status = textInfo.ToTitleCase(Status);
                        }
                    }
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(SenderId, 2);
                    User user = new User();
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        user = GetEmployeeInfo(SenderId);
                        if (user != null)
                        {
                            user = InsertNewUser(user, false);
                        }
                    }
                    if (user != null)
                    {
                        if (user.CompanyId == CompanyId)
                        {
                            int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
                            for (int i = 0; i < listReceive.Length; i++)
                            {
                                if (listReceive[i] != SenderId)
                                {
                                    List<UserDB> dataReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                                    User userReceive = new User();
                                    if (dataReceive.Count > 0)
                                    {
                                        userReceive = getInforUser(dataReceive);
                                    }
                                    else
                                    {
                                        userReceive = GetEmployeeInfo(listReceive[i]);
                                        if (userReceive != null)
                                        {
                                            userReceive = InsertNewUser(userReceive, false);
                                        }
                                    }
                                    if (userReceive != null && userReceive.NotificationOffer == 1)
                                    {
                                        string link = "";
                                        if (String.IsNullOrEmpty(Link))
                                        {
                                            link = "https://vanthu.timviec365.vn/de-xuat-gui-den.html";
                                        }
                                        else
                                        {
                                            link = Link;
                                        }
                                        SendNewNotificationX(userReceive.ID, user.ID, Status + "\nHọ tên: " + user.UserName + "\nNội dung: " + Message, "OfferReceive", link);
                                    }
                                }
                            }
                            List<UserDB> dataCompany = DAOUsers.GetUserByID365(CompanyId, 1);
                            User userCompany = new User();
                            if (dataCompany.Count > 0)
                            {
                                userCompany = getInforUser(dataCompany);
                            }
                            else
                            {
                                userCompany = GetCompanyInfo(CompanyId);
                                if (userCompany != null)
                                {
                                    userCompany = InsertNewUser(userCompany, false);
                                }
                            }
                            if (userCompany != null && userCompany.NotificationOffer == 1)
                            {
                                string link = "";
                                if (String.IsNullOrEmpty(Link))
                                {
                                    link = "https://vanthu.timviec365.vn/de-xuat-gui-di.htm";
                                }
                                else
                                {
                                    link = Link;
                                }
                                SendNewNotificationX(userCompany.ID, user.ID, Status + "\nHọ tên: " + user.UserName + "\nNội dung: " + Message, "OfferReceive", link);
                            }
                            if (!ListFollower.Equals("[0]"))
                            {
                                int[] listFollower = JsonConvert.DeserializeObject<int[]>(ListFollower);
                                for (int i = 0; i < listFollower.Length; i++)
                                {
                                    if (listFollower[i] != SenderId && !listReceive.Contains(listFollower[i]))
                                    {
                                        List<UserDB> dataReceive = DAOUsers.GetUserByID365(listFollower[i], 2);
                                        User userReceive = new User();
                                        if (dataReceive.Count > 0)
                                        {
                                            userReceive = getInforUser(dataReceive);
                                        }
                                        else
                                        {
                                            userReceive = GetEmployeeInfo(listFollower[i]);
                                            if (userReceive != null)
                                            {
                                                userReceive = InsertNewUser(userReceive, false);
                                            }
                                        }
                                        if (userReceive != null && userReceive.NotificationOffer == 1)
                                        {
                                            string link = "";
                                            if (String.IsNullOrEmpty(Link))
                                            {
                                                link = "https://vanthu.timviec365.vn/dang-theo-doi-de-xuat.html";
                                            }
                                            else
                                            {
                                                link = Link;
                                            }
                                            SendNewNotificationX(userReceive.ID, user.ID, Status + "\nHọ tên: " + user.UserName + "\nNội dung: " + Message, "OfferReceive", link);
                                        }
                                    }
                                }
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin nhân viên";
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

        [HttpPost("NotificationOffer_Receive")]
        [AllowAnonymous]
        public NotificationAPI NotificationOffer_Receive()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                string ListReceive = httpRequest.Form["ListReceive"];
                string ListFollower = httpRequest.Form["ListFollower"];
                string Status = httpRequest.Form["Status"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                if (CompanyId != 0 && ListReceive != null && SenderId != 0 && Message != null && Status != null && ListFollower != null)
                {
                    if (!string.IsNullOrEmpty(Status) && Status.Contains(" "))
                    {
                        string[] temp = Status.Split(' ');
                        if (temp.Length > 0)
                        {
                            var k = temp.ToList();
                            k.ForEach(x => x = x[0].ToString().ToUpper() + x.Substring(1));
                            Status = String.Join(" ", k);
                        }
                    }
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(SenderId, 2);
                    User user = new User();
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        user = GetEmployeeInfo(SenderId);
                        if (user != null)
                        {
                            user = InsertNewUser(user, false);
                        }
                    }
                    if (user != null)
                    {
                        if (user.CompanyId == CompanyId)
                        {
                            int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
                            for (int i = 0; i < listReceive.Length; i++)
                            {
                                if (listReceive[i] != SenderId)
                                {
                                    List<UserDB> dataReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                                    User userReceive = new User();
                                    if (dataReceive.Count > 0)
                                    {
                                        userReceive = getInforUser(dataReceive);
                                    }
                                    else
                                    {
                                        userReceive = GetEmployeeInfo(listReceive[i]);
                                        if (userReceive != null)
                                        {
                                            userReceive = InsertNewUser(userReceive, false);
                                        }
                                    }
                                    if (userReceive != null && userReceive.NotificationOffer == 1)
                                    {
                                        string link = "";
                                        if (String.IsNullOrEmpty(Link))
                                        {
                                            link = "https://vanthu.timviec365.vn/de-xuat-gui-den.html";
                                        }
                                        else
                                        {
                                            link = Link;
                                        }
                                        sendNewNotificationText(userReceive.ID, user.ID, userReceive.UserName + " vừa nhận được đề xuất " + Status + " mới\nTừ: " + user.UserName + "\nNội dung: " + Message + "\nĐể duyệt bạn vui lòng truy cập tại đây:\n" + link, link);
                                    }
                                }
                            }
                            List<UserDB> dataCompany = DAOUsers.GetUserByID365(CompanyId, 1);
                            User userCompany = new User();
                            if (dataCompany.Count > 0)
                            {
                                userCompany = getInforUser(dataCompany);
                            }
                            else
                            {
                                userCompany = GetCompanyInfo(CompanyId);
                                if (userCompany != null)
                                {
                                    userCompany = InsertNewUser(userCompany, false);
                                }
                            }
                            if (userCompany != null && userCompany.NotificationOffer == 1)
                            {
                                string link = "";
                                if (String.IsNullOrEmpty(Link))
                                {
                                    link = "https://vanthu.timviec365.vn/de-xuat-gui-di.htm";
                                }
                                else
                                {
                                    link = Link;
                                }
                                sendNewNotificationText(userCompany.ID, user.ID, "Đề xuất " + Status + " của " + user.UserName + " đã được gửi đi\nNội dung: " + Message + "\nĐể xem bạn vui lòng truy cập tại đây:\n" + link, link);
                            }
                            if (!ListFollower.Equals("[0]"))
                            {
                                int[] listFollower = JsonConvert.DeserializeObject<int[]>(ListFollower);
                                for (int i = 0; i < listFollower.Length; i++)
                                {
                                    if (listFollower[i] != SenderId && !listReceive.Contains(listFollower[i]))
                                    {
                                        List<UserDB> dataReceive = DAOUsers.GetUserByID365(listFollower[i], 2);
                                        User userReceive = new User();
                                        if (dataReceive.Count > 0)
                                        {
                                            userReceive = getInforUser(dataReceive);
                                        }
                                        else
                                        {
                                            userReceive = GetEmployeeInfo(listFollower[i]);
                                            if (userReceive != null)
                                            {
                                                userReceive = InsertNewUser(userReceive, false);
                                            }
                                        }
                                        if (userReceive != null && userReceive.NotificationOffer == 1)
                                        {
                                            string link = "";
                                            if (String.IsNullOrEmpty(Link))
                                            {
                                                link = "https://vanthu.timviec365.vn/dang-theo-doi-de-xuat.html";
                                            }
                                            else
                                            {
                                                link = Link;
                                            }
                                            sendNewNotificationText(userReceive.ID, user.ID, userReceive.UserName + " vừa được thêm vào người theo dõi đề xuất " + Status + " mới\nTừ: " + user.UserName + "\nNội dung: " + Message + "\nĐể xem bạn vui lòng truy cập tại đây:\n" + link, link);
                                        }
                                    }
                                }
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin nhân viên";
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
        //()
        [HttpPost("Notification_OfferSent")]
        [AllowAnonymous]
        public NotificationAPI Notification_OfferSent()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                string ListReceive = httpRequest.Form["ListReceive"];
                string ListFollower = httpRequest.Form["ListFollower"];
                string Status = httpRequest.Form["Status"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                if (SenderId != 0 && CompanyId != 0 && EmployeeId != 0 && ListFollower != null && Message != null && Status != null)
                {
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(SenderId, 2);
                    User user = new User();
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        user = GetEmployeeInfo(SenderId);
                        if (user != null)
                        {
                            user = InsertNewUser(user, false);
                        }
                    }
                    if (user != null)
                    {
                        if (user.CompanyId == CompanyId)
                        {
                            List<UserDB> dataEmployee = DAOUsers.GetUserByID365(EmployeeId, 2);
                            User userEmployee = new User();
                            if (SenderId != EmployeeId)
                            {
                                if (dataEmployee.Count > 0)
                                {
                                    userEmployee = getInforUser(dataEmployee);
                                }
                                else
                                {
                                    userEmployee = GetEmployeeInfo(EmployeeId);
                                    if (userEmployee != null)
                                    {
                                        userEmployee = InsertNewUser(userEmployee, false);
                                    }
                                }
                                if (userEmployee != null && userEmployee.NotificationOffer == 1)
                                {
                                    string link = "";
                                    if (String.IsNullOrEmpty(Link))
                                    {
                                        link = "https://vanthu.timviec365.vn/de-xuat-gui-di.html";
                                    }
                                    else
                                    {
                                        link = Link;
                                    }
                                    sendNewNotificationText(userEmployee.ID, user.ID, "Đề xuất " + Status + " của " + userEmployee.UserName + " đã được duyệt\nNội dung: " + Message + "\nĐể xem bạn vui lòng truy cập tại đây:\n" + link, link);
                                }
                            }
                            List<UserDB> dataCompany = DAOUsers.GetUserByID365(CompanyId, 1);
                            User userCompany = new User();
                            if (dataCompany.Count > 0)
                            {
                                userCompany = getInforUser(dataCompany);
                            }
                            else
                            {
                                userCompany = GetCompanyInfo(CompanyId);
                                if (userCompany != null)
                                {
                                    userCompany = InsertNewUser(userCompany, false);
                                }
                            }
                            if (userCompany != null && userCompany.NotificationOffer == 1)
                            {
                                string link = "";
                                if (String.IsNullOrEmpty(Link))
                                {
                                    link = "https://vanthu.timviec365.vn/de-xuat-gui-di.html";
                                }
                                else
                                {
                                    link = Link;
                                }
                                sendNewNotificationText(userCompany.ID, user.ID, "Đề xuất " + Status + " của " + userEmployee.UserName + " đã được duyệt\nNội dung: " + Message + "\nĐể xem bạn vui lòng truy cập tại đây:\n" + link, link);
                            }
                            int[] listFollower = JsonConvert.DeserializeObject<int[]>(ListFollower);
                            for (int i = 0; i < listFollower.Length; i++)
                            {
                                if (listFollower[i] != SenderId && listFollower[i] != EmployeeId)
                                {
                                    List<UserDB> dataReceive = DAOUsers.GetUserByID365(listFollower[i], 2);
                                    User userReceive = new User();
                                    if (dataReceive.Count > 0)
                                    {
                                        userReceive = getInforUser(dataReceive);
                                    }
                                    else
                                    {
                                        userReceive = GetEmployeeInfo(listFollower[i]);
                                        if (userReceive != null)
                                        {
                                            userReceive = InsertNewUser(userReceive, false);
                                        }
                                    }
                                    if (userReceive != null && userReceive.NotificationOffer == 1)
                                    {
                                        string link = "";
                                        if (String.IsNullOrEmpty(Link))
                                        {
                                            link = "https://vanthu.timviec365.vn/dang-theo-doi-de-xuat.html";
                                        }
                                        else
                                        {
                                            link = Link;
                                        }
                                        sendNewNotificationText(userReceive.ID, user.ID, "Đề xuất " + Status + " của " + userEmployee.UserName + " đã được phê duyệt\nNội dung: " + Message + "\nĐể xem bạn vui lòng truy cập tại đây:\n" + link, link);
                                    }
                                }
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin nhân viên";
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
        //()
        [HttpPost("NotificationOfferSent")]
        [AllowAnonymous]
        public NotificationAPI NotificationOfferSent()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                string ListReceive = httpRequest.Form["ListReceive"];
                string ListFollower = httpRequest.Form["ListFollower"];
                string Status = httpRequest.Form["Status"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                int Success = Convert.ToInt32(httpRequest.Form["type"]);
                if (SenderId != 0 && CompanyId != 0 && EmployeeId != 0 && ListFollower != null && Message != null && Status != null)
                {
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(SenderId, 2);
                    User user = new User();
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        user = GetEmployeeInfo(SenderId);
                        if (user != null)
                        {
                            user = InsertNewUser(user, false);
                        }
                    }
                    if (user != null)
                    {
                        string typeOffer = "";
                        if (Success == 0) typeOffer = "AcceptOffer";
                        else typeOffer = "DecilineOffer";
                        DateTime createAt = DateTime.Now;
                        if (user.CompanyId == CompanyId)
                        {
                            List<UserDB> dataEmployee = DAOUsers.GetUserByID365(EmployeeId, 2);
                            User userEmployee = new User();
                            if (SenderId != EmployeeId)
                            {
                                if (dataEmployee.Count > 0)
                                {
                                    userEmployee = getInforUser(dataEmployee);
                                }
                                else
                                {
                                    userEmployee = GetEmployeeInfo(EmployeeId);
                                    if (userEmployee != null)
                                    {
                                        userEmployee = InsertNewUser(userEmployee, false);
                                    }
                                }
                            }
                            List<UserDB> dataCompany = DAOUsers.GetUserByID365(CompanyId, 1);
                            User userCompany = new User();
                            if (dataCompany.Count > 0)
                            {
                                userCompany = getInforUser(dataCompany);
                            }
                            else
                            {
                                userCompany = GetCompanyInfo(CompanyId);
                                if (userCompany != null)
                                {
                                    userCompany = InsertNewUser(userCompany, false);
                                }
                            }
                            List<UserDB> dataSender = DAOUsers.GetUserByID365(SenderId, 2);
                            User userSender = new User();
                            if (SenderId != EmployeeId)
                            {
                                if (dataSender.Count > 0)
                                {
                                    userSender = getInforUser(dataSender);
                                }
                                else
                                {
                                    userSender = GetEmployeeInfo(SenderId);
                                    if (userSender != null)
                                    {
                                        userSender = InsertNewUser(userSender, false);
                                    }
                                }
                            }

                            if (userCompany != null && userCompany.NotificationOffer == 1)
                            {
                                string link = "";
                                if (String.IsNullOrEmpty(Link))
                                {
                                    link = "https://vanthu.timviec365.vn/de-xuat-gui-di.html";
                                }
                                else
                                {
                                    link = Link;
                                }
                                string notiId = $"{createAt.Ticks}_{userCompany.ID}";
                                string mess = "";
                                if (Success == 0) mess = $"{userEmployee.UserName} đã được duyệt đề xuất:\n\"{Status}\"";
                                else
                                {
                                    mess = $"Đề xuất của {userEmployee.UserName} đã bị từ chối:\n{Status}";
                                    if (!string.IsNullOrEmpty(Message)) mess += $"\nGhi chú: \"{Message}\"";
                                }
                                insertNewNotification(notiId, userCompany.ID, userCompany, "", mess, typeOffer, "", 0, createAt, link);
                            }
                            if (SenderId != EmployeeId)
                            {
                                if (userEmployee != null && userEmployee.NotificationOffer == 1)
                                {
                                    string link = "";
                                    if (String.IsNullOrEmpty(Link))
                                    {
                                        link = "https://vanthu.timviec365.vn/de-xuat-gui-di.html";
                                    }
                                    else
                                    {
                                        link = Link;
                                    }
                                    string notiId = $"{createAt.Ticks}_{userEmployee.ID}";
                                    string mess = "";
                                    if (Success == 0) mess = $"Bạn đã được duyệt đề xuất:\n\"{Status}\"";
                                    else
                                    {
                                        mess = $"Đề xuất của Bạn đã bị từ chối:\n{Status}";
                                        if (!string.IsNullOrEmpty(Message)) mess += $"\nGhi chú: \"{Message}\"";
                                    }
                                    if ((Success == 0 && userEmployee.NotificationAcceptOffer == 1) || (Success == 1 && userEmployee.NotificationDecilineOffer == 1))
                                        insertNewNotification(notiId, userEmployee.ID, userCompany, Status, mess, typeOffer, "", 0, createAt, link);
                                }
                                if (userSender != null && userSender.NotificationOffer == 1)
                                {
                                    string link = "";
                                    if (String.IsNullOrEmpty(Link))
                                    {
                                        link = "https://vanthu.timviec365.vn/de-xuat-gui-di.html";
                                    }
                                    else
                                    {
                                        link = Link;
                                    }
                                    string notiId = $"{createAt.Ticks}_{userSender.ID}";
                                    string mess = "";
                                    if (Success == 0) mess = $"Bạn đã duyệt đề xuất của {userEmployee.UserName}:\n\"{Status}\"";
                                    else
                                    {
                                        mess = $"Đề xuất của {userEmployee.UserName} đã bị từ chối:\n{Status}";
                                        if (!string.IsNullOrEmpty(Message)) mess += $"\nGhi chú: \"{Message}\"";
                                    }
                                    if ((Success == 0 && userSender.NotificationAcceptOffer == 1) || (Success == 1 && userSender.NotificationDecilineOffer == 1))
                                        insertNewNotification(notiId, userSender.ID, userCompany, "", mess, typeOffer, "", 0, createAt, link);
                                }
                            }
                            int[] listFollower = JsonConvert.DeserializeObject<int[]>(ListFollower);
                            for (int i = 0; i < listFollower.Length; i++)
                            {
                                if (listFollower[i] != SenderId && listFollower[i] != EmployeeId)
                                {
                                    List<UserDB> dataReceive = DAOUsers.GetUserByID365(listFollower[i], 2);
                                    User userReceive = new User();
                                    if (dataReceive.Count > 0)
                                    {
                                        userReceive = getInforUser(dataReceive);
                                    }
                                    else
                                    {
                                        userReceive = GetEmployeeInfo(listFollower[i]);
                                        if (userReceive != null)
                                        {
                                            userReceive = InsertNewUser(userReceive, false);
                                        }
                                    }
                                    if (userReceive != null && userReceive.NotificationOffer == 1)
                                    {
                                        string link = "";
                                        if (String.IsNullOrEmpty(Link))
                                        {
                                            link = "https://vanthu.timviec365.vn/dang-theo-doi-de-xuat.html";
                                        }
                                        else
                                        {
                                            link = Link;
                                        }
                                        string notiId = $"{createAt.Ticks}_{userReceive.ID}";
                                        string mess = "";
                                        if (Success == 0) mess = $"{userEmployee.UserName} đã được duyệt đề xuất:\n\"{Status}\"";
                                        else
                                        {
                                            mess = $"Đề xuất của {userEmployee.UserName} đã bị từ chối:\n{Status}";
                                            if (!string.IsNullOrEmpty(Message)) mess += $"\nGhi chú: \"{Message}\"";
                                        }
                                        if ((Success == 0 && userReceive.NotificationAcceptOffer == 1) || (Success == 1 && userReceive.NotificationDecilineOffer == 1))
                                            insertNewNotification(notiId, userReceive.ID, userCompany, "", mess, typeOffer, "", 0, createAt, link);
                                    }
                                }
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.result = true;
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin nhân viên";
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
        //()
        [HttpPost("Notification365")]
        [AllowAnonymous]
        public NotificationAPI Notification365()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int TypeSenderId = Convert.ToInt32(httpRequest.Form["TypeSenderId"]);
                string Title = httpRequest.Form["Title"];
                string ListComReceive = httpRequest.Form["ListComReceive"];
                string ListEpReceive = httpRequest.Form["ListEpReceive"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                if (CompanyId != 0 && SenderId != 0 && TypeSenderId != 0 && Title != null && (ListComReceive != null || ListEpReceive != null))
                {
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(SenderId, TypeSenderId);
                    User user;
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        if (TypeSenderId == 1)
                        {
                            user = GetCompanyInfo(SenderId);
                        }
                        else
                        {
                            user = GetEmployeeInfo(SenderId);
                        }
                        if (user != null)
                        {
                            user = InsertNewUser(user, false);
                        }
                    }
                    if (user != null)
                    {
                        if (user.CompanyId == CompanyId)
                        {
                            if (!String.IsNullOrWhiteSpace(ListComReceive))
                            {
                                int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListComReceive);
                                for (int i = 0; i < listReceive.Length; i++)
                                {
                                    if (listReceive[i] != SenderId)
                                    {
                                        List<UserDB> dataReceive = DAOUsers.GetUserByID365(listReceive[i], 1);
                                        User userReceive = new User();
                                        if (dataReceive.Count > 0)
                                        {
                                            userReceive = getInforUser(dataReceive);
                                        }
                                        else
                                        {
                                            userReceive = GetCompanyInfo(listReceive[i]);
                                            if (userReceive != null)
                                            {
                                                userReceive = InsertNewUser(userReceive, false);
                                            }
                                        }
                                        if (userReceive != null && userReceive.NotificationOffer == 1)
                                        {
                                            sendNewNotificationText(userReceive.ID, user.ID, Title + (String.IsNullOrEmpty(Message) ? "" : "\n" + Message) + (String.IsNullOrEmpty(Link) ? "" : "\nĐể xem bạn vui lòng truy cập tại đây:\n" + Link), String.IsNullOrEmpty(Link) ? "" : Link);
                                        }
                                    }
                                }
                            }
                            if (!String.IsNullOrWhiteSpace(ListEpReceive))
                            {
                                int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListEpReceive);
                                for (int i = 0; i < listReceive.Length; i++)
                                {
                                    if (listReceive[i] != SenderId)
                                    {
                                        List<UserDB> dataReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                                        User userReceive = new User();
                                        if (dataReceive.Count > 0)
                                        {
                                            userReceive = getInforUser(dataReceive);
                                        }
                                        else
                                        {
                                            userReceive = GetEmployeeInfo(listReceive[i]);
                                            if (userReceive != null)
                                            {
                                                userReceive = InsertNewUser(userReceive, false);
                                            }
                                        }
                                        if (userReceive != null && userReceive.NotificationOffer == 1)
                                        {
                                            sendNewNotificationText(userReceive.ID, user.ID, Title + (String.IsNullOrEmpty(Message) ? "" : "\n" + Message) + (String.IsNullOrEmpty(Link) ? "" : "\nĐể xem bạn vui lòng truy cập tại đây:\n" + Link), String.IsNullOrEmpty(Link) ? "" : Link);
                                        }
                                    }
                                }
                            }

                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin người gửi";
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

        [HttpPost("SendNewNotification")]
        [AllowAnonymous]
        public NotificationAPI SendNewNotification()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int UserId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                string Title = httpRequest.Form["Title"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                if (UserId != 0 && SenderId != 0 && !string.IsNullOrEmpty(Message))
                {
                    List<UserDB> getUser = DAOUsers.GetInforUserById(UserId);
                    if (getUser.Count > 0)
                    {
                        User u = getInforUser(getUser);
                        List<UserDB> getSender = DAOUsers.GetInforUserById(SenderId);
                        if (getSender.Count > 0)
                        {
                            User sent = getInforUser(getSender);
                            if (u.CompanyId != sent.CompanyId || u.CompanyId == 0)
                            {
                                if (DAOUsers.AddNewContact(sent.ID, u.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", sent.ID, u.ID);
                            }
                            sendNewNotificationText(u.ID, sent.ID, Message, Link);
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.result = true;
                            notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Thông tin người gửi không chính xác";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thông tin người nhận không chính xác";
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

        [HttpPost("SendNewNotification_v2")]
        [AllowAnonymous]
        public NotificationAPI SendNewNotification_v2()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int UserId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int ConversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                string Title = httpRequest.Form["Title"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                string Type = httpRequest.Form["Type"];
                if (SenderId != 0 && !string.IsNullOrEmpty(Message))
                {
                    if (!string.IsNullOrEmpty(Title))
                    {
                        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
                        Title = textInfo.ToTitleCase(Title);
                    }
                    List<UserDB> getsender = DAOUsers.GetInforUserById(SenderId);
                    if (getsender.Count > 0)
                    {
                        if (ConversationId != 0)
                        {
                            if (DAOConversation.GetConversation(ConversationId, SenderId) != null)
                            {
                                string NotiMessage = $"{Title}\n{Message}";
                                //if (Type == "newCandidate") NotiMessage = $"{Title}\n{Message}&candidateId={candidateId}";
                                SendNewNotificationX1(SenderId, ConversationId, NotiMessage, Type, Link);
                                notificationAPI.data = new DataNotification();
                                notificationAPI.data.result = true;
                                notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Cuộc trò chuyện không tồn tại";
                            }
                        }
                        else if (UserId != 0)
                        {
                            if (SenderId == UserId)
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Thông tin người nhận không hợp lệ";
                            }
                            else if (getsender.Count > 0)
                            {
                                User sender = getInforUser(getsender);
                                List<UserDB> getUser = DAOUsers.GetInforUserById(UserId);
                                if (getUser.Count > 0)
                                {
                                    User u = getInforUser(getUser);
                                    if (u.CompanyId != sender.CompanyId || u.CompanyId == 0)
                                    {
                                        if (DAOUsers.AddNewContact(sender.ID, u.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", sender.ID, u.ID);
                                    }
                                    string NotiMessage = $"{Title}\n{Message}";
                                    //if (Type == "newCandidate") NotiMessage = $"{Title}\n{Message}&candidateId={candidateId}";
                                    SendNewNotificationX(u.ID, sender.ID, NotiMessage, Type, Link);
                                    notificationAPI.data = new DataNotification();
                                    notificationAPI.data.result = true;
                                    notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
                                }
                                else
                                {
                                    notificationAPI.error = new Error();
                                    notificationAPI.error.message = "Thông tin người nhận không chính xác";
                                }
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Thông tin người gửi không chính xác";
                            }
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Thiếu thông tin truyền lên";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thiếu thông tin truyền lên";
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

        //()
        [HttpPost("NotificationPersonnelChange")]
        [AllowAnonymous]
        public NotificationAPI NotificationPersonnelChange()
        {
            NotificationAPI notificationAPI = new NotificationAPI();

            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int Status = Convert.ToInt32(httpRequest.Form["Status"]);
                int NewCompanyId = Convert.ToInt32(httpRequest.Form["NewCompanyId"]);
                string Type = httpRequest.Form["Type"];
                string ListReceive = httpRequest.Form["ListReceive"];
                string Department = httpRequest.Form["Department"];
                string NewCompanyName = httpRequest.Form["NewCompanyName"];
                string Position = httpRequest.Form["Position"];
                string CreateAt = httpRequest.Form["CreateAt"];
                if (EmployeeId != 0 && CompanyId != 0 && SenderId != 0 && ListReceive != null && Status != 0 && (Type.Equals("QuitJob") || ((Position != null && Department != null) && (Type.Equals("Appoint") || (Type.Equals("Transfer") && NewCompanyName != null && NewCompanyId != 0)))) && CreateAt != null && Type != null)
                {
                    string huhu = "";
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(SenderId, Status);
                    User user;
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        if (Status == 1)
                        {
                            user = GetCompanyInfo(SenderId);
                        }
                        else
                        {
                            user = GetEmployeeInfo(SenderId);
                        }
                        if (user != null)
                        {
                            user = InsertNewUser(user, false);
                        }
                    }

                    List<UserDB> dataEmpoyee = DAOUsers.GetUserByID365(EmployeeId, 2);
                    User Empoyee;
                    if (dataEmpoyee.Count > 0)
                    {
                        Empoyee = getInforUser(dataEmpoyee);
                    }
                    else
                    {
                        Empoyee = GetEmployeeInfo(EmployeeId);
                        if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                        {
                            Empoyee = InsertNewUser(Empoyee, false);
                        }
                    }

                    if (user != null && Empoyee != null && user.CompanyId == CompanyId && CompanyId == Empoyee.CompanyId)
                    {
                        int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
                        for (int i = 0; i < listReceive.Length; i++)
                        {
                            User userReceive = new User();
                            List<UserDB> dataReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                            if (dataReceive.Count > 0)
                            {
                                userReceive = getInforUser(dataReceive);
                            }
                            else
                            {
                                userReceive = GetEmployeeInfo(listReceive[i]);
                                if (userReceive != null && !String.IsNullOrEmpty(userReceive.Email.Trim()))
                                {
                                    userReceive = InsertNewUser(userReceive, false);
                                }
                            }
                            if (userReceive != null && userReceive.NotificationPersonnelChange == 1 && userReceive.ID != 0 && userReceive.ID != user.ID)
                            {
                                int conversationId = 0;
                                int[] users = new int[2];
                                users[0] = userReceive.ID;
                                users[1] = user.ID;
                                if (DAOConversation.checkEmptyConversation(userReceive.ID, user.ID).Count == 0)
                                {
                                    conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                                    if (conversationId > 0)
                                    {
                                        DAOConversation.insertNewParticipant(conversationId, "", 0, users, userReceive.ID, "Normal");
                                    }
                                }
                                else
                                {
                                    conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(userReceive.ID, user.ID)[0].id);
                                }
                                string messageId = DateTime.Now.Ticks + "_" + user.ID;
                                Messages message = new Messages();
                                if (Type.Equals("QuitJob"))
                                {
                                    message = new Messages(messageId, conversationId, user.ID, "text", Empoyee.UserName + " đã bị cho thôi việc khỏi công ty:\n" + Empoyee.CompanyName + "\n Áp dụng từ: " + Convert.ToDateTime(CreateAt).ToString("dd/MM/yyyy"), 0, DateTime.Now, DateTime.MinValue);
                                }
                                else if (Type.Equals("Appoint"))
                                {
                                    message = new Messages(messageId, conversationId, user.ID, "text", Empoyee.UserName + " đã được bổ nhiệm làm " + Position + "\nPhòng: " + Department + "\nCông ty: " + Empoyee.CompanyName + "\n Áp dụng từ: " + Convert.ToDateTime(CreateAt).ToString("dd/MM/yyyy"), 0, DateTime.Now, DateTime.MinValue);
                                }
                                else if (Type.Equals("Transfer"))
                                {
                                    message = new Messages(messageId, conversationId, user.ID, "text", Empoyee.UserName + " đã được chuyển công tác thành " + Position + "\nPhòng: " + Department + "\nCông ty: " + NewCompanyName + "\n Áp dụng từ: " + Convert.ToDateTime(CreateAt).ToString("dd/MM/yyyy"), 0, DateTime.Now, DateTime.MinValue);
                                }
                                int count = DAOMessages.InsertMessage(message.MessageID, message.ConversationID, message.SenderID, "text", message.Message, "", "", message.CreateAt, new InfoLinkDB(), new List<FileSendDB>(), null, 0, 0, DateTime.MinValue, 0);
                                if (count > 0)
                                {
                                    DAOConversation.MarkUnreaderMessage(conversationId, user.ID, users);
                                    WIO.EmitAsync("SendMessage", message, users);
                                }
                            }
                        }
                        if (Empoyee.ID != 0)
                        {
                            if (Type.Equals("QuitJob"))
                            {
                                DAOUsers.UserQuitJob(Empoyee.ID);
                                WIO.EmitAsync("Quitjob", Empoyee.ID, Empoyee.CompanyId);
                            }
                            else if (Type.Equals("Transfer"))
                            {
                                User newCompany = new User();
                                List<UserDB> dataNewCompany = DAOUsers.GetUserByID365(NewCompanyId, 1);
                                if (dataNewCompany.Count == 0)
                                {
                                    newCompany = GetCompanyInfo(NewCompanyId);
                                    if (newCompany != null)
                                    {
                                        InsertNewUser(newCompany, false);
                                    }
                                }
                                else
                                {
                                    newCompany = getInforUser(dataNewCompany);
                                }
                                DAOUsers.UpdateCompany(Empoyee.ID, newCompany.ID365, newCompany.UserName, Empoyee.ID365);
                                WIO.EmitAsync("Quitjob", Empoyee.ID, Empoyee.CompanyId);
                                WIO.EmitAsync("NewMemberCompany", Empoyee.ID, Empoyee.ID365, Empoyee.CompanyId);
                            }
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty hoặc sai thông tin nhân viên thay đổi " + huhu;
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

        //()
        [HttpPost("Notification_Timviec365")]
        [AllowAnonymous]
        public NotificationAPI Notification_Timviec365()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                string Ep_Email = httpRequest.Form["Ep_Email"];
                string Ep_Password = httpRequest.Form["Ep_Password"];
                string Ep_Name = httpRequest.Form["Ep_Name"];
                string Ep_Avatar = httpRequest.Form["Ep_Avatar"];
                string Link = httpRequest.Form["Link"];
                string Position = httpRequest.Form["Position"];
                if ((EmployeeId != 0 || (Ep_Email != null && Ep_Password != null && Ep_Name != null && Ep_Avatar != null)) && CompanyId != 0 && ((Type == 1) || (Type == 2 && Position != null)) && Link != null)
                {

                    string huhu = "";
                    List<UserDB> dataCompany = DAOUsers.GetUserByID365(CompanyId, 1);
                    User Company;
                    if (dataCompany.Count > 0)
                    {
                        Company = getInforUser(dataCompany);
                    }
                    else
                    {
                        Company = GetCompanyInfo(CompanyId);
                        if (Company != null)
                        {
                            Company = InsertNewUser(Company, false);
                        }
                    }
                    User Empoyee;
                    if (EmployeeId != 0)
                    {
                        List<UserDB> dataEmpoyee = DAOUsers.GetUserByID365(EmployeeId, 2);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            dataEmpoyee = new List<UserDB>();
                            dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 2);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                                if (dataEmpoyee.Count > 0)
                                {
                                    Empoyee = getInforUser(dataEmpoyee);
                                }
                                else
                                {
                                    Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                    if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                    {
                                        Empoyee = InsertNewUser(Empoyee, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<UserDB> dataEmpoyee = new List<UserDB>();
                        dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 2);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                {
                                    Empoyee = InsertNewUser(Empoyee, true);
                                }
                            }
                        }
                    }
                    if (Company != null && Empoyee != null)
                    {
                        int conversationId = 0;
                        int[] users = new int[2];
                        users[0] = Empoyee.ID;
                        users[1] = Company.ID;
                        if (DAOConversation.checkEmptyConversation(Empoyee.ID, Company.ID).Count == 0)
                        {
                            conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                            if (conversationId > 0)
                            {
                                DAOConversation.insertNewParticipant(conversationId, "", 0, users, Empoyee.ID, "Normal");
                            }
                        }
                        else
                        {
                            conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(Empoyee.ID, Company.ID)[0].id);
                        }
                        if (Empoyee.CompanyId != Company.ID365 || Empoyee.CompanyId == 0)
                        {
                            if (DAOUsers.AddNewContact(Empoyee.ID, Company.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", Empoyee.ID, Company.ID);
                        }
                        int userId = 0;
                        if (Type == 1)
                        {
                            userId = Company.ID;
                        }
                        else
                        {
                            userId = Empoyee.ID;
                        }
                        Messages message = new Messages();
                        if (Type == 1)
                        {
                            //insertNewNotification(Empoyee.ID, Company.ID, Company.UserName + " vừa xem hồ sơ của " + Empoyee.UserName + "\nĐể xem thông tin công ty bạn vui lòng truy cập tại đây:\n" + Link, Link);
                        }
                        else
                        {
                            sendNewNotificationText(Company.ID, Empoyee.ID, Empoyee.UserName + " ứng tuyển vào vị trí:\n" + Position + " của " + Company.UserName + "\nĐể xem thông tin ứng viên bạn vui lòng truy cập tại đây:\n" + Link, Link);
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công ";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty hoặc sai thông tin nhân viên thay đổi " + huhu;
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

        [HttpPost("NotificationTimviec365")]
        [AllowAnonymous]
        public NotificationAPI NotificationTimviec365()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                string Ep_Email = httpRequest.Form["Ep_Email"];
                string Ep_Password = httpRequest.Form["Ep_Password"];
                string Ep_Name = httpRequest.Form["Ep_Name"];
                string Ep_Avatar = httpRequest.Form["Ep_Avatar"];
                string Link = httpRequest.Form["Link"];
                string Position = httpRequest.Form["Position"];
                string city = httpRequest.Form["City"];
                string career = httpRequest.Form["Career"];
                if ((EmployeeId != 0 || (Ep_Email != null && Ep_Password != null && Ep_Name != null && Ep_Avatar != null)) && CompanyId != 0 && ((Type == 1) || (Type == 2 && Position != null)) && Link != null)
                {

                    string huhu = "";
                    List<UserDB> dataCompany = DAOUsers.GetUserByID365(CompanyId, 1);
                    User Company;
                    if (dataCompany.Count > 0)
                    {
                        Company = getInforUser(dataCompany);
                    }
                    else
                    {
                        Company = GetCompanyInfo(CompanyId);
                        huhu = Company.AvatarUser.Trim();
                        if (Company != null)
                        {
                            Company = InsertNewUser(Company, false);
                        }
                    }
                    User Empoyee;
                    if (EmployeeId != 0)
                    {
                        List<UserDB> dataEmpoyee = DAOUsers.GetUserByID365(EmployeeId, 2);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            dataEmpoyee = new List<UserDB>();
                            dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 2);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                                if (dataEmpoyee.Count > 0)
                                {
                                    Empoyee = getInforUser(dataEmpoyee);
                                }
                                else
                                {
                                    Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                    if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                    {
                                        Empoyee = InsertNewUser(Empoyee, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<UserDB> dataEmpoyee = new List<UserDB>();
                        dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 2);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                {
                                    Empoyee = InsertNewUser(Empoyee, true);
                                }
                            }
                        }
                    }
                    if (Company != null && Empoyee != null)
                    {
                        int conversationId = 0;
                        int[] users = new int[2];
                        users[0] = Empoyee.ID;
                        users[1] = Company.ID;
                        if (DAOConversation.checkEmptyConversation(Empoyee.ID, Company.ID).Count == 0)
                        {
                            conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                            if (conversationId > 0)
                            {
                                DAOConversation.insertNewParticipant(conversationId, "", 0, users, Empoyee.ID, "Normal");
                            }
                        }
                        else
                        {
                            conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(Empoyee.ID, Company.ID)[0].id);
                        }
                        if (Empoyee.CompanyId != Company.ID365 || Empoyee.CompanyId == 0)
                        {
                            if (DAOUsers.AddNewContact(Empoyee.ID, Company.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", Empoyee.ID, Company.ID);
                        }
                        int userId = 0;
                        if (Type == 1)
                        {
                            userId = Company.ID;
                        }
                        else
                        {
                            userId = Empoyee.ID;
                        }
                        Messages message = new Messages();
                        if (Type == 1)
                        {
                            //insertNewNotification(Empoyee.ID, Company.ID, Company.UserName + " vừa xem hồ sơ của " + Empoyee.UserName + "\nĐể xem thông tin công ty bạn vui lòng truy cập tại đây:\n" + Link, Link);
                        }
                        else
                        {
                            SendNewNotificationX(Company.ID, Empoyee.ID, "UV " + Empoyee.UserName + " đã ứng tuyển tin tuyển dụng của bạn. \nHọ và tên: " + Empoyee.UserName + "\nTỉnh thành: " + city + "\nNgành nghề: " + career, "applying", Link);
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công ";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty hoặc sai thông tin nhân viên thay đổi " + huhu;
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

        [HttpPost("NotificationTimviec365_v2")]
        [AllowAnonymous]
        public NotificationAPI NotificationTimviec365_v2()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                string Com_Email = httpRequest.Form["Com_Email"];
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                string Ep_Email = httpRequest.Form["Ep_Email"];
                string Ep_Password = httpRequest.Form["Ep_Password"];
                string Ep_Name = httpRequest.Form["Ep_Name"];
                string Ep_Avatar = httpRequest.Form["Ep_Avatar"];
                string Link = httpRequest.Form["Link"];
                string Position = httpRequest.Form["Position"];
                string city = httpRequest.Form["City"];
                string career = httpRequest.Form["Career"];
                if ((EmployeeId != 0 || !string.IsNullOrEmpty(Ep_Email) || (Ep_Email != null && Ep_Password != null && Ep_Name != null && Ep_Avatar != null)) && (CompanyId != 0 || !string.IsNullOrEmpty(Com_Email)) && ((Type == 1) || (Type == 2)) && Link != null)
                {

                    string huhu = "";
                    User Company = null;
                    if (CompanyId != 0)
                    {
                        List<UserDB> dataCompany = DAOUsers.GetUserById(CompanyId);
                        if (dataCompany.Count > 0)
                        {
                            Company = getInforUser(dataCompany);
                        }
                        else
                        {
                            Company = null;
                            //Company = GetCompanyInfo(CompanyId);
                            //huhu = Company.AvatarUser.Trim();
                            //if (Company != null)
                            //{
                            //    Company = InsertNewUser(Company, false);
                            //}
                        }
                    }
                    else if (!string.IsNullOrEmpty(Com_Email))
                    {
                        List<UserDB> dataCompany = DAOUsers.GetUsersByEmailAndType365(Com_Email, 1);
                        if (dataCompany.Count > 0)
                        {
                            Company = getInforUser(dataCompany);
                        }
                        else
                        {
                            Company = null;
                            //Company = GetCompanyInfo(CompanyId);
                            //huhu = Company.AvatarUser.Trim();
                            //if (Company != null)
                            //{
                            //    Company = InsertNewUser(Company, false);
                            //}
                        }
                    }
                    User Empoyee;
                    if (EmployeeId != 0)
                    {
                        List<UserDB> dataEmpoyee = DAOUsers.GetUserById(EmployeeId);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            dataEmpoyee = new List<UserDB>();
                            dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 2);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                                if (dataEmpoyee.Count > 0)
                                {
                                    Empoyee = getInforUser(dataEmpoyee);
                                }
                                else
                                {
                                    Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                    if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                    {
                                        Empoyee = InsertNewUser(Empoyee, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<UserDB> dataEmpoyee = new List<UserDB>();
                        dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 2);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                {
                                    Empoyee = InsertNewUser(Empoyee, true);
                                }
                            }
                        }
                    }
                    if (Company != null && Empoyee != null)
                    {
                        int conversationId = 0;
                        int[] users = new int[2];
                        users[0] = Empoyee.ID;
                        users[1] = Company.ID;
                        if (DAOConversation.checkEmptyConversation(Empoyee.ID, Company.ID).Count == 0)
                        {
                            conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                            if (conversationId > 0)
                            {
                                DAOConversation.insertNewParticipant(conversationId, "", 0, users, Empoyee.ID, "Normal");
                            }
                        }
                        else
                        {
                            conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(Empoyee.ID, Company.ID)[0].id);
                        }
                        if (Empoyee.CompanyId != Company.ID365 || Empoyee.CompanyId == 0)
                        {
                            if (DAOUsers.AddNewContact(Empoyee.ID, Company.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", Empoyee.ID, Company.ID);
                        }
                        int userId = 0;
                        if (Type == 1)
                        {
                            userId = Company.ID;
                        }
                        else
                        {
                            userId = Empoyee.ID;
                        }
                        Messages message = new Messages();
                        if (Type == 1)
                        {
                            //insertNewNotification(Empoyee.ID, Company.ID, Company.UserName + " vừa xem hồ sơ của " + Empoyee.UserName + "\nĐể xem thông tin công ty bạn vui lòng truy cập tại đây:\n" + Link, Link);
                        }
                        else
                        {
                            SendNewNotificationX(Company.ID, Empoyee.ID, "UV " + Empoyee.UserName + " đã ứng tuyển tin tuyển dụng của bạn. \nHọ và tên: " + Empoyee.UserName + "\nTỉnh thành: " + city + "\nNgành nghề: " + career, "applying", Link);
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công ";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty hoặc sai thông tin nhân viên thay đổi " + huhu;
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

        //
        [HttpPost("NotificationNewCandidate")]
        [AllowAnonymous]
        public NotificationAPI NotificationNewCandidate()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["userId"]);
                int senderId = Convert.ToInt32(httpRequest.Form["senderId"]);
                int candidateId = Convert.ToInt32(httpRequest.Form["candidateId"]);
                string Ep_Email = httpRequest.Form["Ep_Email"];
                string Ep_Password = httpRequest.Form["Ep_Password"];
                string Ep_Name = httpRequest.Form["Ep_Name"];
                string Ep_Avatar = httpRequest.Form["Ep_Avatar"];
                string Link = httpRequest.Form["Link"];
                if ((candidateId != 0 || (Ep_Email != null && Ep_Password != null && Ep_Name != null && Ep_Avatar != null)) && senderId != 0 && userId != 0)
                {

                    string huhu = "";
                    List<UserDB> datasender = DAOUsers.GetInforUserById(senderId);
                    User sender = null;
                    if (datasender.Count > 0) sender = getInforUser(datasender);
                    List<UserDB> datatuser = DAOUsers.GetInforUserById(userId);
                    User user = null;
                    if (datatuser.Count > 0) user = getInforUser(datatuser);
                    User Empoyee;
                    if (candidateId != 0)
                    {
                        List<UserDB> dataEmpoyee = DAOUsers.GetUserByID365(candidateId, 2);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            dataEmpoyee = new List<UserDB>();
                            dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 2);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                                if (dataEmpoyee.Count > 0)
                                {
                                    Empoyee = getInforUser(dataEmpoyee);
                                }
                                else
                                {
                                    Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                    if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                    {
                                        Empoyee = InsertNewUser(Empoyee, true);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<UserDB> dataEmpoyee = new List<UserDB>();
                        dataEmpoyee = DAOUsers.GetInforUserById(candidateId);
                        if (dataEmpoyee.Count > 0)
                        {
                            Empoyee = getInforUser(dataEmpoyee);
                        }
                        else
                        {
                            //dataEmpoyee = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                            if (dataEmpoyee.Count > 0)
                            {
                                Empoyee = getInforUser(dataEmpoyee);
                            }
                            else
                            {
                                Empoyee = new User(0, 0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1);
                                if (Empoyee != null && !String.IsNullOrEmpty(Empoyee.Email.Trim()))
                                {
                                    Empoyee = InsertNewUser(Empoyee, true);
                                }
                            }
                        }
                    }
                    if (sender != null && user != null)
                    {
                        int conversationId = 0;
                        int[] users = new int[2];
                        users[0] = sender.ID;
                        users[1] = user.ID;
                        if (DAOConversation.checkEmptyConversation(sender.ID, user.ID).Count == 0)
                        {
                            conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                            if (conversationId > 0)
                            {
                                DAOConversation.insertNewParticipant(conversationId, "", 0, users, sender.ID, "Normal");
                            }
                        }
                        else
                        {
                            conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(sender.ID, user.ID)[0].id);
                        }
                        if (user.CompanyId == 0 || sender.CompanyId != user.CompanyId)
                        {
                            if (DAOUsers.AddNewContact(sender.ID, user.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", sender.ID, user.ID);
                        }
                        SendNewNotificationX(user.ID, sender.ID, $"&candidateId={Empoyee.ID}", "newCandidate", Link);
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công ";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty hoặc sai thông tin nhân viên thay đổi " + huhu;
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

        //()
        [HttpPost("OnlineFromTimviec365")]
        [AllowAnonymous]
        public NotificationAPI OnlineFromTimviec365()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                int UserId = Convert.ToInt32(httpRequest.Form["UserId"]);
                int Status = Convert.ToInt32(httpRequest.Form["Status"]);
                string Ep_Email = httpRequest.Form["Ep_Email"];

                if ((Type == 1 && UserId != 0) || (Type == 2 && (UserId != 0 || Ep_Email != null)))
                {

                    User user;
                    if (String.IsNullOrEmpty(Ep_Email))
                    {
                        List<UserDB> dataUser = DAOUsers.GetUserByID365(UserId, Type);
                        if (dataUser.Count > 0)
                        {
                            user = getInforUser(dataUser);
                        }
                        else
                        {
                            if (Type == 1)
                            {
                                user = GetCompanyInfo(UserId);
                            }
                            else
                            {
                                user = GetEmployeeInfo(UserId);
                            }
                            if (user != null)
                            {
                                user = InsertNewUser(user, false);
                            }
                        }
                    }
                    else
                    {
                        List<UserDB> dataUser = DAOUsers.GetUsersByEmailAndType365(Ep_Email, 0);
                        if (dataUser.Count > 0)
                        {
                            user = getInforUser(dataUser);
                        }
                        else
                        {
                            user = null;
                            //user = new User(0, 0, 0, Ep_Email, Ep_Password, "", Ep_Name, Ep_Avatar, "", 1, DateTime.Now, 1, 1, 1, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1);
                            //if (user != null && !String.IsNullOrEmpty(user.Email.Trim()))
                            //{
                            //    user = InsertNewUser(user, true);
                            //}
                        }
                    }
                    if (user != null)
                    {
                        if (Status == 1)
                        {
                            WIO.EmitAsync("Login", user.ID);
                        }
                        else
                        {
                            WIO.EmitAsync("Logout", user.ID);
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thai đổi thông tin trang thái thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty hoặc sai thông tin nhân viên thay đổi ";
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

        //()
        [HttpPost("QuitJob")]
        [AllowAnonymous]
        public NotificationAPI QuitJob()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                string Message = httpRequest.Form["Message"];
                string ListEmployeeId = httpRequest.Form["ListEmployeeId"];
                if (ListEmployeeId != null && Message != null)
                {

                    if (Message.Equals("Hung1008@123"))
                    {
                        int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListEmployeeId);
                        for (int i = 0; i < listReceive.Length; i++)
                        {
                            List<UserDB> dataEmpoyee = DAOUsers.GetUserByID365(listReceive[i], 2);
                            if (dataEmpoyee.Count > 0)
                            {
                                User Empoyee = getInforUser(dataEmpoyee);
                                DAOUsers.UserQuitJob(Empoyee.ID);
                                WIO.EmitAsync("Quitjob", Empoyee.ID, Empoyee.CompanyId);
                            }
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Xóa thông tin người dùng thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin admin";
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

        //()
        [HttpPost("NotificationRewardDiscipline")]
        [AllowAnonymous]
        public NotificationAPI NotificationRewardDiscipline()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int Status = Convert.ToInt32(httpRequest.Form["Status"]);
                string ListReceive = httpRequest.Form["ListReceive"];
                string ListEmployee = httpRequest.Form["ListEmployee"];
                string CreateAt = httpRequest.Form["CreateAt"];
                string Type = httpRequest.Form["Type"];
                string Message = httpRequest.Form["Message"];
                if (SenderId != 0 && ListReceive != null && CompanyId != 0 && ListEmployee != null && CreateAt != null && Type != null && Message != null && Status != 0)
                {
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(SenderId, Status);
                    User user = new User();
                    if (dataUser.Count > 0)
                    {
                        user = getInforUser(dataUser);
                    }
                    else
                    {
                        if (Status == 1)
                        {
                            user = GetEmployeeInfo(SenderId);
                        }
                        else
                        {
                            user = GetCompanyInfo(SenderId);
                        }
                        if (user != null)
                        {
                            user = InsertNewUser(user, false);
                        }
                    }
                    if (user != null && user.CompanyId == CompanyId)
                    {
                        int[] listEmployee = JsonConvert.DeserializeObject<int[]>(ListEmployee);
                        for (int i = 0; i < listEmployee.Length; i++)
                        {
                            List<UserDB> dataEmployee = DAOUsers.GetUserByID365(listEmployee[i], 2);
                            User userEmployee = new User();
                            if (dataEmployee.Count > 0)
                            {
                                userEmployee = getInforUser(dataEmployee);
                            }
                            else
                            {
                                userEmployee = GetEmployeeInfo(listEmployee[i]);
                                if (userEmployee != null)
                                {
                                    userEmployee = InsertNewUser(userEmployee, false);
                                }
                            }
                            string message = "";
                            if (Type.Equals("Reward"))
                            {
                                message = " đã nhận được một khen thưởng\nNội dung khen thưởng: " + Message + "\n Áp dụng từ: " + Convert.ToDateTime(CreateAt).ToString("dd/MM/yyyy");
                            }
                            else if (Type.Equals("Discipline"))
                            {
                                message = " đã bị kỷ luật do vi phạm nội quy\nNội dung kỷ luật: " + Message + "\n Áp dụng từ: " + Convert.ToDateTime(CreateAt).ToString("dd/MM/yyyy");
                            }
                            if (userEmployee.NotificationRewardDiscipline == 1)
                            {
                                sendNewNotificationText(userEmployee.ID, user.ID, "Bạn" + message, "");
                                if (Status != 1)
                                {
                                    List<UserDB> dataCompany = DAOUsers.GetUserByID365(CompanyId, 1);
                                    User company = new User();
                                    if (dataCompany.Count > 0)
                                    {
                                        company = getInforUser(dataCompany);
                                    }
                                    else
                                    {
                                        company = GetCompanyInfo(CompanyId);
                                        if (company != null)
                                        {
                                            company = InsertNewUser(company, false);
                                        }
                                    }
                                    if (company != null)
                                    {
                                        sendNewNotificationText(company.ID, user.ID, userEmployee.UserName + message, "");
                                    }
                                }
                            }
                            int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
                            for (int j = 0; j < listReceive.Length; j++)
                            {

                                List<UserDB> dataUserReceive = DAOUsers.GetUserByID365(listReceive[j], 2);
                                User userReceive = new User();
                                if (dataUserReceive.Count > 0)
                                {
                                    userReceive = getInforUser(dataUserReceive);
                                }
                                else
                                {
                                    userReceive = GetEmployeeInfo(listReceive[j]);
                                    if (userReceive != null)
                                    {
                                        userReceive = InsertNewUser(userReceive, false);
                                    }
                                }
                                if (userReceive.NotificationRewardDiscipline == 1)
                                {
                                    sendNewNotificationText(userReceive.ID, user.ID, userEmployee.UserName + message, "");
                                }
                            }
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thông tin người khen thưởng hoặc công ty không chính xác";
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

        [HttpPost("NotificationNewPersonnel")]
        [AllowAnonymous]
        public NotificationAPI NotificationNewPersonnel()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                string ListEmployeeName = httpRequest.Form["ListEmployeeName"];
                string Position = httpRequest.Form["Position"];
                if (CompanyId != 0 && ListEmployeeName != null && Position != null)
                {

                    string[] listEmployee = JsonConvert.DeserializeObject<string[]>(ListEmployeeName);
                    string[] listPosion = JsonConvert.DeserializeObject<string[]>(Position);
                    if (listEmployee.Length > 0)
                    {
                        string mess = "";
                        for (int j = 0; j < listEmployee.Length; j++)
                        {
                            if (j == 0)
                            {
                                mess = "Danh sách những thành viên mới của công ty:\n" + DAOUsers.GetCompanyName(CompanyId);
                            }
                            mess = mess + "\n" + (j + 1) + ". " + listEmployee[j] + ", " + listPosion[j];
                            if (j == listEmployee.Length - 1)
                            {
                                mess = mess + "\nÁp dụng từ ngày: " + DateTime.Now.ToString("dd/MM/yyyy");
                            }
                        }
                        {
                            int companyId = Convert.ToInt32(DAOUsers.GetUserIdById365(CompanyId, 1).Rows[0]["id"]);
                            DataTable listIdEmployee = DAOUsers.GetListIdUserCompany(CompanyId);
                            if (listIdEmployee.Rows.Count > 0)
                            {
                                int[] listReceive = new int[listIdEmployee.Rows.Count];
                                for (int h = 0; h < listReceive.Length; h++)
                                {
                                    listReceive[h] = Convert.ToInt32(listIdEmployee.Rows[h]["id365"]);
                                }
                                for (int i = 0; i < listReceive.Length; i++)
                                {
                                    User userReceive = getInforUser(DAOUsers.GetUserByID365(listReceive[i], 2));
                                    if (userReceive.NotificationNewPersonnel == 1)
                                    {
                                        string message = mess;
                                        sendNewNotificationText(Convert.ToInt32(userReceive.ID), companyId, message, "");
                                    }
                                    notificationAPI.data = new DataNotification();
                                    notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                                }
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Công ty không tồn tại nhân viên từng đăng nhập vào chat365";
                            }
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty";
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

        //()
        [HttpPost("Notification_Allocation")]
        [AllowAnonymous]
        public NotificationAPI Notification_Allocation()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int CompanySent = Convert.ToInt32(httpRequest.Form["CompanySent"]);
                int ReceiveId = Convert.ToInt32(httpRequest.Form["ReceiveId"]);
                int DepartmentReceive = Convert.ToInt32(httpRequest.Form["DepartmentReceive"]);
                string ListAsset = httpRequest.Form["ListAsset"];
                if (EmployeeId != 0 && CompanySent != 0 && ListAsset != null && ReceiveId != 0)
                {

                    List<UserDB> dataUserSent = DAOUsers.GetUserByID365(CompanySent, 1);
                    User UserSent = new User();
                    if (dataUserSent.Count > 0)
                    {
                        UserSent = getInforUser(dataUserSent);
                        List<UserDB> dataUserReceive = DAOUsers.GetUserByID365(ReceiveId, 2);
                        User UserReceive = new User();
                        if (dataUserReceive.Count > 0)
                        {
                            UserReceive = getInforUser(dataUserReceive);
                        }
                        else
                        {
                            UserReceive = GetEmployeeInfo(ReceiveId);
                            if (UserReceive != null)
                            {
                                UserReceive = InsertNewUser(UserReceive, false);
                            }
                        }
                        if (UserReceive != null)
                        {
                            int companyId = UserSent.ID;
                            List<UserDB> dataUserEmployee = DAOUsers.GetUserByID365(EmployeeId, 2);
                            User UserEmployee = new User();
                            if (dataUserEmployee.Count > 0)
                            {
                                UserEmployee = getInforUser(dataUserEmployee);
                            }
                            else
                            {
                                UserEmployee = GetEmployeeInfo(EmployeeId);
                                if (UserEmployee != null)
                                {
                                    UserEmployee = InsertNewUser(UserEmployee, false);
                                }
                            }
                            if (UserEmployee != null)
                            {
                                if (UserEmployee.NotificationTransferAsset == 1)
                                {
                                    string mess = "";
                                    string[] listAsset = JsonConvert.DeserializeObject<string[]>(ListAsset);
                                    for (int i = 0; i < listAsset.Length; i++)
                                    {
                                        if (i == 0)
                                        {
                                            if (DepartmentReceive == 0)
                                            {
                                                mess = "Bạn được thêm vào làm người bàn giao để cấp phát tài sản công ty đến " + UserReceive.UserName + " những tài sản sau:";
                                            }
                                            else
                                            {
                                                mess = "Bạn được thêm vào làm người bàn giao để cấp phát tài sản công ty đến " + GetDepartmentInfo(DepartmentReceive, UserReceive.CompanyId).dep_name + " những tài sản sau:";
                                            }
                                        }
                                        mess = mess + "\n" + (i + 1) + ". " + listAsset[i];
                                    }
                                    string message = mess;
                                    sendNewNotificationText(Convert.ToInt32(UserEmployee.ID), companyId, message, "");
                                }
                            }
                            if (UserReceive.NotificationTransferAsset == 1)
                            {
                                string mess = "";
                                string[] listAsset = JsonConvert.DeserializeObject<string[]>(ListAsset);
                                for (int i = 0; i < listAsset.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        if (DepartmentReceive == 0)
                                        {
                                            mess = "Công ty cấp phát đến bạn những tài sản sau:";
                                        }
                                        else
                                        {
                                            mess = "Công ty cấp phát đến phòng bạn những tài sản sau:";
                                        }
                                    }
                                    mess = mess + "\n" + (i + 1) + ". " + listAsset[i];
                                }
                                string message = mess;
                                sendNewNotificationText(Convert.ToInt32(UserReceive.ID), companyId, message, "");
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Công ty chưa có nhân viên này đăng nhập vào chat365";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Công ty chưa có nhân viên nào đăng nhập vào chat365";

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
        [HttpPost("NotificationAllocation")]
        [AllowAnonymous]
        public NotificationAPI NotificationAllocation()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int CompanySent = Convert.ToInt32(httpRequest.Form["CompanySent"]);
                int ReceiveId = Convert.ToInt32(httpRequest.Form["ReceiveId"]);
                int DepartmentReceive = Convert.ToInt32(httpRequest.Form["DepartmentReceive"]);
                string ListAsset = httpRequest.Form["ListAsset"];
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                if (EmployeeId != 0 && CompanySent != 0 && ListAsset != null && ReceiveId != 0)
                {

                    List<UserDB> dataUserSent = DAOUsers.GetUserByID365(CompanySent, 1);
                    User UserSent = new User();
                    if (dataUserSent.Count > 0)
                    {
                        UserSent = getInforUser(dataUserSent);
                        List<UserDB> dataUserReceive = DAOUsers.GetUserByID365(ReceiveId, 2);
                        User UserReceive = new User();
                        if (dataUserReceive.Count > 0)
                        {
                            UserReceive = getInforUser(dataUserReceive);
                        }
                        else
                        {
                            UserReceive = GetEmployeeInfo(ReceiveId);
                            if (UserReceive != null)
                            {
                                UserReceive = InsertNewUser(UserReceive, false);
                            }
                        }
                        if (UserReceive != null)
                        {
                            int companyId = UserSent.ID;
                            List<UserDB> dataUserEmployee = DAOUsers.GetUserByID365(EmployeeId, 2);
                            User UserEmployee = new User();
                            DateTime createAt = DateTime.Now;
                            if (dataUserEmployee.Count > 0)
                            {
                                UserEmployee = getInforUser(dataUserEmployee);
                            }
                            else
                            {
                                UserEmployee = GetEmployeeInfo(EmployeeId);
                                if (UserEmployee != null)
                                {
                                    UserEmployee = InsertNewUser(UserEmployee, false);
                                }
                            }
                            if (UserEmployee != null)
                            {
                                if (UserEmployee.NotificationTransferAsset == 1)
                                {
                                    string mess = "";
                                    string asset = "";
                                    string[] listAsset = JsonConvert.DeserializeObject<string[]>(ListAsset);
                                    if (listAsset.Length > 1) asset = $"{listAsset.Length} tài sản";
                                    else if (listAsset.Length > 0) asset = $"tài sản {listAsset[0]}";
                                    if (Type == 0)
                                    {
                                        if (DepartmentReceive == 0)
                                        {
                                            mess = $"Bạn được thêm vào làm người bàn giao để cấp phát {asset} đến {UserReceive.UserName}";
                                        }
                                        else
                                        {
                                            mess = $"Bạn được thêm vào làm người bàn giao để cấp phát {asset} đến {GetDepartmentInfo(DepartmentReceive, UserReceive.CompanyId).dep_name}";
                                        }
                                    }
                                    else
                                    {
                                        if (DepartmentReceive == 0)
                                        {
                                            mess = $"Bạn được thêm vào làm người bàn giao để thu hồi {asset} đến {UserReceive.UserName}";
                                        }
                                        else
                                        {
                                            mess = $"Bạn được thêm vào làm người bàn giao để thu hồi {asset} đến {GetDepartmentInfo(DepartmentReceive, UserReceive.CompanyId).dep_name}";
                                        }
                                    }
                                    string message = mess;
                                    string notiId = $"{createAt.Ticks}_{UserEmployee.ID}";
                                    if (UserEmployee.NotificationAllocationRecall == 1)
                                        insertNewNotification(notiId, UserEmployee.ID, UserSent, "", mess, "AllocationRecall", "", 0, createAt, "");
                                }
                            }
                            if (UserReceive.NotificationTransferAsset == 1)
                            {
                                string mess = "";
                                string asset = "";
                                string[] listAsset = JsonConvert.DeserializeObject<string[]>(ListAsset);
                                if (listAsset.Length > 1) asset = $"{listAsset.Length} tài sản";
                                else if (listAsset.Length > 0) asset = $"tài sản {listAsset[0]}";

                                if (Type == 0)
                                {
                                    if (DepartmentReceive == 0)
                                    {
                                        mess = $"Bạn đã được cấp phát {asset}";
                                    }
                                    else
                                    {
                                        mess = $"Phòng bạn đã được cấp phát {asset}";
                                    }
                                }
                                else
                                {
                                    if (listAsset.Length == 1)
                                    {
                                        mess = $"Tài sản cấp phát {listAsset[0]} đã được thu hồi";
                                    }
                                    else if (listAsset.Length > 1)
                                    {
                                        mess = $"{listAsset.Length} tài sản cấp phát đã được thu hồi";
                                    }
                                }
                                string message = mess;
                                string notiId = $"{createAt.Ticks}_{UserReceive.ID}";
                                if (UserReceive.NotificationAllocationRecall == 1)
                                    insertNewNotification(notiId, UserReceive.ID, UserSent, "", mess, "AllocationRecall", "", 0, createAt, "");
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.result = true;
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Công ty chưa có nhân viên này đăng nhập vào chat365";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Công ty chưa có nhân viên nào đăng nhập vào chat365";

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
        //()
        [HttpPost("NotificationTransferAsset")]
        [AllowAnonymous]
        public NotificationAPI NotificationTransferAsset()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int ReceiveId = Convert.ToInt32(httpRequest.Form["ReceiveId"]);
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                int CompanySent = Convert.ToInt32(httpRequest.Form["CompanySent"]);
                int CompanyReceive = Convert.ToInt32(httpRequest.Form["CompanyReceive"]);
                int DepartmentSent = Convert.ToInt32(httpRequest.Form["DepartmentSent"]);
                int DepartmentReceive = Convert.ToInt32(httpRequest.Form["DepartmentReceive"]);
                string ListAsset = httpRequest.Form["ListAsset"];
                if (EmployeeId != 0 && SenderId != 0 && ReceiveId != 0 && Type != 0 && (Type == 1 || Type == 5 || (Type == 2 && DepartmentReceive != 0) || (Type == 3 && DepartmentSent != 0) || (Type == 4 && DepartmentReceive != 0 && DepartmentSent != 0)))
                {

                    List<UserDB> dataUserSent = DAOUsers.GetUserByID365(SenderId, 2);
                    User UserSent = new User();
                    if (dataUserSent.Count > 0)
                    {
                        UserSent = getInforUser(dataUserSent);
                    }
                    else
                    {
                        UserSent = GetEmployeeInfo(SenderId);
                        if (UserSent != null)
                        {
                            UserSent = InsertNewUser(UserSent, false);
                        }
                    }
                    List<UserDB> dataUserReceive = DAOUsers.GetUserByID365(ReceiveId, 2);
                    User UserReceive = new User();
                    if (dataUserReceive.Count > 0)
                    {
                        UserReceive = getInforUser(dataUserReceive);
                    }
                    else
                    {
                        UserReceive = GetEmployeeInfo(ReceiveId);
                        if (UserReceive != null)
                        {
                            UserReceive = InsertNewUser(UserReceive, false);
                        }
                    }

                    List<UserDB> dataUserEmployee = DAOUsers.GetUserByID365(EmployeeId, 2);
                    User UserEmployee = new User();
                    if (dataUserEmployee.Count > 0)
                    {
                        UserEmployee = getInforUser(dataUserEmployee);
                    }
                    else
                    {
                        UserEmployee = GetEmployeeInfo(EmployeeId);
                        if (UserEmployee != null)
                        {
                            UserEmployee = InsertNewUser(UserEmployee, false);
                        }
                    }
                    if (UserSent.CompanyId == UserEmployee.CompanyId && UserSent.CompanyId == CompanySent && UserReceive.CompanyId == CompanyReceive)
                    {
                        UserFormAPI DepReceive = new UserFormAPI();
                        UserFormAPI DepSent = new UserFormAPI();
                        if (Type == 3 || Type == 4)
                        {
                            DepSent = GetDepartmentInfo(DepartmentSent, UserSent.CompanyId);
                        }
                        if (Type == 2 || Type == 4)
                        {
                            DepReceive = GetDepartmentInfo(DepartmentReceive, UserReceive.CompanyId);
                        }
                        int companyId = Convert.ToInt32(DAOUsers.GetUserIdById365(UserSent.CompanyId, 1).Rows[0]["id"]);
                        if (UserEmployee != null)
                        {
                            if (UserEmployee.NotificationTransferAsset == 1)
                            {
                                string mess = "";
                                string[] listAsset = JsonConvert.DeserializeObject<string[]>(ListAsset);
                                for (int i = 0; i < listAsset.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        if (Type == 1)
                                        {
                                            mess = "Bạn được thêm vào làm người bàn giao để điều chuyển tài sản của " + UserSent.UserName + " đến " + UserReceive.UserName + " những tài sản sau:";
                                        }
                                        else if (Type == 2)
                                        {
                                            mess = "Bạn được thêm vào làm người bàn giao để điều chuyển tài sản của " + UserSent.UserName + " đến " + DepReceive.dep_name + " những tài sản sau:";
                                        }
                                        else if (Type == 3)
                                        {
                                            mess = "Bạn được thêm vào làm người bàn giao để điều chuyển tài sản của phòng " + DepSent.dep_name + " đến " + UserReceive.UserName + " những tài sản sau:";
                                        }
                                        else if (Type == 4)
                                        {
                                            mess = "Bạn được thêm vào làm người bàn giao để điều chuyển tài sản của phòng " + DepSent.dep_name + " đến " + DepReceive.dep_name + " những tài sản sau:";
                                        }
                                        else if (Type == 5)
                                        {
                                            mess = "Bạn được thêm vào làm người bàn giao để điều chuyển tài sản của công ty " + UserSent.CompanyName + " đến " + UserReceive.CompanyName + " những tài sản sau:";
                                        }
                                    }
                                    mess = mess + "\n" + (i + 1) + ". " + listAsset[i];
                                }
                                string message = mess;
                                sendNewNotificationText(Convert.ToInt32(UserEmployee.ID), companyId, message, "");
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        if (UserSent != null)
                        {
                            if (UserSent.NotificationTransferAsset == 1)
                            {
                                string mess = "";
                                string[] listAsset = JsonConvert.DeserializeObject<string[]>(ListAsset);
                                for (int i = 0; i < listAsset.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        if (Type == 1)
                                        {
                                            mess = "Bạn đã điều chuyển tài sản của bạn đến " + UserReceive.UserName + " những tài sản sau:";
                                        }
                                        else if (Type == 2)
                                        {
                                            mess = "Bạn đã điều chuyển tài sản của bạn đến " + DepReceive.dep_name + " những tài sản sau:";
                                        }
                                        else if (Type == 3)
                                        {
                                            mess = "Bạn đã điều chuyển tài sản của phòng bạn đến " + UserReceive.UserName + " những tài sản sau:";
                                        }
                                        else if (Type == 4)
                                        {
                                            mess = "Bạn đã điều chuyển tài sản của phòng bạn đến " + DepReceive.dep_name + " những tài sản sau:";
                                        }
                                        else if (Type == 5)
                                        {
                                            mess = "Bạn đã điều chuyển tài sản của công ty bạn đến " + UserReceive.CompanyName + " những tài sản sau:";
                                        }
                                    }
                                    mess = mess + "\n" + (i + 1) + ". " + listAsset[i];
                                }
                                string message = mess;
                                sendNewNotificationText(Convert.ToInt32(UserSent.ID), companyId, message, "");
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        if (UserReceive != null)
                        {
                            if (UserReceive.NotificationTransferAsset == 1)
                            {
                                string mess = "";
                                string[] listAsset = JsonConvert.DeserializeObject<string[]>(ListAsset);
                                for (int i = 0; i < listAsset.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        if (Type == 1)
                                        {
                                            mess = UserSent.UserName + " đã điều chuyển tài sản đến bạn những tài sản sau:";
                                        }
                                        else if (Type == 2)
                                        {
                                            mess = UserSent.UserName + "đã điều chuyển tài sản đến phòng bạn những tài sản sau:";
                                        }
                                        else if (Type == 3)
                                        {
                                            mess = DepSent.dep_name + " đã điều chuyển tài sản đến bạn những tài sản sau:";
                                        }
                                        else if (Type == 4)
                                        {
                                            mess = DepSent.dep_name + " đã điều chuyển tài sản đến phòng bạn những tài sản sau:";
                                        }
                                        else if (Type == 5)
                                        {
                                            mess = UserSent.CompanyName + "đã điều chuyển tài sản đến công ty bạn những tài sản sau:";
                                        }
                                    }
                                    mess = mess + "\n" + (i + 1) + ". " + listAsset[i];
                                }
                                string message = mess;
                                sendNewNotificationText(Convert.ToInt32(UserReceive.ID), companyId, message, "");
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thông tin công ty với nhân viên không đúng";
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

        //()
        [HttpPost("NotificationChangeProfile")]
        [AllowAnonymous]
        public NotificationAPI NotificationChangeProfile()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int EmployeeId = Convert.ToInt32(httpRequest.Form["EmployeeId"]);
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                string Type = httpRequest.Form["Type"];
                string Message = httpRequest.Form["Message"];
                if (EmployeeId != 0 && Type != null && CompanyId != 0)
                {
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(EmployeeId, 2);
                    if (dataUser.Count > 0)
                    {
                        User user = getInforUser(dataUser);

                        if (user.CompanyId == CompanyId)
                        {
                            if (user.NotificationChangeProfile == 1)
                            {
                                int companyId = Convert.ToInt32(DAOUsers.GetUserIdById365(user.CompanyId, 1).Rows[0]["id"]);
                                string mess = "";
                                string[] listType = JsonConvert.DeserializeObject<string[]>(Type);
                                string[] listMessage = JsonConvert.DeserializeObject<string[]>(Message);
                                for (int i = 0; i < listType.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        mess = "Công ty đã thay đổi thông tin của bạn trên hệ thống chuyển đổi số 365";
                                    }
                                    if (Char.IsLower(listType[i][0]))
                                    {
                                        char[] typeChar = listType[i].ToCharArray();
                                        typeChar[0] = Char.ToUpper(typeChar[0]);
                                        listType[i] = new string(typeChar);
                                    }
                                    mess = mess + "\n" + (i + 1) + ". " + listType[i];
                                    if (!String.IsNullOrWhiteSpace(listMessage[i]) || !listType[i].Equals("Ảnh đại diện"))
                                    {
                                        mess = mess + ": " + listMessage[i];
                                    }
                                }
                                string message = mess;
                                sendNewNotificationText(Convert.ToInt32(user.ID), companyId, message, "");
                            }
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin công ty";
                        }
                    }
                    else
                    {
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Nhân viên chưa đăng nhập vào chat365";
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

        //()
        [HttpPost("SendContractFile")]
        [AllowAnonymous]
        public NotificationAPI SendContractFile()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int SenderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                int Type = Convert.ToInt32(httpRequest.Form["Type"]);
                string ReceiveId = httpRequest.Form["ReceiveId"];
                string InfoFile = httpRequest.Form["InfoFile"];
                string NameFile = httpRequest.Form["NameFile"];
                if (SenderId != 0 && Type != 0 && ReceiveId != null && InfoFile != null && NameFile != null)
                {
                    List<UserDB> dataUserSent = new List<UserDB>();
                    if (Type == 1 || Type == 2 || Type == 5)
                    {
                        dataUserSent = DAOUsers.GetUserByID365(SenderId, 1);
                    }
                    else if (Type == 3 || Type == 4 || Type == 6)
                    {
                        dataUserSent = DAOUsers.GetUserByID365(SenderId, 2);
                    }
                    else if (Type == 7)
                    {
                        dataUserSent = DAOUsers.GetUserByID365(SenderId, 0);
                    }
                    User UserSent = new User();
                    if (dataUserSent.Count > 0)
                    {
                        UserSent = getInforUser(dataUserSent);

                        string[] listLinkFile = JsonConvert.DeserializeObject<string[]>(InfoFile);
                        string[] listFileName = JsonConvert.DeserializeObject<string[]>(NameFile);
                        List<int> errorUserReceive = new List<int>();
                        for (int i = 0; i < listLinkFile.Length; i++)
                        {
                            int[] listReceive = JsonConvert.DeserializeObject<int[]>(ReceiveId);
                            for (int j = 0; j < listReceive.Length; j++)
                            {
                                List<UserDB> dataUserReceive = new List<UserDB>();
                                if (Type == 1 || Type == 3)
                                {
                                    dataUserReceive = DAOUsers.GetUserByID365(listReceive[j], 2);
                                }
                                else if (Type == 2 || Type == 4)
                                {
                                    dataUserReceive = DAOUsers.GetUserByID365(listReceive[j], 1);
                                }
                                else if (Type == 5 || Type == 6 || Type == 7)
                                {
                                    dataUserReceive = DAOUsers.GetUserByID365(listReceive[j], 0);
                                }
                                User UserReceive = new User();
                                if (dataUserReceive.Count > 0)
                                {
                                    UserReceive = getInforUser(dataUserReceive);
                                    if (UserSent.CompanyId != UserReceive.CompanyId)
                                    {
                                        if (DAOUsers.AddNewContact(UserSent.ID, UserReceive.ID) == 1) WIO.EmitAsync("AcceptRequestAddFriend", UserSent.ID, UserReceive.ID);

                                    }
                                    int conversationId;
                                    int[] users = new int[2];
                                    users[0] = UserReceive.ID;
                                    users[1] = UserSent.ID;
                                    if (DAOConversation.checkEmptyConversation(UserReceive.ID, UserSent.ID).Count == 0)
                                    {
                                        conversationId = DAOConversation.insertNewConversation(0, "Normal", 0);
                                        if (conversationId > 0)
                                        {
                                            DAOConversation.insertNewParticipant(conversationId, "", 0, users, UserReceive.ID, "Normal");
                                        }
                                    }
                                    else
                                    {
                                        conversationId = Convert.ToInt32(DAOConversation.checkEmptyConversation(UserReceive.ID, UserSent.ID)[0].id);
                                    }

                                    string messageId = DateTime.Now.Ticks + "_" + UserSent.ID;
                                    Messages message = new Messages();
                                    string fileName = DateTime.Now.Ticks + "-" + listFileName[i];
                                    WebClient webClient = new WebClient();
                                    byte[] dataArr = webClient.DownloadData(listLinkFile[i]);
                                    if (dataArr.Length > 0)
                                    {
                                        int bytesize = dataArr.Length;
                                        var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");
                                        if (!Directory.Exists(filePath)) { Directory.CreateDirectory(filePath); }
                                        System.IO.File.WriteAllBytes(Path.Combine(filePath, fileName), dataArr);

                                        if (fileName.ToUpper().EndsWith(".JFIF") || fileName.ToUpper().EndsWith(".JPEG") || fileName.ToUpper().EndsWith(".JPG") || fileName.ToUpper().EndsWith(".PNG"))
                                        {
                                            //message = new Messages(messageId, conversationId, UserSent.ID, "sendPhoto", 0, "", DateTime.Now, fileName, bytesize.ToString());
                                            message = new Messages(messageId, conversationId, UserSent.ID, "sendPhoto", "", 0, DateTime.Now, DateTime.MinValue);
                                            message.ListFile = new List<InfoFile>();
                                            FileController.CompressPhoto(new MemoryStream(dataArr), Path.Combine(_environment.ContentRootPath, @"wwwroot\uploadsImageSmall") + @"\" + fileName);
                                            message.ListFile.Add(new InfoFile("sendPhoto", fileName, bytesize, 0, 0));
                                        }
                                        else
                                        {
                                            message = new Messages(messageId, conversationId, UserSent.ID, "sendFile", "", 0, DateTime.Now, DateTime.MinValue);
                                            //message = new Messages(messageId, conversationId, UserSent.ID, "sendFile", 0, "", DateTime.Now, fileName, bytesize.ToString());
                                            message.ListFile = new List<InfoFile>();
                                            message.ListFile.Add(new InfoFile("sendFile", fileName, bytesize, 0, 0));
                                        }
                                        int count = DAOMessages.InsertMessage(message.MessageID, message.ConversationID, message.SenderID, message.MessageType, message.Message, "", "", message.CreateAt, new InfoLinkDB(), new List<FileSendDB>() { new FileSendDB(bytesize, fileName, 0, 0) }, _environment, 0, 0, DateTime.MinValue, 0);
                                        if (count > 0)
                                        {
                                            DAOConversation.MarkUnreaderMessage(conversationId, UserSent.ID, users);
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
                                else if (!errorUserReceive.Contains(listReceive[j])) errorUserReceive.Add(listReceive[j]);
                            }
                        }
                        if (errorUserReceive.Count > 0)
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = $"Người dùng {JsonConvert.SerializeObject(errorUserReceive).Replace("[", "").Replace("]", "")} chưa có tài khoản chat";
                        }
                        else
                        {
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.result = true;
                            notificationAPI.data.message = "Gửi văn bản đến chat365 thành công";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Tài khoản gửi chưa đăng nhập chat365";
                    }

                }
                else
                {
                    notificationAPI.error = new Error();
                    notificationAPI.error.message = SenderId + "\n" + Type + "\n" + ReceiveId + "\n" + InfoFile + "\n" + NameFile;
                }
            }
            catch (Exception ex)
            {
                notificationAPI.error = new Error();
                notificationAPI.error.message = ex.ToString();
            }
            return notificationAPI;
        }

        //()
        [HttpPost("SendNotificationTimekeeping")]
        [AllowAnonymous]
        public NotificationAPI SendNotificationTimekeeping()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                string Message = httpRequest.Form["Message"];
                string Title = httpRequest.Form["Title"];
                string ListEmployeeId = httpRequest.Form["ListEmployeeId"];
                if (ListEmployeeId != null && Title != null && Message != null)
                {
                    int[] listEmployee = JsonConvert.DeserializeObject<int[]>(ListEmployeeId);

                    for (int i = 0; i < listEmployee.Length; i++)
                    {
                        List<UserDB> dataUser = DAOUsers.GetUserByID365(listEmployee[i], 2);
                        if (dataUser.Count > 0)
                        {
                            User user = getInforUser(dataUser);
                            List<UserDB> dataCompany = new List<UserDB>();
                            dataCompany = DAOUsers.GetUserByID365(user.CompanyId, 1);
                            if (dataCompany.Count > 0)
                            {
                                DateTime CreateAt = DateTime.Now;
                                string notificationId = DateTime.Now.Ticks + "_" + user.ID;
                                User company = getInforUser(dataCompany);
                                if (DAONotification.CheckSpamNotification(user.ID, company.ID, Title, Message, CreateAt))
                                {
                                    if (String.IsNullOrWhiteSpace(company.AvatarUser.Trim()))
                                    {
                                        string letter = RemoveUnicode(company.UserName.Substring(0, 1).ToLower()).ToUpper();
                                        try
                                        {
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                company.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                            }
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                company.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                            }
                                            if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                            {
                                                company.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                            }
                                            else
                                            {
                                                company.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                            }
                                        }
                                        catch
                                        {
                                            company.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    else
                                    {
                                        company.LinkAvatar = "https://mess.timviec365.vn/avatarUser/" + company.ID + "/" + company.AvatarUser;
                                        company.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + company.ID + "/" + company.AvatarUser;
                                    }
                                    int count = DAONotification.InsertNotification(notificationId, user.ID, company.ID, Title, Message, "Timekeeing", "", 0, CreateAt, "");
                                    if (count > 0)
                                    {
                                        WIO.EmitAsync("SendNotification", user.ID, new Notifications(notificationId, user.ID, company, Title, Message, 1, "Timekeeing", "", 0, CreateAt, ""));
                                    }
                                }
                            }
                        }
                    }
                    notificationAPI.data = new DataNotification();
                    notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
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

        [HttpPost("SendNotificationComment")]
        [AllowAnonymous]
        public NotificationAPI SendNotificationComment()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                string Title = httpRequest.Form["Title"];
                int userID = Convert.ToInt32(httpRequest.Form["UserId"]);
                int senderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                string link = httpRequest.Form["Link"];
                if (userID != 0 && senderId != 0 && !string.IsNullOrEmpty(link) && !string.IsNullOrEmpty(Title))
                {
                    var getUsers = DAOUsers.GetUserById(userID);
                    if (getUsers.Count > 0)
                    {
                        var user = getInforUser(getUsers);
                        List<UserDB> getSender = DAOUsers.GetUserById(senderId);
                        if (getSender.Count > 0)
                        {
                            var sender = getInforUser(getSender);
                            DateTime createAt = DateTime.Now;
                            string notiId = $"{createAt.Ticks}_{userID}";
                            string message = $"{sender.UserName} đã nhắc đến bạn trong một bình luận";
                            if (DAONotification.InsertNotification(notiId, user.ID, sender.ID, Title, message, "CommentTag", "", 0, createAt, link) > 0)
                            {
                                WIO.EmitAsync("SendNotification", user.ID, new Notifications(notiId, user.ID, sender, Title, message, 1, "CommentTag", "", 0, createAt, link));
                                notificationAPI.data = new DataNotification();
                                notificationAPI.data.result = true;
                                notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Gửi thông báo thất bại";
                            }
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin người gửi";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin user";
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

        [HttpPost("SendNotification")]
        [AllowAnonymous]
        public NotificationAPI SendNotification()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                string Title = httpRequest.Form["Title"];
                string Message = httpRequest.Form["Message"];
                string type = httpRequest.Form["Type"];
                string messageId = httpRequest.Form["MessageId"];
                int conversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                int userID = Convert.ToInt32(httpRequest.Form["UserId"]);
                int senderId = Convert.ToInt32(httpRequest.Form["SenderId"]);
                string link = httpRequest.Form["Link"];
                if (senderId == userID)
                {
                    notificationAPI.error = new Error();
                    notificationAPI.error.message = "Thông tin người nhận không hợp lệ";
                }
                else if (userID != 0 && !string.IsNullOrEmpty(type) && type.ToLower() == "SendCandidate".ToLower())
                {
                    var getUsers = DAOUsers.GetUserById(userID);
                    if (getUsers.Count > 0)
                    {
                        var user = getInforUser(getUsers);
                        if (user.NotificationSendCandidate == 1)
                        {
                            if (senderId == 0)
                            {
                                List<UserDB> getSender = DAOUsers.GetUserById(58384);
                                if (getSender.Count > 0)
                                {
                                    var sender = getInforUser(getSender);
                                    DateTime createAt = DateTime.Now;
                                    string notiId = $"{createAt.Ticks}_{userID}";
                                    if (DAONotification.InsertNotification(notiId, user.ID, sender.ID, Title, Message, type, messageId, conversationId, createAt, link) > 0)
                                    {
                                        WIO.EmitAsync("SendNotification", user.ID, new Notifications(notiId, user.ID, sender, Title, Message, 1, type, messageId, conversationId, createAt, link));
                                        notificationAPI.data = new DataNotification();
                                        notificationAPI.data.result = true;
                                        notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
                                    }
                                    else
                                    {
                                        notificationAPI.error = new Error();
                                        notificationAPI.error.message = "Gửi thông báo thất bại";
                                    }
                                }
                                else
                                {
                                    notificationAPI.error = new Error();
                                    notificationAPI.error.message = "Sai thông tin người gửi";
                                }
                            }
                            else
                            {
                                List<UserDB> getSender = DAOUsers.GetUserByID365(senderId, 2);
                                if (getSender.Count > 0)
                                {
                                    var sender = getInforUser(getSender);
                                    DateTime createAt = DateTime.Now;
                                    string notiId = $"{createAt.Ticks}_{userID}";
                                    if (DAONotification.InsertNotification(notiId, user.ID, sender.ID, Title, Message, type, messageId, conversationId, createAt, link) > 0)
                                    {
                                        WIO.EmitAsync("SendNotification", user.ID, new Notifications(notiId, user.ID, sender, Title, Message, 1, type, messageId, conversationId, createAt, link));
                                        notificationAPI.data = new DataNotification();
                                        notificationAPI.data.result = true;
                                        notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
                                    }
                                    else
                                    {
                                        notificationAPI.error = new Error();
                                        notificationAPI.error.message = "Gửi thông báo thất bại";
                                    }
                                }
                                else
                                {
                                    notificationAPI.error = new Error();
                                    notificationAPI.error.message = "Sai thông tin người gửi";
                                }
                            }
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "User đã tắt thông báo này";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin user";
                    }
                }
                else if (userID != 0 && senderId != 0 && !string.IsNullOrEmpty(type))
                {

                    var getUsers = DAOUsers.GetUserById(userID);
                    if (getUsers.Count > 0)
                    {
                        var user = getInforUser(getUsers);
                        List<UserDB> getSender = DAOUsers.GetUserById(senderId);
                        if (getSender.Count > 0)
                        {
                            var sender = getInforUser(getSender);
                            bool allow = true;
                            if (type == "NTD")
                            {
                                if (!string.IsNullOrEmpty(Title) && Title.ToLower().Contains("hết điểm") && user.NotificationNTDPoint == 0) allow = false;
                                if (!string.IsNullOrEmpty(Title) && Title.ToLower().Contains("hết hạn ghim tin") && user.NotificationNTDExpiredPin == 0) allow = false;
                                if (!string.IsNullOrEmpty(Title) && Title.ToLower().Contains("hết hạn nộp hồ sơ") && user.NotificationNTDExpiredRecruit == 0) allow = false;
                                if (!string.IsNullOrEmpty(Title) && Title.ToLower().Contains("bình luận") && user.NotificationCommentFromTimViec == 0) allow = false;
                            }
                            if (allow)
                            {
                                DateTime createAt = DateTime.Now;
                                string notiId = $"{createAt.Ticks}_{userID}";
                                if (DAONotification.InsertNotification(notiId, user.ID, sender.ID, Title, Message, type, messageId, conversationId, createAt, link) > 0)
                                {
                                    WIO.EmitAsync("SendNotification", user.ID, new Notifications(notiId, user.ID, sender, Title, Message, 1, type, messageId, conversationId, createAt, link));
                                    notificationAPI.data = new DataNotification();
                                    notificationAPI.data.result = true;
                                    notificationAPI.data.message = "Gửi thông báo đến chat365 thành công";
                                }
                                else
                                {
                                    notificationAPI.error = new Error();
                                    notificationAPI.error.message = "Gửi thông báo thất bại";
                                }
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Người nhận đã tắt thông báo";
                            }
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Sai thông tin người gửi";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin user";
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

        private void RunSendListNotification(List<NotificationNTD> getData)
        {
            foreach (NotificationNTD noti in getData)
            {
                var getUsers = DAOUsers.GetUserById(noti.UserId);
                if (getUsers.Count > 0)
                {
                    var user = getInforUser(getUsers);
                    List<UserDB> getSender = DAOUsers.GetUserById(noti.SenderId);
                    if (getSender.Count > 0)
                    {
                        var sender = getInforUser(getSender);
                        DateTime createAt = DateTime.Now;
                        string notiId = $"{createAt.Ticks}_{noti.UserId}";
                        if (DAONotification.InsertNotification(notiId, user.ID, sender.ID, noti.Title, noti.Message, noti.Type, "", 0, createAt, noti.Link) > 0)
                        {
                            WIO.EmitAsync("SendNotification", user.ID, new Notifications(notiId, user.ID, sender, noti.Title, noti.Message, 1, noti.Type, "", 0, createAt, noti.Link));
                        }
                    }
                }
            }

        }

        [HttpPost("SendListNotification")]
        [AllowAnonymous]
        public NotificationAPI SendListNotification()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                string data = httpRequest.Form["Data"];
                if (!string.IsNullOrEmpty(data))
                {
                    List<NotificationNTD> getData = JsonConvert.DeserializeObject<List<NotificationNTD>>(data);
                    if (getData.Count > 0)
                    {
                        Task task = new Task(() =>
                        {
                            RunSendListNotification(getData);
                        });
                        task.ContinueWith((t) =>
                        {
                            task.Dispose();
                        });
                        task.Start();
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.result = true;
                        notificationAPI.data.message = "gửi tin nhắn đến chat365 thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thiếu thông tin truyền lên";
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

        [HttpPost("InsertNewNotification")]
        [AllowAnonymous]
        public NotificationAPI InsertNewNotification()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["userId"]);
                int ConversationId = Convert.ToInt32(httpRequest.Form["ConversationId"]);
                int paticipantId = Convert.ToInt32(httpRequest.Form["paticipantId"]);
                string Title = httpRequest.Form["Title"];
                string Type = httpRequest.Form["Type"];
                string MessageId = httpRequest.Form["MessageId"];
                string NotificationId = httpRequest.Form["NotificationId"];
                string Message = httpRequest.Form["Message"];
                string Link = httpRequest.Form["Link"];
                if (userId != 0 && Title != null && Message != null && Type != null && MessageId != null && ConversationId != 0 && NotificationId != null && paticipantId != 0)
                {
                    if (DAONotification.InsertNotification(NotificationId, userId, paticipantId, Title, Message, Type, MessageId, ConversationId, DateTime.Now, Link) > 0)
                    {
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Gửi thông báo đến thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Có lỗi xảy ra khi thêm thông báo";
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

        [HttpPost("NotificationCRM")]
        [AllowAnonymous]
        public NotificationAPI NotificationCRM()
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                var httpRequest = HttpContext.Request;
                int CompanyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                string Type = httpRequest.Form["Type"];
                string ListReceive = httpRequest.Form["ListReceive"];
                string Content = httpRequest.Form["Content"];
                string CustomerId = httpRequest.Form["CustomerId"];
                string CustomerName = httpRequest.Form["CustomerName"];
                string GroupName = httpRequest.Form["GroupName"];
                string Link = httpRequest.Form["Link"];
                string Phone = httpRequest.Form["Phone"];
                if (CompanyId != 0 && ListReceive != null && CustomerId != null && Content != null && CustomerName != null && GroupName != null && Link != null && Type != null && Phone != null)
                {
                    DataTable dataCompany = DAOUsers.GetUserIdById365(CompanyId, 1);
                    if (dataCompany.Rows.Count > 0)
                    {
                        int companyId = Convert.ToInt32(dataCompany.Rows[0]["id"]);
                        int[] listReceive = JsonConvert.DeserializeObject<int[]>(ListReceive);
                        int[] listCustomerId = JsonConvert.DeserializeObject<int[]>(CustomerId);
                        string[] listContent = Content.Split(",");
                        string[] listCustomerName = CustomerName.Split(",");
                        string[] listGroupName = GroupName.Split(",");
                        string[] listLink = Link.Split(",");
                        int[] listType = JsonConvert.DeserializeObject<int[]>(Type);
                        string[] listPhone = Phone.Split(",");
                        for (int i = 0; i < listReceive.Length; i++)
                        {
                            List<UserDB> dataUserReceive = DAOUsers.GetUserByID365(listReceive[i], 2);
                            if (dataUserReceive.Count > 0 && (Convert.ToInt32(dataUserReceive[0].companyId) == CompanyId))
                            {
                                string mess = listContent[i] + "\nVui lòng truy cập tại link:\n" + listLink[i];
                                sendNewNotificationText(Convert.ToInt32(dataUserReceive[0].id), companyId, mess, listLink[i]);
                            }
                            else
                            {
                                notificationAPI.error = new Error();
                                notificationAPI.error.message = "Tồn tại thông tin nhân viên không chính xác";
                            }
                            WIO.EmitAsync("CRMNotification", listReceive[i], listContent[i], listCustomerName[i], listCustomerId[i], listGroupName[i], listLink[i], listType[i], listPhone[i]);
                        }
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Thông báo đến tài khoản Chat365 thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Sai thông tin công ty";
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

        [HttpGet("GetListNotification/userId={userId}")]
        [AllowAnonymous]
        public NotificationAPI GetListNotification(int userId)
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                if (userId != 0)
                {
                    List<NotificationsDB> dataListNotification = DAONotification.GetListNotification(userId);
                    if (dataListNotification.Count > 0)
                    {
                        List<Notifications> listNotification = new List<Notifications>();
                        foreach (NotificationsDB row in dataListNotification)
                        {
                            User userInfo = new User();
                            List<UserDB> getUser = DAOUsers.GetUserSendMessById(row.paticipantId);
                            if (getUser.Count != 0)
                            {
                                userInfo.ID = getUser[0].id;
                                userInfo.UserName = getUser[0].userName;
                                userInfo.AvatarUser = getUser[0].avatarUser;
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
                                listNotification.Add(new Notifications(row.id, row.userId, userInfo, row.title, row.message, row.isUndeader, row.type, row.messageId, row.conversationId, row.createAt.ToLocalTime(), row.link));
                            }
                        }
                        if (listNotification.Count > 0)
                        {
                            notificationAPI.data = new DataNotification();
                            notificationAPI.data.listNotification = listNotification;
                            notificationAPI.data.message = "lấy danh sách thông báo thành công";
                        }
                        else
                        {
                            notificationAPI.error = new Error();
                            notificationAPI.error.message = "Người dùng không tồn tại thông báo nào";
                        }
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Người dùng không tồn tại thông báo nào";
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

        [HttpGet("DeleteAllNotification/userId={userId}")]
        [AllowAnonymous]
        public NotificationAPI DeleteAllNotification(int userId)
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                if (userId != 0)
                {
                    DAONotification.DeleteAllNotification(userId);
                    notificationAPI.data = new DataNotification();
                    notificationAPI.data.message = "xóa thông báo thành công";
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

        [HttpGet("DeleteNotification/IDNotification={IDNotification}")]
        [AllowAnonymous]
        public NotificationAPI DeleteNotification(string IDNotification)
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                if (IDNotification != null)
                {
                    int count = DAONotification.DeleteNotification(IDNotification);
                    if (count > 0)
                    {
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Xóa thông báo thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thông báo không còn tồn tại";
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

        [HttpGet("ReadNotification/IDNotification={IDNotification}")]
        [AllowAnonymous]
        public NotificationAPI ReadNotification(string IDNotification)
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                if (IDNotification != null)
                {
                    int count = DAONotification.ReadNotification(IDNotification);
                    if (count > 0)
                    {
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Đọc thông báo thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thông báo không còn tồn tại";
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

        [HttpGet("ReadAllNotification/userId={userId}")]
        [AllowAnonymous]
        public NotificationAPI ReadAllNotification(int userId)
        {
            NotificationAPI notificationAPI = new NotificationAPI();
            try
            {
                if (userId != 0)
                {
                    int count = DAONotification.ReadAllNotification(userId);
                    if (count > 0)
                    {
                        notificationAPI.data = new DataNotification();
                        notificationAPI.data.message = "Đọc thông báo thành công";
                    }
                    else
                    {
                        notificationAPI.error = new Error();
                        notificationAPI.error.message = "Thông báo không còn tồn tại";
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
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Chat365.Server.Model.Entity;
using Chat365.Server.Model.DAO;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Chat365.Server.Model.EntityAPI;
using SocketIOClient;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing;
using APIChat365.Model.MongoEntity;
using APIChat365.Model.Entity;
using MongoDB.Bson.Serialization.Serializers;
using DateTime = System.DateTime;
using APIChat365.Model.DAO;
using System.ServiceModel.Channels;

namespace Chat365.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
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
        public UserController(ILogger<UserController> logger, IWebHostEnvironment environment)
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
        //ok
        [HttpPost("GetAppWasOpen")]
        [AllowAnonymous]
        public APIOTP GetAppWasOpen([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                WIO.EmitAsync("GetAppWasOpen", user.ID, user.UserName);
                APIotp.data = new DataOTP();
                APIotp.data.result = true;
                APIotp.data.message = "Check App Winform mở thành công";
            }
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }
        //get username and password when phone scan qr
        [HttpPost("getInforQRLogin")]
        [AllowAnonymous]
        public APIOTP getInforQRLogin()
        {
            APIOTP APIotp = new APIOTP();
            var http = HttpContext.Request;
            string IdQr = http.Form["QrId"];
            string Email = http.Form["Email"];
            string Password = http.Form["Password"];
            string IdDevice = http.Form["IdDevice"];
            string NameDevice = http.Form["NameDevice"];
            try
            {
                if (!string.IsNullOrEmpty(IdQr) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
                {
                    WIO.EmitAsync("QRLogin", IdQr, Email, Password);
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "Quét mã thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "Thiếu Thông tin truyền lên";
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

        [HttpGet("GetLastLersion")]
        [AllowAnonymous]
        public string GetLastLersion()
        {
            return DAOUsers.GetLastVersion("Chat365").Rows[0]["lastVersion"].ToString();
        }

        [HttpGet("GetLastLersionHR")]
        [AllowAnonymous]
        public string GetLastLersionHR()
        {
            return DAOUsers.GetLastVersion("HR").Rows[0]["lastVersion"].ToString();
        }

        [HttpGet("GetLastLersionGetLastLersionQuanLyTaiSan")]
        [AllowAnonymous]
        public string GetLastLersionGetLastLersionQuanLyTaiSan()
        {
            return DAOUsers.GetLastVersion("QuanLyTaiSan").Rows[0]["lastVersion"].ToString();
        }

        [HttpGet("GetLastLersionTimViec365")]
        [AllowAnonymous]
        public string GetLastLersionAppTimViec365()
        {
            return DAOUsers.GetLastVersion("TimViec365").Rows[0]["lastVersion"].ToString();
        }

        public User getInforUser(DataTable getUser)
        {
            User user = new User(Convert.ToInt32(getUser.Rows[0]["id"]),
                        Convert.ToInt32(getUser.Rows[0]["id365"]),
                        Convert.ToInt32(getUser.Rows[0]["idTimViec"]),
                        Convert.ToInt32(getUser.Rows[0]["type365"]),
                         getUser.Rows[0]["email"].ToString().Trim(),
                      getUser.Rows[0]["password"].ToString().Trim(),
                         getUser.Rows[0]["phone"].ToString().Trim(),
                         getUser.Rows[0]["userName"].ToString().Trim(),
                          getUser.Rows[0]["avatarUser"].ToString().Trim(),
                           getUser.Rows[0]["status"].ToString().Trim(),
                        Convert.ToInt32(getUser.Rows[0]["statusEmotion"]),
                                               Convert.ToDateTime(getUser.Rows[0]["lastActive"]),
 Convert.ToInt32(getUser.Rows[0]["active"]),
                        Convert.ToInt32(getUser.Rows[0]["isOnline"]),
                        Convert.ToInt32(getUser.Rows[0]["looker"]),
                         Convert.ToInt32(getUser.Rows[0]["companyId"]),
                         getUser.Rows[0]["companyName"].ToString(),
                              Convert.ToInt32(getUser.Rows[0]["NotificationPayoff"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationCalendar"]),
                          Convert.ToInt32(getUser.Rows[0]["NotificationReport"]),
                          Convert.ToInt32(getUser.Rows[0]["NotificationOffer"]),
                          Convert.ToInt32(getUser.Rows[0]["NotificationPersonnelChange"]),
                          Convert.ToInt32(getUser.Rows[0]["NotificationRewardDiscipline"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationNewPersonnel"]),
                          Convert.ToInt32(getUser.Rows[0]["NotificationChangeProfile"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationTransferAsset"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationMissMessage"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationCommentFromTimViec"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationCommentFromRaoNhanh"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationTag"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationSendCandidate"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationChangeSalary"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationAllocationRecall"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationAcceptOffer"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationDecilineOffer"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationNTDPoint"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationNTDExpiredPin"]),
                         Convert.ToInt32(getUser.Rows[0]["NotificationNTDExpiredRecruit"]),
                         getUser.Rows[0]["fromWeb"].ToString(),
                         Convert.ToInt32(getUser.Rows[0]["NotificationNTDApplying"]));

            if (String.IsNullOrWhiteSpace(user.AvatarUser.Trim()))
            {
                string letter = RemoveUnicode(user.UserName.Substring(0, 1).ToLower()).ToUpper();
                try
                {
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                    }
                    else
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                    }
                }
                catch (Exception ex)
                {
                    user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                }
            }
            else
            {
                user.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + user.ID + "/" + user.AvatarUser;
                user.LinkAvatar = user.AvatarUser;
            }

            return user;
        }

        public User getInforUser(List<UserDB> getUser)
        {
            User user = new User(getUser[0].id, getUser[0].id365, getUser[0].idTimViec, getUser[0].type365, getUser[0].email, getUser[0].password, getUser[0].phone, getUser[0].userName, getUser[0].avatarUser, getUser[0].status, getUser[0].statusEmotion, getUser[0].lastActive.ToLocalTime(), getUser[0].active, getUser[0].isOnline, getUser[0].looker, getUser[0].companyId, getUser[0].companyName, getUser[0].notificationPayoff, getUser[0].notificationCalendar, getUser[0].notificationReport, getUser[0].notificationOffer, getUser[0].notificationPersonnelChange, getUser[0].notificationRewardDiscipline, getUser[0].notificationNewPersonnel, getUser[0].notificationTransferAsset, getUser[0].notificationChangeProfile, getUser[0].notificationMissMessage, getUser[0].notificationCommentFromTimViec, getUser[0].notificationCommentFromRaoNhanh, getUser[0].notificationTag, getUser[0].notificationSendCandidate, getUser[0].notificationChangeSalary, getUser[0].notificationAllocationRecall, getUser[0].notificationAcceptOffer, getUser[0].notificationDecilineOffer, getUser[0].notificationNTDPoint, getUser[0].notificationNTDExpiredPin, getUser[0].notificationNTDExpiredRecruit, getUser[0].fromWeb, getUser[0].notificationNTDApplying);

            if (String.IsNullOrWhiteSpace(user.AvatarUser.Trim()))
            {
                string letter = RemoveUnicode(user.UserName.Substring(0, 1).ToLower()).ToUpper();
                try
                {
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                    }
                    if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                    }
                    else
                    {
                        user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                    }
                }
                catch (Exception ex)
                {
                    user.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                }
            }
            else
            {
                user.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + user.ID + "/" + user.AvatarUser;
                user.LinkAvatar = user.AvatarUser;
            }

            return user;
        }
        public string ToMD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in bHash)
            {
                sbHash.Append(String.Format("{0:x2}", b));
            }
            return sbHash.ToString();
        }
        private User InsertNewUser(User user, bool isFullLink, string fromWeb)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] dataArr = new byte[1];
                    long bytesize;
                    if (!String.IsNullOrWhiteSpace(user.AvatarUser) && !(user.AvatarUser.Trim().Length == 0))
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
                    if (true)
                    {
                        user.IDTimViec = getIdTimViec(user.Email, user.Type365.ToString());
                    }
                    if (DAOUsers.checkMailEmpty365(user.Email, 0).Count > 0)
                    {
                        user.ID = Convert.ToInt32(DAOUsers.checkMailEmpty365(user.Email, 0)[0].id);
                        var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                        string fileName = "";
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
                            fileName = DateTime.Now.Ticks + "_" + user.ID + ".jpg";
                            try
                            {
                                FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + user.ID + @"\" + fileName);
                            }
                            catch (Exception ex)
                            {
                                System.IO.File.WriteAllBytes(filePath + @"\" + user.ID + @"\" + fileName, dataArr);
                            }
                        }
                        DAOUsers.UpdateInfoUser(user.ID, user.ID365, user.Type365, user.UserName, fileName, user.Password, user.CompanyId, user.CompanyName, user.IDTimViec);
                    }
                    else
                    {
                        user.ID = DAOUsers.InsertNewUser(user.UserName, user.ID365, user.IDTimViec, user.Type365, user.Email, user.Password, user.CompanyId, user.CompanyName, fromWeb);
                        if (user.ID > 0 && dataArr.Length > 1)
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
                            string fileName = DateTime.Now.Ticks + "_" + user.ID + ".jpg";
                            try
                            {
                                FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + user.ID + @"\" + fileName);
                            }
                            catch (Exception ex)
                            {
                                System.IO.File.WriteAllBytes(filePath + @"\" + user.ID + @"\" + fileName, dataArr);
                            }
                            DAOUsers.UpdateAvatarUser(fileName, user.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return getInforUser(DAOUsers.GetUsersById(user.ID));
        }
        private User InsertNewUser1(User user, bool isFullLink, string fromWeb)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] dataArr = new byte[1];
                    long bytesize;
                    if (!String.IsNullOrWhiteSpace(user.AvatarUser) && !(user.AvatarUser.Trim().Length == 0))
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
                    if (true)
                    {
                        user.IDTimViec = getIdTimViec(user.Email, user.Type365.ToString());
                    }
                    if (DAOUsers.checkIdMailAndType365(user.Email, user.ID, 0).Count > 0)
                    {
                        user.ID = Convert.ToInt32(DAOUsers.checkIdMailAndType365(user.Email, user.ID, 0)[0].id);
                        var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                        string fileName = "";
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
                            fileName = DateTime.Now.Ticks + "_" + user.ID + ".jpg";
                            try
                            {
                                FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + user.ID + @"\" + fileName);
                            }
                            catch (Exception ex)
                            {
                                System.IO.File.WriteAllBytes(filePath + @"\" + user.ID + @"\" + fileName, dataArr);
                            }
                        }
                        DAOUsers.UpdateInfoUser(user.ID, user.ID365, user.Type365, user.UserName, fileName, user.Password, user.CompanyId, user.CompanyName, user.IDTimViec);
                    }
                    else
                    {
                        user.ID = DAOUsers.InsertNewUser(user.UserName, user.ID365, user.IDTimViec, user.Type365, user.Email, user.Password, user.CompanyId, user.CompanyName, fromWeb);
                        if (user.ID > 0 && dataArr.Length > 1)
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
                            string fileName = DateTime.Now.Ticks + "_" + user.ID + ".jpg";
                            try
                            {
                                FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + user.ID + @"\" + fileName);
                            }
                            catch (Exception ex)
                            {
                                System.IO.File.WriteAllBytes(filePath + @"\" + user.ID + @"\" + fileName, dataArr);
                            }
                            DAOUsers.UpdateAvatarUser(fileName, user.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //LogError(ex.ToString());
                return null;
            }
            return getInforUser(DAOUsers.GetUsersById(user.ID));
        }
        //(4/8/22)ok
        [HttpPost("GetIdChat365")]
        [AllowAnonymous]
        public APIOTP GetIdChat365([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            if (user.Email != null)
            {
                try
                {
                    List<UserDB> dataTable = DAOUsers.checkMailEmpty365(user.Email, user.Type365);
                    if (dataTable.Count > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = dataTable[0].id.ToString();
                    }
                    else
                    {
                        dataTable = DAOUsers.checkMaiAndIdlEmpty365(user.Email, 0, user.ID365);
                        if (dataTable.Count > 0)
                        {
                            APIotp.data = new DataOTP();
                            APIotp.data.result = true;
                            APIotp.data.message = dataTable[0].id.ToString();
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = "Tài khoản không tồn tại";
                        }
                    }
                }
                catch (Exception ex)
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = ex.Message;
                }
            }
            else
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = "Thiếu thông tin tài khoản";
            }
            return APIotp;
        }

        //(4/8/22)ok
        [HttpPost("GetAcceptMessStranger")]
        [AllowAnonymous]
        public APIOTP GetAcceptMessStranger([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            if (user.ID != 0)
            {
                try
                {
                    List<UserDB> dataTable = DAOUsers.getAcceptMessStranger(user.ID);
                    if (dataTable.Count > 0 && dataTable[0].acceptMessStranger == 1)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Tài khoản không tồn tại hoặc đang tắt chức năng nhận tin nhắn từ người lạ";
                    }
                }
                catch (Exception ex)
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = ex.Message;
                }
            }
            else
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = "Thiếu thông tin tài khoản";
            }
            return APIotp;
        }

        //()ok
        [HttpPost("AccountFromTimViec365")]
        [AllowAnonymous]
        public APIOTP AccountFromTimViec365([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            if (user.Email != null && user.Password != null && user.UserName != null && (user.IDTimViec != 0 || user.ID365 != 0))
            {
                try
                {
                    if (DAOUsers.checkMailEmpty365(user.Email, user.Type365).Count == 0)
                    {
                        user = InsertNewUser(new User(0, user.ID365, user.IDTimViec, user.Type365 == 1 ? 1 : 0, user.Email, user.Password, "", user.UserName, user.AvatarUser, "", 1, DateTime.Now, 1, 1, 1, user.Type365 == 1 ? user.ID365 : 0, user.Type365 == 1 ? "" : user.UserName, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1), true, "timviec365");
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Thêm tài khoản thành công";
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Tài khoản đã tồn tại";
                    }
                }
                catch (Exception ex)
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = ex.Message;
                }
            }
            else
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = "Thiếu thông tin tài khoản";
            }
            return APIotp;
        }
        //
        [HttpPost("AccountFrom_TimViec365")]
        [AllowAnonymous]
        public APIOTP AccountFrom_TimViec365([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            if (user.Email != null && user.Password != null && user.UserName != null && (user.ID != 0 || user.ID365 != 0))
            {
                try
                {
                    if (user.ID == DAOCounter.getNextID("Users"))
                    {
                        user = InsertNewUser1(new User(user.ID, user.ID365, user.IDTimViec, user.Type365 == 1 ? 1 : 0, user.Email, user.Password, "", user.UserName, user.AvatarUser, "", 1, DateTime.Now, 1, 1, 1, user.Type365 == 1 ? user.ID365 : 0, user.Type365 == 1 ? "" : user.UserName, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "timviec365", 1), true, "timviec365");
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Thêm tài khoản thành công";
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Thêm tài khoản thất bại";
                    }

                }
                catch (Exception ex)
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = ex.Message;
                }
            }
            else
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = "Thiếu thông tin tài khoản";
            }
            return APIotp;
        }
        //ok
        [HttpPost("NewAccountFromTimViec365")]
        [AllowAnonymous]
        public APIOTP NewIdFromTimViec365()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                //DataTable dt = DAOUsers.getAllEmailAndType();
                //if (dt.Rows.Count > 0)
                //{
                //    foreach (DataRow row in dt.Rows)
                //    {
                //        try
                //        {
                //            DAOUsers.UpdateIdTimViec(row["email"] + "", Convert.ToInt32(row["type365"]), getIdTimViec(row["email"].ToString(), Convert.ToInt32(row["type365"]) == 1 ? "1" : "0"));
                //        }
                //        catch (Exception ex)
                //        {
                //            APIotp.error = new Error();
                //            APIotp.error.code = 200;
                //            APIotp.error.message = ex.Message;
                //        }
                //    }
                //}
                //else
                //{
                //    APIotp.error = new Error();
                //    APIotp.error.code = 200;
                //    APIotp.error.message = "Tài khoản đã tồn tại";
                //}
            }
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.Message;
            }
            return APIotp;
        }

        //()
        [HttpPost("NewAccountFromQLC")]
        [AllowAnonymous]
        public APIOTP NewIdFromQLC()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                List<UserDB> dt = DAOUsers.getAllEmailAndTypeQLC();
                if (dt.Count > 0)
                {
                    foreach (UserDB row in dt)
                    {
                        try
                        {
                            DAOUsers.UpdateIdQLC(row.email + "", Convert.ToInt32(row.type365), getIdQLC(row.email.ToString(), row.type365.ToString()));
                        }
                        catch (Exception ex)
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = ex.Message;
                        }
                    }
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "Tài khoản đã tồn tại";
                }
            }
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;


                APIotp.error.message = ex.Message;
            }
            return APIotp;
        }
        private int getIdTimViec(string email, string type)
        {
            try
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(email), "email");
                content.Add(new StringContent(type), "type");
                HttpClient httpClient = new HttpClient();
                Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/get_id_email.php", content);
                APIGetFormQLC receiveInfo = JsonConvert.DeserializeObject<APIGetFormQLC>(response.Result.Content.ReadAsStringAsync().Result);
                if (receiveInfo.data != null && receiveInfo.data.result == true)
                {
                    return Convert.ToInt32(receiveInfo.data.id);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private int getIdQLC(string email, string type)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            content.Add(new StringContent(email), "email");
            content.Add(new StringContent(type), "type");
            HttpClient httpClient = new HttpClient();
            Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/check_email_exits2.php", content);
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

        //()
        [HttpPost("UpdateInfomation365")]
        [AllowAnonymous]
        public APIOTP UpdateInfomation365([FromForm] User user)
        {
            var http = HttpContext.Request;
            string from = http.Form["from"];
            APIOTP APIotp = new APIOTP();
            if (user.Email != null && user.Type365 != 0 && user.IDTimViec != 0 && (user.UserName != null || user.AvatarUser != null || user.Password != null))
            {
                try
                {
                    if (string.IsNullOrEmpty(from))
                    {
                        from = "";
                    }
                    user.ID = 0;
                    List<UserDB> dataUser = new List<UserDB>();
                    dataUser = DAOUsers.checkMaiAndIdlEmpty365(user.Email, user.Type365, user.ID365);
                    bool flag = false;
                    if (dataUser.Count > 0)
                    {
                        user.ID = Convert.ToInt32(dataUser[0].id);
                        flag = true;
                    }
                    else
                    {
                        dataUser = DAOUsers.checkMaiAndIdlEmpty365(user.Email, 0, user.ID365);
                        if (dataUser.Count > 0)
                        {
                            user.ID = Convert.ToInt32(dataUser[0].id);
                            flag = true;
                        }
                    }
                    if (user.ID != 0)
                    {
                        if (!String.IsNullOrEmpty(user.AvatarUser))
                        {
                            using (WebClient webClient = new WebClient())
                            {
                                byte[] dataArr = dataArr = webClient.DownloadData(user.AvatarUser);
                                long bytesize = dataArr.Length;
                                var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                                string fileName = "";
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
                                    fileName = DateTime.Now.Ticks + "_" + user.ID + ".jpg";
                                    try
                                    {
                                        FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + user.ID + @"\" + fileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        System.IO.File.WriteAllBytes(filePath + @"\" + user.ID + @"\" + fileName, dataArr);
                                    }
                                }
                                if (flag && from != "qlc365")
                                {
                                    MultipartFormDataContent content = new MultipartFormDataContent();
                                    //content.Add(new StringContent(@"https://mess.timviec365.vn\avatarUser" + @"\" + user.ID + @"\" + fileName), "link");
                                    content.Add(new StringContent(user.AvatarUser), "link");
                                    content.Add(new StringContent(user.Email), "email");
                                    content.Add(new StringContent(user.Type365.ToString()), "type");
                                    HttpClient httpClient = new HttpClient();
                                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/update_avatar.php", content);
                                    string re = response.Result.Content.ReadAsStringAsync().Result;
                                    if (re.Contains("<br />")) re = re.Substring(re.LastIndexOf("<br />") + 7);
                                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(re);
                                    if (receiveInfo.data != null)
                                    {
                                        DAOUsers.UpdateAvatarUser(fileName, user.ID);
                                    }
                                }
                                else
                                {
                                    DAOUsers.UpdateAvatarUser(fileName, user.ID);
                                }
                            }
                        }
                        if (!String.IsNullOrEmpty(user.UserName))
                        {
                            if (flag && from != "qlc365")
                            {
                                using (HttpClient httpClient = new HttpClient())
                                {
                                    var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", user.Email }, { "user_name", user.UserName }, { "type", user.Type365.ToString() } });
                                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/update_user_info.php", infoLogin);
                                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                                    if (receiveInfo.data != null)
                                    {
                                        DAOUsers.UpdateNameUser(user.ID, user.UserName);
                                    }
                                }
                            }
                            else
                            {
                                DAOUsers.UpdateNameUser(user.ID, user.UserName);
                            }
                        }
                        if (!String.IsNullOrEmpty(user.Password))
                        {
                            if (flag && from != "qlc365")
                            {
                                using (HttpClient httpClient = new HttpClient())
                                {
                                    var infoUpdate = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", user.Email }, { "new_pass", user.Password }, { "type", user.Type365.ToString() } });
                                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/forget_pass.php", infoUpdate);
                                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                                    if (receiveInfo.data != null)
                                    {
                                        DAOUsers.UpdatePassword(user.ID, ToMD5(user.Password));
                                    }
                                }
                            }
                            else
                            {
                                DAOUsers.UpdatePassword(user.ID, ToMD5(user.Password));
                            }
                        }
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật thông tin thành công " + flag.ToString();
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Tài khoàn không tồn tại";
                    }
                }
                catch (Exception ex)
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = ex.Message;
                }
            }
            else
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = "Thiếu thông tin tài khoản";
            }
            return APIotp;
        }

        private void AutoAddFriendWithHHP(int userId)
        {
            if (userId != 56387)
            {
                if (DAOUsers.AddFriend(56387, userId, 0) == 1) DAOUsers.AcceptRequestAddFriend(userId, 56387);
                if (DAOUsers.AddNewContact(56387, userId) == 1) WIO.EmitAsync("AcceptRequestAddFriend", userId, 56387);
            }
        }
        //(4/8/22)ok
        [HttpPost("Login")]
        [AllowAnonymous]
        public APIUser Login([FromForm] User user)
        {
            APIUser userAPI = new APIUser();
            var httpRequest = HttpContext.Request;
            string From = httpRequest.Form["From"];
            string strIsCountConversation = httpRequest.Form["isCountConversation"];
            string ClientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();
            int IsCountConversation = 1;
            if (!string.IsNullOrEmpty(strIsCountConversation)) IsCountConversation = Convert.ToInt32(strIsCountConversation);
            if (user.Email != null && user.Password != null)
            {
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", user.Email }, { "pass", (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password) }, { "os", "os" }, { "from", "chat365" }, { "type", user.Type365.ToString() } });
                        Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/login_chat_h.php", infoLogin);
                        InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                        if (receiveInfo.data != null)
                        {
                            bool flag = false;
                            List<UserDB> getUser = DAOUsers.GetListUsersByEmailAndPassword(user.Email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password), user.Type365);
                            if (getUser.Count == 0 && user.Type365 == 0)
                            {
                                getUser = DAOUsers.GetUsersByEmailAndType365(user.Email, 2);
                                flag = true;
                            }
                            if (getUser.Count > 0)
                            {
                                user = getInforUser(getUser);
                                AutoAddFriendWithHHP(user.ID);
                                user.AcceptMessStranger = Convert.ToInt32(getUser[0].acceptMessStranger);
                                user.IDTimViec = Convert.ToInt32(getUser[0].idTimViec);
                                userAPI.data = new DataUser();
                                userAPI.data.result = true;
                                userAPI.data.message = "đăng nhập thành công";
                                if (IsCountConversation == 1) userAPI.data.countConversation = DAOConversation.GetCountConversation(user.ID);
                                userAPI.data.currentTime = DateTime.Now.Ticks;
                                userAPI.data.user_info = user;
                                if (flag)
                                {
                                    userAPI.data.user_info.ID365 = 0;
                                    userAPI.data.user_info.CompanyId = 0;
                                    userAPI.data.user_info.Type365 = 0;
                                    userAPI.data.user_info.CompanyName = "";
                                }
                                if (userAPI.data.user_info.IDTimViec != 0)
                                {
                                    WIO2.EmitAsync("checkonlineUser", new { uid = user.IDTimViec + "", uid_type = user.Type365 == 1 ? "1" : "0" });
                                }
                            }
                            else
                            {
                                if (user.Type365 == 1)
                                {
                                    user = InsertNewUser(new User(-1, receiveInfo.data.user_info.com_id, 0, user.Type365, receiveInfo.data.user_info.com_email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password), receiveInfo.data.user_info.com_phone, receiveInfo.data.user_info.com_name, receiveInfo.data.user_info.com_logo, "", 0, DateTime.Now, 1, 0, 0, receiveInfo.data.user_info.com_id, receiveInfo.data.user_info.com_name, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "quanlychung365", 1), false, "quanlychung365");
                                }
                                else if (user.Type365 == 2)
                                {
                                    user = InsertNewUser(new User(-1, receiveInfo.data.user_info.ep_id, 0, user.Type365, receiveInfo.data.user_info.ep_email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password), receiveInfo.data.user_info.ep_phone, receiveInfo.data.user_info.ep_name, receiveInfo.data.user_info.ep_image, "", 0, DateTime.Now, 1, 0, 0, receiveInfo.data.user_info.com_id, receiveInfo.data.user_info.com_name, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "quanlychung365", 1), false, "quanlychung365"); ;
                                }
                                else
                                {
                                    user = InsertNewUser(new User(-1, 0, 0, user.Type365, receiveInfo.data.user_info.ep_email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password), receiveInfo.data.user_info.ep_phone, receiveInfo.data.user_info.ep_name, receiveInfo.data.user_info.ep_image, "", 0, DateTime.Now, 1, 0, 0, 0, "", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, "quanlychung365", 1), false, "quanlychung365");
                                }
                                if (user != null)
                                {
                                    if (user.CompanyId != 0 && DAOUsers.GetUserByID365(receiveInfo.data.user_info.com_id, 1).Count == 0)
                                    {
                                        Task<HttpResponseMessage> response2 = httpClient.GetAsync("https://chamcong.24hpay.vn/api_tinhluong/list_com.php?id_com=" + receiveInfo.data.user_info.com_id);
                                        InforFromAPI receiveInfo2 = JsonConvert.DeserializeObject<InforFromAPI>(response2.Result.Content.ReadAsStringAsync().Result);
                                        if (receiveInfo2.data != null)
                                        {
                                            user = new User(-1, receiveInfo.data.user_info.com_id, 0, 1, receiveInfo2.data.items[0].com_email, receiveInfo2.data.items[0].com_pass, receiveInfo2.data.items[0].com_phone, receiveInfo2.data.items[0].com_name, receiveInfo2.data.items[0].com_logo, "", 0, DateTime.Now, 1, 0, 0, receiveInfo.data.user_info.com_id, receiveInfo2.data.items[0].com_name, "quanlychung365");
                                            try
                                            {
                                                using (WebClient webClient = new WebClient())
                                                {
                                                    byte[] dataArr = new byte[1];
                                                    long bytesize;
                                                    if (!String.IsNullOrWhiteSpace(user.AvatarUser) && !(user.AvatarUser.Trim().Length == 0))
                                                    {
                                                        dataArr = webClient.DownloadData("https://chamcong.24hpay.vn/upload/company/logo/" + user.AvatarUser);
                                                        bytesize = dataArr.Length;
                                                    }
                                                    int userId = DAOUsers.InsertNewUser(user.UserName, user.ID365, user.IDTimViec, user.Type365, user.Email, user.Password, user.CompanyId, user.CompanyName, "quanlychung365");
                                                    if (userId > 0)
                                                    {
                                                        if (dataArr.Length > 1)
                                                        {
                                                            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                                                            if (!Directory.Exists(filePath))
                                                            {
                                                                Directory.CreateDirectory(filePath);
                                                            }
                                                            if (!Directory.Exists(filePath + @"\" + userId))
                                                            {
                                                                Directory.CreateDirectory(filePath + @"\" + userId);
                                                            }
                                                            System.IO.FileInfo[] di = (new DirectoryInfo(filePath + @"\" + userId)).GetFiles();
                                                            if (di.Length > 0)
                                                            {
                                                                for (int i = 0; i < di.Length; i++)
                                                                {
                                                                    di[i].Delete();
                                                                }
                                                            }
                                                            string fileName = DateTime.Now.Ticks + "_" + userId + ".jpg";
                                                            try
                                                            {
                                                                FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + userId + @"\" + fileName);
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                System.IO.File.WriteAllBytes(filePath + @"\" + userId + @"\" + fileName, dataArr);
                                                            }
                                                            DAOUsers.UpdateInfoUser(userId, user.ID365, user.Type365, user.UserName, fileName, user.Password, user.CompanyId, user.CompanyName, 0);
                                                        }
                                                    }
                                                }
                                            }
                                            catch (WebException ex)
                                            {
                                                userAPI.error = new Error();
                                                userAPI.error.code = 200;
                                                userAPI.error.message = ex.Message;
                                            }
                                        }
                                    }
                                    AutoAddFriendWithHHP(user.ID);
                                    userAPI.data = new DataUser();
                                    userAPI.data.result = true;
                                    userAPI.data.message = "đăng nhập thành công";
                                    userAPI.data.currentTime = DateTime.Now.Ticks;
                                    if (IsCountConversation == 1) userAPI.data.countConversation = DAOConversation.GetCountConversation(user.ID);
                                    userAPI.data.user_info = user;
                                    userAPI.data.user_info.AcceptMessStranger = 1;
                                    if (userAPI.data.user_info.IDTimViec != 0 && (From == null || !From.Equals("timviec365")))
                                    {
                                        WIO2.EmitAsync("checkonlineUser", new { uid = user.IDTimViec + "", uid_type = user.Type365 == 1 ? "1" : "0" });
                                    }
                                }
                                else
                                {
                                    userAPI.error = new Error();
                                    userAPI.error.code = 200;
                                    userAPI.error.message = "Đã có lỗi xảy ra";
                                }
                            }
                        }
                        else
                        {
                            if (receiveInfo.error != null && receiveInfo.error.code == 401)
                            {
                                userAPI.error = new Error();
                                userAPI.error.code = 301;
                                userAPI.error.message = receiveInfo.error.message;
                            }
                            else if (user.Type365 != 0)
                            {
                                DataTable getUser = DAOUsers.GetUsersByEmailAndPassword(user.Email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password).Trim(), user.Type365);
                                if (getUser.Rows.Count != 0)
                                {
                                    var uu = getInforUser(getUser);
                                    AutoAddFriendWithHHP(uu.ID);
                                    userAPI.data = new DataUser();
                                    userAPI.data.result = true;
                                    userAPI.data.message = "đăng nhập thành công";
                                    userAPI.data.currentTime = DateTime.Now.Ticks;
                                    if (IsCountConversation == 1) userAPI.data.countConversation = DAOConversation.GetCountConversation(uu.ID);
                                    userAPI.data.user_info = uu;
                                }
                                else
                                {
                                    List<UserDB> list = DAOUsers.CheckUsersByEmailAndPassword(user.Email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password).Trim());
                                    if (list.Count > 0)
                                    {
                                        List<int> listType = new List<int>();
                                        foreach (UserDB userDB in list)
                                        {
                                            if (!listType.Contains(userDB.type365)) listType.Add(userDB.type365);
                                        }
                                        UserDB u = list[0];
                                        userAPI.error = new Error();
                                        if (listType.Contains(0) && listType.Contains(1))
                                        {
                                            userAPI.error.code = 300;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng cá nhân, vui lòng chọn đúng loại tài khoản";
                                        }
                                        else if (u.type365 == 0)
                                        {
                                            userAPI.error.code = 300;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng cá nhân, vui lòng chọn đúng loại tài khoản";
                                        }
                                        else if (u.type365 == 1)
                                        {
                                            userAPI.error.code = 303;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng công ty, vui lòng chọn đúng loại tài khoản";
                                        }
                                        else if (u.type365 == 2)
                                        {
                                            userAPI.error.code = 302;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng nhân viên, vui lòng chọn đúng loại tài khoản";
                                        }

                                    }
                                    else
                                    {
                                        userAPI.error = new Error();
                                        userAPI.error.code = Convert.ToInt32(receiveInfo.error.code);
                                        userAPI.error.message = receiveInfo.error.message;
                                    }
                                }
                            }
                            else
                            {
                                DataTable getUser = new DataTable();
                                getUser = DAOUsers.GetUsersByEmailAndPassword(user.Email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password).Trim(), 2);
                                if (getUser.Rows.Count == 0)
                                {
                                    getUser = DAOUsers.GetUsersByEmailAndPassword(user.Email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password).Trim(), 0);
                                }
                                if (getUser.Rows.Count != 0)
                                {
                                    DAOUsers.UpdateIsOnlineUser(Convert.ToInt32(getUser.Rows[0]["id"]), 1);
                                    AutoAddFriendWithHHP(Convert.ToInt32(getUser.Rows[0]["id"]));
                                    userAPI.data = new DataUser();
                                    userAPI.data.result = true;
                                    userAPI.data.message = "đăng nhập thành công";
                                    userAPI.data.currentTime = DateTime.Now.Ticks;
                                    if (IsCountConversation == 1) userAPI.data.countConversation = DAOConversation.GetCountConversation(Convert.ToInt32(getUser.Rows[0]["id"]));
                                    userAPI.data.user_info = getInforUser(getUser);
                                    userAPI.data.user_info.AcceptMessStranger = Convert.ToInt32(getUser.Rows[0]["AcceptMessStranger"]);
                                    userAPI.data.user_info.IDTimViec = Convert.ToInt32(getUser.Rows[0]["idTimViec"]);
                                    if (userAPI.data.user_info.IDTimViec != 0)
                                    {
                                        WIO2.EmitAsync("checkonlineUser", new { uid = userAPI.data.user_info.IDTimViec + "", uid_type = userAPI.data.user_info.Type365 == 1 ? "1" : "0" });
                                    }
                                    userAPI.data.user_info.ID365 = 0;
                                    userAPI.data.user_info.CompanyId = 0;
                                    userAPI.data.user_info.Type365 = 0;
                                    userAPI.data.user_info.CompanyName = "";
                                }
                                else
                                {
                                    List<UserDB> list = DAOUsers.CheckUsersByEmailAndPassword(user.Email, (user.Type_Pass == 0 ? ToMD5(user.Password) : user.Password).Trim());
                                    if (list.Count > 0)
                                    {
                                        List<int> listType = new List<int>();
                                        foreach (UserDB userDB in list)
                                        {
                                            if (!listType.Contains(userDB.type365)) listType.Add(userDB.type365);
                                        }
                                        UserDB u = list[0];
                                        userAPI.error = new Error();
                                        if (listType.Contains(0) && listType.Contains(1))
                                        {
                                            userAPI.error.code = 300;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng cá nhân, vui lòng chọn đúng loại tài khoản";
                                        }
                                        else if (u.type365 == 0)
                                        {
                                            userAPI.error.code = 300;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng cá nhân, vui lòng chọn đúng loại tài khoản";
                                        }
                                        else if (u.type365 == 1)
                                        {
                                            userAPI.error.code = 303;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng công ty, vui lòng chọn đúng loại tài khoản";
                                        }
                                        else if (u.type365 == 2)
                                        {
                                            userAPI.error.code = 302;
                                            userAPI.error.message = "Tài khoản đang thuộc diện khách hàng nhân viên, vui lòng chọn đúng loại tài khoản";
                                        }

                                    }
                                    else
                                    {
                                        userAPI.error = new Error();
                                        userAPI.error.code = 200;
                                        userAPI.error.message = "Thông tin tài khoản hoặc mật khẩu không chính xác";
                                    }

                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    userAPI.error = new Error();
                    userAPI.error.code = 200;
                    userAPI.error.message = ex.Message;
                }
            }
            else
            {
                userAPI.error = new Error();
                userAPI.error.code = 200;
                userAPI.error.message = "Thiếu thông tin tài khoản";
            }
            return userAPI;
        }

        //(4/8/22)ok
        [HttpPost("UpdatePassword")]
        [AllowAnonymous]
        public APIOTP UpdatePassword([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.Email != null && user.Password != null)
                {
                    List<UserDB> checkEmail = DAOUsers.checkMailEmpty365(user.Email, user.Type365);
                    if (checkEmail.Count > 0)
                    {
                        if (user.Type365 != 0)
                        {
                            using (HttpClient httpClient = new HttpClient())
                            {
                                var infoUpdate = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", user.Email }, { "new_pass", user.Password }, { "type", user.Type365.ToString() } });
                                Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/forget_pass.php", infoUpdate);
                                InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                                if (receiveInfo.data != null)
                                {
                                    if (DAOUsers.checkMailEmpty365(user.Email, user.Type365).Count == 0)
                                    {
                                        APIotp.data = new DataOTP();
                                        APIotp.data.result = true;
                                        APIotp.data.message = "Cập nhật mật khẩu thành công";
                                    }
                                    else
                                    {
                                        if (DAOUsers.UpdatePasswordByEmail(user.Email, ToMD5(user.Password), user.Type365) > 0)
                                        {
                                            APIotp.data = new DataOTP();
                                            APIotp.data.result = true;
                                            APIotp.data.message = "Cập nhật mật khẩu thành công";
                                        }
                                        else if (checkEmail.Count > 0 && checkEmail[0].password == ToMD5(user.Password))
                                        {
                                            APIotp.data = new DataOTP();
                                            APIotp.data.result = true;
                                            APIotp.data.message = "Cập nhật mật khẩu thành công";
                                        }
                                        else
                                        {
                                            APIotp.error = new Error();
                                            APIotp.error.code = 200;
                                            APIotp.error.message = "Email không tồn tại";
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (DAOUsers.UpdatePasswordByEmail(user.Email, ToMD5(user.Password), user.Type365) > 0)
                            {
                                APIotp.data = new DataOTP();
                                APIotp.data.result = true;
                                APIotp.data.message = "Cập nhật mật khẩu thành công";
                            }
                            else if (checkEmail.Count > 0 && checkEmail[0].password == ToMD5(user.Password))
                            {
                                APIotp.data = new DataOTP();
                                APIotp.data.result = true;
                                APIotp.data.message = "Cập nhật mật khẩu thành công";
                            }
                            else
                            {
                                APIotp.error = new Error();
                                APIotp.error.code = 200;
                                APIotp.error.message = "Email không tồn tại";

                            }
                        }
                        var check = DAOUsers.GetIdTimViec(user.Email, user.Type365);
                        if (check.Count > 0 && Convert.ToInt32(check[0].idTimViec) != 0)
                        {
                            MultipartFormDataContent content = new MultipartFormDataContent();
                            content.Add(new StringContent(ToMD5(user.Password)), "pass");
                            if (user.Type365 != 0)
                            {
                                content.Add(new StringContent(user.Email), "email");
                                content.Add(new StringContent(user.Type365.ToString()), "type");
                            }
                            else
                            {
                                content.Add(new StringContent(user.ID365.ToString()), "id");
                            }
                            HttpClient httpClient = new HttpClient();
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                            InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                            if (receiveInfo.data != null)
                            {
                            }
                        }
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Email không tồn tại";
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

        //(4/8/22)ok
        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public APIOTP ForgetPassword([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.Email != null)
                {
                    if (user.Type365 == 0)
                    {
                        List<UserDB> getUser = DAOUsers.checkMailEmpty365(user.Email, user.Type365);
                        if (getUser.Count != 0)
                        {
                            var random = new System.Random();
                            string codeConfirm = random.Next(100000, 999999).ToString();
                            if (user.Email.Contains("@"))
                            {
                                SmtpClient smtp = new SmtpClient();
                                try
                                {
                                    smtp.Host = "smtp.mailgun.org";
                                    smtp.Port = 587;
                                    smtp.EnableSsl = true;
                                    smtp.Credentials = new NetworkCredential("postmaster@mailtimviec365.vn", "bcbe2993383b34c696e1d1c5603b1618-100b5c8d-51aa397c");
                                    smtp.Send("chat365@mailtimviec365.vn", user.Email, "Confirm forget password AppChat Timviec365", "Chúng tôi nhận được yêu cầu quên mật khẩu của bạn vui lòng nhập mã xác nhận để hoàn thành thiết lập, mã code xác nhận của bạn là: " + codeConfirm);
                                    APIotp.data = new DataOTP();
                                    APIotp.data.result = true;
                                    APIotp.data.message = "Gửi mã xác thực thành công";
                                    APIotp.data.otp = codeConfirm;
                                }
                                catch (Exception ex)
                                {
                                    APIotp.error = new Error();
                                    APIotp.error.code = 100;
                                    APIotp.error.message = ex.Message;
                                }
                            }
                            else
                            {
                                using (HttpClient httpClient2 = new HttpClient())
                                {
                                    OTPByPhone otpByPhone = new OTPByPhone("TIMVIEC365 mã OTP của nhân viên tại https://timviec365.vn/ : " + codeConfirm, user.Email);
                                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/");
                                    request.Content = new StringContent(JsonConvert.SerializeObject(otpByPhone), System.Text.Encoding.UTF8, "application/json");
                                    var response2 = httpClient2.SendAsync(request);
                                    if (response2.Result.Content.ReadAsStringAsync().Result.Contains("SMSID"))
                                    {
                                        APIotp.data = new DataOTP();
                                        APIotp.data.result = true;
                                        APIotp.data.message = "Gửi mã xác thực thành công";
                                        APIotp.data.otp = codeConfirm;
                                    }
                                    else
                                    {
                                        APIotp.error = new Error();
                                        APIotp.error.code = 200;
                                        APIotp.error.message = "Số điện thoại không chính xác";
                                    }
                                }
                            }
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 300;
                            APIotp.error.message = "Email hoặc sđt không tồn tại";
                        }
                    }
                    else
                    {
                        using (HttpClient httpClient = new HttpClient())
                        {
                            var infoUser = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", user.Email }, { "type", user.Type365.ToString() } });
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/send_otp.php", infoUser);
                            InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                            if (receiveInfo.data != null)
                            {
                                APIotp.data = new DataOTP();
                                APIotp.data.result = true;
                                APIotp.data.message = "Gửi mã xác thực thành công";
                                APIotp.data.otp = receiveInfo.data.otp + "";
                            }
                            else
                            {

                                APIotp.error = new Error();
                                APIotp.error.code = 300;
                                APIotp.error.message = "Email hoặc sđt không tồn tại";
                            }
                        }
                    }
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 400;
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

        //(4/8/22)ok
        [HttpPost("Register")]
        [AllowAnonymous]
        public APIOTP Register([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.Email != null)
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var infoUser = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", user.Email } });
                        Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/check_email_exits.php", infoUser);
                        InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                        if (receiveInfo.data != null && String.IsNullOrWhiteSpace(receiveInfo.data.message))
                        {
                            List<UserDB> getUser = DAOUsers.checkMailEmpty(user.Email);
                            if (getUser.Count == 0)
                            {
                                var random = new System.Random();
                                string codeConfirm = random.Next(100000, 999999).ToString();
                                if (user.Email.Contains("@"))
                                {
                                    SmtpClient smtp = new SmtpClient();
                                    try
                                    {
                                        smtp.Host = "smtp.mailgun.org";
                                        smtp.Port = 587;
                                        smtp.EnableSsl = true;
                                        smtp.Credentials = new NetworkCredential("postmaster@mailtimviec365.vn", "bcbe2993383b34c696e1d1c5603b1618-100b5c8d-51aa397c");
                                        smtp.Send("chat365@mailtimviec365.vn", user.Email, "Register password AppChat Timviec365", "Chào mừng bạn đến với AppChat Timviec365 mã code xác nhận của bạn là: " + codeConfirm);
                                        APIotp.data = new DataOTP();
                                        APIotp.data.result = true;
                                        APIotp.data.message = "Gửi mã xác thực thành công";
                                        APIotp.data.otp = codeConfirm;
                                    }
                                    catch (Exception ex)
                                    {
                                        APIotp.error = new Error();
                                        APIotp.error.code = 100;
                                        APIotp.error.message = ex.Message;
                                    }
                                }
                                else
                                {
                                    using (HttpClient httpClient2 = new HttpClient())
                                    {
                                        OTPByPhone otpByPhone = new OTPByPhone("TIMVIEC365 mã OTP của nhân viên tại https://timviec365.vn/ : " + codeConfirm, user.Email);
                                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/");
                                        request.Content = new StringContent(JsonConvert.SerializeObject(otpByPhone), System.Text.Encoding.UTF8, "application/json");
                                        var response2 = httpClient2.SendAsync(request);
                                        if (response2.Result.Content.ReadAsStringAsync().Result.Contains("SMSID"))
                                        {
                                            APIotp.data = new DataOTP();
                                            APIotp.data.result = true;
                                            APIotp.data.message = "Gửi mã xác thực thành công";
                                            APIotp.data.otp = codeConfirm;
                                        }
                                        else
                                        {
                                            APIotp.error = new Error();
                                            APIotp.error.code = 200;
                                            APIotp.error.message = "Số điện thoại không chính xác";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                APIotp.error = new Error();
                                APIotp.error.code = 300;
                                APIotp.error.message = "Email hoặc sđt đã tồn tại";
                            }
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 300;
                            APIotp.error.message = "Email hoặc sđt đã tồn tại";

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

        //(4/8/22)ok
        [HttpPost("RegisterSuccess")]
        [AllowAnonymous]
        public APIOTP RegisterSuccess([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.UserName != null && user.Email != null && user.Password != null)
                {
                    if (DAOUsers.checkMailEmpty(user.Email).Count == 0)
                    {
                        if (DAOUsers.InsertNewUser(user.UserName, 0, 0, 0, user.Email, ToMD5(user.Password), 0, "", "chat365") > 0)
                        {
                            APIotp.data = new DataOTP();
                            APIotp.data.result = true;
                            APIotp.data.message = "Đăng ký thành công";
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = "Đăng ký thất bại";
                        }
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Sđt hoặc email đã được sử dụng";
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

        //(4/8/22)ok
        [HttpPost("GetAllUserOnline")]
        [AllowAnonymous]
        public APIUser GetAllUserOnline([FromForm] User user)
        {
            APIUser APIUser = new APIUser();
            try
            {
                if (user.ID != 0)
                {
                    List<UserDB> listUser = DAOUsers.GetListAllUserOnline(user.ID);
                    if (listUser.Count > 0)
                    {
                        APIUser.data = new DataUser();
                        APIUser.data.result = true;
                        APIUser.data.ListUserOnline = new int[listUser.Count];
                        int index = 0;
                        foreach (UserDB data in listUser)
                        {
                            APIUser.data.ListUserOnline[index] = Convert.ToInt32(data.id);
                            index++;
                        }

                        APIUser.data.message = "lấy danh sách người dùng online thành công";
                    }
                    else
                    {
                        APIUser.error = new Error();
                        APIUser.error.code = 200;
                        APIUser.error.message = "không có người dùng nào online";
                    }
                }
                else
                {
                    APIUser.error = new Error();
                    APIUser.error.code = 200;
                    APIUser.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                APIUser.error = new Error();
                APIUser.error.code = 200;
                APIUser.error.message = ex.ToString();
            }
            return APIUser;
        }

        //(4/8/22)ok
        [HttpPost("ChangeUserName")]
        [AllowAnonymous]
        public APIOTP ChangeUserName([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.UserName != null && user.ID != 0)
                {
                    string NameChange = user.UserName;
                    user = getInforUser(DAOUsers.GetUsersById(user.ID));
                    //if (user.Type365 == 0)
                    //{
                    //    if (DAOUsers.UpdateNameUser(user.ID, user.UserName) > 0)
                    //    {
                    //        APIotp.data = new DataOTP();
                    //        APIotp.data.result = true;
                    //        APIotp.data.message = "Cập nhật tên thành công";
                    //    }
                    //    else
                    //    {
                    //        APIotp.error = new Error();
                    //        APIotp.error.code = 200;
                    //        APIotp.error.message = "Email không tồn tại";
                    //    }
                    //}
                    //else
                    //{

                    //}
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", user.Email }, { "user_name", NameChange }, { "type", user.Type365.ToString() } });
                        Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/update_user_info.php", infoLogin);
                        InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                        if (DAOUsers.UpdateNameUser(user.ID, NameChange) > 0)
                        {
                            APIotp.data = new DataOTP();
                            APIotp.data.result = true;
                            APIotp.data.message = "Cập nhật tên thành công";
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = "Email không tồn tại";
                        }
                        //if (receiveInfo.data != null)
                        //{

                        //}
                        //else
                        //{
                        //    APIotp.error = new Error();
                        //    APIotp.error.code = 200;
                        //    APIotp.error.message = "Email không tồn tại";
                        //}
                    }
                    if (Convert.ToInt32(DAOUsers.GetIdTimViec(user.Email, user.Type365)[0].idTimViec) != 0)
                    {
                        MultipartFormDataContent content = new MultipartFormDataContent();
                        content.Add(new StringContent(NameChange), "name");
                        content.Add(new StringContent(user.Email), "email");
                        content.Add(new StringContent(user.Type365.ToString()), "type");
                        //if (user.Type365 != 0)
                        //{

                        //}
                        //else
                        //{
                        //    content.Add(new StringContent(user.ID365.ToString()), "id");
                        //}
                        HttpClient httpClient = new HttpClient();
                        Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                        InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                        if (receiveInfo.data != null)
                        {
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

        //ok
        [HttpPost("ChangeActive")]
        [AllowAnonymous]
        public APIOTP ChangeActive([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            if (user.Active != 0 && user.ID != 0)
            {
                if (DAOUsers.UpdateActiveUser(user.ID, user.Active) > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "Cập nhật trạng thái thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "Id không tồn tại";
                }
            }
            else
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = "Thiếu thông tin truyền lên";
            }
            return APIotp;
        }
        [HttpPost("Logout_v2")]
        [AllowAnonymous]
        public APIOTP Logout_v2()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userId = Convert.ToInt32(http.Form["ID"]);
                string[] fromWeb = JsonConvert.DeserializeObject<string[]>(http.Form["fromWeb"]);
                if (userId != 0 && fromWeb.Length > 0)
                {
                    foreach (string item in fromWeb)
                    {
                        DAOUsers.UpdateIsOnlineUser(userId, 0);
                        WIO.EmitAsync("Logout", userId, item);
                    }
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "Đăng xuất thành công";
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
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }
        [HttpPost("Logout_all")]
        [AllowAnonymous]
        public APIOTP Logout_all()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userId = Convert.ToInt32(http.Form["ID"]);
                string fromWeb = http.Form["fromWeb"];
                if (userId != 0 && !string.IsNullOrEmpty(fromWeb))
                {
                    DAOUsers.UpdateIsOnlineUser(userId, 0);
                    WIO.EmitAsync("Logout_all", userId, fromWeb);
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "Đăng xuất thành công";
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
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }
        //ok
        [HttpPost("Logout")]
        [AllowAnonymous]
        public APIOTP Logout([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            if (user.ID != 0)
            {
                if (DAOUsers.UpdateIsOnlineUser(user.ID, 0) > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "Cập nhật đăng xuất thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "Id không tồn tại";
                }
            }
            else
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = "Thiếu thông tin truyền lên";
            }
            return APIotp;
        }

        //(4/8/22)ok
        [HttpPost("ChangePassword")]
        [AllowAnonymous]
        public APIOTP ChangePassword([FromForm] APIChangePass inforChange)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (inforChange.ID != 0 && inforChange.oldPassword != null && inforChange.Email != null && inforChange.newPassword != null)
                {
                    List<UserDB> dataUser = DAOUsers.GetUsersByEmailAndPasswordAndId(inforChange.Email, ToMD5(inforChange.oldPassword), inforChange.ID);
                    if (dataUser.Count > 0)
                    {
                        User user = getInforUser(dataUser);
                        //if (user.Type365 == 0)
                        //{
                        //    if (DAOUsers.UpdatePassword(user.ID, ToMD5(inforChange.newPassword)) > 0)
                        //    {
                        //        APIotp.data = new DataOTP();
                        //        APIotp.data.result = true;
                        //        APIotp.data.message = "Cập nhật mật khẩu thành công";
                        //        WIO.EmitAsync("changedPassword", user.ID, ToMD5(inforChange.newPassword));
                        //    }
                        //    else
                        //    {
                        //        APIotp.error = new Error();
                        //        APIotp.error.code = 200;
                        //        APIotp.error.message = "Email không tồn tại hoặc sai mật khẩu cũ";
                        //    }
                        //}
                        //else
                        {
                            using (HttpClient httpClient = new HttpClient())
                            {
                                var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "email", inforChange.Email }, { "new_pass", inforChange.newPassword }, { "old_pass", inforChange.oldPassword }, { "type", user.Type365.ToString() } });
                                Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/change_pass_chat.php", infoLogin);
                                InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                                if (receiveInfo.data != null)
                                {
                                    if (DAOUsers.UpdatePassword(user.ID, ToMD5(inforChange.newPassword)) > 0)
                                    {
                                        APIotp.data = new DataOTP();
                                        APIotp.data.result = true;
                                        APIotp.data.message = "Đổi mật khẩu thành công";
                                        WIO.EmitAsync("changedPassword", user.ID, ToMD5(inforChange.newPassword));
                                    }
                                    else
                                    {
                                        APIotp.error = new Error();
                                        APIotp.error.code = 200;
                                        APIotp.error.message = "Email không tồn tại hoặc sai mật khẩu cũ";
                                    }
                                }
                                else
                                {
                                    APIotp.error = new Error();
                                    APIotp.error.code = 200;
                                    APIotp.error.message = "Email không tồn tại hoặc sai mật khẩu cũ";
                                }
                            }
                        }
                        if (Convert.ToInt32(DAOUsers.GetIdTimViec(user.Email, user.Type365)[0].idTimViec) != 0)
                        {
                            MultipartFormDataContent content = new MultipartFormDataContent();
                            content.Add(new StringContent(ToMD5(inforChange.newPassword)), "pass");
                            content.Add(new StringContent(user.Email), "email");
                            content.Add(new StringContent(user.Type365.ToString()), "type");
                            //if (user.Type365 != 0)
                            //{

                            //}
                            //else
                            //{
                            //    content.Add(new StringContent(user.ID365.ToString()), "id");
                            //}
                            HttpClient httpClient = new HttpClient();
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                            InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                            if (receiveInfo.data != null)
                            {
                            }
                        }
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Email không tồn tại hoặc sai mật khẩu cũ";
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
        [HttpPost("GetListFriend")]
        [AllowAnonymous]
        public APIFriend GetListFriend()
        {
            APIFriend userAPI = new APIFriend();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["ID"]);
                int id365 = Convert.ToInt32(httpRequest.Form["ID365"]);
                int companyId = Convert.ToInt32(httpRequest.Form["CompanyID"]);
                int type = Convert.ToInt32(httpRequest.Form["Type365"]);
                int offset = Convert.ToInt32(httpRequest.Form["Offset"]);
                int limit = Convert.ToInt32(httpRequest.Form["Limit"]);
                if (userId != 0 || id365 != 0)
                {
                    List<UserDB> getUser = new List<UserDB>();
                    if (id365 != 0) getUser = DAOUsers.GetUserByID365(id365, type);
                    if (userId != 0) getUser = DAOUsers.GetUserById(userId);
                    if (getUser.Count > 0 && (getUser[0].companyId == companyId || companyId == 0))
                    {
                        var u = getInforUser(getUser);
                        List<UserDB> getListContact = DAOUsers.GetListFriend(u.ID, companyId == 0 ? getUser[0].companyId : companyId, offset, limit == 0 ? 200 : limit);
                        List<FriendRequest> listContact = new List<FriendRequest>();
                        if (getListContact.Count > 0)
                        {
                            foreach (UserDB member in getListContact)
                            {
                                FriendRequest userInfo = new FriendRequest(member.id, member.id365, member.userName, member.avatarUser, member.isOnline,member.type365);
                                if (String.IsNullOrWhiteSpace(userInfo.avatarUser.Trim()))
                                {
                                    string letter = RemoveUnicode(userInfo.userName.Substring(0, 1).ToLower()).ToUpper();
                                    try
                                    {
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                        }
                                        else
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                else
                                {
                                    userInfo.avatarUser = "https://mess.timviec365.vn/avatarUser/" + userInfo.id + "/" + userInfo.avatarUser;
                                }

                                listContact.Add(userInfo);
                            }
                        }
                        userAPI.data = new DataFriend();
                        userAPI.data.result = true;
                        userAPI.data.listFriend = listContact;
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "User Không tồn tại";
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

        [HttpPost("GetListFriend365")]
        [AllowAnonymous]
        public APIFriend GetListFriend365()
        {
            APIFriend userAPI = new APIFriend();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["ID"]);
                int id365 = Convert.ToInt32(httpRequest.Form["ID365"]);
                int companyId = Convert.ToInt32(httpRequest.Form["CompanyID"]);
                int type = Convert.ToInt32(httpRequest.Form["Type365"]);
                int offset = Convert.ToInt32(httpRequest.Form["Offset"]);
                int limit = Convert.ToInt32(httpRequest.Form["Limit"]);
                if (userId != 0 || id365 != 0)
                {
                    List<UserDB> getUser = new List<UserDB>();
                    if (id365 != 0) getUser = DAOUsers.GetUserByID365(id365, type);
                    if (userId != 0) getUser = DAOUsers.GetUserById(userId);
                    if (getUser.Count > 0 && (getUser[0].companyId == companyId || companyId == 0))
                    {

                        List<UserDB> getListContact = DAOUsers.GetListFriend(userId, companyId == 0 ? getUser[0].companyId : companyId, offset, limit == 0 ? 200 : limit);
                        List<FriendRequest> listContact = new List<FriendRequest>();
                        if (getListContact.Count > 0)
                        {
                            foreach (UserDB member in getListContact)
                            {
                                FriendRequest userInfo = new FriendRequest(member.id, member.id365, member.userName, member.avatarUser, member.isOnline,member.type365);
                                if (String.IsNullOrWhiteSpace(userInfo.avatarUser.Trim()))
                                {
                                    string letter = RemoveUnicode(userInfo.userName.Substring(0, 1).ToLower()).ToUpper();
                                    try
                                    {
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                        }
                                        if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                        }
                                        else
                                        {
                                            userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        userInfo.avatarUser = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                    }
                                }
                                else
                                {
                                    userInfo.avatarUser = "https://mess.timviec365.vn/avatarUser/" + userInfo.id + "/" + userInfo.avatarUser;
                                }

                                listContact.Add(userInfo);
                            }
                        }
                        userAPI.data = new DataFriend();
                        userAPI.data.result = true;
                        userAPI.data.listFriend = listContact;
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "User Không tồn tại";
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
        //(4/8/22)ok
        [HttpPost("GetListContact")]
        [AllowAnonymous]
        public APIUser GetListContact()
        {
            APIUser userAPI = new APIUser();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["ID"]);
                int countContact = Convert.ToInt32(httpRequest.Form["countContact"]);
                int countLoad = Convert.ToInt32(httpRequest.Form["countLoad"]);
                if (userId != 0)
                {
                    List<UserDB> getListContact = DAOUsers.GetListContact(userId, countContact, countLoad == 0 ? 500 : countLoad);
                    List<Contact> listContact = new List<Contact>();
                    if (getListContact.Count > 0)
                    {
                        foreach (UserDB member in getListContact)
                        {
                            Contact userInfo = new Contact(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), member.companyId, member.type365);
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

                            userInfo.FriendStatus = "friend";

                            listContact.Add(userInfo);
                        }

                    }
                    listContact.Sort(new CompareContactByName());
                    userAPI.data = new DataUser();
                    userAPI.data.result = true;
                    userAPI.data.user_list = listContact;
                    userAPI.data.total = countLoad + countContact;
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

        [HttpPost("GetListContactPrivate")]
        [AllowAnonymous]
        public APIUser1 GetListContactPrivate()
        {
            APIUser1 userAPI = new APIUser1();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["ID"]);
                int countContact = Convert.ToInt32(httpRequest.Form["countContact"]);
                int countLoad = Convert.ToInt32(httpRequest.Form["countLoad"]);
                if (userId != 0)
                {
                    DataTable getuser = DAOUsers.GetUserIdById365(userId);
                    if (getuser.Rows.Count > 0)
                    {
                        User u = getInforUser(getuser);
                        List<UserDB> getListContact = DAOUsers.GetListContactPrivate(u.ID, countContact, countLoad == 0 ? 500 : countLoad);
                        List<Contact1> listContact = new List<Contact1>();
                        if (getListContact.Count > 0)
                        {
                            foreach (UserDB member in getListContact)
                            {
                                Contact1 userInfo = new Contact1(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), member.companyId, member.type365, member.id365);
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

                                userInfo.FriendStatus = "friend";

                                listContact.Add(userInfo);
                            }

                        }
                        userAPI.data = new DataUser1();
                        userAPI.data.result = true;
                        userAPI.data.user_list = listContact;
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "User này không tồn tại";
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

        [HttpPost("CheckContact")]
        [AllowAnonymous]
        public APIUser CheckContact()
        {
            APIUser api = new APIUser();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["ID"]);
                int contactId = Convert.ToInt32(httpRequest.Form["ContactId"]);
                if (userId != 0 && contactId != 0)
                {
                    if (DAOUsers.CheckContact(userId, contactId) > 0)
                    {
                        List<UserDB> info = DAOUsers.GetInforUserById(contactId);
                        api.data = new DataUser();
                        api.data.result = true;
                        api.data.message = "Bạn đã kết bạn với user này";
                        api.data.user_info = getInforUser(info);
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "Bạn chưa kết bạn với user này";
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
        //(4/8/22)ok
        [HttpPost("GetContactCompany")]
        [AllowAnonymous]
        public APIUserCompany GetContactCompany()
        {
            APIUserCompany userAPI = new APIUserCompany();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["ID"]);
                int companyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int countContact = Convert.ToInt32(httpRequest.Form["countContact"]);
                int countLoad = Convert.ToInt32(httpRequest.Form["countLoad"]);
                if (userId != 0 && companyId != 0)
                {
                    List<UserDB> getListContact = DAOUsers.GetListContactCompany(companyId, countContact, countLoad == 0 ? 500 : countLoad);
                    List<ContactCompany> listContact = new List<ContactCompany>();
                    var request = DAOUsers.getRequestContact(userId);
                    var friend = DAOUsers.GetListContact(userId, 0, 10000);
                    foreach (UserDB member in getListContact)
                    {
                        if (member.id == userId)
                        {
                            countLoad--;
                            continue;
                        }
                        ContactCompany userInfo = new ContactCompany(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), companyId);
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

                        if (friend.Any(x => x.id == member.id)) userInfo.FriendStatus = "friend";
                        else
                        {
                            int requestIndex = request.FindIndex(x => x.contactId == member.id);
                            if (requestIndex > -1)
                            {
                                userInfo.FriendStatus = request[requestIndex].status;
                            }
                        }

                        listContact.Add(userInfo);
                    }
                    userAPI.data = new DataUserCompany();
                    userAPI.data.result = true;
                    userAPI.data.user_list = listContact;
                    userAPI.data.total = countLoad + countContact;
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

        [HttpPost("GetContactList")]
        [AllowAnonymous]
        public APIUserCompany GetContactList()
        {
            APIUserCompany userAPI = new APIUserCompany();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["ID"]);
                int companyId = Convert.ToInt32(httpRequest.Form["CompanyId"]);
                int countContact = Convert.ToInt32(httpRequest.Form["countContact"]);
                int countLoad = Convert.ToInt32(httpRequest.Form["countLoad"]);
                if (userId != 0 && companyId != 0)
                {
                    List<UserDB> getListContact = DAOUsers.GetListContactCompany(companyId, countContact, countLoad == 0 ? 500 : countLoad);
                    List<ContactCompany> listContact = new List<ContactCompany>();
                    var request = DAOUsers.getRequestContact(userId);
                    var friend = DAOUsers.GetListContact(userId, 0, 10000);
                    foreach (UserDB member in getListContact)
                    {
                        if (member.id == userId)
                        {
                            countLoad--;
                            continue;
                        }
                        ContactCompany userInfo = new ContactCompany(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), companyId);
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

                        if (friend.Any(x => x.id == member.id)) userInfo.FriendStatus = "friend";
                        else
                        {
                            int requestIndex = request.FindIndex(x => x.contactId == member.id);
                            if (requestIndex > -1)
                            {
                                userInfo.FriendStatus = request[requestIndex].status;
                            }
                        }

                        listContact.Add(userInfo);
                    }

                    if (listContact.Count < countLoad)
                    {
                        List<UserDB> getListContact1 = DAOUsers.GetListContact(userId, countContact, countLoad == 0 ? 500 : countLoad);
                        foreach (UserDB member in getListContact1)
                        {
                            if (member.id == userId)
                            {
                                countLoad--;
                                continue;
                            }
                            ContactCompany userInfo = new ContactCompany(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), companyId);
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

                            if (friend.Any(x => x.id == member.id)) userInfo.FriendStatus = "friend";
                            else
                            {
                                int requestIndex = request.FindIndex(x => x.contactId == member.id);
                                if (requestIndex > -1)
                                {
                                    userInfo.FriendStatus = request[requestIndex].status;
                                }
                            }

                            listContact.Add(userInfo);
                        }
                    }

                    userAPI.data = new DataUserCompany();
                    userAPI.data.result = true;
                    userAPI.data.user_list = listContact;
                    userAPI.data.total = countLoad + countContact;
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

        //(4/8/22)ok
        [HttpPost("GetInfoUser")]
        [AllowAnonymous]
        public APIUser GetInfoUser()
        {
            APIUser userAPI = new APIUser();
            try
            {
                var httpRequest = HttpContext.Request;
                int ID = Convert.ToInt32(httpRequest.Form["ID"]);
                string secretCode = httpRequest.Form["secretCode"];
                if (ID != 0)
                {
                    List<UserDB> getUser = DAOUsers.GetUsersById(ID);

                    if (getUser.Count != 0)
                    {
                        userAPI.data = new DataUser();
                        userAPI.data.result = true;
                        userAPI.data.message = "lấy thông tin thành công";
                        userAPI.data.user_info = getInforUser(getUser);
                        QRFriend friend = new QRFriend(userAPI.data.user_info.ID, userAPI.data.user_info.Type365);
                        QRInfo qr = new QRInfo("QRAddFriend", friend, DateTime.Now.AddHours(1));
                        string code = JsonConvert.SerializeObject(qr).MaxEncode();
                        userAPI.data.user_info.userQr = code;
                        if (!String.IsNullOrEmpty(secretCode) && secretCode.Equals("f0c8f3b2312aa616be5f1ce38cd96da6"))
                        {

                            userAPI.data.countConversation = DAOConversation.GetCountConversation(ID);
                            userAPI.data.currentTime = DateTime.Now.Ticks;
                            userAPI.data.user_info.secretCode = getUser[0].secretCode;
                        }
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "Id không tồn tại";
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

        //(4/8/22)ok
        [HttpPost("GetUserName")]
        [AllowAnonymous]
        public APIUser GetUserName([FromForm] User user)
        {
            APIUser userAPI = new APIUser();
            try
            {
                if (user.ID != 0)
                {
                    List<UserDB> getUser = DAOUsers.GetUserById(user.ID);

                    if (getUser.Count != 0)
                    {
                        userAPI.data = new DataUser();
                        userAPI.data.result = true;
                        userAPI.data.userName = getUser[0].userName;
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "Id không tồn tại";
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

        //(4/8/22)ok
        [HttpPost("GetInfoUserSendMessage")]
        [AllowAnonymous]
        public APIUser GetInfoUserSendMessage([FromForm] User user)
        {
            APIUser userAPI = new APIUser();
            try
            {
                if (user.ID != 0)
                {
                    List<UserDB> getUser = DAOUsers.GetUserSendMessById(user.ID);
                    if (getUser.Count != 0)
                    {
                        User userCurrent = new User();
                        userCurrent.ID = user.ID;
                        userCurrent.UserName = getUser[0].userName;
                        userCurrent.AvatarUser = getUser[0].avatarUser;
                        if (String.IsNullOrWhiteSpace(userCurrent.AvatarUser.Trim()))
                        {
                            string letter = RemoveUnicode(userCurrent.UserName.Substring(0, 1).ToLower()).ToUpper();
                            try
                            {
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    userCurrent.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_1" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    userCurrent.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_2" + ".png";
                                }
                                if (Encoding.ASCII.GetBytes(letter)[0] % 4 == 0)
                                {
                                    userCurrent.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_3" + ".png";
                                }
                                else
                                {
                                    userCurrent.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                                }
                            }
                            catch (Exception ex)
                            {
                                userCurrent.LinkAvatar = "https://mess.timviec365.vn/avatar/" + letter + "_4" + ".png";
                            }
                        }
                        else
                        {
                            userCurrent.AvatarUser = "https://mess.timviec365.vn/avatarUser/" + userCurrent.ID + "/" + userCurrent.AvatarUser;
                            userCurrent.LinkAvatar = userCurrent.AvatarUser;
                        }
                        userAPI.data = new DataUser();
                        userAPI.data.result = true;
                        userAPI.data.user_info = userCurrent;
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "Id không tồn tại";
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

        //ok
        [HttpPost("GetListRequest")]
        [AllowAnonymous]
        public APIRequestContact GetListRequest([FromForm] User user)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (user.ID != 0)
                {
                    List<RequestContact> getListRequest = DAOUsers.getRequestContact(user.ID);
                    if (getListRequest.Count != 0)
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.ListRequestContact = getListRequest;
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "User không có lời mời nào";
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
        //ok
        [HttpPost("GetListRequestFriend")]
        [AllowAnonymous]
        public APIRequestFriend GetListRequestFriend([FromForm] User user)
        {
            APIRequestFriend requestAPI = new APIRequestFriend();
            try
            {
                var http = HttpContext.Request;
                int contactId = 0;
                if (!string.IsNullOrEmpty(http.Form["ContactId"]))
                {
                    contactId = Convert.ToInt32(http.Form["ContactId"]);
                }
                if (user.ID != 0)
                {
                    List<RequestFriend> getListRequest = DAOUsers.getRequestFriend(user.ID, contactId);
                    if (getListRequest.Count != 0)
                    {
                        requestAPI.data = new DataRequestFriend();
                        requestAPI.data.result = true;
                        requestAPI.data.ListRequestFriend = getListRequest;
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "User không có lời mời nào";
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

        [HttpPost("GetRequestList")]
        [AllowAnonymous]
        public APIRequestContact GetRequestList([FromForm] User user)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                var http = HttpContext.Request;
                int contactId = Convert.ToInt32(http.Form["ContactId"]);
                if (user.ID != 0)
                {
                    List<RequestContact1> getListRequest = DAOUsers.getRequestContact1(user.ID, contactId);
                    List<RequestContact1> getListUserRequest = DAOUsers.getUserRequestContact1(user.ID, contactId);
                    if (getListRequest.Count != 0 || getListUserRequest.Count != 0)
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.RequestListContact = getListRequest;
                        requestAPI.data.ListUserSendRequest = getListUserRequest;
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "User không có lời mời nào";
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

        //(5/8/22)ok
        [HttpPost("AddFriend")]
        [AllowAnonymous]
        public APIRequestContact AddFriend([FromForm] RequestContact requestContact)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (requestContact.userId != 0 && requestContact.contactId != 0)
                {
                    int addFriend = DAOUsers.AddFriend(requestContact.userId, requestContact.contactId, requestContact.type365);
                    if (addFriend > 0)
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.message = "Gửi lời mời kết bạn thành công";
                        requestAPI.data.conversationId = GetConversationId(requestContact.userId, requestContact.contactId);
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "User đã tồn tại lời mời";
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

        //ok
        [HttpPost("DeleteContact")]
        [AllowAnonymous]
        public APIRequestContact DeleteContact([FromForm] RequestContact requestContact)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (requestContact.userId != 0 && requestContact.contactId != 0)
                {
                    int addFriend = DAOUsers.DeleteContact(requestContact.userId, requestContact.contactId);
                    if (addFriend > 0)
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.message = "xóa liên hệ thành công";
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "xóa liên hệ thất bại";
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

        //ok
        [HttpPost("AcceptRequestAddFriend")]
        [AllowAnonymous]
        public APIRequestContact AcceptRequestAddFriend([FromForm] RequestContact requestContact)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (requestContact.userId != 0 && requestContact.contactId != 0)
                {
                    int deleteRequest = DAOUsers.AcceptRequestAddFriend(requestContact.userId, requestContact.contactId);

                    if (deleteRequest > 0)
                    {
                        DAOUsers.AddNewContact(requestContact.userId, requestContact.contactId);
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.message = "Châp nhận lời mời thành công";
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "Lời mời không tồn tại";
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

        //ok
        [HttpPost("DecilineRequestAddFriend")]
        [AllowAnonymous]
        public APIRequestContact DecilineRequestAddFriend([FromForm] RequestContact requestContact)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (requestContact.userId != 0 && requestContact.contactId != 0)
                {
                    int deleteRequest = DAOUsers.DecilineRequestAddFriend(requestContact.userId, requestContact.contactId);

                    if (deleteRequest > 0)
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.message = "Từ chối lời mời thành công";
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "Lời mời không tồn tại";
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

        //ok
        [HttpPost("DeleteRequestAddFriend")]
        [AllowAnonymous]
        public APIRequestContact DeleteRequestAddFriend([FromForm] RequestContact requestContact)
        {
            APIRequestContact requestAPI = new APIRequestContact();
            try
            {
                if (requestContact.userId != 0 && requestContact.contactId != 0)
                {
                    int deleteRequest = DAOUsers.DeleteRequestAddFriend(requestContact.userId, requestContact.contactId);

                    if (deleteRequest > 0)
                    {
                        requestAPI.data = new DataRequest();
                        requestAPI.data.result = true;
                        requestAPI.data.message = "Xóa lời mời thành công";
                    }
                    else
                    {
                        requestAPI.error = new Error();
                        requestAPI.error.code = 200;
                        requestAPI.error.message = "Lời mời không tồn tại";
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

        //(4/8/22)ok
        [HttpPost("UpdateUserName")]
        [AllowAnonymous]
        public APIOTP UpdateUserName([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.UserName != null && user.ID365 != 0)
                {
                    List<UserDB> Getuser = DAOUsers.GetUserByID365(user.ID365, user.Type365);
                    if (Getuser.Count > 0)
                    {
                        User userUpdate = getInforUser(Getuser);
                        if (DAOUsers.UpdateNameUser(userUpdate.ID, user.UserName) > 0)
                        {
                            APIotp.data = new DataOTP();
                            APIotp.data.result = true;
                            APIotp.data.message = "Cập nhật tên thành công";

                            MultipartFormDataContent content = new MultipartFormDataContent();
                            content.Add(new StringContent(user.UserName), "name");
                            content.Add(new StringContent(userUpdate.Email), "email");
                            content.Add(new StringContent(userUpdate.Type365.ToString()), "type");
                            HttpClient httpClient = new HttpClient();
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                            InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                            if (receiveInfo.data != null)
                            {
                            }
                            WIO.EmitAsync("changeName", userUpdate.ID, user.UserName);
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = "Email không tồn tại";
                        }
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Thiếu thông tin truyền lên";
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

        //(4/8/22)ok
        [HttpPost("UpdatePasswordUser")]
        [AllowAnonymous]
        public APIOTP UpdatePasswordUser([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.Password != null && user.ID365 != 0 && user.Type365 != 0)
                {
                    if (DAOUsers.GetUserByID365(user.ID365, user.Type365).Count > 0)
                    {
                        User userUpdate = getInforUser(DAOUsers.GetUserByID365(user.ID365, user.Type365));
                        if (DAOUsers.UpdatePassword(userUpdate.ID, ToMD5(user.Password)) > 0)
                        {
                            APIotp.data = new DataOTP();
                            APIotp.data.result = true;
                            APIotp.data.message = "Cập nhật mật khẩu thành công";

                            MultipartFormDataContent content = new MultipartFormDataContent();
                            content.Add(new StringContent(ToMD5(user.Password)), "pass");
                            content.Add(new StringContent(userUpdate.Email), "email");
                            content.Add(new StringContent(userUpdate.Type365.ToString()), "type");
                            HttpClient httpClient = new HttpClient();
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                            InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                            if (receiveInfo.data != null)
                            {
                            }
                            WIO.EmitAsync("changedPassword", userUpdate.ID, user.Password);
                        }
                        else
                        {
                            APIotp.error = new Error();
                            APIotp.error.code = 200;
                            APIotp.error.message = "Email không tồn tại";
                        }
                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Thiếu thông tin truyền lên";
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

        //(4/8/22)ok
        [HttpPost("UpdateAvatarUser")]
        [AllowAnonymous]
        public APIOTP UpdateAvatarUser([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.AvatarUser != null && user.ID365 != 0)
                {
                    List<UserDB> dataUser = DAOUsers.GetUserByID365(user.ID365, user.Type365);
                    if (dataUser.Count == 0)
                    {
                        dataUser = DAOUsers.GetUserByID365(user.ID365, 0);
                    }
                    if (dataUser.Count > 0)
                    {
                        User userUpdate = getInforUser(dataUser);
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] dataArr = new byte[1];
                            long bytesize;
                            if (user.Type365 == 1)
                            {
                                try
                                {
                                    dataArr = webClient.DownloadData("https://chamcong.24hpay.vn/upload/company/logo/" + user.AvatarUser);
                                    bytesize = dataArr.Length;
                                }
                                catch (WebException ex)
                                {

                                }
                            }
                            else
                            {
                                try
                                {
                                    dataArr = webClient.DownloadData("https://chamcong.24hpay.vn/upload/employee/" + user.AvatarUser);
                                    bytesize = dataArr.Length;
                                }
                                catch (WebException ex)
                                {

                                }
                            }
                            if (dataArr.Length > 1)
                            {
                                var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                                if (!Directory.Exists(filePath))
                                {
                                    Directory.CreateDirectory(filePath);
                                }
                                if (!Directory.Exists(filePath + @"\" + userUpdate.ID))
                                {
                                    Directory.CreateDirectory(filePath + @"\" + userUpdate.ID);
                                }
                                System.IO.FileInfo[] di = (new DirectoryInfo(filePath + @"\" + userUpdate.ID)).GetFiles();
                                if (di.Length > 0)
                                {
                                    for (int i = 0; i < di.Length; i++)
                                    {
                                        di[i].Delete();
                                    }
                                }
                                string fileName = DateTime.Now.Ticks + "_" + userUpdate.ID + ".jpg";
                                try
                                {
                                    FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + userUpdate.ID + @"\" + fileName);
                                }
                                catch (Exception ex)
                                {
                                    System.IO.File.WriteAllBytes(filePath + @"\" + userUpdate.ID + @"\" + fileName, dataArr);
                                }
                                MultipartFormDataContent content = new MultipartFormDataContent();
                                content.Add(new StreamContent(new MemoryStream(dataArr)), "file", fileName);
                                content.Add(new StringContent(userUpdate.Email), "email");
                                content.Add(new StringContent(userUpdate.Type365.ToString()), "type");
                                HttpClient httpClient = new HttpClient();
                                Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                                InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                                if (receiveInfo.data != null)
                                {
                                }
                                DAOUsers.UpdateAvatarUser(fileName, userUpdate.ID);
                                APIotp.data = new DataOTP();
                                APIotp.data.result = true;
                                APIotp.data.message = "Cập nhật avatar thành công";
                                WIO.EmitAsync("changeAvatarUser", userUpdate.ID, "https://mess.timviec365.vn/avatarUser/" + userUpdate.ID + @"/" + fileName);
                            }
                        }

                    }
                    else
                    {
                        APIotp.error = new Error();
                        APIotp.error.code = 200;
                        APIotp.error.message = "Tài khoản không tồn tại";
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

        //(4/8/22)ok
        [HttpPost("searchContactInHomePage")]
        [AllowAnonymous]
        public APIUser searchContactInHomePage([FromForm] Conversation con)
        {
            APIUser userAPI = new APIUser();
            try
            {
                if (con.senderId != 0)
                {
                    if (con.message == null)
                    {
                        con.message = "";
                    }
                    List<UserDB> getListContact = DAOUsers.searchContactInHomePage(con.message, con.senderId, con.companyId, con.countMessage == 0 ? 20 : con.countMessage);
                    if (getListContact.Count > 0)
                    {
                        var listRequest = DAOUsers.getRequestContact(con.senderId);
                        var listFriend = DAOUsers.GetListContact(con.senderId, 0, 10000);
                        List<Contact> listContact = new List<Contact>();
                        foreach (UserDB member in getListContact)
                        {
                            Contact userInfo = new Contact(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), member.companyId, member.type365);
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
                        userAPI.data = new DataUser();
                        userAPI.data.user_list = listContact;
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

        //(4/8/22)ok
        [HttpPost("searchByCompanyContactInHomePage")]
        [AllowAnonymous]
        public APIUser searchByCompanyContactInHomePage([FromForm] Conversation con)
        {
            APIUser userAPI = new APIUser();
            try
            {
                if (con.senderId != 0 && con.companyId != 0)
                {
                    if (con.message == null)
                    {
                        con.message = "";
                    }
                    List<UserDB> getListContact = DAOUsers.searchByCompanyContactInHomePage(con.message, con.senderId, con.companyId, con.countMessage == 0 ? 20 : con.countMessage);
                    var listRequest = DAOUsers.getRequestContact(con.senderId);
                    var listFriend = DAOUsers.GetListContact(con.senderId, 0, 10000);
                    if (getListContact.Count > 0)
                    {
                        List<Contact> listContact = new List<Contact>();
                        foreach (UserDB member in getListContact)
                        {
                            Contact userInfo = new Contact(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), member.companyId, member.type365);
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
                        userAPI.data = new DataUser();
                        userAPI.data.user_list = listContact;
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

        //ok
        [HttpPost("UpdateNotificationPayoff")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationPayoff([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationPayoff(user.ID, user.NotificationPayoff) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationCalendar")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationCalendar([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationCalendar(user.ID, user.NotificationCalendar) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationOffer")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationOffer([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationOffer(user.ID, user.NotificationOffer) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationReport")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationReport([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationReport(user.ID, user.NotificationReport) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationPersionalChange")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationPersionalChange([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationPersionalChange(user.ID, user.NotificationPersonnelChange) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationRewardDiscipline")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationRewardDiscipline([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationRewardDiscipline(user.ID, user.NotificationRewardDiscipline) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationNewPersonnel")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationNewPersonnel([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationNewPersonnel(user.ID, user.NotificationNewPersonnel) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationChangeProfile")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationChangeProfile([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationChangeProfile(user.ID, user.NotificationChangeProfile) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateNotificationTransferAsset")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationTransferAsset([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateNotificationTransferAsset(user.ID, user.NotificationTransferAsset) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        //ok
        [HttpPost("UpdateAcceptMessStranger")]
        [AllowAnonymous]
        public APIOTP UpdateAcceptMessStranger([FromForm] User user)
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (user.ID != 0)
                {
                    if (DAOUsers.UpdateStatusAcceptMessStranger(user.ID, user.AcceptMessStranger) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("GetListOfferContactByPhone")]
        [AllowAnonymous]
        public APIUser GetListOfferContactByPhone()
        {
            var httpRequest = HttpContext.Request;
            string phone = httpRequest.Form["phone"];
            int userId = Convert.ToInt32(httpRequest.Form["userId"]);
            int companyId = Convert.ToInt32(httpRequest.Form["companyId"]);
            APIUser userAPI = new APIUser();
            try
            {
                if (!string.IsNullOrEmpty(phone) && userId != 0)
                {
                    List<UserDB> getListContact = DAOUsers.GetListOfferContactByPhone(userId, companyId, JsonConvert.DeserializeObject<string[]>(phone));
                    List<Contact> listContact = new List<Contact>();
                    foreach (UserDB member in getListContact)
                    {
                        Contact userInfo = new Contact(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), member.companyId, member.type365);
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
                        listContact.Add(userInfo);
                    }
                    if (listContact.Count > 0)
                    {
                        listContact.Sort(new CompareContactByName());
                        userAPI.data = new DataUser();
                        userAPI.data.result = true;
                        userAPI.data.user_list = listContact;
                    }
                    else
                    {
                        userAPI.error = new Error();
                        userAPI.error.code = 200;
                        userAPI.error.message = "user không tồn tại";
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

        [HttpPost("GetListSuggesContact")]
        [AllowAnonymous]
        public APIUser GetListSuggesContact([FromForm] User user)
        {
            APIUser userAPI = new APIUser();
            try
            {
                var http = HttpContext.Request;
                int CompanyId = 0;
                if (!string.IsNullOrEmpty(http.Form["CompanyId"]))
                {
                    CompanyId = Convert.ToInt32(http.Form["CompanyId"]);
                }
                else
                {
                    var getUser = DAOUsers.GetInforUserById(user.ID);
                    if (getUser.Count > 0)
                    {
                        CompanyId = getUser[0].companyId;
                    }
                }

                if (user.ID != 0)
                {
                    List<UserDB> getListContact = DAOUsers.getListSuggesContact(user.ID, user.CompanyId, 20, 10);
                    List<Contact> listContact = new List<Contact>();
                    foreach (UserDB member in getListContact)
                    {
                        Contact userInfo = new Contact(member.id, member.userName, member.email, member.avatarUser, member.status, member.active, member.isOnline, member.looker, member.statusEmotion, member.lastActive.ToLocalTime(), member.companyId, member.type365);
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
                        listContact.Add(userInfo);
                    }
                    userAPI.data = new DataUser();
                    userAPI.data.result = true;
                    userAPI.data.message = "lấy thông tin thành công ";
                    userAPI.data.user_list = listContact;
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

        [HttpPost("AddNewFieldUser")]
        [AllowAnonymous]
        public APIConversation AddNewFieldUser()
        {
            APIConversation APIConversation = new APIConversation();
            int count = DAOUsers.AddNewFieldUser();
            if (count != 0)
            {
                APIConversation.data = new DataConversation();
                APIConversation.data.result = true;
                APIConversation.data.message = "Sửa Trạng thái user thành công :" + count;
            }
            else
            {
                APIConversation.error = new Error();
                APIConversation.error.code = 200;
                APIConversation.error.message = "Sửa Trạng thái user thất bại";
            }
            return APIConversation;
        }

        [HttpPost("GetInfoUserFromHHP365")]
        [AllowAnonymous]
        public APIUserHHP365 GetInfoUserFromHHP365()
        {
            APIUserHHP365 responseUser = new APIUserHHP365();
            try
            {
                var httpRequest = HttpContext.Request;
                int typeUser = Convert.ToInt32(httpRequest.Form["typeUser"]);
                string nameUser = httpRequest.Form["nameUser"];
                string passUser = httpRequest.Form["passUser"];
                string avatarUser = httpRequest.Form["avatarUser"];
                string emailUser = httpRequest.Form["emailUser"];
                string fromWeb = httpRequest.Form["fromWeb"];
                string address = httpRequest.Form["address"];
                string phone = httpRequest.Form["phone"];
                int cc365 = 0;
                if (!string.IsNullOrEmpty(httpRequest.Form["cc365"]))
                {
                    cc365 = Convert.ToInt32(httpRequest.Form["cc365"]);
                }
                int flagCheckRequest = 0;
                if (String.IsNullOrEmpty(nameUser) || String.IsNullOrEmpty(emailUser))
                {
                    flagCheckRequest = 1;
                }
                if (String.IsNullOrEmpty(fromWeb))
                {
                    flagCheckRequest = 2;
                }
                if (flagCheckRequest != 0)
                {
                    responseUser.error = new Error();
                    if (flagCheckRequest == 1)
                    {
                        responseUser.error.code = 100;
                        responseUser.error.message = "Thiếu thông tin người dùng";
                    }
                    else
                    {
                        responseUser.error.code = 300;
                        responseUser.error.message = "Thiếu thông tin trang web";
                    }
                }
                else
                {
                    List<UserDB> dataUser = new List<UserDB>();
                    if (cc365 != 1)
                    {
                        using (WebClient web = new WebClient())
                        {
                            web.QueryString.Add("email", emailUser);
                            web.QueryString.Add("password", passUser);
                            web.QueryString.Add("name", nameUser);
                            web.QueryString.Add("phone", phone);
                            web.QueryString.Add("address", address);
                            web.QueryString.Add("from", fromWeb);
                            web.QueryString.Add("type", typeUser.ToString());
                            web.UploadValuesAsync(new Uri("https://chamcong.24hpay.vn/api_chat365/rg_user_form_chat.php"), web.QueryString);
                        }
                    }
                    dataUser = DAOUsers.GetUsersByEmailAndType365(emailUser, typeUser);
                    if (dataUser.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(passUser) && (dataUser[0].password.Equals(passUser) || String.IsNullOrEmpty(dataUser[0].password)))
                        {
                            if (String.IsNullOrEmpty(dataUser[0].password))
                            {
                                DAOUsers.UpdatePassword(dataUser[0].id, passUser);
                            }
                            if (dataUser[0].idTimViec == 0)
                            {
                                DAOUsers.UpdateIdTimViec1(dataUser[0].id, getIdTimViec(dataUser[0].email, dataUser[0].type365.ToString()));
                            }
                            if (fromWeb == "timviec365" && dataUser[0].fromWeb != fromWeb)
                            {
                                DAOUsers.UpdateFromWeb(dataUser[0].id, fromWeb);
                                dataUser[0].fromWeb = "timviec365";
                            }
                            responseUser.data = new DataUserHHP365();
                            responseUser.data.result = true;
                            responseUser.data.message = "lấy thông tin user thành công";
                            responseUser.data.userId = dataUser[0].id;
                            responseUser.data.secretCode = dataUser[0].secretCode;
                            responseUser.data.fromWeb = dataUser[0].fromWeb;
                        }
                        else
                        {
                            responseUser.data = new DataUserHHP365();
                            responseUser.data.result = true;
                            responseUser.data.message = "thông tin mật khẩu không chính xác";
                            responseUser.data.userId = dataUser[0].id;
                            responseUser.data.fromWeb = dataUser[0].fromWeb;
                        }
                    }
                    else
                    {
                        using (WebClient webClient = new WebClient())
                        {
                            byte[] dataArr = new byte[1];
                            long bytesize;
                            if (!String.IsNullOrWhiteSpace(avatarUser))
                            {
                                dataArr = webClient.DownloadData(avatarUser);
                                bytesize = dataArr.Length;
                            }
                            int userId = DAOUsers.InsertNewUser(nameUser, 0, 0, typeUser == 1 || typeUser == 2 ? typeUser : 0, emailUser, String.IsNullOrEmpty(passUser) ? "" : passUser, 0, "", fromWeb);
                            if (userId > 0)
                            {
                                if (dataArr.Length > 1)
                                {
                                    var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                                    if (!Directory.Exists(filePath))
                                    {
                                        Directory.CreateDirectory(filePath);
                                    }
                                    if (!Directory.Exists(filePath + @"\" + userId))
                                    {
                                        Directory.CreateDirectory(filePath + @"\" + userId);
                                    }
                                    System.IO.DirectoryInfo di = new DirectoryInfo(filePath + @"\" + userId);
                                    if (di.GetFiles().Length > 0)
                                    {
                                        di.GetFiles()[0].Delete();
                                    }
                                    string fileName = DateTime.Now.Ticks + "_" + userId + ".jpg";
                                    try
                                    {
                                        FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + userId + @"\" + fileName);
                                    }
                                    catch (Exception ex)
                                    {
                                        System.IO.File.WriteAllBytes(filePath + @"\" + userId + @"\" + fileName, dataArr);
                                    }
                                    DAOUsers.UpdateAvatarUser(fileName, userId);
                                }
                                List<UserDB> user = DAOUsers.GetInforUserById(userId);
                                if (user.Count > 0)
                                {
                                    if (user[0].idTimViec == 0)
                                    {
                                        DAOUsers.UpdateIdTimViec1(user[0].id, getIdTimViec(user[0].email, user[0].type365.ToString()));
                                    }
                                    responseUser.data = new DataUserHHP365();
                                    responseUser.data.result = true;
                                    responseUser.data.message = "lấy thông tin user thành công";
                                    responseUser.data.userId = user[0].id;
                                    responseUser.data.secretCode = user[0].secretCode;
                                    responseUser.data.fromWeb = user[0].fromWeb;
                                }
                                else
                                {
                                    responseUser.error = new Error();
                                    responseUser.error.code = 400;
                                    responseUser.error.message = "đã có lỗi xảy ra";
                                }
                            }
                            else
                            {
                                responseUser.error = new Error();
                                responseUser.error.code = 300;
                                responseUser.error.message = "Có lỗi sảy ra";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                responseUser.error = new Error();
                responseUser.error.code = 300;
                responseUser.error.message = ex.ToString();
            }

            return responseUser;
        }

        [HttpPost("ConfirmPasswordFromHHP365")]
        [AllowAnonymous]
        public APIUserHHP365 ConfirmPasswordFromHHP365()
        {
            APIUserHHP365 responseUser = new APIUserHHP365();
            try
            {
                var httpRequest = HttpContext.Request;
                int idUser = Convert.ToInt32(httpRequest.Form["idUser"]);
                string passUser = httpRequest.Form["passUser"];

                if (idUser == 0 || String.IsNullOrEmpty(passUser))
                {
                    responseUser.error = new Error();
                    responseUser.error.code = 100;
                    responseUser.error.message = "Thiếu thông tin người dùng";
                }
                else
                {
                    List<UserDB> dataUser = DAOUsers.GetUsersByPasswordAndId(passUser, idUser);
                    if (dataUser.Count > 0)
                    {
                        responseUser.data = new DataUserHHP365();
                        responseUser.data.result = true;
                        responseUser.data.message = "lấy thông tin user thành công";
                        responseUser.data.userId = dataUser[0].id;
                        responseUser.data.secretCode = dataUser[0].secretCode;
                        responseUser.data.fromWeb = dataUser[0].fromWeb;
                    }
                    else
                    {
                        responseUser.error = new Error();
                        responseUser.error.code = 500;
                        responseUser.error.message = "sai mật khẩu xác thực";
                    }
                }
            }
            catch (Exception ex)
            {
                responseUser.error = new Error();
                responseUser.error.code = 300;
                responseUser.error.message = ex.ToString();
            }

            return responseUser;
        }
        [HttpPost("GetConversationIdFromHHP365")]
        [AllowAnonymous]
        public APIUser GetConversationIdFromHHP365()
        {
            APIUser responseUser = new APIUser();
            try
            {
                var httpRequest = HttpContext.Request;
                int idUser = Convert.ToInt32(httpRequest.Form["idUser"]);
                int idContact = Convert.ToInt32(httpRequest.Form["idContact"]);
                int typeContact = Convert.ToInt32(httpRequest.Form["typeContact"]);
                string nameContact = httpRequest.Form["nameContact"];
                string passContact = httpRequest.Form["passContact"];
                string avatarContact = httpRequest.Form["avatarContact"];
                string emailContact = httpRequest.Form["emailContact"];
                string fromWeb = httpRequest.Form["fromWeb"];

                int flagCheckRequest = 0;
                if (idUser == 0)
                {
                    flagCheckRequest = 1;
                }
                if ((idContact == 0) && (String.IsNullOrEmpty(nameContact) || String.IsNullOrEmpty(emailContact) || String.IsNullOrEmpty(passContact)))
                {
                    flagCheckRequest = 2;
                }
                if (String.IsNullOrEmpty(fromWeb))
                {
                    flagCheckRequest = 3;
                }
                if (flagCheckRequest != 0)
                {
                    responseUser.error = new Error();
                    if (flagCheckRequest == 1)
                    {
                        responseUser.error.code = 100;
                        responseUser.error.message = "Thiếu thông tin người gửi";
                    }
                    else if (flagCheckRequest == 2)
                    {
                        responseUser.error.code = 200;
                        responseUser.error.message = "Thiếu thông tin người nhận";
                    }
                    else
                    {
                        responseUser.error.code = 300;
                        responseUser.error.message = "Thiếu thông tin trang web";
                    }
                }
                else
                {
                    List<UserDB> dataUser = DAOUsers.GetUsersById(idUser);
                    if (dataUser.Count > 0)
                    {
                        List<UserDB> dataContact = new List<UserDB>();
                        if (idContact != 0)
                        {
                            dataContact = DAOUsers.GetUsersById(idContact);
                        }
                        else
                        {
                            dataContact = DAOUsers.GetUsersByEmailAndType365(emailContact, 0);
                            if (dataUser.Count == 0 && (typeContact == 2 || typeContact == 0))
                            {
                                dataUser = DAOUsers.GetUsersByEmailAndType365(emailContact, typeContact == 2 ? 0 : 2);
                            }
                            if (dataContact.Count == 0)
                            {
                                using (WebClient webClient = new WebClient())
                                {
                                    byte[] dataArr = new byte[1];
                                    long bytesize;
                                    if (!String.IsNullOrWhiteSpace(avatarContact))
                                    {
                                        dataArr = webClient.DownloadData(avatarContact);
                                        bytesize = dataArr.Length;
                                    }
                                    int userId = DAOUsers.InsertNewUser(nameContact, 0, 0, typeContact == 1 || typeContact == 2 ? typeContact : 0, emailContact, passContact, 0, "", fromWeb);
                                    if (userId > 0)
                                    {
                                        if (dataArr.Length > 1)
                                        {
                                            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                                            if (!Directory.Exists(filePath))
                                            {
                                                Directory.CreateDirectory(filePath);
                                            }
                                            if (!Directory.Exists(filePath + @"\" + userId))
                                            {
                                                Directory.CreateDirectory(filePath + @"\" + userId);
                                            }
                                            System.IO.DirectoryInfo di = new DirectoryInfo(filePath + @"\" + userId);
                                            if (di.GetFiles().Length > 0)
                                            {
                                                di.GetFiles()[0].Delete();
                                            }
                                            string fileName = DateTime.Now.Ticks + "_" + userId + ".jpg";
                                            try
                                            {
                                                FileController.Compressimage(new MemoryStream(dataArr), filePath + @"\" + userId + @"\" + fileName);
                                            }
                                            catch (Exception ex)
                                            {
                                                System.IO.File.WriteAllBytes(filePath + @"\" + userId + @"\" + fileName, dataArr);
                                            }
                                            DAOUsers.UpdateAvatarUser(fileName, userId);
                                        }
                                        dataContact = DAOUsers.GetInforUserById(userId);
                                    }
                                    else
                                    {
                                        responseUser.error = new Error();
                                        responseUser.error.code = 400;
                                        responseUser.error.message = "đã có lỗi xảy ra";
                                    }
                                }
                            }
                        }
                        if (dataContact.Count > 0)
                        {
                            responseUser.data = new DataUser();
                            responseUser.data.result = true;
                            responseUser.data.message = "lấy thông tin user thành công";
                            responseUser.data.conversationId = GetConversationId(dataUser[0].id, dataContact[0].id);
                        }
                        else
                        {
                            responseUser.error = new Error();
                            responseUser.error.code = 300;
                            responseUser.error.message = "Sai thông tin người nhận";
                        }
                    }
                    else
                    {
                        responseUser.error = new Error();
                        responseUser.error.code = 300;
                        responseUser.error.message = "Sai thông tin người gửi";
                    }
                }
            }
            catch (Exception ex)
            {
                responseUser.error = new Error();
                responseUser.error.code = 300;
                responseUser.error.message = ex.ToString();
            }

            return responseUser;
        }

        [HttpPost("DeleteAccount")]
        [AllowAnonymous]
        public APIOTP DeleteAccount()
        {
            APIOTP api = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                string email = httpRequest.Form["email"];
                int id = Convert.ToInt32(httpRequest.Form["id"]);
                int id365 = Convert.ToInt32(httpRequest.Form["id365"]);
                if (!string.IsNullOrEmpty(email) && id != 0 && id365 != 0)
                {
                    if (DAOUsers.checkMailIdAndId365(email, id, id365).Count > 0)
                    {
                        using (HttpClient httpClient = new HttpClient())
                        {
                            var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "ep_id", id365.ToString() }, { "ep_mail", email } });
                            Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/service/delete_ep_from_chat.php", infoLogin);
                            string re = response.Result.Content.ReadAsStringAsync().Result;
                            if (re.Contains("<br />")) re = re.Substring(re.LastIndexOf("<br />") + 7);
                            InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(re);
                            if (receiveInfo.data != null)
                            {
                                if (DAOUsers.DeleteAccount(id) > 0)
                                {
                                    api.data = new DataOTP();
                                    api.data.result = true;
                                    api.data.message = "Xóa tài khoản thành công";
                                }
                                else
                                {
                                    api.error = new Error();
                                    api.error.code = 300;
                                    api.error.message = "User này không tồn tại";
                                }
                            }
                            else
                            {
                                api.error = new Error();
                                api.error.code = 300;
                                api.error.message = "User này không tồn tại";
                            }
                        }
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 300;
                        api.error.message = "User này không tồn tại";
                    }
                }
                else
                {
                    api.error = new Error();
                    api.error.code = 300;
                    api.error.message = "Thông tin truyền lên không đầy đủ";
                }
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.code = 300;
                api.error.message = ex.ToString();
            }
            return api;
        }

        [HttpPost("runxx")]
        [AllowAnonymous]
        public APIOTP runxx()
        {
            APIOTP api = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                int id = Convert.ToInt32(httpRequest.Form["id"]);
                WIO.EmitAsync("LogoutAllDevice", id);
                api.data = new DataOTP();
                api.data.result = true;
                api.data.message = "Xóa tài khoản thành công" + id;
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.code = 300;
                api.error.message = ex.ToString();
            }
            return api;
        }

        [HttpPost("getUserNonIdTV")]
        [AllowAnonymous]
        public APIOTP getUserNonIdTV()
        {
            APIOTP api = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                var z = DAOUsers.getNonIdTv();
                foreach (var item in z)
                {
                    DAOUsers.UpdateIdTimViec1(item.id, getIdTimViec(item.email, item.type365.ToString()));
                }
                api.data = new DataOTP();
                api.data.result = true;
                api.data.message = "done";
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.code = 300;
                api.error.message = ex.ToString();
            }
            return api;
        }
        [HttpPost("CountUserNonIdTV")]
        [AllowAnonymous]
        public APIUserCheck CountUserNonIdTV()
        {
            APIUserCheck api = new APIUserCheck();
            try
            {
                var httpRequest = HttpContext.Request;
                var z = DAOUsers.getNonIdTv();
                api.data = new DataUserCheck();
                api.data.result = true;
                api.data.message = z.Count.ToString();
                api.data.user_list = z.GetRange(0, 10);
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.code = 300;
                api.error.message = ex.ToString();
            }
            return api;
        }

        [HttpPost("getIdUserError")]
        [AllowAnonymous]
        public APIOTP getIdUserError()
        {
            APIOTP api = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                List<UserDB> users = DAOUsers.getErrorUser();
                if (users.Count > 0)
                {
                    api.data = new DataOTP();
                    api.data.result = true;
                    api.data.otp = users.Count.ToString();
                    api.data.message = JsonConvert.SerializeObject(users.Select(x => x.id).ToList());
                }
                else
                {
                    api.error = new Error();
                    api.error.code = 300;
                }
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.code = 300;
                api.error.message = ex.ToString();
            }
            return api;
        }

        [HttpPost("RemoveSugges")]
        [AllowAnonymous]
        public APIUser RemoveSugges()
        {
            APIUser api = new APIUser();
            try
            {
                var httpRequest = HttpContext.Request;
                int userId = Convert.ToInt32(httpRequest.Form["userId"]);
                int contactId = Convert.ToInt32(httpRequest.Form["contactId"]);
                if (userId != 0 && contactId != 0)
                {
                    if (DAOUsers.updateRemoveSugges(userId, contactId) > 0)
                    {
                        api.data = new DataUser();
                        api.data.result = true;
                        api.data.user_info = new User() { ID = contactId };
                        api.data.message = "Xóa gợi ý thành công";
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 300;
                        api.error.message = "Xóa gợi ý thất bại";
                    }
                }
                else
                {
                    api.error = new Error();
                    api.error.code = 300;
                    api.error.message = "thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.code = 300;
                api.error.message = ex.ToString();
            }
            return api;
        }

        [HttpPost("deleteErrorUser")]
        [AllowAnonymous]
        public APIOTP deleteErrorUser()
        {
            APIOTP api = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                if (DAOUsers.deleteErrorUser() > 0)
                {
                    api.data = new DataOTP();
                    api.data.result = true;
                    api.data.message = "ok";
                }
                else
                {
                    api.error = new Error();
                    api.error.code = 300;
                }
            }
            catch (Exception ex)
            {

                api.error = new Error();
                api.error.code = 300;
                api.error.message = ex.ToString();
            }
            return api;
        }

        [HttpPost("UpdateNotificationMissMessage")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationMissMessage()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationMissMessage(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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
        [HttpPost("UpdateNotificationCommentFromTimViec")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationCommentFromTimViec()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationCommentFromTimViec(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationCommentFromRaoNhanh")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationCommentFromRaoNhanh()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationCommentFromRaoNhanh(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationTag")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationTag()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationTag(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationChangeSalary")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationChangeSalary()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationChangeSalary(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationAllocationRecall")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationAllocationRecall()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationAllocationRecall(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationAcceptOffer")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationAcceptOffer()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationAcceptOffer(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationDecilineOffer")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationDecilineOffer()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationDecilineOffer(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationNTDPoint")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationNTDPoint()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationNTDPoint(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationNTDExpiredPin")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationNTDExpiredPin()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationNTDExpiredPin(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationNTDExpiredRecruit")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationNTDExpiredRecruit()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationNTDExpiredRecruit(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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

        [HttpPost("UpdateNotificationSendCandidate")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationSendCandidate()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationSendCandidate(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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
        [HttpPost("UpdateNotificationNTDApplying")]
        [AllowAnonymous]
        public APIOTP UpdateNotificationNTDApplying()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int userid = Convert.ToInt32(http.Form["userId"]);
                int status = Convert.ToInt32(http.Form["status"]);
                if (userid != 0 && (status == 0 || status == 1))
                {
                    if (DAOUsers.UpdateNotificationNTDApplying(userid, status) > 0)
                    {
                        APIotp.data = new DataOTP();
                        APIotp.data.result = true;
                        APIotp.data.message = "Cập nhật trạng thái thành công";
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
    }
}

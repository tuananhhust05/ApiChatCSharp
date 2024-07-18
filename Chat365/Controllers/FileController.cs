using APIChat365.Model.Entity;
using APIChat365.Model.MongoEntity;
using Chat365.Server.Model.DAO;
using Chat365.Server.Model.Entity;
using Chat365.Server.Model.EntityAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using System.Net.Http;
using System.Threading.Tasks;
using static APIChat365.Controllers.RunToolController;
using APIChat365.Model.DAO;
using System.Diagnostics.Metrics;

namespace Chat365.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        private readonly IWebHostEnvironment _environment;
        public static SocketIO WIO = new SocketIO(new Uri("http://43.239.223.142:3000/"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });
        public FileController(ILogger<FileController> logger,
            IWebHostEnvironment environment)
        {
            if (WIO.Disconnected)
            {
                WIO.ConnectAsync();
            }
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public User getInforUser(DataTable getUser)
        {
            User user = new User(Convert.ToInt32(getUser.Rows[0]["id"]),
                        Convert.ToInt32(getUser.Rows[0]["id365"]),
                        Convert.ToInt32(getUser.Rows[0]["idTimViec"]),
                        Convert.ToInt32(getUser.Rows[0]["type365"]),
                         getUser.Rows[0]["email"].ToString(),
                        "",
                         getUser.Rows[0]["phone"].ToString(),
                         getUser.Rows[0]["userName"].ToString(),
                          getUser.Rows[0]["avatarUser"].ToString(),
                           getUser.Rows[0]["status"].ToString(),
                        Convert.ToInt32(getUser.Rows[0]["statusEmotion"]),
                         Convert.ToDateTime(getUser.Rows[0]["lastActive"]),
    Convert.ToInt32(getUser.Rows[0]["active"]),
                        Convert.ToInt32(getUser.Rows[0]["isOnline"]),
                        Convert.ToInt32(getUser.Rows[0]["looker"]),
                         Convert.ToInt32(getUser.Rows[0]["companyId"]),
                         getUser.Rows[0]["companyName"].ToString(),
                         getUser.Rows[0]["fromWeb"].ToString());
            return user;
        }
        public User getInforUser(List<UserDB> getUser)
        {
            User user = new User(getUser[0].id, getUser[0].id365, getUser[0].idTimViec, getUser[0].type365, getUser[0].email, "", getUser[0].phone, getUser[0].userName, getUser[0].avatarUser, getUser[0].status, getUser[0].statusEmotion, getUser[0].lastActive, getUser[0].active, getUser[0].isOnline, getUser[0].looker, getUser[0].companyId, getUser[0].companyName, getUser[0].fromWeb);
            return user;
        }

        private Byte[] reduceResolution(Byte[] bytes)
        {
            Bitmap bmp1 = new Bitmap(new MemoryStream(bytes));
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 20L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            var memoryStream = new MemoryStream();
            bmp1.Save(memoryStream, jpgEncoder, myEncoderParameters);
            return memoryStream.ToArray();
        }

        [HttpPost("UploadFile")]
        public async Task<APIOTP> UploadFile()
        {
            APIOTP response = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;

                if (httpRequest.Form.Files.Count > 0)
                {
                    string[] listFileName = new string[httpRequest.Form.Files.Count];
                    int count = 0;
                    var fileNewPath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploadsImageSmall");
                    var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");
                    foreach (var file in httpRequest.Form.Files)
                    {
                        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                        if (!Directory.Exists(fileNewPath)) Directory.CreateDirectory(fileNewPath);
                        string NameFile = file.FileName.RemoveSpecChar();
                        listFileName[count] = DateTime.Now.Ticks + "-" + NameFile;
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            System.IO.File.WriteAllBytes(filePath + @"\" + listFileName[count], memoryStream.ToArray());
                            if (file.FileName.ToUpper().EndsWith(".JFIF") || file.FileName.ToUpper().EndsWith(".JPEG") || file.FileName.ToUpper().EndsWith(".JPG") || file.FileName.ToUpper().EndsWith(".PNG"))
                            {
                                CompressPhoto(memoryStream, fileNewPath + @"\" + listFileName[count]);
                            }
                        }
                        count++;
                    }
                    response.data = new DataOTP();
                    response.data.result = true;
                    response.data.message = "Upload File thành công";
                    response.data.listNameFile = listFileName;
                }
                else
                {
                    response.data = new DataOTP();
                    response.data.message = "vui lòng chọn file muốn Upload";
                }
            }
            catch (Exception e)
            {
                response.error = new Error();
                response.error.message = e.ToString();
            }

            return response;
        }

        [HttpPost("UploadCode")]
        public async Task<APIOTP> UploadCode()
        {
            APIOTP response = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;

                if (httpRequest.Form.Files.Count > 0)
                {
                    string[] listFileName = new string[httpRequest.Form.Files.Count];
                    int count = 0;
                    var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\UploadCode");
                    foreach (var file in httpRequest.Form.Files)
                    {
                        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                        string NameFile = file.FileName.RemoveSpecChar();
                        listFileName[count] = DateTime.Now.Ticks + "-" + NameFile;
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            System.IO.File.WriteAllBytes(filePath + @"\" + listFileName[count], memoryStream.ToArray());
                        }
                        count++;
                    }
                    response.data = new DataOTP();
                    response.data.result = true;
                    response.data.message = "Upload File thành công";
                    response.data.listNameFile = listFileName;
                }
                else
                {
                    response.data = new DataOTP();
                    response.data.message = "vui lòng chọn file muốn Upload";
                }
            }
            catch (Exception e)
            {
                response.error = new Error();
                response.error.message = e.ToString();
            }

            return response;
        }

        [HttpPost("UploadNewFile")]
        public async Task<APIOTP> UploadNewFile()
        {
            APIOTP response = new APIOTP();
            try
            {
                var httpRequest = HttpContext.Request;
                string MessageID = httpRequest.Form["MessageID"];
                string ConversationID = httpRequest.Form["ConversationID"];
                string SenderID = httpRequest.Form["SenderID"];
                string MessageType = httpRequest.Form["MessageType"];
                string ListFile = httpRequest.Form["ListFile"];
                string DeleteTime = httpRequest.Form["DeleteTime"];
                string MemberList = httpRequest.Form["MemberList"];

                if (httpRequest.Form.Files.Count > 0)
                {
                    int count = 0;
                    List<InfoFile> listFile = JsonConvert.DeserializeObject<List<InfoFile>>(ListFile);
                    List<FileSendDB> fileSend = new List<FileSendDB>();
                    foreach (InfoFile info in listFile)
                    {
                        info.FullName = info.FullName.RemoveSpecChar();
                        info.NameDisplay = info.FullName.getDisplayNameFile();
                        fileSend.Add(new FileSendDB(info.SizeFile, info.FullName, info.Height, info.Width));
                    }
                    var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");
                    var fileNewPath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploadsImageSmall");
                    for (int i = 0; i < httpRequest.Form.Files.Count; i++)
                    {
                        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                        if (!Directory.Exists(fileNewPath)) Directory.CreateDirectory(fileNewPath);
                        using (var memoryStream = new MemoryStream())
                        {
                            await httpRequest.Form.Files[i].CopyToAsync(memoryStream);
                            System.IO.File.WriteAllBytes(filePath + @"\" + fileSend[i].nameFile, memoryStream.ToArray());
                            if (fileSend[i].nameFile.ToUpper().EndsWith(".JFIF") || fileSend[i].nameFile.ToUpper().EndsWith(".JPEG") || fileSend[i].nameFile.ToUpper().EndsWith(".JPG") || fileSend[i].nameFile.ToUpper().EndsWith(".PNG"))
                            {
                                CompressPhoto(memoryStream, fileNewPath + @"\" + fileSend[i].nameFile);
                            }
                        }
                        count++;
                    }
                    DAOMessages.UpdateStatusMessage(MessageID);
                    Messages mess = new Messages(MessageID, Convert.ToInt32(ConversationID), Convert.ToInt32(SenderID), MessageType, "", 0, DateTime.Now.ToLocalTime(), DateTime.MinValue, 0);
                    mess.ListFile = listFile;
                    var conver = DAOConversation.GetConversation(mess.ConversationID, mess.SenderID);
                    List<int> listMember = new List<int>();
                    List<string> listDevices = new List<string>();
                    List<string> listfromWeb = new List<string>();
                    List<int> isOnline = new List<int>();
                    ParticipantsDB userSender = null;
                    string currentWeb = "";
                    if (conver != null)
                    {
                        for (int i = 0; i < conver.memberList.Count; i++)
                        {
                            if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || conver.typeGroup == "liveChat")
                            {
                                if (conver.memberList[i].liveChat != null && !string.IsNullOrEmpty(conver.memberList[i].liveChat.clientId))
                                {
                                    if (!listDevices.Contains(conver.memberList[i].liveChat.clientId)) listDevices.Add(conver.memberList[i].liveChat.clientId);
                                    if (listMember.Contains(conver.memberList[i].memberId)) listMember.Remove(conver.memberList[i].memberId);
                                    if (conver.memberList[i].memberId == mess.SenderID) currentWeb = conver.memberList[i].liveChat.fromWeb;
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
                    if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || (conver != null && conver.typeGroup == "liveChat"))
                    {
                        var date = DateTime.Now;
                        if ((mess.LiveChat != null && !string.IsNullOrEmpty(mess.LiveChat.ClientId)) || (userSender != null && userSender.liveChat != null && !string.IsNullOrEmpty(userSender.liveChat.clientId)))
                            mess.CreateAt = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second + 4);
                        var checkUser = DAOUsers.GetInforUserById(mess.SenderID);
                        if (checkUser.Count > 0)
                        {
                            mess.SenderName = checkUser[0].userName;
                        }
                        WIO.EmitAsync("SendMessage", mess, listMember, listDevices, "SuppportOtherWeb", currentWeb);
                    }
                    else WIO.EmitAsync("SendMessage", mess, JsonConvert.DeserializeObject<int[]>(MemberList));

                    response.data = new DataOTP();
                    response.data.message = MessageID;
                }
                else
                {
                    response.data = new DataOTP();
                    response.data.message = "vui lòng chọn file muốn Upload";
                }
            }
            catch (Exception e)
            {
                response.error = new Error();
                response.error.message = e.ToString();
            }

            return response;
        }

        [HttpGet("DownloadFile/{fileName}")]
        public FileContentResult DownLoadFile(string fileName)
        {
            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            fileName = fileName.Replace("%20", " ").Replace("%Plush", "+");
            if (System.IO.File.ReadAllBytes(filePath + @"\" + fileName).Length == 0)
            {
                var data = File(reduceResolution(System.IO.File.ReadAllBytes(filePath + @"\" + "group.png")), "application/ocset-stream", fileName);
                return data;
            }
            else
            {
                var data = File(System.IO.File.ReadAllBytes(filePath + @"\" + fileName), "application/ocset-stream", fileName);
                return data;
            }
        }

        [HttpGet("DownloadSmallFile/{fileName}")]
        public FileContentResult DownLoadSmallFile(string fileName)
        {
            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            if (System.IO.File.ReadAllBytes(filePath + @"\" + fileName).Length < 50 * 1024)
            {
                if (System.IO.File.ReadAllBytes(filePath + @"\" + fileName).Length == 0)
                {
                    var data = File(reduceResolution(System.IO.File.ReadAllBytes(filePath + @"\" + "group.png")), "application/ocset-stream", fileName);
                    return data;
                }
                else
                {
                    var data = File(System.IO.File.ReadAllBytes(filePath + @"\" + fileName), "application/ocset-stream", fileName);
                    return data;
                }
            }
            else
            {
                var data = File(reduceResolution(System.IO.File.ReadAllBytes(filePath + @"\" + fileName)), "application/ocset-stream", fileName);
                return data;
            }
        }

        [HttpPost("UploadAvatarGroup")]
        public async Task<APIConversation> UploadAvatarGroup()
        {
            APIConversation APIConversation = new APIConversation();
            var httpRequest = HttpContext.Request;
            if (httpRequest.Form.Files.Count > 0)
            {
                foreach (var file in httpRequest.Form.Files)
                {
                    string userId = "";
                    string fileName = "";
                    if (file.FileName.Contains(".jpg"))
                    {
                        fileName = file.FileName.Replace(".jpg", "");
                    }
                    else
                    {
                        fileName = file.FileName;
                    }
                    for (int i = 0; i < fileName.Length; i++)
                    {
                        if (fileName[i] == '_')
                        {
                            userId = fileName.Substring(i + 1);
                            break;
                        }
                    }
                    fileName = fileName.RemoveSpecChar();
                    var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarGroup");

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    if (!Directory.Exists(filePath + @"\" + userId))
                    {
                        Directory.CreateDirectory(filePath + @"\" + userId);
                    }
                    System.IO.DirectoryInfo di = new DirectoryInfo(filePath + @"\" + userId);
                    if (di.GetFiles().Length >= 10)
                    {
                        di.GetFiles()[0].Delete();
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        System.IO.File.WriteAllBytes(Path.Combine(filePath + @"\" + userId + @"\", fileName + ".jpg"), memoryStream.ToArray());
                        Compressimage(memoryStream, filePath + @"\" + userId + @"\" + fileName + ".jpg");
                    }
                    int count = DAOConversation.ChangeAvatarGroup(Convert.ToInt32(userId), fileName + ".jpg");
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
            }
            return APIConversation;
        }


        [HttpGet("DownloadAvatarGroup/{fileName}")]
        public FileContentResult Get(string fileName)
        {
            string userId = "";
            for (int i = 0; i < fileName.Length; i++)
            {
                if (fileName[i] == '_')
                {
                    for (int j = fileName.Length - 1; j >= 0; j--)
                    {
                        if (fileName[j] == '.')
                        {
                            userId = fileName.Substring(i + 1, j - i - 1);
                            break;
                        }
                    }
                    break;
                }
            }
            var filePath = "";
            if (String.IsNullOrWhiteSpace(userId))
            {
                filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatar");
            }
            else
            {
                filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarGroup" + @"\" + userId);
            }
            try
            {
                if (System.IO.File.ReadAllBytes(filePath + @"\" + fileName).Length < 50 * 1024)
                {
                    if (System.IO.File.ReadAllBytes(filePath + @"\" + fileName).Length == 0)
                    {
                        var data = File(System.IO.File.ReadAllBytes(filePath + @"\" + "group.png"), "application/ocset-stream", fileName);
                        return data;
                    }
                    else
                    {
                        var data = File(System.IO.File.ReadAllBytes(filePath + @"\" + fileName), "application/ocset-stream", fileName);
                        return data;
                    }

                }
                else
                {
                    var data = File(reduceResolution(System.IO.File.ReadAllBytes(filePath + @"\" + fileName)), "application/ocset-stream", fileName);
                    return data;
                }
            }
            catch (FileNotFoundException)
            {
                var data = File(reduceResolution(System.IO.File.ReadAllBytes(filePath + @"\" + "group.png")), "application/ocset-stream", fileName);
                return data;
            }
        }

        [HttpPost("UploadAvatar")]
        public async Task<APIUser> UploadAvatar()
        {
            APIUser userAPI = new APIUser();
            try
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    foreach (var file in httpRequest.Form.Files)
                    {
                        string userId = "";
                        string fileName = "";
                        if (file.FileName.Contains(".jpg"))
                        {
                            fileName = file.FileName.Replace(".jpg", "");
                        }
                        else
                        {
                            fileName = file.FileName;
                        }
                        for (int i = 0; i < fileName.Length; i++)
                        {
                            if (fileName[i] == '_')
                            {
                                userId = fileName.Substring(i + 1);
                                break;
                            }
                        }
                        if (Convert.ToInt32(userId) != 0)
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
                            if (di.GetFiles().Length >= 10)
                            {
                                di.GetFiles()[0].Delete();
                            }
                            User currentUser = getInforUser(DAOUsers.GetUsersById(Convert.ToInt32(userId)));
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                try
                                {
                                    Compressimage(memoryStream, filePath + @"\" + userId + @"\" + fileName + ".jpg");
                                }
                                catch
                                {
                                    System.IO.File.WriteAllBytes(Path.Combine(filePath + @"\" + userId + @"\", fileName + ".jpg"), memoryStream.ToArray());
                                }
                                if (currentUser.Type365 != 0)
                                {
                                    MultipartFormDataContent content = new MultipartFormDataContent();
                                    content.Add(new StringContent("https://mess.timviec365.vn/avatarUser/" + userId + "/" + fileName + ".jpg"), "link");
                                    content.Add(new StringContent(currentUser.Email), "email");
                                    content.Add(new StringContent(currentUser.Type365.ToString()), "type");
                                    HttpClient httpClient = new HttpClient();
                                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/update_avatar.php", content);
                                    string re = response.Result.Content.ReadAsStringAsync().Result;
                                    if (re.Contains("<br />")) re = re.Substring(re.LastIndexOf("<br />") + 7);
                                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(re);
                                    if (receiveInfo.data != null)
                                    {
                                    }
                                }
                                if (currentUser.ID365 != 0)
                                {
                                    MultipartFormDataContent content = new MultipartFormDataContent();
                                    content.Add(new StreamContent(new FileStream(filePath + @"\" + userId + @"\" + fileName + ".jpg", FileMode.Open)), "file", fileName + ".jpg");
                                    if (currentUser.Type365 != 0)
                                    {
                                        content.Add(new StringContent(currentUser.Email), "email");
                                        content.Add(new StringContent(currentUser.Type365.ToString()), "type");
                                    }
                                    else
                                    {
                                        content.Add(new StringContent(currentUser.ID365.ToString()), "id");
                                    }
                                    HttpClient httpClient = new HttpClient();
                                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                                    if (receiveInfo.data != null)
                                    {
                                    }
                                }
                            }
                            if (DAOUsers.UpdateAvatarUser(fileName + ".jpg", currentUser.ID) > 0)
                            {
                                userAPI.data = new DataUser();
                                userAPI.data.result = true;
                                userAPI.data.message = "Cập nhật ảnh đại diện thành công";
                            }
                            else
                            {
                                userAPI.error = new Error();
                                userAPI.error.code = 200;
                                userAPI.error.message = "cập nhật ảnh đại diện thất bại";
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                userAPI.error = new Error();
                userAPI.error.code = 200;
                userAPI.error.message = e.Message;
                return userAPI;
            }
            return userAPI;
        }

        [HttpPost("UploadNewAvatarGroup")]
        public async Task<APIConversation> UploadNewAvatarGroup()
        {
            APIConversation APIConversation = new APIConversation();
            var httpRequest = HttpContext.Request;
            if (httpRequest.Form.Files.Count > 0)
            {
                string conversationId = httpRequest.Form["conversationId"];
                foreach (var file in httpRequest.Form.Files)
                {
                    string fileName = DateTime.Now.Ticks + "_" + file.FileName;
                    var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarGroup");

                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    if (!Directory.Exists(filePath + @"\" + conversationId))
                    {
                        Directory.CreateDirectory(filePath + @"\" + conversationId);
                    }
                    System.IO.DirectoryInfo di = new DirectoryInfo(filePath + @"\" + conversationId);
                    if (di.GetFiles().Length >= 10)
                    {
                        di.GetFiles()[0].Delete();
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        System.IO.File.WriteAllBytes(Path.Combine(filePath + @"\" + conversationId + @"\", fileName + ".jpg"), memoryStream.ToArray());
                        Compressimage(memoryStream, filePath + @"\" + conversationId + @"\" + fileName + ".jpg");
                    }
                    int count = DAOConversation.ChangeAvatarGroup(Convert.ToInt32(conversationId), fileName + ".jpg");
                    if (count != 0)
                    {
                        APIConversation.data = new DataConversation();
                        APIConversation.data.result = true;
                        APIConversation.data.message = fileName + ".jpg";
                    }
                    else
                    {
                        APIConversation.error = new Error();
                        APIConversation.error.code = 200;
                        APIConversation.error.message = "Thay đổi ảnh nhóm thất bại";
                    }
                }
            }
            return APIConversation;
        }


        [HttpPost("UploadNewAvatar")]
        public async Task<APIUser> UploadNewAvatar()
        {
            APIUser userAPI = new APIUser();
            try
            {
                var httpRequest = HttpContext.Request;
                if (httpRequest.Form.Files.Count > 0)
                {
                    string userId = httpRequest.Form["ID"];
                    foreach (var file in httpRequest.Form.Files)
                    {
                        string fileName = DateTime.Now.Ticks + "_" + file.FileName;
                        if (Convert.ToInt32(userId) != 0)
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
                            if (di.GetFiles().Length >= 10)
                            {
                                di.GetFiles()[0].Delete();
                            }
                            User currentUser = getInforUser(DAOUsers.GetUsersById(Convert.ToInt32(userId)));
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                try
                                {
                                    Compressimage(memoryStream, filePath + @"\" + userId + @"\" + fileName);
                                }
                                catch
                                {
                                    System.IO.File.WriteAllBytes(Path.Combine(filePath + @"\" + userId + @"\", fileName), memoryStream.ToArray());
                                }
                                di = null;
                                if (true)
                                {
                                    MultipartFormDataContent content = new MultipartFormDataContent();
                                    content.Add(new StringContent("https://mess.timviec365.vn/avatarUser/" + userId + "/" + fileName), "link");
                                    content.Add(new StringContent(currentUser.Email), "email");
                                    content.Add(new StringContent(currentUser.Type365.ToString()), "type");
                                    HttpClient httpClient = new HttpClient();
                                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/update_avatar.php", content);
                                    string re = response.Result.Content.ReadAsStringAsync().Result;
                                    if (re.Contains("<br />")) re = re.Substring(re.LastIndexOf("<br />") + 7);
                                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(re);
                                    if (receiveInfo.data != null)
                                    {
                                    }
                                }
                                if (currentUser.IDTimViec != 0)
                                {
                                    MultipartFormDataContent content = new MultipartFormDataContent();
                                    content.Add(new StreamContent(new FileStream(filePath + @"\" + userId + @"\" + fileName, FileMode.Open)), "file", fileName);
                                    content.Add(new StringContent(currentUser.Email), "email");
                                    content.Add(new StringContent(currentUser.Type365.ToString()), "type");
                                    //if (currentUser.Type365 != 0)
                                    //{

                                    //}
                                    //else
                                    //{
                                    //    content.Add(new StringContent(currentUser.ID365.ToString()), "id");
                                    //}
                                    HttpClient httpClient = new HttpClient();
                                    Task<HttpResponseMessage> response = httpClient.PostAsync("https://timviec365.vn/api_app/update_tt_chat365.php", content);
                                    InforFromAPI receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                                    if (receiveInfo.data != null)
                                    {
                                    }
                                }
                            }
                            if (DAOUsers.UpdateAvatarUser(fileName, currentUser.ID) > 0)
                            {
                                userAPI.data = new DataUser();
                                userAPI.data.result = true;
                                userAPI.data.message = fileName;
                            }
                            else
                            {
                                userAPI.error = new Error();
                                userAPI.error.code = 200;
                                userAPI.error.message = "cập nhật ảnh đại diện thất bại";
                            }
                        }
                        else
                        {
                            userAPI.error = new Error();
                            userAPI.error.code = 200;
                            userAPI.error.message = "thieu id nguoi dung";
                        }
                    }
                }
                else
                {
                    userAPI.error = new Error();
                    userAPI.error.code = 200;
                    userAPI.error.message = "file truyền lên không tồn tại";
                }
            }
            catch (Exception e)
            {
                userAPI.error = new Error();
                userAPI.error.code = 200;
                userAPI.error.message = e.Message;
                return userAPI;
            }
            return userAPI;
        }

        [HttpGet("SetupNewAvatar")]
        public string SetupNewAvatar()
        {

            try
            {
                DataTable data = DAOUsers.SetupNewAvatar();
                var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser");
                foreach (DataRow row in data.Rows)
                {

                    using (var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes(filePath + @"\" + row["id"].ToString() + @"\" + row["avatarUser"].ToString())))
                    {
                        System.IO.FileInfo[] di = (new DirectoryInfo(filePath + @"\" + row["id"].ToString())).GetFiles();
                        if (di.Length > 0)
                        {
                            for (int i = 0; i < di.Length; i++)
                            {
                                di[i].Delete();
                            }
                        }
                        Compressimage(memoryStream, filePath + @"\" + row["id"].ToString() + @"\" + row["avatarUser"].ToString());
                    }
                }
                return "xong";
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }
        }
        //đã sửa
        [HttpGet("SetupNewAvatarGroup")]
        public string SetupNewAvatarGroup()
        {

            try
            {
                List<ConversationsDB> data = DAOConversation.SetupNewAvatarGroup();

                var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarGroup");
                foreach (ConversationsDB row in data)
                {

                    using (var memoryStream = new MemoryStream(System.IO.File.ReadAllBytes(filePath + @"\" + row.id.ToString() + @"\" + row.avatarConversation.ToString())))
                    {
                        System.IO.FileInfo[] di = (new DirectoryInfo(filePath + @"\" + row.id.ToString())).GetFiles();
                        if (di.Length > 0)
                        {
                            for (int i = 0; i < di.Length; i++)
                            {
                                di[i].Delete();
                            }
                        }
                        Compressimage(memoryStream, filePath + @"\" + row.id.ToString() + @"\" + row.avatarConversation.ToString());
                    }
                }
                return "xong";
            }
            catch (Exception ex)
            {

                return ex.ToString();
            }
        }

        [HttpGet("DownloadAvatar/{fileName}")]
        public FileContentResult DownLoadAvatar(string fileName)
        {
            string userId = "";
            for (int i = 0; i < fileName.Length; i++)
            {
                if (fileName[i] == '_')
                {
                    for (int j = fileName.Length - 1; j >= 0; j--)
                    {
                        if (fileName[j] == '.')
                        {
                            userId = fileName.Substring(i + 1, j - i - 1);
                            break;
                        }
                    }
                    break;
                }
            }
            var filePath = "";
            if (String.IsNullOrWhiteSpace(userId))
            {
                filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatar");
            }
            else
            {
                filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\avatarUser" + @"\" + userId);
            }
            try
            {
                if (System.IO.File.ReadAllBytes(filePath + @"\" + fileName).Length < 50 * 1024)
                {
                    if (System.IO.File.ReadAllBytes(filePath + @"\" + fileName).Length == 0)
                    {
                        var data = File(reduceResolution(System.IO.File.ReadAllBytes(filePath + @"\" + "group.png")), "application/ocset-stream", fileName);
                        return data;
                    }
                    else
                    {
                        var data = File(System.IO.File.ReadAllBytes(filePath + @"\" + fileName), "application/ocset-stream", fileName);
                        return data;
                    }
                }
                else
                {
                    var data = File(reduceResolution(System.IO.File.ReadAllBytes(filePath + @"\" + fileName)), "application/ocset-stream", fileName);
                    return data;
                }
            }
            catch (FileNotFoundException)
            {
                var data = File(reduceResolution(System.IO.File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, @"wwwroot\avatar") + @"\" + "group.png")), "application/ocset-stream", fileName);
                return data;
            }
        }

        public static void CompressPhoto(Stream sourcePath, string targetPath)
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


        public static void Compressimage(Stream sourcePath, string targetPath)
        {
            try
            {
                using (var image = Image.FromStream(sourcePath))
                {
                    float maxHeight = 120;
                    float maxWidth = 120;
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
                    extension = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
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

        [HttpGet("CheckFile")]
        public APIOTP CheckFile()
        {
            APIOTP api = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                string fileName = http.Form["fileName"];
                var fileNewPath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploadsImageSmall");
                var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads");
                if (System.IO.File.Exists(Path.Combine(fileNewPath, fileName)) || System.IO.File.Exists(Path.Combine(filePath, fileName)))
                {
                    api.data = new DataOTP();
                    api.data.result = true;
                    api.data.message = "file tồn tại";
                }
                else
                {
                    api.error = new Error();
                    api.error.message = "file không tồn tại" + filePath;
                }
            }
            catch (Exception)
            {
                api.error = new Error();
                api.error.message = "thất bại";
            }
            return api;
        }
    }
}

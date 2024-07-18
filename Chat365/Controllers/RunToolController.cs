using APIChat365.Model.DAO;
using APIChat365.Model.Entity;
using APIChat365.Model.MongoEntity;
using Chat365.Model.DAO;
using Chat365.Server.Model.DAO;
using Chat365.Server.Model.EntityAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIChat365.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RunToolController : ControllerBase
    {
        private readonly ILogger<RunToolController> _logger;
        private readonly IWebHostEnvironment _environment;
        public static SocketIO WIO = new SocketIO(new Uri("http://43.239.223.142:3000/"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });
        public static SocketIO WIO2 = new SocketIO(new Uri("https://socket.timviec365.vn:3009/"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });
        public RunToolController(ILogger<RunToolController> logger,
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

        [HttpPost("RunDeleteTime")]
        [AllowAnonymous]
        public APIOTP RunDeleteTime()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunDeleteTime() > 0)
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
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }

        [HttpPost("RunDeleteTimeMember")]
        [AllowAnonymous]
        public APIOTP RunDeleteTimeMember()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunDeleteTimeMember() > 0)
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
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }

        [HttpPost("RunDeleteType")]
        [AllowAnonymous]
        public APIOTP RunDeleteType()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunDeleteType() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "Thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "User không tồn tại";
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

        [HttpPost("RunDeleteTypeMember")]
        [AllowAnonymous]
        public APIOTP RunDeleteTypeMember()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunDeleteTypeMember() > 0)
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
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }

        [HttpPost("RunDeleteDate")]
        [AllowAnonymous]
        public APIOTP RunDeleteDate()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunDeleteDate() > 0)
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
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }

        [HttpPost("RunfavoriteMessage")]
        [AllowAnonymous]
        public APIOTP RunfavoriteMessage()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunfavoriteMessage() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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
        [HttpPost("RunLinkNoti")]
        [AllowAnonymous]
        public APIOTP RunLinkNoti()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunLinkNoti() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

        [HttpPost("RunAddNoti")]
        [AllowAnonymous]
        public APIOTP RunAddNoti()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunAddNoti() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

        public class info
        {
            public int userId { get; set; }
            public string fromWeb { get; set; }
        }

        [HttpPost("Runx")]
        [AllowAnonymous]
        public APIOTP Runx()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                string ck = http.Form["run"];
                int id = Convert.ToInt32(http.Form["id"]);
                if (!string.IsNullOrEmpty(ck) && ck == "1") WIO.EmitAsync("Login_v2", new info() { userId = id, fromWeb = "chat365" });
                if (!string.IsNullOrEmpty(ck) && ck == "2") WIO.EmitAsync("Logout_v2", new info() { userId = id, fromWeb = "chat365" });

                APIotp.data = new DataOTP();
                APIotp.data.result = true;
                APIotp.data.message = "ok";
            }
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }

        [HttpPost("RunUserForNode")]
        [AllowAnonymous]
        public APIOTP RunUserForNode()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunAddUserNode() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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


        public class ErrorPass
        {
            public ErrorPass() { }

            public ErrorPass(int id, int type365)
            {
                this.id = id;
                this.type365 = type365;
            }

            public int id { get; set; }
            public int type365 { get; set; }
        }
        [HttpPost("CheckErrorPass")]
        [AllowAnonymous]
        public APIOTP CheckErrorPass()
        {
            APIOTP api = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                int type = Convert.ToInt32(http.Form["type"]);
                if (true)
                {
                    var z = DAOUsers.getErrorPassTV(type);
                    if (z.Count > 0)
                    {
                        List<ErrorPass> t = new List<ErrorPass>();
                        foreach (var item in z)
                        {
                            t.Add(new ErrorPass(item.id, item.type365));
                        }
                        api.data = new DataOTP();
                        api.data.result = true;
                        api.data.message = JsonConvert.SerializeObject(t);
                    }
                    else
                    {
                        api.error = new Error();
                        api.error.code = 200;
                        api.error.message = "không tồn tại";
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


        [HttpPost("RunNotiApplying")]
        [AllowAnonymous]
        public APIOTP RunNotiApplying()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunNotiApplying() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

        [HttpPost("RunLiveChat")]
        [AllowAnonymous]
        public APIOTP RunLiveChat()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunLiveChat() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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


        [HttpPost("RunInfoSupport")]
        [AllowAnonymous]
        public APIOTP RunInfoSupport()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                DaoTool.OldInfoSupport();
                if (DaoTool.RunInfoSupport() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

        [HttpPost("RunA1")]
        [AllowAnonymous]
        public APIOTP RunA1()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var http = HttpContext.Request;
                string data = http.Form["data"];
                InfoSupportDB info = JsonConvert.DeserializeObject<InfoSupportDB>(data);
                APIotp.data = new DataOTP();
                APIotp.data.result = true;
            }
            catch (Exception ex)
            {
                APIotp.error = new Error();
                APIotp.error.code = 200;
                APIotp.error.message = ex.ToString();
            }
            return APIotp;
        }


        [HttpPost("RunFixLiveChat")]
        [AllowAnonymous]
        public APIOTP RunFixLiveChat()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunFixLiveChat() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

        [HttpPost("RunAddNonUnicodeName")]
        [AllowAnonymous]
        public APIOTP RunAddNonUnicodeName()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunAddNonUnicodeName() > 0)
                {
                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thêm trường thành công";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

        [HttpPost("RunDupMember")]
        [AllowAnonymous]
        public APIOTP RunDupMember()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                var re = DaoTool.RunDupMember();
                if (re.Length > 0)
                {

                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = JsonConvert.SerializeObject(re);
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

        [HttpPost("RunClicked")]
        [AllowAnonymous]
        public APIOTP RunClicked()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunClicked() > 0)
                {

                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thanh cong";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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
        [HttpPost("RunFrom")]
        [AllowAnonymous]
        public APIOTP RunFrom()
        {
            APIOTP APIotp = new APIOTP();
            try
            {
                if (DaoTool.RunFrom() > 0)
                {

                    APIotp.data = new DataOTP();
                    APIotp.data.result = true;
                    APIotp.data.message = "thanh cong";
                }
                else
                {
                    APIotp.error = new Error();
                    APIotp.error.code = 200;
                    APIotp.error.message = "thêm trường thất bại";
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

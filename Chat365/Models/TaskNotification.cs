using Chat365.Server.Model.DAO;
using Chat365.Server.Model.Entity;
using Newtonsoft.Json;
using Quartz;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chat365.Models
{
    public class TaskNotification : IJob
    {
        public static SocketIO WIO = new SocketIO(new Uri("http://192.168.11.46:3002/"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });

        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (!WIO.Connected)
                {
                    WIO.ConnectAsync();
                }

                DataTable listUser = DAOUsers.GetUserBySchedul("day");
                using (HttpClient httpClient = new HttpClient())
                {
                    var infoLogin = new FormUrlEncodedContent(new Dictionary<string, string> { { "pass", listUser.Rows[0]["password"].ToString() }, { "epid", listUser.Rows[0]["id365"].ToString() }, { "comid", listUser.Rows[0]["companyId"].ToString() } });
                    InforFromAPI receiveInfo = new InforFromAPI();
                    try
                    {
                        Task<HttpResponseMessage> response = httpClient.PostAsync("https://chamcong.24hpay.vn/api_chat365/login_chat.php", infoLogin);
                        receiveInfo = JsonConvert.DeserializeObject<InforFromAPI>(response.Result.Content.ReadAsStringAsync().Result);
                        WIO.EmitAsync("hihi", receiveInfo);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
            return task;
        }
    }
}

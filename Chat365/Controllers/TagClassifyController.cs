using APIChat365.Model.DAO;
using APIChat365.Model.Entity;
using APIChat365.Model.EntityAPI;
using APIChat365.Model.MongoEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocketIOClient;
using System;
using System.Collections.Generic;

namespace APIChat365.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TagClassifyController : ControllerBase
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IWebHostEnvironment _environment;
        public static SocketIO WIO = new SocketIO(new Uri("https://socket.timviec365.vn/"), new SocketIOOptions
        {
            Query = new Dictionary<string, string>
                {
                    {"token", "V3" }
                },
        });
        public TagClassifyController(ILogger<CommentController> logger,
    IWebHostEnvironment environment)
        {
            if (WIO.Disconnected)
            {
                WIO.ConnectAsync();
            }
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        //add new tag
        [HttpPost("AddNewTagClassify")]
        [AllowAnonymous]
        public APITagClassify AddNewComment()
        {
            APITagClassify TagClassifyAPI = new APITagClassify();
            try
            {
                var httpRequest = HttpContext.Request;
                string TagName = httpRequest.Form["TagName"];
                string TagColor = httpRequest.Form["TagColor"];
                if (!string.IsNullOrEmpty(TagName) && !string.IsNullOrEmpty(TagColor))
                {
                    if (DAOTagClassify.InsertTagClassify(TagName, TagColor) > 0)
                    {
                        TagClassifyAPI.data = new DataTagClassify();
                        TagClassifyAPI.data.result = true;
                        TagClassifyAPI.data.message = "Thêm gán thẻ thành công";
                    }
                    else
                    {
                        TagClassifyAPI.error = new Error();
                        TagClassifyAPI.error.code = 200;
                        TagClassifyAPI.error.message = "Thêm gán thẻ thất bại";
                    }
                }
                else
                {
                    TagClassifyAPI.error = new Error();
                    TagClassifyAPI.error.code = 200;
                    TagClassifyAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                TagClassifyAPI.error = new Error();
                TagClassifyAPI.error.code = 200;
                TagClassifyAPI.error.message = ex.ToString();
            }
            return TagClassifyAPI;
        }

        //delete
        [HttpPost("DeleteTagClassify")]
        [AllowAnonymous]
        public APITagClassify DeleteComment()
        {
            APITagClassify TagClassifyAPI = new APITagClassify();
            try
            {
                var httpRequest = HttpContext.Request;
                int id = Convert.ToInt32(httpRequest.Form["id"]);
                if (id > 0)
                {
                    DAOTagClassify.DeleteTagClassify(id);
                    TagClassifyAPI.data = new DataTagClassify();
                    TagClassifyAPI.data.result = true;
                    TagClassifyAPI.data.message = "Xoá gán thẻ thành công";
                }
                else
                {
                    TagClassifyAPI.error = new Error();
                    TagClassifyAPI.error.code = 200;
                    TagClassifyAPI.error.message = "Thiếu thông tin truyền lên";
                }

            }
            catch (Exception ex)
            {
                TagClassifyAPI.error = new Error();
                TagClassifyAPI.error.code = 200;
                TagClassifyAPI.error.message = ex.ToString();
            }
            return TagClassifyAPI;
        }
        //update
        [HttpPost("UpdateComment")]
        [AllowAnonymous]
        public APITagClassify UpdateComment()
        {
            APITagClassify TagClassifyAPI = new APITagClassify();
            try
            {
                var httpRequest = HttpContext.Request;
                int id = Convert.ToInt32(httpRequest.Form["id"]);
                string TagName = httpRequest.Form["TagName"];
                string TagColor = httpRequest.Form["TagColor"];
                if (id > 0)
                {
                    if (DAOTagClassify.UpdateTagClassify(id, TagName, TagColor) > 0)
                    {
                        TagClassifyAPI.data = new DataTagClassify();
                        TagClassifyAPI.data.result = true;
                        TagClassifyAPI.data.message = "Update gán thẻ thành công";
                    }
                    else
                    {
                        TagClassifyAPI.error = new Error();
                        TagClassifyAPI.error.code = 200;
                        TagClassifyAPI.error.message = "Update găn thẻ thất bại";
                    }
                }
                else
                {
                    TagClassifyAPI.error = new Error();
                    TagClassifyAPI.error.code = 200;
                    TagClassifyAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch
            {
                TagClassifyAPI.error = new Error();
                TagClassifyAPI.error.code = 200;
                TagClassifyAPI.error.message = "Thiếu thông tin truyền lên";
            }
            return TagClassifyAPI;
        }
        public APITagClassify getListTagByid()
        {
            APITagClassify TagClassifyAPI = new APITagClassify();
            try
            {
                var httpRequest = HttpContext.Request;
                int id = Convert.ToInt32(httpRequest.Form["id"]);
                
                //int skip = Convert.ToInt32(httpRequest.Form["skip"]);
                //int limit = Convert.ToInt32(httpRequest.Form["limit"]);
                //string order = (httpRequest.Form["order"]);
                List<TagClassifyDB> listTagClassify = DAOTagClassify.getTagById(id);
                if (id>0)
                {
                    if (listTagClassify.Count > 0)
                    {
                        TagClassifyAPI.data = new DataTagClassify();
                        TagClassifyAPI.data.result = true;
                        TagClassifyAPI.data.message = "Lấy danh sách thành công";
                        TagClassifyAPI.data.listTag = listTagClassify;
                    }
                    else
                    {
                        TagClassifyAPI.error = new Error();
                        TagClassifyAPI.error.code = 200;
                        TagClassifyAPI.error.message = "Lấy danh sách thất bại";
                    }
                }
                else
                {
                    TagClassifyAPI.error = new Error();
                    TagClassifyAPI.error.code = 200;
                    TagClassifyAPI.error.message = "Thiếu thông tin truyền lên";    
                }
            }
            catch (Exception ex)
            {
                TagClassifyAPI.error = new Error();
                TagClassifyAPI.error.code = 200;
                TagClassifyAPI.error.message = ex.ToString();
            }
            return TagClassifyAPI;
        }

    }

}                                       

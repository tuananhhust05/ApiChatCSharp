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
    public class CommentController : ControllerBase
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
        public CommentController(ILogger<CommentController> logger,
    IWebHostEnvironment environment)
        {
            if (WIO.Disconnected)
            {
                WIO.ConnectAsync();
            }
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        //add
        [HttpPost("AddNewComment")]
        [AllowAnonymous]
        public APIComment AddNewComment([FromForm] CommentDB com)
        {
            APIComment commentAPI = new APIComment();
            try
            {
                var httpRequest = HttpContext.Request;
                string url = httpRequest.Form["url"];
                int id_user_url = Convert.ToInt32(httpRequest.Form["id_user_url"]);
                int parent_id = Convert.ToInt32(httpRequest.Form["parent_id"]);
                string comment = httpRequest.Form["comment"];
                int id_user_comment = Convert.ToInt32(httpRequest.Form["id_user_comment"]);
                int ip_comment = Convert.ToInt32(httpRequest.Form["ip_comment"]);
                string id_user_like = httpRequest.Form["id_user_like"];
                int type = Convert.ToInt32(httpRequest.Form["type"]);
                if(!string.IsNullOrEmpty(url) && id_user_url >= 0 && parent_id >= 0 && !string.IsNullOrEmpty(comment) && id_user_comment >= 0 && ip_comment >= 0 && type >= 0 && !string.IsNullOrEmpty(id_user_like))
                {
                    if (DAOComment.InsertCmt(url, id_user_url, parent_id, comment, id_user_comment, ip_comment, type, id_user_like) > 0)
                    {
                        commentAPI.data = new DataComment();
                        commentAPI.data.result = true;
                        commentAPI.data.message = "Thêm Comment thành công";
                    }
                    else
                    {
                        commentAPI.error = new Error();
                        commentAPI.error.code = 200;
                        commentAPI.error.message = "Thêm Comment thất bại";
                    }
                }
                else
                {
                    commentAPI.error = new Error();
                    commentAPI.error.code = 200;
                    commentAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                commentAPI.error = new Error();
                commentAPI.error.code = 200;
                commentAPI.error.message = ex.ToString();
            }
            return commentAPI;
        }

        [HttpPost("getListCommentByUrl")]
        [AllowAnonymous]
        public APIComment getListCommentByUrl([FromForm] CommentDB com)
        {
            APIComment commentAPI = new APIComment();
            try
            {
                var httpRequest = HttpContext.Request;
                string url = httpRequest.Form["url"];
                int skip = Convert.ToInt32(httpRequest.Form["skip"]);
                int limit = Convert.ToInt32(httpRequest.Form["limit"]);
                string order = (httpRequest.Form["order"]);
                List<CommentDB> listComment = DAOComment.getListCommentByUrl(url, skip, limit, order);
                if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(order) && skip >= 0 && limit > 0)
                {
                    if (listComment.Count > 0)
                    {
                        commentAPI.data = new DataComment();
                        commentAPI.data.result = true;
                        commentAPI.data.message = "Lấy danh sách thành công";
                        commentAPI.data.listComment = listComment;
                    }
                    else
                    {
                        commentAPI.error = new Error();
                        commentAPI.error.code = 200;
                        commentAPI.error.message = "Lấy danh sách thất bại";
                    }
                }
                else
                {
                    commentAPI.error = new Error();
                    commentAPI.error.code = 200;
                    commentAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch (Exception ex)
            {
                commentAPI.error = new Error();
                commentAPI.error.code = 200;
                commentAPI.error.message = ex.ToString();
            }
            return commentAPI;
        }

        [HttpPost("getListCommentByParent_id")]
        [AllowAnonymous]
        public APIComment getListCommentByParent_id([FromForm] CommentDB com)
        {
            APIComment commentAPI = new APIComment();
            try
            {
                var httpRequest = HttpContext.Request;
                int parent_id = Convert.ToInt32(httpRequest.Form["parent_id"]);
                int skip = Convert.ToInt32(httpRequest.Form["skip"]);
                int limit = Convert.ToInt32(httpRequest.Form["limit"]);
                string order = (httpRequest.Form["order"]);
                List<CommentDB> listComment = DAOComment.getListCommentByParent_id(parent_id, skip, limit, order);
                if (parent_id > 0 && !string.IsNullOrEmpty(order) && skip >= 0 && limit > 0)
                {
                    if (listComment.Count > 0)
                    {
                        commentAPI.data = new DataComment();
                        commentAPI.data.result = true;
                        commentAPI.data.message = "Lấy danh sách thành công";
                        commentAPI.data.listComment = listComment;
                    }
                    else
                    {
                        commentAPI.error = new Error();
                        commentAPI.error.code = 200;
                        commentAPI.error.message = "Lấy danh sách thất bại";
                    }
                }
                else
                {
                    commentAPI.error = new Error();
                    commentAPI.error.code = 200;
                    commentAPI.error.message = "Thiếu thông tin truyền lên";
                }

            }
            catch (Exception ex)
            {
                commentAPI.error = new Error();
                commentAPI.error.code = 200;
                commentAPI.error.message = ex.ToString();
            }
            return commentAPI;
        }
        //delete
        [HttpPost("DeleteComment")]
        [AllowAnonymous]
        public APIComment DeleteComment([FromForm] CommentDB com)
        {
            APIComment commentAPI = new APIComment();
            try
            {
                var httpRequest = HttpContext.Request;
                int id = Convert.ToInt32(httpRequest.Form["id"]);
                if (id > 0)
                {
                    DAOComment.DeleteCmt(id);
                    commentAPI.data = new DataComment();
                    commentAPI.data.result = true;
                    commentAPI.data.message = "Xoá comment thành công";
                }
                else
                {
                    commentAPI.error = new Error();
                    commentAPI.error.code = 200;
                    commentAPI.error.message = "Thiếu thông tin truyền lên";
                }

            }
            catch (Exception ex)
            {
                commentAPI.error = new Error();
                commentAPI.error.code = 200;
                commentAPI.error.message = ex.ToString();
            }
            return commentAPI;
        }
        //update
        [HttpPost("UpdateComment")]
        [AllowAnonymous]
        public APIComment UpdateComment([FromForm] CommentDB com)
        {
            APIComment commentAPI = new APIComment();
            try
            {
                var httpRequest = HttpContext.Request;
                int id = Convert.ToInt32(httpRequest.Form["id"]);
                string url = httpRequest.Form["url"];
                int id_user_url = Convert.ToInt32(httpRequest.Form["id_user_url"]);
                int parent_id = Convert.ToInt32(httpRequest.Form["parent_id"]);
                string comment = httpRequest.Form["comment"];
                int id_user_comment = Convert.ToInt32(httpRequest.Form["id_user_comment"]);
                int ip_comment = Convert.ToInt32(httpRequest.Form["ip_comment"]);
                string id_user_like = httpRequest.Form["id_user_like"];
                int type = Convert.ToInt32(httpRequest.Form["type"]);
                if(id > 0)
                {
                    if (DAOComment.UpdateCmt(id, url, id_user_url, parent_id, comment, id_user_comment, ip_comment, type, id_user_like) > 0)
                    {
                        commentAPI.data = new DataComment();
                        commentAPI.data.result = true;
                        commentAPI.data.message = "Update comment thành công";
                    }
                    else
                    {
                        commentAPI.error = new Error();
                        commentAPI.error.code = 200;
                        commentAPI.error.message = "Update comment thất bại";
                    }
                }
                else
                {
                    commentAPI.error = new Error();
                    commentAPI.error.code = 200;
                    commentAPI.error.message = "Thiếu thông tin truyền lên";
                }
            }
            catch
            {
                commentAPI.error = new Error();
                commentAPI.error.code = 200;
                commentAPI.error.message = "Thiếu thông tin truyền lên";
            }
            return commentAPI;
        }
    }
}

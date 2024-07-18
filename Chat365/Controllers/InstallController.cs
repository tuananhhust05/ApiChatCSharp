using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstallController : ControllerBase
    {
        private readonly ILogger<InstallController> _logger;
        private readonly IWebHostEnvironment _environment;

        public InstallController(ILogger<InstallController> logger,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        [HttpGet("Chat365")]
        public FileContentResult Chat365()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\Chat365\Install");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            if (System.IO.File.ReadAllBytes(filePath + @"\Chat365.exe").Length != 0)
            {
                var data = File(System.IO.File.ReadAllBytes(filePath + @"\Chat365.exe"), "application/ocset-stream", "Chat365.exe");
                return data;
            }
            return null;
        }

        [HttpGet("TimViec365")]
        public FileContentResult TimViec365()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\TimViec365\Install");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            if (System.IO.File.ReadAllBytes(filePath + @"\TimViec365.exe").Length != 0)
            {
                var data = File(System.IO.File.ReadAllBytes(filePath + @"\TimViec365.exe"), "application/ocset-stream", "TimViec365.exe");
                return data;
            }
            return null;
        }

        [HttpGet("HR")]
        public FileContentResult HR()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\HR\Install");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            if (System.IO.File.ReadAllBytes(filePath + @"\HR.exe").Length != 0)
            {
                var data = File(System.IO.File.ReadAllBytes(filePath + @"\HR.exe"), "application/ocset-stream", "HR.exe");
                return data;
            }
            return null;
        }

        [HttpGet("QuanLyTaiSan")]
        public FileContentResult QuanLyTaiSan()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\QuanLyTaiSan\Install");

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            if (System.IO.File.ReadAllBytes(filePath + @"\QuanLyTaiSan.exe").Length != 0)
            {
                var data = File(System.IO.File.ReadAllBytes(filePath + @"\QuanLyTaiSan.exe"), "application/ocset-stream", "QuanLyTaiSan.exe");
                return data;
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Server.Model.EntityAPI
{
    public class APIChangePass
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}

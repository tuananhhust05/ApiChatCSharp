using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat365.Server.Model.EntityAPI
{
    public class APILoadListConversation
    {
        public int userId { get; set; }
        public int companyId { get; set; }
        public string message { get; set; }
        public int page { get; set; }
        public int countConversation { get; set; }
        public int countConversationLoad { get; set; }
        public DateTime lastTimeMess { get; set; }
    }
}

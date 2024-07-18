using System;

namespace Chat365.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
    }
}

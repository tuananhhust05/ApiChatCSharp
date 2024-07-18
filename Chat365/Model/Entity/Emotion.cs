using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat365.Model.Entity
{
    public class Emotion
    {
        public Emotion(int type, string[] listUserId, string linkEmotion)
        {
            Type = type;
            ListUserId = listUserId;
            LinkEmotion = linkEmotion;
        }

        public Emotion(int type, string linkEmotion, bool isChecked)
        {
            IsChecked = isChecked;
            Type = type;
            LinkEmotion = linkEmotion;
        }
        public int Type { get; set; }   
        public string[] ListUserId { get; set; }
        public string LinkEmotion { get; set; }
        public bool IsChecked { get; set; }
    }
}

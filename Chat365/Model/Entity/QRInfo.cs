using System;

namespace APIChat365.Model.Entity
{
    public class QRInfo
    {
        public QRInfo(string qRType, object data, DateTime time)
        {
            QRType = qRType;
            this.data = data;
            Time = time;
        }

        public string QRType { get; set; }
        public object data { get; set; }
        public DateTime Time { get; set; }
    }

    public class QRFriend
    {
        public QRFriend(int userId, int type365)
        {
            this.userId = userId;
            this.type365 = type365;
        }

        public int userId { get; set; }
        public int type365 { get; set; }
    }
    public class QRJoinGroup
    {
        public QRJoinGroup(int conversationId, string conversationName)
        {
            ConversationId = conversationId;
            ConversationName = conversationName;
        }

        public int ConversationId { get; set; }
        public string ConversationName { get; set; }
    }
}

namespace APIChat365.Model.MongoEntity
{
    public class LiveChatDB
    {
        public LiveChatDB()
        {
            this.clientId = "";
            this.clientName = "";
            this.fromWeb = "";
        }

        public LiveChatDB(string clientId, string clientName, string fromWeb)
        {
            this.clientId = clientId;
            this.clientName = clientName;
            this.fromWeb = fromWeb;
        }

        public string clientId { get; set; }
        public string clientName { get; set; }
        public string fromWeb { get; set; }
    }
}

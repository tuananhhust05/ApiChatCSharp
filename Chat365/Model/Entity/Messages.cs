using Chat365.Model.Entity;
using Chat365.Server.Model.DAO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chat365.Server.Model.Entity
{
    public class InfoLink
    {
        public InfoLink()
        {
        }

        public InfoLink(string messageId, string title, string description, string linkHome, string image, int isNotification)
        {
            MessageID = messageId;
            Description = description;
            Title = title;
            LinkHome = linkHome;
            Image = image;
            if (Image == null || Image.Equals("null") || String.IsNullOrWhiteSpace(Image.Trim()))
            {
                HaveImage = "False";
            }
            else
            {
                HaveImage = "True";
            }
            IsNotification = isNotification;
            TypeLink = "link";
        }

        public string MessageID { get; set; }
        public string TypeLink { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string LinkHome { get; set; }
        public string Image { get; set; }
        public string HaveImage { get; set; }
        public int IsNotification { get; set; }

    }
    public class Messages
    {
        public Messages()
        {
        }

        public Messages(string messageID, int conversationID, int senderID, string messageType, string message, string listTag, DateTime deleteDate, int deleteTime = 0, int deleteType = 0, int isFavorite = 0)
        {
            MessageID = messageID;
            ConversationID = conversationID;
            SenderID = senderID;
            MessageType = messageType;
            Message = message;
            ListTag = listTag;
            DeleteTime = deleteTime;
            DeleteType = deleteType;
            DeleteDate = deleteDate;
            IsFavorite = isFavorite;
        }

        public Messages(string messageID, int conversationID, int senderID, string messageType, string messages, int isEdited, DateTime createAtTime, DateTime deleteDate, int deleteTime = 0, int deleteType = 0, int isFavorite = 0)
        {
            MessageID = messageID;
            ConversationID = conversationID;
            SenderID = senderID;
            MessageType = messageType;
            Message = messages;
            CreateAt = createAtTime;
            IsEdited = isEdited;
            DeleteTime = deleteTime;
            DeleteType = deleteType;
            DeleteDate = deleteDate;
            IsFavorite = isFavorite;
        }


        public string MessageID { get; set; }
        public List<Emotion> EmotionMessage { get; set; }
        public int ConversationID { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatar { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
        public int IsEdited { get; set; }
        public MessageQuote QuoteMessage { get; set; }
        public InfoLink InfoLink { get; set; }
        public DateTime CreateAt { get; set; }
        public List<InfoFile> ListFile { get; set; }
        public User UserProfile { get; set; }
        public string File { get; set; }
        public string Quote { get; set; }
        public string Link { get; set; }
        public string Profile { get; set; }
        public string ListTag { get; set; }
        public int DeleteTime { get; set; }
        public int DeleteType { get; set; }
        public DateTime DeleteDate { get; set; }
        public int IsFavorite { get; set; }
        public string LinkNotification { get; set; }
        public InfoSupport InfoSupport { get; set; }
        public InfoLiveChat LiveChat { get; set; }
        public int IsClicked { get; set; }
    }

    public class InfoSupport
    {
        public InfoSupport()
        {
        }

        public InfoSupport(string title, string message, string supportId, int userId, string userName, int status, DateTime time, int haveConversation)
        {
            Title = title;
            Message = message;
            SupportId = supportId;
            UserId = userId;
            this.userName = userName;
            Status = status;
            Time = time;
            HaveConversation = haveConversation;
        }

        public string Title { get; set; }
        public string Message { get; set; }
        public string SupportId { get; set; }
        public int UserId { get; set; }
        public string userName { get; set; }
        public int Status { get; set; }
        public DateTime Time { get; set; }
        public int HaveConversation { get; set; }
    }

    public class InfoLiveChat
    {
        public InfoLiveChat()
        {
        }

        public InfoLiveChat(string clientID, string clientName, string clientAvatar, string fromWeb)
        {
            ClientId = clientID;
            ClientName = clientName;
            ClientAvatar = clientAvatar;
            FromWeb = fromWeb;
        }

        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAvatar { get; set; }
        public string FromWeb { get; set; }
    }

    public class Messages_v2
    {
        public Messages_v2()
        {
        }

        public Messages_v2(string messageID, int conversationID, int senderID,long displaymess, string messageType, string message, string listTag, DateTime deleteDate, int deleteTime = 0, int deleteType = 0, int isFavorite = 0)
        {
            MessageID = messageID;
            ConversationID = conversationID;
            SenderID = senderID;
            MessageType = messageType;
            Message = message;
            ListTag = listTag;
            DeleteTime = deleteTime;
            DeleteType = deleteType;
            DeleteDate = deleteDate;
            IsFavorite = isFavorite;
            this.DisplayMessage= displaymess;
        }

        public Messages_v2(string messageID, int conversationID, int senderID,long displaymess, string messageType, string messages, int isEdited, string createAtTime, DateTime deleteDate, int deleteTime = 0, int deleteType = 0, int isFavorite = 0)
        {
            MessageID = messageID;
            ConversationID = conversationID;
            SenderID = senderID;
            MessageType = messageType;
            Message = messages;
            CreateAt = createAtTime;
            IsEdited = isEdited;
            DeleteTime = deleteTime;
            DeleteType = deleteType;
            DeleteDate = deleteDate;
            IsFavorite = isFavorite;
            this.DisplayMessage= displaymess;
        }


        public string MessageID { get; set; }
        public List<Emotion> EmotionMessage { get; set; }
        public int ConversationID { get; set; }
        public int SenderID { get; set; }
        public long DisplayMessage { get; set; }
        public string SenderName { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
        public int IsEdited { get; set; }
        public int IsSeen { get; set; }
        public MessageQuote QuoteMessage { get; set; }
        public InfoLink InfoLink { get; set; }
        public string CreateAt { get; set; }
        public List<InfoFile> ListFile { get; set; }
        public User UserProfile { get; set; }
        public string File { get; set; }
        public string Quote { get; set; }
        public string Link { get; set; }
        public string Profile { get; set; }
        public string ListTag { get; set; }
        public int DeleteTime { get; set; }
        public int DeleteType { get; set; }
        public DateTime DeleteDate { get; set; }
        public int IsFavorite { get; set; }
        public string LinkNotification { get; set; }
        public InfoSupport InfoSupport { get; set; }
        public InfoLiveChat LiveChat { get; set; }
    }

    public class InfoFile
    {
        public string TypeFile { get; set; }
        public string FullName { get; set; }
        public string ImageSource { get; set; }
        public string FileSizeInByte { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public Int64 SizeFile { get; set; }
        public string NameDisplay { get; set; }

        public InfoFile()
        {
        }

        public InfoFile(string typeFile, string fullName, Int64 sizeFile, IWebHostEnvironment _environment, string MessageID)
        {
            try
            {
                TypeFile = typeFile;
                FullName = fullName;
                SizeFile = sizeFile;
                if (!String.IsNullOrWhiteSpace(fullName))
                {
                    for (int i = 0; i < fullName.Length; i++)
                    {
                        if (fullName[i] == '-')
                        {
                            NameDisplay = fullName.Substring(i + 1);
                            if (NameDisplay.Length > 25)
                            {
                                NameDisplay = NameDisplay.Substring(0, 23) + "...";
                            }
                            break;
                        }
                    }
                }

                if (Convert.ToInt64(sizeFile) / 1024 < 1)
                {
                    FileSizeInByte = Convert.ToInt64(sizeFile) + " bytes";
                }
                else if (Convert.ToInt64(sizeFile) / 1024 < 1024)
                {
                    FileSizeInByte = ((double)Convert.ToInt64(sizeFile) / 1024).ToString("0.00") + " KB";
                }
                else
                {
                    FileSizeInByte = ((double)Convert.ToInt64(sizeFile) / (1024 * 1024)).ToString("0.00") + " MB";
                }

                //if (typeFile.Equals("sendPhoto"))
                //{
                //    try
                //    {
                //        System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads\") + FullName)));
                //        Width = img.Width;
                //        Height = img.Height;
                //        DAOMessages.UpdateInfoPhoto(MessageID, Height, Width);

                //    }
                //    catch (Exception ex)
                //    {
                //        NameDisplay = ex.ToString();
                //        Height = 0;
                //        Width = 0;
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public InfoFile(string typeFile, string fullName, Int64 sizeFile, double height, double width)
        {
            try
            {
                TypeFile = typeFile;
                FullName = fullName;
                SizeFile = sizeFile;
                Height = height;
                Width = width;
                if (!String.IsNullOrWhiteSpace(fullName))
                {
                    for (int i = 0; i < fullName.Length; i++)
                    {
                        if (fullName[i] == '-')
                        {
                            NameDisplay = fullName.Substring(i + 1);
                            if (NameDisplay.Length > 25)
                            {
                                NameDisplay = NameDisplay.Substring(0, 23) + "...";
                            }
                            break;
                        }
                    }
                }

                if (Convert.ToInt64(sizeFile) / 1024 < 1)
                {
                    FileSizeInByte = Convert.ToInt64(sizeFile) + " bytes";
                }
                else if (Convert.ToInt64(sizeFile) / 1024 < 1024)
                {
                    FileSizeInByte = ((double)Convert.ToInt64(sizeFile) / 1024).ToString("0.00") + " KB";
                }
                else
                {
                    FileSizeInByte = ((double)Convert.ToInt64(sizeFile) / (1024 * 1024)).ToString("0.00") + " MB";
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class MessageQuote
    {
        public string MessageID { get; set; }
        public int SenderID { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
        public DateTime CreateAt { get; set; }
        public string SenderName { get; set; }

        public MessageQuote()
        {
            MessageID = "";
            SenderID = 0;
            SenderName = "";
            MessageType = "";
            Message = "";
            CreateAt = DateTime.Now;
        }

        public MessageQuote(string messageID, string senderName, int senderID, string messageType, string message, DateTime createAt)
        {
            MessageID = messageID;
            SenderID = senderID;
            MessageType = messageType;
            Message = message;
            CreateAt = createAt;
            SenderName = senderName;
        }
    }

    public class CompareByTimeMessage : IComparer<Messages>
    {
        public int Compare(Messages x, Messages y)
        {
            return y.CreateAt.CompareTo(x.CreateAt);
        }
    }
}

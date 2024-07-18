using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace APIChat365.Model.MongoEntity
{
    public class FileSendDB
    {
        public FileSendDB()
        {
        }

        public FileSendDB(long sizeFile, string nameFile, double height, double width)
        {
            this.sizeFile = sizeFile;
            this.nameFile = nameFile;
            this.height = height;
            this.width = width;
        }

        public void UpdateWidthHeightPhoto(IWebHostEnvironment _environment)
        {
            try
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(File.ReadAllBytes(Path.Combine(_environment.ContentRootPath, @"wwwroot\uploads\") + nameFile)));
                this.width = img.Width;
                this.height = img.Height;
            }
            catch
            {
                this.height = 0;
                this.width = 0;
            }
        }

        [BsonElement("sizeFile")]
        public long sizeFile { get; set; }

        [BsonElement("nameFile")]
        public string nameFile { get; set; }

        [BsonElement("height")]
        public double height { get; set; }

        [BsonElement("width")]
        public double width { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;

namespace APIChat365.Model.MongoEntity
{
    public class EmotionMessageDB
    {
        public EmotionMessageDB()
        {
            Emotion1 = "";
            Emotion2 = "";
            Emotion3 = "";
            Emotion4 = "";
            Emotion5 = "";
            Emotion6 = "";
            Emotion7 = "";
            Emotion8 = "";
        }

        public EmotionMessageDB(string emotion1, string emotion2, string emotion3, string emotion4, string emotion5, string emotion6, string emotion7, string emotion8)
        {
            Emotion1 = emotion1;
            Emotion2 = emotion2;
            Emotion3 = emotion3;
            Emotion4 = emotion4;
            Emotion5 = emotion5;
            Emotion6 = emotion6;
            Emotion7 = emotion7;
            Emotion8 = emotion8;
        }

        public string Emotion1 { get; set; }
        public string Emotion2 { get; set; }
        public string Emotion3 { get; set; }
        public string Emotion4 { get; set; }
        public string Emotion5 { get; set; }
        public string Emotion6 { get; set; }
        public string Emotion7 { get; set; }
        public string Emotion8 { get; set; }
    }
}

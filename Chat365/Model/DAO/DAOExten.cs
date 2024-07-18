using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIChat365.Model.DAO
{
    public static class DAOExten
    {
        public static string RemoveUnicode(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            else
            {
                string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ", };
                string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "d", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "e", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "o", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "u", "y", "y", "y", "y", "y", };
                for (int i = 0; i < arr1.Length; i++)
                {
                    text = text.Replace(arr1[i], arr2[i]);
                    text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
                }
                return text;
            }
        }
        public static string RemoveUnicodeP1(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            else
            {
                string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ", "đ", "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ", "í", "ì", "ỉ", "ĩ", "ị", "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ", "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự", "ý", "ỳ", "ỷ", "ỹ", "ỵ", };
                string[] arr2 = new string[] { "a", "a", "a", "a", "a", "â", "â", "â", "â", "â", "â", "ă", "ă", "ă", "ă", "ă", "ă", "đ", "e", "e", "e", "e", "e", "ê", "ê", "ê", "ê", "ê", "ê", "i", "i", "i", "i", "i", "o", "o", "o", "o", "o", "ô", "ô", "ô", "ô", "ô", "ô", "ơ", "ơ", "ơ", "ơ", "ơ", "ơ", "u", "u", "u", "u", "u", "ư", "ư", "ư", "ư", "ư", "ư", "y", "y", "y", "y", "y", };
                for (int i = 0; i < arr1.Length; i++)
                {
                    text = text.Replace(arr1[i], arr2[i]);
                    text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
                }
                return text;
            }
        }
        public static string MaxEncode(this string txt)
        {
            try
            {
                string ourText = txt;
                string Return = null;
                string _key = "HHP889@@";
                string privatekey = "hgfedcba";
                byte[] privatekeyByte = { };
                privatekeyByte = Encoding.UTF8.GetBytes(privatekey);
                byte[] _keybyte = { };
                _keybyte = Encoding.UTF8.GetBytes(_key);
                byte[] inputtextbyteArray = System.Text.Encoding.UTF8.GetBytes(ourText);
                using (DESCryptoServiceProvider dsp = new DESCryptoServiceProvider())
                {
                    var memstr = new MemoryStream();
                    var crystr = new CryptoStream(memstr, dsp.CreateEncryptor(_keybyte, privatekeyByte), CryptoStreamMode.Write);
                    crystr.Write(inputtextbyteArray, 0, inputtextbyteArray.Length);
                    crystr.FlushFinalBlock();
                    return Convert.ToBase64String(memstr.ToArray());
                }
                return Return;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public static string MaxDecode(this string txt)
        {
            try
            {
                string ourText = txt;
                string x = null;
                string _key = "HHP889@@";
                string privatekey = "hgfedcba";
                byte[] privatekeyByte = { };
                privatekeyByte = Encoding.UTF8.GetBytes(privatekey);
                byte[] _keybyte = { };
                _keybyte = Encoding.UTF8.GetBytes(_key);
                byte[] inputtextbyteArray = new byte[ourText.Replace(" ", "+").Length];
                //This technique reverses base64 encoding when it is received over the Internet.
                inputtextbyteArray = Convert.FromBase64String(ourText.Replace(" ", "+"));
                using (DESCryptoServiceProvider dEsp = new DESCryptoServiceProvider())
                {
                    var memstr = new MemoryStream();
                    var crystr = new CryptoStream(memstr, dEsp.CreateDecryptor(_keybyte, privatekeyByte), CryptoStreamMode.Write);
                    crystr.Write(inputtextbyteArray, 0, inputtextbyteArray.Length);
                    crystr.FlushFinalBlock();
                    return Encoding.UTF8.GetString(memstr.ToArray());
                }
                return x;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RemoveSpecChar(this string txt)
        {

            string NameFile = "";
            string exten = "";
            string id = "";
            if (txt.Contains("."))
            {
                List<string> split = txt.Split('.').ToList();
                if (split.Count >= 2)
                {
                    exten = $".{split[split.Count - 1]}";
                    split.RemoveAt(split.Count - 1);
                }
                NameFile = string.Join(".", split);
                if (NameFile.Contains("-"))
                {
                    List<string> splitId = NameFile.Split('-').ToList();
                    if (splitId.Count > 0)
                    {
                        id = $"{splitId[0]}-";
                        splitId.RemoveAt(0);
                    }
                    NameFile = string.Join("-", splitId);
                }
            }
            string[] charsToRemove = new string[] { " ", "+", "~", "`", "!", "@", "#", "$", "%", "^", "&", "*", "=", ":", ";", "'", "<", ">", "?", "/", @"\", "|", @"""" };
            foreach (var c in charsToRemove)
            {
                NameFile = NameFile.Replace(c, string.Empty);
            }
            if (string.IsNullOrEmpty(NameFile))
            {
                NameFile = RandomString(8);
            }
            return $"{id}{NameFile}{exten}";
        }
        public static string getDisplayNameFile(this string fullName)
        {
            string NameDisplay = "";
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
            return NameDisplay;
        }
    }
}

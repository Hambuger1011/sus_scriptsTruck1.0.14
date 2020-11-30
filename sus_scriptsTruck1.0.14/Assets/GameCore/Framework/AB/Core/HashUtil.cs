namespace AB
{

    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    [XLua.LuaCallCSharp, XLua.Hotfix]
    public class HashUtil
    {
        public static string Get(Stream fs)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            byte[] bytes = ha.ComputeHash(fs);
            fs.Close();
            return ToHexString(bytes);
        }

        public static string Get(string s)
        {
            if (s == null)
            {
                return s;
            }

            return Get(Encoding.UTF8.GetBytes(s));
        }

        public static string Get(byte[] data)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            byte[] bytes = ha.ComputeHash(data);
            return ToHexString(bytes);//.Substring(8, 8); //MD5 8位
        }

        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString().ToLower();
            }
            return hexString;
        }

        public static string GetFileMd5(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }
            using(var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Get(fs);
            }
        }
    }
}

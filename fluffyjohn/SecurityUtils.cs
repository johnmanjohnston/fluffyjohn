using System.Security.Cryptography;
using System.Text;

namespace fluffyjohn
{
    public static class SecurityUtils
    {
        private static string GenerateMD5(MD5 md, string plain)
        {
            StringBuilder sb = new StringBuilder();
            byte[] data = md.ComputeHash(Encoding.UTF8.GetBytes(plain));

            for (int i = 0; i < plain.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public static string MD5Hash(string plain)
        {
            using (MD5 md5 = MD5.Create())
            {
                string generatedHash = GenerateMD5(md5, plain);
                return generatedHash;
            }
        }
    }
}

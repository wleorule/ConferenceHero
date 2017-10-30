using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DaniKarijera.Common
{
    public class CalculateHash
    {
        public static string Hash(string input)
        {
            string salt = "57A613F6F8CB2844FE3EE5CD6360BC97A308736F";
            using (SHA512Managed sha1 = new SHA512Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(salt+input+salt));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Uptime.Core {
    public class Hash {
        public static string Md5(string input) {
            if (string.IsNullOrEmpty(input)) {
                return "";
            }

            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();

        }
    }
}

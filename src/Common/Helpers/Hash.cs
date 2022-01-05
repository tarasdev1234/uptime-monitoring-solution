using System.Security.Cryptography;
using System.Text;

namespace Helpers {
    public class Hash {
        public static string Md5 (string input) {
            if (string.IsNullOrEmpty(input)) {
                return "";
            }

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();

            foreach (var t in hash) {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}

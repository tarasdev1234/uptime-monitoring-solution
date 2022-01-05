using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Uptime.Monitoring.Model.Validation
{
    public class RegexCheck : ValidationAttribute {
        private string[] patterns;

        public RegexCheck (params string[] patterns) {
            this.patterns = patterns;
        }

        public override bool IsValid (object value) {
            var val = value.ToString();

            foreach (var pattern in patterns) {
                if (Regex.IsMatch(val, pattern)) {
                    return true;
                }
            }

            return false;
        }
    }
}

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Uptime.Theme
{
    public static class BigNumbersFormatter
    {
        public static HtmlString FormatAsBigNumber(this IHtmlHelper html, int number)
        {
            if (number >= 1_000_000_000)
            {
                return new HtmlString($"{number / 1_000_000_000.0:0.#}B+");
            }

            if (number >= 1_000_000)
            {
                return new HtmlString($"{number / 1_000_000.0:0.#}M+");
            }

            if (number >= 1000)
            {
                return new HtmlString($"{number / 1000}K+");
            }

            return new HtmlString(number.ToString());
        }
    }
}

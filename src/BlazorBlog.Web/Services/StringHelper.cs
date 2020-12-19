using System.Globalization;
using System.Text;

namespace BlazorBlog.Web.Services
{
    public static class StringHelper
    {
        /// <summary>
        /// Creates a URL And SEO friendly slug
        /// </summary>
        /// <param name="text">Text to slugify</param>
        /// <param name="preserveFrontSlash">Preserves front slash, and converts back slash to front slash</param>
        /// <returns>URL and SEO friendly string</returns>
        public static string UrlFriendly(string text, bool preserveFrontSlash = false)
        {
            var normalizedString = text
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var stringBuilder = new StringBuilder();
            var previousDash = false;

            foreach (var c in normalizedString)
            {
                if (preserveFrontSlash && (c == '/' || c == '\\'))
                {
                    stringBuilder.Append('/');
                    previousDash = false;
                }
                else if (IsCharacter(c))
                {
                    stringBuilder.Append(c);
                    previousDash = false;
                }
                else if (IsWhiteSpace(c) && !previousDash)
                {
                    stringBuilder.Append('-');
                    previousDash = true;
                }
            }

            // Trim excess hyphens
            return stringBuilder.ToString().Trim('-');
        }

        private static bool IsWhiteSpace(char c)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            return unicodeCategory == UnicodeCategory.SpaceSeparator ||
                   unicodeCategory == UnicodeCategory.ConnectorPunctuation ||
                   unicodeCategory == UnicodeCategory.DashPunctuation ||
                   unicodeCategory == UnicodeCategory.OtherPunctuation ||
                   unicodeCategory == UnicodeCategory.MathSymbol;
        }

        private static bool IsCharacter(char c)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            return unicodeCategory == UnicodeCategory.LowercaseLetter ||
                   unicodeCategory == UnicodeCategory.UppercaseLetter ||
                   unicodeCategory == UnicodeCategory.DecimalDigitNumber;
        }
    }
}

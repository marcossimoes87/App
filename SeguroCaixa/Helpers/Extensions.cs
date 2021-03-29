using System.Globalization;
using System.Text;

namespace SeguroCaixa.Helpers
{
    public static class Extensions
    {
        public static bool IsSuccessStatusCode(this int statusCode)
        {
            return ((int)statusCode >= 200) && ((int)statusCode <= 299);
        }

        public static string RemoveAcento(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}

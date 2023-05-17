using System.Globalization;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MaGeek.AppFramework.Utils
{
    public abstract class StringExtension
    {
        public static string RemoveDiacritics(string Text)
        {
            ReadOnlySpan<char> normalizedString = Text.Normalize(NormalizationForm.FormD);
            int i = 0;
            Span<char> span = Text.Length < 1000
                ? stackalloc char[Text.Length]
                : new char[Text.Length];

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    span[i++] = c;
            }

            return new string(span).Normalize(NormalizationForm.FormC);
        }
    }
}

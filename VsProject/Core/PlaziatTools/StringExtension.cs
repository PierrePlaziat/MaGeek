using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace PlaziatTools
{

    public static class StringExtension
    {

        /// <summary>
        /// Replace diatrics characters in a string by simple characters
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>The resulting string</returns>
        public static string RemoveDiacritics(this string str)
        {
            ReadOnlySpan<char> normalizedString = str.Normalize(NormalizationForm.FormD);
            int i = 0;
            Span<char> span = str.Length < 1000
                ? stackalloc char[str.Length]
                : new char[str.Length];

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    span[i++] = c;
            }

            return new string(span).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Splits the tring at regular intervals
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="n">interval</param>
        /// <returns>An IEnumerable containing resulting strings</returns>
        /// <exception cref="ArgumentException">Wrong parameters</exception>
        public static IEnumerable<string> Split(this string str, int n)
        {
            if (string.IsNullOrEmpty(str) || n < 1) { throw new ArgumentException(); }
            return Enumerable
                .Range(0, str.Length / n)
                .Select(i => str.Substring(i * n, n));
        }

        public static char[] SubArray(this char[] input, int startIndex, int length)
        {
            List<char> result = new List<char>();
            for (int i = startIndex; i < length; i++)
            {
                result.Add(input[i]);
            }

            return result.ToArray();
        }

    }

}

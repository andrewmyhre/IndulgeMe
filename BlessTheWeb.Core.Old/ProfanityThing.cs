using System.Text.RegularExpressions;

namespace BlessTheWeb.Core
{
    public class ProfanityThing
    {
        private static Regex profanityRegex = new Regex("(shit|fuck|cunt|piss|penis|dick|cock)", RegexOptions.IgnoreCase);

        public static string CleanRudeWords(string message)
        {
            string cleaned = message;
            var matches = profanityRegex.Matches(message);

            foreach (Match m in matches)
            {
                cleaned = cleaned.Substring(0, m.Index)
                    + m.Value[0] + "".PadRight(m.Length - 2, '*') + m.Value[m.Length - 1]
                    + cleaned.Substring(m.Index + m.Length);
            }

            return cleaned;
        }

    }
}

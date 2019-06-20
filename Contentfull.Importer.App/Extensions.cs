using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Contentful.Importer.App.Extensions
{
    public static class Extensions
    {


        public static string[] ExtractJsonArray(this string input)
        {
            List<string> data = new List<string>();

            if (!string.IsNullOrEmpty(input) && input.StartsWith("[") && input.EndsWith("]"))
            {
                var tempinput = input.ToString();
                tempinput = tempinput.ReplaceFirst("[", "");
                tempinput = tempinput.ReplaceLastOccurrence("]", "");

                data.AddRange(Regex.Split(tempinput, ","));

                return data.Select(p => p.Replace("\r", "").Replace("\n", "").Replace("\"", "")).ToArray();
            }
            return data.ToArray();

        }
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }
        public static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }
    }
}

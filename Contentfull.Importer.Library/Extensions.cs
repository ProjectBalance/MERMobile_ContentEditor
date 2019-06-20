using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Contentful.Importer.Library.Extensions
{
    public static class Extensions
    {
        public enum Datatype
        {
            Text,
            Symbol,
            Integer,
            Object,
            Link,
            Date,
            Boolean,
            Array
        }
        public static string[] GetRequiredFieldValues(this Field field)
        {           
            if (field.Validations != null && field.Validations.Count() > 0)
            {
                foreach (var validation in field.Validations)
                {
                    if (validation.GetType() == typeof(Contentful.Core.Models.Management.InValuesValidator))
                    {
                        var validator = (Contentful.Core.Models.Management.InValuesValidator)validation;
                        return validator.RequiredValues.ToArray();
                    }
                }
            }
            return null;
        }
        public static Datatype GetDataType(this Field field)
        {
            switch (field.Type)
            {
                case "Integer":
                    return Datatype.Integer;
                    break;
                case "Symbol":
                    return Datatype.Symbol;
                    break;
                case "Text":
                    return Datatype.Text;
                    break;
                case "Date":
                    return Datatype.Date;
                    break;
                case "Boolean":
                    return Datatype.Boolean;
                    break;
                case "Object":
                    return Datatype.Object;
                    break;
                case "Link":
                    return Datatype.Link;
                    break;
                case "Array":
                    return Datatype.Array;
                    break;

                default:
                    return Datatype.Symbol;
                        
            }
        }
        public static bool IsCountryType(this Field field)
        {
            return field.Name.ToLower().Trim().Equals("country");
        }

        public static bool IsStringLongOrArrayData(this string input)
        {
            if (!string.IsNullOrEmpty(input) &&
                (
                (input.Length > 50)
                ||
                (input.StartsWith("[") && input.EndsWith("]"))
                ||
                (input.StartsWith("{") && input.EndsWith("}"))
                )
                
               )
            {
                return true;

            }
            return false;
        }
        public static bool IsExportDeniedDataFound(this string input)
        {
            if (!string.IsNullOrEmpty(input) &&
                (
                (input.Length > 50)
                ||
                (input.StartsWith("[") && input.EndsWith("]"))
                ||
                (input.StartsWith("{") && input.EndsWith("}"))
                )

               )
            {
                if (input.Length > 32000)
                {
                    return true;
                }
                var countnewline = Regex.Split(input,"\n").Length + Regex.Split(input, Environment.NewLine).Length;
                if(countnewline > 253)
                {
                    return true;
                }
            }
            return false;
        }
        public static string[] ExtractJsonArray(this string input)
        {
            List<string> data = new List<string>();

            if (!string.IsNullOrEmpty(input) && input.StartsWith("[") && input.EndsWith("]"))
            {
                var tempinput = input.ToString();
                tempinput = tempinput.ReplaceFirst("[", "");
                tempinput = tempinput.ReplaceLastOccurrence("]", "");

                data.AddRange(Regex.Split(tempinput, ","));

                return data.Select(p => p.Replace("\r", "").Replace("\n", "").Replace("\"", "").Trim()).ToArray();
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

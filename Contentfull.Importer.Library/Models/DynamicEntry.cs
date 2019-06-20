using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Importer.Library.Extensions;

namespace Contentful.Importer.Library.Models
{
    public class DynamicEntry
    {
        public string SysID { get;  private set; }
        public List<JProperty> FieldData { get; private set; }
        public Dictionary<string, Dictionary<string, string>> LineData { get; set; }
        public List<string> LocaleKeys { get; private set; }
        /// <summary>
        /// Returns the lines as a Dictionary of key value pairs for a specific locale
        /// Returns null if locale does not exist
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetLineDataForLocale(string locale)
        {
            if (LocaleKeys.Contains(locale) && LineData.Count() > 0)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach(KeyValuePair<string, Dictionary<string,string>> line in LineData)
                {
                    if (line.Value.ContainsKey(locale))
                    {
                        data.Add(line.Key, line.Value[locale]);
                    }
                    else
                    {
                        data.Add(line.Key, "");
                    }
                }
                return data;
            }
            return null;
        }

        public DynamicEntry(string ID)
        {
            this.SysID = ID;
            this.FieldData = new List<JProperty>();
            this.LocaleKeys = new List<string>();            
        }
        public DynamicEntry()
        {
        }
        private void LoadDistinctLocaleKey(string value)
        {
            var exists = LocaleKeys.FirstOrDefault(p => p.Equals(value)) != null;
            if (!exists)
            {
                LocaleKeys.Add(value);
            }
        }
        internal void LoadFieldDictionary(string typeID)
        {
            LineData = new Dictionary<string, Dictionary<string, string>>();
            foreach(var field in FieldData)
            {
                bool bigDatafound = false;
                var tokenvalue = new  Dictionary<string, string>();
                var fieldChildren = field.Value.Children<JProperty>().ToArray();
                foreach (var content in fieldChildren)
                {
                    if (!bigDatafound)
                    {//check if value should exluded column from display
                        bigDatafound = content.Value.ToString().Trim().IsStringLongOrArrayData();
                    }
                    tokenvalue.Add(content.Name, content.Value.ToString());
                    LoadDistinctLocaleKey(content.Name);                    
                }
                LineData.Add(field.Name, tokenvalue);
                if (bigDatafound)
                {
                    Contentful.Importer.Library.ContentFul.Client.ContentTypes.First(p => p.TypeID.Equals(typeID)).ExcludeFieldFromGrid(field.Name);
                }
                
            }
        }
        
    }
}

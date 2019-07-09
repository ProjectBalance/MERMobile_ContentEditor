using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Importer.Library.Extensions;
using Newtonsoft.Json;

namespace Contentful.Importer.Library.Models
{
    public class ContentFulRowData
    {

        public string SysID { get; set; }
        public int VersionNo { get; set; }
        public Dictionary<string, string> FieldData { get; set; }

        public ContentFulRowData()
        {
            FieldData = new Dictionary<string, string>();
        }
        public ContentFulRowData(string sysID, int version, string typeid, Entry<dynamic> entry)
        {
            this.SysID = sysID;
            this.VersionNo = version;
            FieldData = new Dictionary<string, string>();
            LoadDictionary(entry, typeid);
        }
        /// <summary>
        /// Gets the dynamic entry used for Adding or Updating data on contentful
        /// </summary>
        /// <param name="fields"> Field list</param>
        /// <returns></returns>
        public Entry<dynamic> GetDynamicEntry(Core.Models.Field[] fields)
        {
            Entry<dynamic> entryData = new Entry<dynamic>();
            Dictionary<string, object> dataDictionary = new Dictionary<string, object>();
            List<JProperty> props = new List<JProperty>();
            foreach (var field in fields)
            {
                var dobj = GetFieldDictionary(field);
                if (dobj != null)
                {
                    dataDictionary.Add(field.Id, dobj);
                }
            }
            entryData.SystemProperties = new SystemProperties();
            if (!string.IsNullOrEmpty(SysID))
            {
                entryData.SystemProperties.Id = SysID;
                entryData.SystemProperties.Version = VersionNo;
            }
            else
            {
                entryData.SystemProperties.Id = Guid.NewGuid().ToString();
            }
            entryData.Fields = dataDictionary;
            return entryData;
        }
        private JProperty GetField(Core.Models.Field field)
        {
            object data = null;
            var value = GetDataByFieldID(field.Id);
            if (field.GetDataType() == Extensions.Extensions.Datatype.Integer)
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = "-1";
                }
                data = new Dictionary<string, int>();
                (data as Dictionary<string, int>).Add("en-US", int.Parse(value));
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Object)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var val = JsonConvert.DeserializeObject(value);
                    data = new Dictionary<string, object>();
                    (data as Dictionary<string, object>).Add("en-US", val);
                }
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Link)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var val = JsonConvert.DeserializeObject(value);
                    data = new Dictionary<string, object>();
                    (data as Dictionary<string, object>).Add("en-US", val);
                }
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Date)
            {
                var val = DateTime.Parse(value);
                data = new Dictionary<string, string>();
                (data as Dictionary<string, string>).Add("en-US", value);
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Boolean)
            {

                data = new Dictionary<string, bool>();
                (data as Dictionary<string, bool>).Add("en-US", bool.Parse(value));
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Array)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var val = JsonConvert.DeserializeObject<object[]>(value);
                    data = new Dictionary<string, object[]>();
                    (data as Dictionary<string, object[]>).Add("en-US", val);
                }
            }
            else
            {
                data = new Dictionary<string, string>();
                (data as Dictionary<string, string>).Add("en-US", value);
            }
            return new JProperty(field.Id, new Dictionary<string, string>());
        }
        public string GetDataByFieldID(string fieldID)
        {
            try
            {
                return FieldData[fieldID].Replace("\n", Environment.NewLine);
            }
            catch
            {
                return "";
            }
        }
        public object GetFieldDictionary(Core.Models.Field field)
        {
            object data = null;
            var value = FieldData[field.Id];
            if (field.GetDataType() == Extensions.Extensions.Datatype.Integer)
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = "-1";
                }
                data = new Dictionary<string, int>();
                (data as Dictionary<string, int>).Add("en-US", int.Parse(value));
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Object)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var val = JsonConvert.DeserializeObject(value);
                    data = new Dictionary<string, object>();
                    (data as Dictionary<string, object>).Add("en-US", val);
                }
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Link)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var val = JsonConvert.DeserializeObject(value);
                    data = new Dictionary<string, object>();
                    (data as Dictionary<string, object>).Add("en-US", val);
                }
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Date)
            {
                var val = DateTime.Parse(value);
                data = new Dictionary<string, string>();
                (data as Dictionary<string, string>).Add("en-US", value);
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Boolean)
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = "false";
                }
                data = new Dictionary<string, bool>();
                (data as Dictionary<string, bool>).Add("en-US", bool.Parse(value));
            }
            else if (field.GetDataType() == Extensions.Extensions.Datatype.Array)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var val = JsonConvert.DeserializeObject<object[]>(value);
                    data = new Dictionary<string, object[]>();
                    (data as Dictionary<string, object[]>).Add("en-US", val);
                }
            }
            else
            {
                data = new Dictionary<string, string>();
                (data as Dictionary<string, string>).Add("en-US", value);
            }
            return data;
        }
        private void LoadDictionary(Entry<dynamic> entry, string typeID)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (JProperty field in entry.Fields)
            {

                bool bigDatafound = false;
                bool exportDeniedDataFound = false;
                var tokenvalue = new Dictionary<string, string>();
                var fieldChildren = field.Value.Children<JProperty>();
                foreach (var fieldChild in fieldChildren)
                {
                    if (fieldChild.Name.Equals("en-US"))
                    {
                        if (!bigDatafound)
                        {//check if value should exluded column from display
                            bigDatafound = fieldChild.Value.ToString().Trim().IsStringLongOrArrayData();
                        }
                        if (!exportDeniedDataFound)
                        {
                            exportDeniedDataFound = fieldChild.Value.ToString().Trim().IsExportDeniedDataFound();
                        }
                        data.Add(field.Name, fieldChild.Value.ToString());
                    }
                    else
                    {
                        data.Add(field.Name, "");
                    }
                }

                if (bigDatafound)
                {
                    Contentful.Importer.Library.ContentFul.Client.ContentTypes.First(p => p.TypeID.Equals(typeID)).ExcludeFieldFromGrid(field.Name);
                }
                


            }
            this.FieldData = data;
        }

        public bool ValidateRow(Core.Models.Field[] fields, int rowIndex, out string[] reportresult)
        {
            bool result = true;
            var report = new List<string>();
            foreach (var field in fields)
            {

                var value = FieldData[field.Id];
                if (string.IsNullOrEmpty(value) && field.Required)
                {
                    if (rowIndex > -1)
                    {
                        report.Add("Fail on row " + rowIndex + " column " + field.Name + " can not be empty");
                    }
                    else
                    {
                        report.Add(field.Name + " can not be empty");
                    }
                    result = false;
                }
                if (field.IsCountryType())
                {
                    var userCountry = Library.ContentFul.Client.AvailableCountries[Library.ContentFul.Client.SelectedCountryIndex];
                    if (!value.Equals(userCountry))
                    {
                        if (rowIndex > -1)
                        {
                            report.Add("Fail on row " + rowIndex + " column " + field.Name + " user is currently configured to use country " + userCountry);
                        }
                        else
                        {
                            report.Add(field.Name + " user is currently configured to use country " + userCountry);

                        }
                        result = false;
                    }
                }
                else if (field.GetDataType() == Extensions.Extensions.Datatype.Integer && field.Required && string.IsNullOrEmpty(value))
                {
                    int val;
                    if (!int.TryParse(value, out val))
                    {
                        if (rowIndex> -1)
                        {
                            report.Add("Fail on row " + rowIndex + " column " + field.Name + " does not have a valid integer value");
                        }
                        else
                        {
                            report.Add( field.Name + " does not have a valid integer value");
                        }
                        
                        result = false;
                    }

                }
                else if(field.GetDataType() == Extensions.Extensions.Datatype.Link || field.GetDataType() == Extensions.Extensions.Datatype.Object)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            JsonConvert.DeserializeObject(value);
                        }
                        catch
                        {
                            var message = "Fail on row: " + rowIndex + " column ";
                            if (rowIndex < 0)
                            {
                                message = "";
                            }
                            message += "'" + field.Name + "' needs to be in JSON Format";
                            report.Add(message);
                            result = false;
                        }

                    }
                }
                else if (field.GetDataType() == Extensions.Extensions.Datatype.Array)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            JsonConvert.DeserializeObject<object[]>(value);
                        }
                        catch
                        {
                            var message = "Fail on row: " + rowIndex + " column ";
                            if (rowIndex < 0)
                            {
                                message = "";
                            }
                            message += "'" + field.Name + "' needs to be in JSON Array Format";
                            report.Add(message);
                            result = false;
                        }

                    }
                }
                else if (field.Validations != null && field.Validations.Count() > 0)
                {
                    foreach (var validation in field.Validations)
                    {
                        if (validation.GetType() == typeof(Contentful.Core.Models.Management.InValuesValidator))
                        {
                            var validator = (Contentful.Core.Models.Management.InValuesValidator)validation;
                            if (!validator.RequiredValues.Contains(value))
                            {
                                var message = "Fail on row: " + rowIndex + " column ";
                                if (rowIndex < 0)
                                {
                                    message = "";
                                }
                                message += "'" + field.Name + "' needs to have a value of: " + string.Join(",", validator.RequiredValues);
                                message = message.ReplaceLastOccurrence(",", " or ");
                                report.Add(message);
                                result = false;
                            }
                        }
                    }
                }
            }
            reportresult = report.ToArray();
            return result;
        }
        public bool IsAllDataEmpty
        {
            get
            {
                foreach (var field in FieldData)
                {
                    if (!string.IsNullOrEmpty(field.Value))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public object JSonConvert { get; private set; }
    }
}

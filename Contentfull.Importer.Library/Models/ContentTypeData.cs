using Contentful.Core.Models;
using Contentful.Importer.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contentful.Importer.Library.Models
{
    public class ContentTypeData
    {

        public ContentType ContentFullType { get; set; }
        public List<ContentFulRowData> Entries { get; set; }

        public string TypeID { get { return ContentFullType.SystemProperties.Id; } }
        public string Label { get { return ContentFullType.Name; } }

        public List<string> ExcludeFieldIDs { get; set; }
        public bool ExcludeExport
        {
            get
            {

                foreach (var rows in Entries)
                {                    
                    var valid  = rows.FieldData.Where(p => p.Value.IsExportDeniedDataFound()).Count() > 0;
                    if (valid)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        internal void ExcludeFieldFromGrid(string fieldID)
        {
            if (!ExcludeFieldIDs.Contains(fieldID))
            {
                ExcludeFieldIDs.Add(fieldID);
            }
        }

        public ContentTypeData(ContentType contentFullType)
        {
            this.ContentFullType = contentFullType;
            Entries = new List<ContentFulRowData>();
            ExcludeFieldIDs = new List<string>();
        }
        public ContentTypeData() { }
        public List<Dictionary<string, string>> GetLineData()
        {
            List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();
            foreach (var entry in Entries)
            {
                var edata = entry.FieldData;
                if (edata != null)
                {
                    data.Add(edata);
                }
            }
            return data;
        }
        public override string ToString()
        {
            return TypeID;
        }
        public void LoadEntries(bool refresh = false)
        {
            if (Entries.Count() == 0 || refresh)
            {
                this.LoadEntries(ContentFul.Client.Instance);
            }
        }
        public void LoadEntries(ContentFul.Client apiClient)
        {
            this.Entries = apiClient.GetContentEntries(this.TypeID);
            var countryfield = this.ContentFullType.Fields.FirstOrDefault(p => p.IsCountryType() == true);
            if (countryfield != null)
            {
                var country = Contentful.Importer.Library.ContentFul.Client.AvailableCountries[Contentful.Importer.Library.ContentFul.Client.SelectedCountryIndex];
                this.Entries = Entries.Where(p => p.FieldData.ContainsKey(countryfield.Id) && p.FieldData[countryfield.Id].Equals(country)).ToList();

            }
        }
        public string[] GetColumns(bool byFieldID)
        {

            List<string> fieldIDlist = new List<string>();
            fieldIDlist.Add("SysID");
            foreach (var field in ContentFullType.Fields)
            {
                if (byFieldID)
                {
                    fieldIDlist.Add(field.Id);
                }
                else
                {
                    fieldIDlist.Add(field.Name);
                }

            }
            return fieldIDlist.ToArray();
        }


        public List<string[]> GetRowData(bool skipExcludedColumns = false, string[] spacerlabelsleft = null, string locale = "en-US")
        {
            var fieldIDlist = ContentFullType.Fields.Select(p => p.Id);
            List<string[]> list = new List<string[]>();
            foreach (var dataline in Entries)
            {
                List<string> row = new List<string>();
                if (spacerlabelsleft != null)
                {
                    foreach (var label in spacerlabelsleft)
                    {
                        row.Add(label);
                    }

                }
                row.Add(dataline.SysID);

                foreach (var fieldid in fieldIDlist)
                {
                    if (skipExcludedColumns && ExcludeFieldIDs.FirstOrDefault(p => p.Equals(fieldid)) != null)
                    {
                        continue;
                    }
                    if (dataline.FieldData.ContainsKey(fieldid))
                    {

                        row.Add(dataline.FieldData[fieldid]);

                    }
                    else
                    {
                        row.Add("");

                    }
                }
                list.Add(row.ToArray());
            }
            return list;
        }

        public List<string[]> GetWorksheetData()
        {
            List<string[]> list = new List<string[]>();
            list.Add(GetColumns(false));
            var rows = GetRowData();

            foreach (var row in rows)
            {
                foreach (var val in row)
                {
                    if (val.Length > 50000)
                    {
                        throw new Exception("Cell data is too big to be exported to a spreadsheet");
                    }
                }
            }
            list.AddRange(GetRowData());
            return list;
        }

    
    }
}

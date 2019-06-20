using Contentful.Importer.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Contentful.Importer.App.Interfaces
{
    public partial class ExcelImport : Form
    {
        private OfficeOpenXml.ExcelWorksheet WorkSheet { get; set; }
        private ContentTypeData TypeData { get; set; }
        bool ContainsSysID { get; set; } // If sheet is intended to update existing records
        int SysIDIndex { get; set; }
        Dictionary<string, int> FieldIndexDictionary { get; set; }

        private Dictionary<int, ContentFulRowData> RowData { get; set; }

        private string[] ImportColumns { get; set; }

        public ExcelImport()
        {
            InitializeComponent();
        }
        public ExcelImport(OfficeOpenXml.ExcelWorksheet workSheet, ContentTypeData typeData)
        {
            InitializeComponent();
            this.WorkSheet = workSheet;
            this.TypeData = typeData;
            Load += ExcelImport_Load;
        }

        #region Write to Logs
        private void AddColValidationLine(string value)
        {
            List<string> lines = new List<string>();
            lines.AddRange(txtColumn.Lines.Reverse());
            lines.Add(value);
            txtColumn.Lines = lines.ToArray().Reverse().ToArray();
        }
        private void AddDataValidationLine(string value)
        {
            List<string> lines = new List<string>();
            lines.AddRange(txtDataValidation.Lines.Reverse());
            lines.Add(value);
            txtDataValidation.Lines = lines.ToArray().Reverse().ToArray();
        }
        private void AddDataValidationLine(string[] value)
        {
            List<string> lines = new List<string>();
            lines.AddRange(txtDataValidation.Lines.Reverse());
            lines.AddRange(value);
            txtDataValidation.Lines = lines.ToArray().Reverse().ToArray();
        }
        #endregion
        private void ExcelImport_Load(object sender, EventArgs e)
        {
            if (ColumnValidation())
            {
                if (DataValidation())
                {                   
                    lblNewRows.Text = RowData.Where(p => string.IsNullOrEmpty(p.Value.SysID)).Count().ToString();
                    lblUpdateRows.Text = RowData.Where(p => !string.IsNullOrEmpty(p.Value.SysID)).Count().ToString();
                    btnImport.Enabled = true;
                }
            }

        }
        public bool ColumnValidation()
        {
            string[] importColumns = ExtractColumnNamesFromSheet();
            ImportColumns = importColumns;
            bool passed = true;

            ContainsSysID = importColumns.FirstOrDefault(p => p.ToLower().Contains("sysid")) != null;
            foreach (var field in TypeData.ContentFullType.Fields)
            {
                var importLine = importColumns.FirstOrDefault(p => p.Contains(field.Name.ToLower().Trim()));
                if (importLine != null)
                {
                    AddColValidationLine(field.Name + ":  Found");
                }
                else
                {
                    AddColValidationLine(field.Name + ":  Not Found");
                    passed = false;
                }

            }
            if (passed)
            {
                BuildFieldIndexDictionary();
                AddColValidationLine("FieldIndexes Mapped");
                AddColValidationLine("Column Validation Passed");
                lblColValidation.Text = "Passed";
                lblColValidation.ForeColor = Color.Green;

            }
            else
            {
                AddColValidationLine("Column Validation Failed");
                lblColValidation.Text = "Failed";
                lblColValidation.ForeColor = Color.Red;
            }
            return passed;
        }

        public bool DataValidation()
        {
            bool passed = true;
            var rowData = ExtractRowData();
            lblTotalRows.Text = rowData.Count().ToString();
            
            var fields = TypeData.ContentFullType.Fields.ToArray();
            List<int> failedRows = new List<int>();
            foreach (var rowItem in rowData)
            {
                AddDataValidationLine("Checking row: " + rowItem.Key);
                string[] report;
                var rowResult = rowItem.Value.ValidateRow(fields, rowItem.Key, out report);
                AddDataValidationLine(report);
                if (!rowResult)
                {
                    passed = false;
                    failedRows.Add(rowItem.Key);
                }
                //var entry = rowItem.Value.GetDynamicEntry(fields);
                //string entryData = JsonConvert.SerializeObject(entry);
                //int j = 0;
            }

            if (passed)
            {
                lblbDataValidation.Text = "Passed";
                AddDataValidationLine("Data Validation Passed");
                lblbDataValidation.ForeColor = Color.Green;
                this.RowData = rowData;
            }
            else
            {
                lblbDataValidation.Text = "Failed";
                lblbDataValidation.ForeColor = Color.Red;
                AddDataValidationLine("Data Validation Failed on lines :" + string.Join(",", failedRows));
            }

            return passed;
        }


        private string[] ExtractColumnNamesFromSheet()
        {
            List<string> headerLines = new List<string>();
            bool endofRowFound = false;
            int colCounter = 1;
            while (!endofRowFound)
            {
                if (WorkSheet.Cells[1, colCounter].Value != null)
                {
                    var value = WorkSheet.Cells[1, colCounter].Value.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        headerLines.Add(value.Trim().ToLower());
                        colCounter++;
                    }
                    else
                    {
                        endofRowFound = true;
                    }
                }
                else
                {
                    endofRowFound = true;
                }
            }


            return headerLines.ToArray();
        }
        private void BuildFieldIndexDictionary()
        {
            FieldIndexDictionary = new Dictionary<string, int>();

            if (ContainsSysID)
            {
                int index = -1;
                int runningIndex = 0;
                foreach (var col in ImportColumns)
                {
                    if (col.ToLower().Contains("sysid"))
                    {
                        ContainsSysID = true;
                        index = runningIndex;
                        SysIDIndex = index;
                        break;
                    }
                    else
                    {
                        runningIndex++;
                    }
                }
                lblRotalFields.Text = (TypeData.ContentFullType.Fields.Count() + 1).ToString();
            }
            else
            {
                lblRotalFields.Text = (TypeData.ContentFullType.Fields.Count()).ToString();
            }

            foreach (var field in TypeData.ContentFullType.Fields)
            {

                int index = -1;
                int runningIndex = 0;
                foreach (var col in ImportColumns)
                {
                    if (col.Contains(field.Name.ToLower().Trim()))
                    {
                        index = runningIndex;
                        FieldIndexDictionary.Add(field.Id, index);
                        break;
                    }
                    else
                    {
                        runningIndex++;
                    }
                }
            }
        }


        /// <summary>
        /// Row (FieldID,data) representaion of the data 
        /// /// </summary>
        private Dictionary<int, ContentFulRowData> ExtractRowData()
        {
            var data = new Dictionary<int, ContentFulRowData>();
            int rowIndex = 2;
            bool EndOfRowFound = false;
            while (!EndOfRowFound)
            {
                ContentFulRowData rowData = new ContentFulRowData();
                if (ContainsSysID)
                {
                    rowData.SysID = GetCellData(rowIndex, SysIDIndex + 1);
                }
                foreach (var field in FieldIndexDictionary.Keys)
                {
                    int index = FieldIndexDictionary[field];
                    string value = GetCellData(rowIndex, (index + 1));
                    rowData.FieldData.Add(field, value);
                }
                if (!rowData.IsAllDataEmpty)
                {
                    data.Add(rowIndex, rowData);
                    rowIndex++;
                }
                else
                {
                    EndOfRowFound = true;
                }

            }
            return data;
        }
        public string GetCellData(int row, int column)
        {
            return WorkSheet.Cells[row, column].Value != null ? WorkSheet.Cells[row, column].Value.ToString() : "";
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            var fields = TypeData.ContentFullType.Fields.ToArray();

            BackgroundWorker bw = new BackgroundWorker();

            var newrowcount = lblNewRows.Text;
            var updaterowcount = lblUpdateRows.Text;
        
            bw.DoWork += delegate
            {
                SetButtonsEnabledDisabled(false);
                int counter = 1;
                foreach (var row in RowData.Where(p => string.IsNullOrEmpty(p.Value.SysID)))
                {
                    var entry = row.Value.GetDynamicEntry(fields);
                    try
                    {
                        SetImportProgressLabel("Importing new data "+counter+" of "+newrowcount);
                        var result = Library.ContentFul.Client.Instance.AddContent(entry, TypeData.TypeID);
                        var publishResult = Library.ContentFul.Client.Instance.PublishContent(result.SystemProperties.Id, result.SystemProperties.Version ?? 1);
                        int j = 0;
                        counter++;
                        
                    }
                    catch (Exception err)
                    {
                        int j = 0;
                        counter++;
                    }
                }
                counter = 1;
                foreach (var row in RowData.Where(p => !string.IsNullOrEmpty(p.Value.SysID)))
                {
                    var entry = row.Value.GetDynamicEntry(fields);
                    try
                    {
                        SetImportProgressLabel("Importing update data " + counter + " of " + updaterowcount);
                        var result = Library.ContentFul.Client.Instance.UpdateContent(entry,TypeData.TypeID);
                        int j = 0;
                        counter++;
                    }
                    catch (Exception err)
                    {
                        int j = 0;
                        counter++;
                    }
                }
                SetButtonsEnabledDisabled(true);
                Library.ContentFul.Client.Instance.UpdateLanguageCountryVersion();
                ReturnDialogOK() ;
            };


            bw.RunWorkerAsync();


        }
        private void ReturnDialogOK()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }));
        }
        private void SetButtonsEnabledDisabled(bool enabled )
        {
            this.Invoke(new MethodInvoker(delegate
            {
                btnCancel.Enabled = enabled;
                btnImport.Enabled = enabled;
            }));
        }
        private void SetImportProgressLabel(string value)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                lblProgress.Text = value;
            }));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

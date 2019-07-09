using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Contentful.Importer.Library.Models;

namespace Contentful.Importer.App.Interfaces
{
    public partial class ContentTypeGrid : UserControl
    {
        private ContentTypeData TypeData { get; set; }
        private bool Initialized = false;
      
        public ContentTypeGrid()
        {
            InitializeComponent();
            grid.AllowUserToAddRows = false;
        }

        public void SetContentfulType(Library.Models.ContentTypeData type)
        {
            this.TypeData = type;
        }
        private void LoadContentfulType()
        {
            TypeData.LoadEntries(true);
            
            grid.Columns.Add("SysID", "SysID");
            
            grid.Columns[2].Visible = false;
            List<string> fieldIDlist = new List<string>();
            foreach (var field in this.TypeData.ContentFullType.Fields.Where(p => !this.TypeData.ExcludeFieldIDs.Contains(p.Id)))
            {
                grid.Columns.Add("col_" + field.Id, field.Name);
                fieldIDlist.Add(field.Id);
            }
            Initialized = true;
            LoadDataRows(false);

        }

        public void LoadDataRows(bool refresh = true)
        {
            if (!Initialized)
            {
                
                LoadContentfulType();
                
                return;

            }
            if (refresh)
            {
                TypeData.LoadEntries(true);
            }
            grid.Rows.Clear();
            bool exportDeniedDatafound = false;
            foreach (var row in this.TypeData.GetRowData(true, new string[] { "Edit","Remove" }))
            {
                DataGridViewRow dgview = new DataGridViewRow();
                       foreach (var celldata in row)
                {
                    dgview.Cells.Add(new DataGridViewTextBoxCell() { Value = celldata });
                   
                }
                grid.Rows.Add(dgview);
            }
            grid.Refresh();
            if (TypeData.ExcludeExport)
            {
                btnBulkUpload.Visible = false;
                btnExportAsNew.Visible = false;
                btnUploadToGSheets.Visible = false;
            }
        }

        private void btnUploadToGSheets_Click(object sender, EventArgs e)
        {
            var result = openExcelFile.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(openExcelFile.FileName))
                {
                    try
                    {                        
                        using (var excelWorkbook = new Library.ExcelDataTable(openExcelFile.FileName))
                        {
                            var sheet = excelWorkbook.Workbook.Worksheets[TypeData.Label];
                            if (sheet != null)
                            {
                                var confirmresult = MessageBox.Show(this, "Found an existing sheet named "+TypeData.Label+", all data will be cleared, \ndo you wish to proceed ?", "confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (confirmresult == DialogResult.Yes)
                                {
                                    excelWorkbook.ClearSheetData(sheet);
                                    excelWorkbook.SavePackage();
                                }
                                var data = TypeData.GetWorksheetData();
                                int rowindex = 1;
                                foreach (var row in data)
                                {
                                    int columnindex = 1;
                                    foreach (var value in row)
                                    {
                                        sheet.SetValue(rowindex, columnindex, value);
                                        columnindex++;
                                    }
                                    rowindex++;
                                }
                                excelWorkbook.SavePackage();
                            }
                            else
                            {
                                excelWorkbook.Workbook.Worksheets.Add(TypeData.Label);
                                sheet = excelWorkbook.Workbook.Worksheets[TypeData.Label];                               
                                var data = TypeData.GetWorksheetData();
                                int rowindex = 1;
                                foreach(var row in data)
                                {
                                    int columnindex = 1;
                                    foreach(var value in row)
                                    {
                                        sheet.SetValue(rowindex, columnindex, value);
                                        columnindex++;
                                    }
                                    rowindex++;
                                }
                                excelWorkbook.SavePackage();
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(this, "Error Loading workbook , please ensure it is not open in Excel.\n Error Info:" + err.Message, "Workbook error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var entry = new EditSingleEntry();
            entry.LoadContentType(TypeData);
            var result = entry.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                LoadDataRows();
            }
        }
        private void grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == grid.Columns["Edit_col_btn_col"].Index && e.RowIndex >= 0)
            {
                var ID = grid.Rows[e.RowIndex].Cells[2].Value;
                var rowdata = TypeData.Entries.First(p => p.SysID.Equals(ID));
                var entry = new EditSingleEntry();
                entry.LoadContentType(TypeData, rowdata);
                var result = entry.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    LoadDataRows();
                }
            }
            else if(e.ColumnIndex == grid.Columns["Delete_col_btn_col"].Index && e.RowIndex >= 0)
            {
                var ID = grid.Rows[e.RowIndex].Cells[2].Value;
                var rowdata = TypeData.Entries.First(p => p.SysID.Equals(ID));
                var result = MessageBox.Show(this, "Are you sure? \nThe record will be permanently deleted.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    Contentful.Importer.Library.ContentFul.Client.Instance.DeleteEntry(ID.ToString());
                    MessageBox.Show(this, "Record deleted.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Library.ContentFul.Client.Instance.UpdateLanguageCountryVersion();
                    LoadDataRows();
                }                
            }
        }

        private void btnBulkUpload_Click(object sender, EventArgs e)
        {
            var result = openExcelFile.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(openExcelFile.FileName))
                {
                    try
                    {
                        using (var excelWorkbook = new Library.ExcelDataTable(openExcelFile.FileName))
                        {

                            var sheet = excelWorkbook.Workbook.Worksheets[TypeData.Label];
                            if (sheet != null)
                            {
                                var importer = new ExcelImport(sheet, TypeData);
                                var importResult = importer.ShowDialog(this);
                                if (importResult == DialogResult.OK)
                                {
                                    LoadDataRows();
                                }
                            }
                            else
                            {
                                MessageBox.Show(this, "Could not find sheet with name: " + TypeData.Label, "Validation Error - Data import Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(this, "Error Loading workbook , please ensure it is not open in Excel.\n Error Info:" + err.Message, "Workbook error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }

        private void btnExportAsNew_Click(object sender, EventArgs e)
        {
            var result = saveExcelFile.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(saveExcelFile.FileName))
                {
                    try
                    {
                        using (var excelWorkbook = new Library.ExcelDataTable())
                        {
                          
                            var sheet = excelWorkbook.Workbook.Worksheets[TypeData.Label];
                            if (sheet == null)
                            {
                                excelWorkbook.Workbook.Worksheets.Add(TypeData.Label);
                                sheet = excelWorkbook.Workbook.Worksheets[TypeData.Label];
                                var data = TypeData.GetWorksheetData();
                                int rowindex = 1;
                                foreach (var row in data)
                                {
                                    int columnindex = 1;
                                    foreach (var value in row)
                                    {
                                        sheet.SetValue(rowindex, columnindex, value);
                                        columnindex++;
                                    }
                                    rowindex++;
                                }
                                excelWorkbook.SaveAs(saveExcelFile.FileName);
                            }
                          
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(this, "Error Loading workbook , please ensure it is not open in Excel.\n Error Info:" + err.Message, "Workbook error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }
    }
}

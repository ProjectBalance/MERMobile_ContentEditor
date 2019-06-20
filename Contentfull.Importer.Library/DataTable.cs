using OfficeOpenXml;
using System;

namespace Contentful.Importer.Library
{
    public class ExcelDataTable : IDisposable
    {
        public ExcelWorkbook Workbook { get; private set; }
        ExcelPackage package { get; set; }
        public ExcelDataTable(string filepath)
        {
            LoadExcelPackage(filepath);
        }
        public ExcelDataTable()
        {
            package = new ExcelPackage();
            var workbook = package.Workbook;
            this.Workbook = workbook;

        }

        private void LoadExcelPackage(string localPath)
        {
            package = new ExcelPackage(new System.IO.FileInfo(localPath));

            var workbook = package.Workbook;
            this.Workbook = workbook;
            //var worksheet = workbook.Worksheets.First();
            //worksheet.Cells[0, 0].GetValue<string>();           

        }

        public void ClearSheetData( ExcelWorksheet sheet)
        {
            int endofcolumns = 1;
            int endofrows = 1;
            var lastvalue = "test";
            //Column Check
            while (!string.IsNullOrEmpty(lastvalue))
            {
                lastvalue = sheet.Cells[endofrows, endofcolumns].Value != null ? sheet.Cells[endofrows, endofcolumns].Value.ToString() : null;
                if (!string.IsNullOrEmpty(lastvalue))
                {
                    endofcolumns++;
                }
            }
            //RowCheck
            
            for (int col = 1; col <= endofcolumns; col++)
            {
                lastvalue = "test";
                int runningrow = 1;
                while (!string.IsNullOrEmpty(lastvalue))
                {
                    lastvalue = sheet.Cells[runningrow, col].Value != null ? sheet.Cells[runningrow, col].Value.ToString() : null;
                    if (!string.IsNullOrEmpty(lastvalue))
                    {
                        runningrow++;
                    }
                }

                if (endofrows < runningrow)
                {
                    endofrows = runningrow;
                }
            }
            for(int x = 1; x<= endofcolumns; x++)
            {
                for (int y = 1; y <= endofrows; y++)
                {
                    sheet.SetValue(y, x, "");
                }
            }

        }
        public void SavePackage()
        {
            package.Save();
        }
        public void SaveAs(string filename)
        {
            package.SaveAs(new System.IO.FileInfo(filename));
        }

        public void Dispose()
        {
            package = null;
            // package.Dispose();
        }
    }
}

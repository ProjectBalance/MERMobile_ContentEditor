using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace Contentful.Importer.Library
{
    public class GoogleSheets
    {
        static string ApplicationName = "ContentfulManagement";
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private SheetsService Service { get; set; }
        public GoogleSheets()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

        }
        public string[] ColSelector = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", };

        public BatchUpdateSpreadsheetResponse CreateSheet(string sheetName)
        {
            String spreadsheetId = "13CwpWvOWW1B-lyB_kzzvcCeQPukM5AgGoZ32l7bl5e4";
            
            var addSheetRequest = new AddSheetRequest();
            addSheetRequest.Properties = new SheetProperties();
            addSheetRequest.Properties.Title = sheetName;
            BatchUpdateSpreadsheetRequest batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest();
            batchUpdateSpreadsheetRequest.Requests = new List<Request>();
            batchUpdateSpreadsheetRequest.Requests.Add(new Request
            {
                AddSheet = addSheetRequest
            });

            var batchUpdateRequest =
                Service.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, spreadsheetId);
            try
            {
                var res = batchUpdateRequest.Execute();
                return res;
            }
            catch { }
            return null;

        }
        public UpdateValuesResponse UploadData(IList<IList<object>> values)
        {
            String spreadsheetId = "13CwpWvOWW1B-lyB_kzzvcCeQPukM5AgGoZ32l7bl5e4";
            String rangeIP = "IP Report!A1:B1";
            String rangeDate = "IP Report!B1";
            ValueRange DataRange = new ValueRange();
            IList<IList<object>> vals = new List<IList<object>>();
            DataRange.Values = vals;
            DataRange.Values.Clear();

            // IpRange.Values.Add(new String[] { IP, DateTime.Now.ToString() });

            var reqUpdateIP = Service.Spreadsheets.Values.Update(DataRange, spreadsheetId, rangeIP);
            reqUpdateIP.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var response = reqUpdateIP.Execute();
            return response;
        }

        public UpdateValuesResponse UploadData(string SheetName, List<string[]> values)
        {
            CreateSheet(SheetName);
            int yLimit = values.Count();
            int xLimit = values.First().Length;
            string colSelect = ColSelector[xLimit - 1];
            String spreadsheetId = "13CwpWvOWW1B-lyB_kzzvcCeQPukM5AgGoZ32l7bl5e4";
            String rangeIP = SheetName + "!A1:" + colSelect + yLimit;
            ValueRange DataRange = new ValueRange();
            IList<IList<object>> vals = new List<IList<object>>();
            DataRange.Values = vals;
            DataRange.Values.Clear();
            foreach (var row in values)
            {
                DataRange.Values.Add(row);
            }

            var updateRequest = Service.Spreadsheets.Values.Update(DataRange, spreadsheetId, rangeIP);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            var response = updateRequest.Execute();
            return response;
        }
    }
}

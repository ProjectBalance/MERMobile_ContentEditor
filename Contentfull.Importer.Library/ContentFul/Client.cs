using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Contentful.Importer.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Contentful.Importer.Library.Extensions;
using System.Windows.Forms;

namespace Contentful.Importer.Library.ContentFul
{
    public class Client
    {
        public static Client Instance { get; set; }
        public static string SelectedUsername{get;set;}
        public static int SelectedCountryIndex { get; set; }
        public static string[] AvailableCountries { get; set; }
        public static ContentTypeData[] ContentTypes { get; set; }
    
        private string AuthToken { get; set; }
        private readonly ContentfulManagementClient _apiClient;
        private readonly string Space;        
        public Client(string AuthToken, string Space)
        {
            this.AuthToken = AuthToken;
            this.Space = Space;
            var httpClient = new HttpClient();
            _apiClient = new ContentfulManagementClient(httpClient, AuthToken, Space);

        }
        private void Initialize()
        {
            ContentTypes = GetcontentTypeData();
        }
        public bool ValidateUser(ref string ResultText, string username)
        {
            //try
            //{
                Initialize();
                var userType = Client.ContentTypes.FirstOrDefault(p => p.TypeID.Equals("importUsers"));
                if(userType != null)
                {
                    userType.LoadEntries();
                    var data = userType.GetLineData();
                    var userLine = data.FirstOrDefault(p => p["userName"].ToLower().Equals(username.ToLower()));
                    if(userLine != null)
                    {
                        Library.ContentFul.Client.AvailableCountries = userLine["contentAccess"].ExtractJsonArray() ;
                        if (Library.ContentFul.Client.AvailableCountries.Length > 0)
                        {
                            SelectedCountryIndex = 0;
                            Library.ContentFul.Client.SelectedUsername = userLine["userName"];
                        }
                        else
                        {
                            ResultText = "No countries available for user";
                            return false;
                        }
                    }
                    else
                    {
                        ResultText = "The username could not be found";
                        return false;
                    }
                    

                }
                else
                {
                    ResultText = "Error on contentful , could not find type ID: importUsers";
                    return false;
                }

                return true;
            //}
            //catch(Exception err)
            //{
            //    ResultText = err.Message;
            //    return false;
            //}
        }       

        public IEnumerable<Core.Models.ContentType> GetcontentTypes()
        {
            return _apiClient.GetContentTypes(Space).Result;
        }
        public ContentTypeData[] GetcontentTypeData(bool loadEntries = true)
        {
            List<ContentTypeData> data = new List<ContentTypeData>();
            var ctData = GetcontentTypes();
            foreach( var cttype in ctData)
            {
                data.Add(new ContentTypeData(cttype));
            }

            return data.ToArray();
        }

        public Entry<dynamic> AddContent(Entry<dynamic> newData, string typeid)
        {
            return _apiClient.CreateEntry(newData, typeid, Space).Result;
        }
        public Entry<dynamic> PublishContent(string entryID,int lastVersion)
        {
            return _apiClient.PublishEntry(entryID, lastVersion, Space).Result;            
        }
        public Entry<dynamic> UpdateContent(Entry<dynamic> newData,string contenttypeid)
        {
            var current = _apiClient.GetEntry(newData.SystemProperties.Id).Result;

            newData.SystemProperties = current.SystemProperties;
            //newData.SystemProperties.Version++;
            var task =  _apiClient.CreateOrUpdateEntry(newData,Space, contenttypeid, newData.SystemProperties.Version.Value).Result;
            PublishContent(current.SystemProperties.Id, task.SystemProperties.Version ?? 1);
            //_apiClient.CreateOrUpdateEntry(newData)
            return task;
            
        }
        public void DeleteEntry(string entryID)
        {
            var up = UnpublishEntry(entryID);
            var current = _apiClient.GetEntry(entryID).Result;
            var res = _apiClient.DeleteEntry(entryID, current.SystemProperties.Version ?? 1, Space);
            res.Wait();           
           
        }

        public Entry<dynamic> UnpublishEntry(string entryID)
        {
            var current = _apiClient.GetEntry(entryID).Result;
            if ((current.SystemProperties.PublishedCounter ?? 0) > 0)
            {
                return _apiClient.UnpublishEntry(entryID, current.SystemProperties.Version ?? 1, Space).Result;
            }
            else
            {
                return current;
            }
        }


        public void UpdateLanguageCountryVersion()
        {
            var systemContentType = ContentTypes.FirstOrDefault(p => p.Label.Equals("_System"));
            systemContentType.LoadEntries(true);
            //This returns only one field value , this is becuase of the currently selected country 
            //which is good and acts as a second safeguard to updating values for only the country involved
            var currentCountry = AvailableCountries[SelectedCountryIndex];
            var rowData = systemContentType.GetRowData();
            var entry = systemContentType.Entries[0];
            if (entry.FieldData["country"].Equals(currentCountry))
            {
                var value = entry.FieldData["value"];
                var valueData = value.Split('.');
                if(valueData.Length != 2)
                {
                    MessageBox.Show("Error", "Error : system versioning for " + currentCountry + "does not conform to the required {date}.{version} format", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var valDate = valueData[0];
                int valVersion = 0;
                try
                {
                    valVersion = int.Parse(valueData[1]);
                }
                catch { }

                var currentDateString = DateTime.Now.ToString("yyyy-MM-dd");
                if (!valDate.Equals(currentDateString))
                {
                    valVersion = 0;
                }
                valVersion = valVersion + 1;
                string newValueString = currentDateString + "." + valVersion;
                if (valVersion < 10)
                {
                    newValueString = currentDateString + ".0" +valVersion;
                }
                

                entry.FieldData["value"] = newValueString;

                var dynUpdate = entry.GetDynamicEntry(systemContentType.ContentFullType.Fields.ToArray());

                UpdateContent(dynUpdate, systemContentType.TypeID);


                
            }
        }
        public List<ContentFulRowData> GetContentEntries(string typeID)
        {
            int skip = 0;
            int take = 50;
            var  data = new List<Models.ContentFulRowData>();


            // var categories = await _apiClient.GetEntriesByType("category", QueryBuilder<Category>.New.Include(5).LocaleIs(HttpContext?.Session?.GetString(Startup.LOCALE_KEY) ?? CultureInfo.CurrentCulture.ToString()));
            var type = ContentTypes.First(p => p.TypeID.Equals(typeID));
            bool allData = false;
            while (!allData)
            {
                
                var queryBuilder = QueryBuilder<Entry<dynamic>>.New.ContentTypeIs(typeID);
                
                queryBuilder = queryBuilder.Skip(skip).Limit(take).Include(10).OrderBy("-sys.createdAt");
                var entries = _apiClient.GetEntriesCollection<Entry<dynamic>>(queryBuilder).Result;
               
                foreach (var entry in entries)
                {                    
                    data.Add(new ContentFulRowData(entry.SystemProperties.Id, entry.SystemProperties.Version ?? 1, typeID, entry));                
                }

                if (entries.Items.Count() < take)
                {
                    allData = true;
                }
                else
                {
                    skip += take;
                }
            }
            return data;
        }

        




    }
}

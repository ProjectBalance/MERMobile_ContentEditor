using System;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Windows.Forms;
using mshtml;

namespace Contentful.Importer.App
{
    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    public partial class oAuthForm : Form
    {        
        public string ResultCode { get; set; }
        private WebBrowser2 wb = new WebBrowser2();
        public oAuthForm()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;
            WebBrowserHelper.ClearCache();
            //DeleteCache();
            wb.Dock = DockStyle.Fill;
            wb.ScriptErrorsSuppressed = true;
            wb.NavigateError += new WebBrowserNavigateErrorEventHandler(wb_NavigateError);
            
            wb.DocumentCompleted += Wb_DocumentCompleted;
            Controls.Add(wb);
            //wb.Refresh(WebBrowserRefreshOption.Completely);
            //wb.Navigate("javascript:void((function(){var a,b,c,e,f;f=0;a=document.cookie.split('; ');for(e=0;e<a.length&&a[e];e++){f++;for(b='.'+location.host;b;b=b.replace(/^(?:%5C.|[^%5C.]+)/,'')){for(c=location.pathname;c;c=c.replace(/.$/,'')){document.cookie=(a[e]+'; domain='+b+'; path='+c+'; expires='+new Date((new Date()).getTime()-1e11).toGMTString());}}}})())");
            // this is a basic code sample, quick & dirty way to get the Authentication string
            string authorizeURL = @"https://be.contentful.com/oauth/authorize?response_type=token&client_id=af524113ed68b8e8db2c7fe7b9ec8c2a0332e278adf9e6ae291b6c92e7d6058b&redirect_uri=https://127.0.0.1&scope=content_management_manage";
            //wb.Refresh(WebBrowserRefreshOption.Completely);
            //// now let's open the Authorize page.
            wb.Navigate(authorizeURL);
            


        }

        private void Wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = ((WebBrowser2)sender);
            IHTMLDocument2 doc = (browser.Document.DomDocument) as IHTMLDocument2;
            // The first parameter is the url, the second is the index of the added style sheet.
            IHTMLStyleSheet ss = doc.createStyleSheet("", 0);
          
            // Now that you have the style sheet you have a few options:
            // 1. You can just set the content as text.
            ss.cssText = @".account__header{display:none !important;} .account__title{display:none !important;} ";
            var meta = doc.createElement("meta");
            meta.setAttribute("http-equiv", "X-UA-Compatible");
            meta.setAttribute("content", "IE=edge");
            //wb.DocumentText= wb.DocumentText.Replace("IE=8", "IE=10");



            int j = 0;
        }

        private void wb_NavigateError(
      object sender, WebBrowserNavigateErrorEventArgs e)
        {
            // This will track errors: we want to track the 404 when the login
            // page redirects to our callback URL, let's check if is the error
            // we're tracking.
            Uri callbackURL = new Uri(e.Url.Replace("#","?"));
            if (e.Url.IndexOf("127.0.0.1") == -1)
            {
                MessageBox.Show("Sorry, the authorization failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // extract the code
            var query = HttpUtility.ParseQueryString(callbackURL.Query);
            string code = query["access_token"];
            this.ResultCode = code;
            this.DialogResult = DialogResult.OK;

            if (DialogResult == DialogResult.OK)
            {
                this.Close();
            }
            //wb.Navigate("javascript:void((function(){var a,b,c,e,f;f=0;a=document.cookie.split('; ');for(e=0;e<a.length&&a[e];e++){f++;for(b='.'+location.host;b;b=b.replace(/^(?:%5C.|[^%5C.]+)/,'')){for(c=location.pathname;c;c=c.replace(/.$/,'')){document.cookie=(a[e]+'; domain='+b+'; path='+c+'; expires='+new Date((new Date()).getTime()-1e11).toGMTString());}}}})())");

            // now we have the code, let's make a Http call to 
            // /authentication/v1/gettoken 
            // and get the access_token...

            // you can use RestSharp for it, but I'll stop the sample here

            // you may want to close this form..

        }
        public static void DeleteCache()
        {
            ClearFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)));

        }

        private static void ClearFolder(DirectoryInfo folder)
        {
            // Iterate each file
            foreach (FileInfo file in folder.GetFiles())
            {
                try
                {
                    // Delete the file, ignoring any exceptions
                    file.Delete();
                }
                catch (Exception)
                {
                }
            }

            // For each folder in the specified folder
            foreach (DirectoryInfo subfolder in folder.GetDirectories())
            {
                // Clear all the files in the folder
                ClearFolder(subfolder);
            }
        }

    }


}

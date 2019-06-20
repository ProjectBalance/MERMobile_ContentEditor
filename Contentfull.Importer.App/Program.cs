using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Threading;
using System.Windows.Forms;

namespace Contentful.Importer.App
{
    static class Program
    {
        static private List<PrivateFontCollection> _fontCollections;

        public static SplashScreen SplashScreen { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            /*DirectoryInfo dInfo = new DirectoryInfo(Application.StartupPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(WindowsIdentity.GetCurrent().Name, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);*/
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            //Dispose all used PrivateFontCollections when exiting
            Application.ApplicationExit += delegate {
                if (_fontCollections != null)
                {
                    foreach (var fc in _fontCollections) if (fc != null) fc.Dispose();
                    _fontCollections = null;
                }
            };
            SplashScreen = new SplashScreen();
            SplashScreen.Show();
            Application.Run(new MainForm());
        }
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if(ExtractInnerExceptions(e.Exception).Contains("The remote name could not be resolved"))
            {
                MessageBox.Show("Action could not be performed as network connectivity seems to be down, please restore your connection before proceeding", "Internet connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(ExtractInnerExceptions(e.Exception));
            }
        }
        static string ExtractInnerExceptions(Exception error)
        {
            string message = error.Message;
            if(error.InnerException != null)
            {
                message += "|" + ExtractInnerExceptions(error.InnerException);
            }

            return message;
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (ExtractInnerExceptions(e.ExceptionObject as Exception).Contains("The remote name could not be resolved"))
            {
                MessageBox.Show("Action could not be performed as network connectivity seems to be down, please restore your connection before proceeding", "Internet connection error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show(ExtractInnerExceptions(e.ExceptionObject as Exception));
            }
        }
    }
}

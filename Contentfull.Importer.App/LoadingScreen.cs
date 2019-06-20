using System;
using System.Windows.Forms;

namespace Contentful.Importer.App
{
    public partial class LoadingScreen : Form
    {
        
        public LoadingScreen()
        {
            InitializeComponent();
            Load += SplashScreen_Load;
            Shown += SplashScreen_Shown;
        }

        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            
            
        }
        

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {            
            
        }
    }
}

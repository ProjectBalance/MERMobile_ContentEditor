using System;
using System.Windows.Forms;

namespace Contentful.Importer.App.Interfaces.EditTemplates
{
    public partial class InsertLink : Form
    {
        public string ResultLink { get; set; }
        public InsertLink()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(txtLinkText.Text.Trim().Length > 0 && txtUrl.Text.Trim().Length > 0)
            {
                if(txtTitle.Text.Trim().Length > 0)
                {
                    ResultLink = "["+txtLinkText.Text+"]("+ txtUrl.Text+" \""+txtTitle.Text+"\")";
                }
                else
                {
                    ResultLink = "[" + txtLinkText.Text + "](" + txtUrl.Text + ")";
                }
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}

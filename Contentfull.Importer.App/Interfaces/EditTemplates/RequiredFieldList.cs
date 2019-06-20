using System.Windows.Forms;
using Contentful.Core.Models;
using Contentful.Importer.Library.Extensions;
using Contentful.Importer.Library.ContentFul;

namespace Contentful.Importer.App.Interfaces.EditTemplates
{
    public partial class RequiredFieldList : UserControl, Template
    {
        public Field Field { get; set; }
        public string[] RequiredValues { get; set; }
        public void SetFieldValue(Field field)
        {

            this.Field = field;
            RequiredValues = Field.GetRequiredFieldValues();
            cboList.DataSource = RequiredValues;

            if (Field.IsCountryType())
            {//Lock user country field               
                var usercountry = Client.AvailableCountries[Client.SelectedCountryIndex];
                
                for(int i = 0; i< RequiredValues.Length; i++ )
                {
                    if (usercountry.Equals(RequiredValues[i]))
                    {
                        cboList.SelectedIndex = i;
                    }
                }
                cboList.Enabled = false;
            }
           
            if (Field.Required)
            {
                SetLabel(field.Name + "*");
            }
            else
            {
                SetLabel(field.Name);
            }
        }
        public Field GetFieldValue()
        {
            return Field;
        }

        public RequiredFieldList()
        {
            InitializeComponent();
        }
        public string GetValue()
        {
            return cboList.Items[cboList.SelectedIndex].ToString();
        }



        public void SetLabel(string value)
        {
            lblHeading.Text = value;
        }

        public void SetValue(string value)
        {
            //  txtArea.Text = value;
            for (int i = 0; i < RequiredValues.Length; i++)
            {
                if (value.Equals(RequiredValues[i]))
                {
                    cboList.SelectedIndex = i;
                }
            }
        }

        private void txtArea_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


    }
}

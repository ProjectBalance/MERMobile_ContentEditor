using System.Windows.Forms;
using Contentful.Core.Models;
using Contentful.Importer.Library.Extensions;

namespace Contentful.Importer.App.Interfaces.EditTemplates
{
    public partial class IntegerField : UserControl, Template
    {
        public Field Field { get; set; }
        public void SetFieldValue(Field field)
        {
            this.Field = field;
            if (Field.Required)
            {
                SetLabel(field.Name + "*");
            }
            else
            {
                SetLabel(field.Name);
            }
            var validations = field.GetRequiredFieldValues();
            if(validations != null && validations.Length > 0)
            {
                lblRequired.Text = "Range: " + string.Join(",", validations) ;
                lblRequired.Text = lblRequired.Text.ReplaceLastOccurrence(",", " or ");
            }
        }
        public Field GetFieldValue()
        {
            return Field;
        }

        public IntegerField()
        {
            InitializeComponent();
        }
        public string GetValue()
        {
            return txtArea.Text;
        }



        public void SetLabel(string value)
        {
            lblHeading.Text = value;
        }

        public void SetValue(string value)
        {
            txtArea.Text = value;
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

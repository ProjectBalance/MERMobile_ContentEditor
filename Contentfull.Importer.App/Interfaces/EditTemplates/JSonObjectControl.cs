using System.Windows.Forms;
using Contentful.Core.Models;

namespace Contentful.Importer.App.Interfaces.EditTemplates
{
    public partial class JSonObjectControl : UserControl, Template
    {
        public Field Field { get; set; }
        public void SetFieldValue(Field field)
        {
            this.Field = field;
            if (Field.Required)
            {
                SetLabel(field.Name+"*");
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
        public JSonObjectControl()
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
    }
}

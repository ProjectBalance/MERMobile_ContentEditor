using System;
using System.Windows.Forms;
using Contentful.Core.Models;

namespace Contentful.Importer.App.Interfaces.EditTemplates
{
    public partial class DatePickerControl : UserControl, Template
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
        public DatePickerControl()
        {
            InitializeComponent();
        }
        public string GetValue()
        {
            return txtDatePicker.Value.ToString("yyyy-MM-dd")+"T00:00";
        }

        public void SetLabel(string value)
        {
            lblHeading.Text = value;
        }

        public void SetValue(string value)
        {
            try
            {
                txtDatePicker.Value = DateTime.Parse(value);
                //txtArea.Text = value;
            }
            catch
            {
                txtDatePicker.Value = DateTime.UtcNow;
            }
        }
    }
}

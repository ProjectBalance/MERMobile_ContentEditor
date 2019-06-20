using Contentful.Importer.Library.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Contentful.Importer.Library.Extensions;

namespace Contentful.Importer.App.Interfaces
{
    public partial class EditSingleEntry : Form
    {
        List<EditTemplates.Template> Templates { get; set; }
        public ContentTypeData ContentType { get; set; }
        public ContentFulRowData RowData { get; set; }
        public EditSingleEntry()
        {
            InitializeComponent();
            Templates = new List<EditTemplates.Template>();
        }

        public void LoadContentType(ContentTypeData contentType)
        {
            this.ContentType = contentType;
            LoadFields();
            RowData = null;
        }
        public void LoadContentType(ContentTypeData contentType, ContentFulRowData rowData)
        {
            this.ContentType = contentType;
            this.RowData = rowData;
            LoadFields();

        }
        public void LoadFields()
        {
            foreach (var field in ContentType.ContentFullType.Fields)
            {

                switch (field.GetDataType())
                {
                    case Library.Extensions.Extensions.Datatype.Text:
                        {
                            var control = new EditTemplates.TextAreaControl();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                    case Library.Extensions.Extensions.Datatype.Integer:
                        {
                            var control = new EditTemplates.IntegerField();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                    case Library.Extensions.Extensions.Datatype.Boolean:
                        {
                            var control = new EditTemplates.CheckboxControl();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                    case Library.Extensions.Extensions.Datatype.Date:
                        {
                            var control = new EditTemplates.DatePickerControl();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                    case Library.Extensions.Extensions.Datatype.Object:
                        {
                            var control = new EditTemplates.JSonObjectControl();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                    case Library.Extensions.Extensions.Datatype.Link:
                        {
                            var control = new EditTemplates.JSonObjectControl();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                    case Library.Extensions.Extensions.Datatype.Array:
                        {
                            var control = new EditTemplates.ArrayObjectControl();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                    case Library.Extensions.Extensions.Datatype.Symbol:
                        {

                            if (field.GetRequiredFieldValues() != null)
                            { //Dropdown list
                                var control = new EditTemplates.RequiredFieldList();
                                control.SetFieldValue(field);
                                control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                                flowLayoutPanel.Controls.Add(control);
                                Templates.Add(control);
                            }
                            else
                            {
                                var control = new EditTemplates.TextfieldControl();
                                control.SetFieldValue(field);
                                control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                                flowLayoutPanel.Controls.Add(control);
                                Templates.Add(control);
                            }
                        }
                        break;

                    default:
                        {
                            var control = new EditTemplates.TextfieldControl();
                            control.SetFieldValue(field);
                            control.SetValue(RowData != null ? RowData.GetDataByFieldID(field.Id) : "");
                            flowLayoutPanel.Controls.Add(control);
                            Templates.Add(control);
                        }
                        break;
                }
            }
        }

        private void UpdateFieldDataToRows()
        {
            foreach (var template in Templates)
            {
                var field = template.GetFieldValue();
                RowData.FieldData[field.Id] = template.GetValue();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var update = RowData != null;
            if (!update)
            {
                RowData = new ContentFulRowData();
                RowData.FieldData = new Dictionary<string, string>();
                foreach (var template in Templates)
                {
                    var field = template.GetFieldValue();
                    RowData.FieldData.Add(field.Id, template.GetValue());
                }

                string[] validationData = new string[0];
                if (!RowData.ValidateRow(ContentType.ContentFullType.Fields.ToArray(), -1, out validationData))
                {
                    MessageBox.Show(this, string.Join(Environment.NewLine, validationData), "Failed Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    RowData = null;
                    return;
                }
                else
                {
                    try
                    {
                        var entry = RowData.GetDynamicEntry(ContentType.ContentFullType.Fields.ToArray());
                        var result = Library.ContentFul.Client.Instance.AddContent(entry, ContentType.TypeID);
                        var publishResult = Library.ContentFul.Client.Instance.PublishContent(result.SystemProperties.Id, result.SystemProperties.Version ?? 1);
                        MessageBox.Show(this, "Data uploaded and published", "Uploaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception err)
                    {
                        var message = err.Message;
                        if(err.InnerException != null)
                        {
                            message += Environment.NewLine + err.InnerException.Message;
                        }
                        MessageBox.Show(this, message, "Failed Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }
                }
            }
            else
            {
                UpdateFieldDataToRows();
                string[] validationData = new string[0];
                if (!RowData.ValidateRow(ContentType.ContentFullType.Fields.ToArray(), -1, out validationData))
                {
                    MessageBox.Show(this, string.Join(Environment.NewLine, validationData), "Failed Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    var entry = RowData.GetDynamicEntry(ContentType.ContentFullType.Fields.ToArray());
                    try
                    {

                        var result = Library.ContentFul.Client.Instance.UpdateContent(entry, ContentType.TypeID);
                        MessageBox.Show(this, "Data updated", "Uploaded", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch (Exception err)
                    {
                        var message = err.Message;
                        if (err.InnerException != null)
                        {
                            message += Environment.NewLine + err.InnerException.Message;
                        }
                        MessageBox.Show(this, err.Message, "Failed Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;

                    }
                }

            }
            Library.ContentFul.Client.Instance.UpdateLanguageCountryVersion();
            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

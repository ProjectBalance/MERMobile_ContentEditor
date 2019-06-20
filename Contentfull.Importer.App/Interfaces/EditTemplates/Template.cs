using Contentful.Core.Models;

namespace Contentful.Importer.App.Interfaces.EditTemplates
{
    public interface Template
    {
        void SetFieldValue(Field field);
        Field GetFieldValue();
        void  SetLabel(string value);
        void SetValue(string value);
        string GetValue();
                
    }
}

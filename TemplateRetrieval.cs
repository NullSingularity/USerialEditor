using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public interface TemplateRetrieval
    {
        PropertyTemplate GetStructTemplate(string name, TemplateRetrieval requestOrigin = null, bool flatSearch = false);

        PropertyTemplate GetEnumTemplate(string name, TemplateRetrieval requestOrigin = null);

        List<StructTemplate> GetAllStructTemplates();

        List<EnumTemplate> GetAllEnumTemplates();
    }
}

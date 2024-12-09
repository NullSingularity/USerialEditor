using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public interface TemplateStorage
    {
        bool AddStructTemplate(string name, PropertyTemplate template);
        bool AddEnumTemplate(string name, PropertyTemplate template);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public interface ExtendedTemplate
    {
        PropertyTemplate GetSuperTemplate();
        void SetSuperTemplate(PropertyTemplate template);

    }
}

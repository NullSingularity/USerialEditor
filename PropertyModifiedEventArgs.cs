using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class PropertyModifiedEventArgs: EventArgs
    {
        public readonly UProperty Property;
        public PropertyModifiedEventArgs(UProperty property)
        {
            Property = property;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This groups properties together (typically the properties inherited from superclasses that go unmodified).
//This should have no representation in any files read or written.

namespace USerialEditor
{
    public class PropertyGroup: UProperty
    {
        public PropertyGroup() 
        {
            this.Name = "";
            this.PropertyType = "";
            this.Subtype = "";
            this.Specifiers = new List<string>();
            this.StaticArrayIndex = 0;
            this.Value = new ByteString();
            SubProperties = new List<UProperty>();
        }

        public override ByteString Serialize()
        {
            ByteString result = new ByteString();
            foreach(var subProperty in SubProperties)
            {
                result += subProperty.Serialize();
            }
            return result;
        }

        public override String GetHumanReadableType()
        {
            return "Group of properties (purely for organization)";
        }
    }
}

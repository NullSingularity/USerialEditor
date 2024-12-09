using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class is more or less purely for internal organization.
//In the serialized format, static arrays are represented by identical copies of the same property, with only the values and array indexes changed.

namespace USerialEditor
{
    public class StaticArrayProperty: UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            StaticArrayProperty instance = new StaticArrayProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override ByteString Serialize()
        {
            ByteString result = new ByteString();
            foreach(var subproperty in SubProperties)
            {
                result.AddByteString(subproperty.Serialize());
            }
            return result;
        }

        public override String GetHumanReadableType()
        {
            return "Static array";
        }
    }
}

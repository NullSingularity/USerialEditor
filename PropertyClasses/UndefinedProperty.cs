using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class UndefinedProperty : UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            UndefinedProperty instance = new UndefinedProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }
        public override String GetHumanReadableType()
        {
            return "Unknown";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class ObjectReferenceProperty : StringProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            ObjectReferenceProperty instance = new ObjectReferenceProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override String GetHumanReadableType()
        {
            return "Object reference";
        }
    }
}

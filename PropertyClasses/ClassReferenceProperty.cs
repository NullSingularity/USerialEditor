using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class ClassReferenceProperty: ObjectReferenceProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            ClassReferenceProperty instance = new ClassReferenceProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }
        public override string GetPropertyType()
        {
            return "ObjectProperty";
        }
        public override string GetHumanReadableType()
        {
            return "Class reference";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ClassProperty acts as a root node for all the properties of a given object.
//It is not meant to serialize into anything on its own.

namespace USerialEditor
{
    public class ClassProperty : UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            ClassProperty instance = new ClassProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override ByteString Serialize()
        {
            ByteString result = new ByteString();
            result.AddInt(0);
            result.AddByte(0xFF);
            result.AddByte(0xFF);
            result.AddByte(0xFF);
            result.AddByte(0xFF);
            foreach (UProperty subProperty in SubProperties)
            {
                if(!subProperty.IsModified())
                {
                    continue;
                }
                result.AddByteString(subProperty.Serialize());
            }
            result.AddSerialFormatString("None");
            return result;
        }

        public override String GetHumanReadableType()
        {
            return "Class";
        }
    }
}

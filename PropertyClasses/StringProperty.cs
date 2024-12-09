using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class StringProperty : UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            StringProperty instance = new StringProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override String GetHumanReadableType()
        {
            return "String";
        }

        public String GetStringValue()
        {
            if (Value == null)
            {
                return "";
            }
            return Value.GetString(0, Value.GetLength());
        }

        public void ModifyValue(string newString)
        {
            Value = new ByteString(newString);
            isModified = true;
        }

        public override ByteString SerializeValue()
        {
            ByteString result = new ByteString();
            ByteString formattedValue = new ByteString();
            formattedValue.AddSerialFormatString(GetStringValue());
            result.AddInt(formattedValue.GetLength());
            result.AddInt(GetStaticArrayIndex());
            result.AddByteString(formattedValue);

            return result;
        }

        public override ByteString SerializeArray()
        {
            ByteString result = new ByteString();
            result.AddSerialFormatString(GetStringValue());
            return result;
        }
    }
}

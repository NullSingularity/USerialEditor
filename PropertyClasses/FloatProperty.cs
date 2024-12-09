using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class FloatProperty: UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            FloatProperty instance = new FloatProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override String GetHumanReadableType()
        {
            return "Floating-point number";
        }

        public float GetFloatValue()
        {
            if (Value == null || Value.GetLength() < 4)
            {
                return 0;
            }
            return Value.GetFloat(0, 4);
        }

        public override ByteString SerializeArray()
        {
            ByteString result = new ByteString(GetFloatValue());
            return result;
        }

        public void ModifyValue(float value)
        {
            ByteString newValue = new ByteString();
            newValue.AddFloat(value);
            SetValue(newValue);
            isModified = true;
        }
    }
}

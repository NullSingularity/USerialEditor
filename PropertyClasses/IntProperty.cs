using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class IntProperty: UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            IntProperty instance = new IntProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override String GetHumanReadableType()
        {
            return "Integer number";
        }

        public int GetIntegerValue()
        {
            if(Value == null || Value.GetLength() < 4)
            {
                return 0;
            }
            return Value.GetInt(0, 4);
        }

        public void ModifyValue(int value)
        {
            ByteString newValue = new ByteString();
            newValue.AddInt(value);
            SetValue(newValue);
            isModified = true;
        }
    }
}

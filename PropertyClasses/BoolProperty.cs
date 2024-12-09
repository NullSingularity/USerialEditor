using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class BoolProperty: UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            BoolProperty instance = new BoolProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public void ModifyValue(bool value)
        {
            ByteString newValue = new ByteString();
            if(value)
            {
                newValue.AddByte(1);
            }
            else
            {
                newValue.AddByte(0);
            }
            SetValue(newValue);
            isModified = true;
        }

        public override String GetHumanReadableType()
        {
            return "Boolean";
        }

        public bool GetBooleanValue()
        {
            if(Value == null || Value.GetLength() < 1)
            {
                return false;
            }
            return Value[0] != 0x0;
        }

        public override ByteString SerializeValue()
        {
            ByteString result = new ByteString();
            result.AddInt(0);   //Value length for bool properties is always 0 for some reason, perhaps because they're actually represented by a single bit?
            result.AddInt(GetStaticArrayIndex());
            if (GetBooleanValue())
            {
                result.AddByte(1);
            }
            else
            {
                result.AddByte(0);
            }
            return result;
        }

        public override ByteString SerializeArray()
        {
            ByteString result = new ByteString();
            if (GetBooleanValue())
            {
                result.AddByte(1);
            }
            else
            {
                result.AddByte(0);
            }
            return new ByteString(result);
        }

        /* public bool GetReadableValue()
         {
             if(Value == null)
                 return false;
             return (Value[0] == 0);
         }*/
    }
}

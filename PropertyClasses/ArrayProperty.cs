using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class ArrayProperty: UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            ArrayProperty instance = new ArrayProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override ByteString Serialize()
        {
            return base.Serialize();
        }

        public override ByteString SerializeValue()
        {
            ByteString result = new ByteString();
            ByteString totalValue = new ByteString(SubProperties.Count);
            foreach (var subProperty in SubProperties)
            {
                totalValue.AddByteString(subProperty.SerializeArray());
            }
            result.AddInt(totalValue.GetLength());
            result.AddInt(GetStaticArrayIndex());
            result.AddByteString(totalValue);
            return result;
        }

        public override String GetHumanReadableType()
        {
            return "Array";
        }

        public void ModifyValue()
        {
            Value = new ByteString(SubProperties.Count);
            isModified = true;
        }
    }
}

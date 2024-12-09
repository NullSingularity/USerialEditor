using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class StructProperty : UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            StructProperty instance = new StructProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override String GetHumanReadableType()
        {
            return "Struct";
        }

        private ByteString SerializeInnerValue()
        {
            ByteString result = new ByteString();
            bool isImmutable = false;
            foreach (string specifier in Specifiers)
            {
                if (String.Equals(specifier, "immutable", StringComparison.OrdinalIgnoreCase))
                {
                    isImmutable = true;
                }
            }

            if (!isImmutable)
            {
                foreach (var subProperty in SubProperties)
                {
                    if (!subProperty.IsModified())
                    {
                        continue;
                    }
                    result.AddByteString(subProperty.Serialize());
                }
                result.AddSerialFormatString("None");
            }
            else
            {
                foreach (var subProperty in SubProperties)
                {
                    result.AddByteString(subProperty.SerializeArray());
                }
            }
            return result;
        }

        public override ByteString SerializeValue()
        {
            ByteString result = new ByteString();
            ByteString totalValue = SerializeInnerValue();

            result.AddInt(totalValue.GetLength());
            result.AddInt(GetStaticArrayIndex());
            result.AddSerialFormatString(GetSubtype());
            result.AddByteString(totalValue);

            return result;
        }

        public override ByteString SerializeArray()
        {
            return SerializeInnerValue();
        }
    }
}

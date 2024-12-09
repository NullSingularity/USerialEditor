using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class ByteProperty : UProperty
    {
        public static new UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            ByteProperty instance = new ByteProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        public override String GetHumanReadableType()
        {
            if(Subtype != null && Subtype != "")
            {
                return "Enumeration";
            }
            return "Byte number";
        }

        public byte GetByteValue()
        {
            if(Value == null || Value.GetLength() == 0)
            {
                return 0;
            }
            return Value[0];
        }

        public string GetEnumerationValue()
        {
            return GetEnumerationString(GetByteValue());
        }

        public string GetEnumerationString(int index)
        {
            if (index < 0)
            {
                return "%ERROR INVAL ENUM INDEX%";
            }
            PropertyTemplate template = null;
            if (GetDefinitionTemplate() != null)
            {
                template = GetDefinitionTemplate();
            }
            else
            {
                GetTemplateScope().GetEnumTemplate(GetSubtype());
            }
            if (template == null || !(template is EnumTemplate))
            {
                return "%ERROR UNKNOWN TEMPLATE%";
            }
            List<String> options = ((EnumTemplate)template).GetAllOptions();
            if (options == null || index >= options.Count)
            {
                return "%ERROR INVAL ENUM INDEX%";
            }
            return options[index];
        }

        public List<string> GetEnumOptions()
        {
            PropertyTemplate template = null;
            if (GetDefinitionTemplate() != null)
            {
                template = GetDefinitionTemplate();
            }
            else
            {
                template = GetTemplateScope().GetEnumTemplate(Subtype);
            }
                
            if (template == null || !(template is EnumTemplate))
            {
                return null;
            }
            List<string> options = ((EnumTemplate)template).GetAllOptions();
            if(options == null)
            {
                return null;
            }
            return options.ToList();
        }

        public void ModifyValue(byte newValue)
        {
            Value = new ByteString(newValue);   
            isModified = true;
        }

        public override ByteString SerializeValue()
        {
            ByteString result = new ByteString();
            string enumType = GetSubtype();
            if(enumType == "")
            {
                result.AddInt(1);
                result.AddInt(GetStaticArrayIndex());
                result.AddSerialFormatString("None");
                result.AddByte(GetByteValue());
            }
            else
            {
                ByteString enumValue = new ByteString();
                enumValue.AddSerialFormatString(GetEnumerationString(GetByteValue()));
                result.AddInt(enumValue.GetLength());
                result.AddInt(GetStaticArrayIndex());
                result.AddSerialFormatString(enumType);
                result.AddByteString(enumValue);
            }
            return result;
        }

        public override ByteString SerializeArray()
        {
            ByteString result = new ByteString();
            if(GetSubtype() == "")
            {
                result.AddByte(GetByteValue());
            }
            else
            {
                result.AddSerialFormatString(GetEnumerationString(GetByteValue()));
            }
            
            return result;
        }
        //TODO: Dummying this out pending the final decision on how enums should be handled
        /*     public void ModifyValue(string newValue)
             {
                 Value = new ByteString(newValue);
                 isModified = true;
             }*/
    }
}

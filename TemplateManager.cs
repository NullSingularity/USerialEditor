using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static USerialEditor.PropertyFactory;

//DEPRECATED

namespace USerialEditor
{
    public class TemplateManager
    {

        //public delegate UProperty PropertyConstructor(String Name,  String PropertyType, ByteString Value, String Subtype = "");
       // Dictionary<String, PropertyConstructor> Properties;
        Dictionary<String, PropertyTemplate> StructTemplates;
        Dictionary<String, PropertyTemplate> EnumTemplates;
        Dictionary<String, PropertyTemplate> ClassTemplates;

        public TemplateManager()
        {
           
            StructTemplates = new Dictionary<String, PropertyTemplate>();
            ClassTemplates = new Dictionary<String, PropertyTemplate>();
            EnumTemplates = new Dictionary<String, PropertyTemplate>();
        }

        /*public PropertyConstructor GetPropertyConstructor(String Name)
        {
            PropertyConstructor result;
            if (!Properties.ContainsKey(Name))
            {
                result = null;
            }
            else
            {
                result = Properties[Name];
            }
            return result;
        }

        public bool AddPropertyConstructor(String Name, PropertyConstructor NewProperty)
        {
            if (Properties.ContainsKey(Name))
            {
                return false;
            }
            Properties.Add(Name, NewProperty);
            return true;
        }*/

        public PropertyTemplate GetClassTemplate(String Name) 
        {
            if(!ClassTemplates.ContainsKey(Name))
            {
                return null;
            }
            else
            {
                return ClassTemplates[Name];
            }
        }

        public bool AddClassTemplate(String Name, PropertyTemplate NewClass)
        {
            if(ClassTemplates.ContainsKey(Name)) 
            {
                return false;
            }
            ClassTemplates.Add(Name, NewClass);
            return true;
        }

        public PropertyTemplate GetStructTemplate(String Name) 
        {
            if(!StructTemplates.ContainsKey(Name))
            {
                return null;
            }
            return StructTemplates[Name];
        }

        public bool AddStructTemplate(String Name, PropertyTemplate NewStruct)
        {
            if(StructTemplates.ContainsKey(Name))
            {
                return false;
            }
            StructTemplates.Add(Name, NewStruct);
            return true;
        }

        public bool AddEnumTemplate(String Name, PropertyTemplate NewEnum)
        {
            if (EnumTemplates.ContainsKey(Name))
            {
                return false;
            }
            EnumTemplates.Add(Name, NewEnum);
            return true;
        }

    }
}

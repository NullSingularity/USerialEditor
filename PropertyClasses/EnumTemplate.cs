using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class EnumTemplate: PropertyTemplate
    {
        List<string> enumOptions;
        public EnumTemplate(string name, string subtype = "")
        {
            this.Name = name;
            this.Subtype = subtype;
            enumOptions = new List<string>();
        }

        public List<string> GetAllOptions()
        {
            return enumOptions;
        }

        public int GetOptionCount()
        {
            return enumOptions.Count;
        }

        public string GetOption(int index) 
        {
            return enumOptions[index];
        }

        public bool AddEnumOption (string option)
        {
            if(enumOptions.Contains(option)) 
            {
                return false;
            }
            else
            {
                enumOptions.Add(option);
                return true;
            }
        }
    }
}

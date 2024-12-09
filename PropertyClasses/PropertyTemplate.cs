using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TemplateProperty represents a "template", or a model, of a property or structure of properties.
//These are created during the class file parsing process, and form a template "tree" from which instances of UProperties can be made.
//They can represent Class definitions, Struct definitions, or others.
//For example, instances of structs are made by building corresponding UProperties with
//the exact properties and relations as a tree of given TemplateProperties.
//TemplateProperties should not need to be modified after the tree has been fully constructed.

namespace USerialEditor
{
    public class PropertyTemplate: UProperty
    {
        
        public PropertyTemplate()
        {
            this.Name = "";
            this.PropertyType = "";
            this.Subtype = "";
            this.Specifiers = new List<string>();
            this.StaticArrayIndex = 0;
            this.Value = new ByteString();
            SubProperties = new List<UProperty>();
        }
        public PropertyTemplate(String Name, String PropertyType, string Subtype = "") 
        {
            this.Name = Name;
            this.PropertyType = PropertyType;
            this.Subtype = Subtype;
            this.Specifiers = new List<string>();
            this.StaticArrayIndex = 0;
            this.Value = new ByteString();
            SubProperties = new List<UProperty>();
        }
    }
}

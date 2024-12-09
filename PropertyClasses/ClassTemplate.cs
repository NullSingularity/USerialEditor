using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ClassTemplate models a single UE3 class source file (in Unrealscript).
//All properties, the superclass, interfaces, and any struct/enum definitions (but not those nested within other definitions) are represented.
//Inherited properties, definitions etc are not represented, and must be retrieved via the superclass or interfaces.
//Constructed by the class file parser.

namespace USerialEditor
{
    public class ClassTemplate: PropertyTemplate,
                                TemplateRetrieval,
                                TemplateStorage,
                                ExtendedTemplate
    {
        Dictionary<String, StructTemplate> StructTemplates; //All struct definitions in this class, represented as structured template trees.
        Dictionary<String, EnumTemplate> EnumTemplates; //All enum definitions in this class.

        String SuperclassName;  //The class name of the superclass, stored in case the superclass in question hasn't been read in yet.
        ClassTemplate Superclass;   //Object reference to the superclass ClassTemplate.

        List<String> InterfaceNames;    //Class names of each implemented interface, stored in case the interface files haven't been read yet.
        List<ClassTemplate> Interfaces; //Object references to each of the interface ClassTemplates.

        bool isPartialTemplate;     //Whether or not this class template is a "partial class" which needs to be combined with a base class

        public ClassTemplate() 
        {
            StructTemplates = new Dictionary<String, StructTemplate>(StringComparer.OrdinalIgnoreCase);
            EnumTemplates = new Dictionary<String, EnumTemplate>(StringComparer.OrdinalIgnoreCase);
            SuperclassName = "";
            Superclass = null;
            isPartialTemplate = false;

            InterfaceNames = new List<String>();
            Interfaces = new List<ClassTemplate>();
        }

        public void SetPartialTemplate(bool value)
        {
            isPartialTemplate = value;
        }

        public bool IsPartialTemplate()
        {
            return isPartialTemplate;
        }

        public void SetSuperclassName(string name)
        {
            SuperclassName = name;
        }
        public string GetSuperclassName() 
        {
            return SuperclassName;
        }
        public void SetSuperTemplate(PropertyTemplate template)
        {
            if(template != null && template is ClassTemplate)
            {
                Superclass = (ClassTemplate)template;
            }
        }

        public PropertyTemplate GetSuperTemplate() 
        {
            return Superclass;
        }
        public void AddInterfaceName(string name)
        {
            InterfaceNames.Add(name);
        }
        public List<string> GetInterfaceNames()
        {
            return InterfaceNames;
        }

        public void AddInterface(ClassTemplate classTemplate)
        {
            if (classTemplate != null)
            {
                InterfaceNames.Add(classTemplate.Name);
            }
        }


        public bool AddStructTemplate(string name, PropertyTemplate property)
        {
            if (StructTemplates.ContainsKey(name))
            {
                return false;
            }
            if (!(property is StructTemplate))
            {
                return false;
            }
            StructTemplates.Add(name, (StructTemplate)property);
            ((StructTemplate)property).SetTemplateScope(this);
            return true;
        }

        public bool AddEnumTemplate(string name, PropertyTemplate property)
        {
            if(EnumTemplates.ContainsKey(name))
            {
                return false;
            }
            if (!(property is EnumTemplate))
            {
                return false;
            }
            EnumTemplates.Add(name, (EnumTemplate)property);
            return true;
        }

        //=========================================================================================
        //GetStructTemplate(): Retrieves the relevant struct template for the scope of this struct
        //  definition.
        //  This is a depth-first search. Each struct template, in definition order, is checked against the name,
        //  then a deeper search in the template is run, before moving to the next. This recurses for each
        //  level of the search.
        //  If the above does not produce a result, it searches in the superclass.
        //=========================================================================================
        public PropertyTemplate GetStructTemplate(string name, TemplateRetrieval requestOrigin = null, bool flatSearch = false)
        {
            if(flatSearch)
            {
                StructTemplate flatSearchStruct = null;
                StructTemplates.TryGetValue(name, out flatSearchStruct);
                return flatSearchStruct;
            }

            PropertyTemplate result = null;
            foreach (var tuple in StructTemplates) 
            {
                if(tuple.Value == requestOrigin)
                {
                    continue;
                }
                if(String.Equals(tuple.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    result = tuple.Value;
                    break;
                }
                else
                {
                    if(tuple.Value is TemplateRetrieval)
                    {
                        result = ((TemplateRetrieval)tuple.Value).GetStructTemplate(name, this);
                        if (result != null)
                        {
                            break;
                        }
                    }
                }
            }

            if(result != null)
            {
                return result;
            }

            if(Superclass != null && Superclass != requestOrigin)
            {
                result = Superclass.GetStructTemplate(name, this);
            }

            return result;

            /*StructTemplates.TryGetValue(name, out temp);
            result = temp;
            if (result != null || flatSearch)
            {
                return result;
            }
            else
            {
                foreach (StructTemplate s in StructTemplates.Values)
                {
                    if (requestOrigin == s)
                        continue;
                    result = s.GetStructTemplate(name, this);
                    if (result != null)
                    {
                        break;
                    }
                }
            }
            if (result == null)
            {
                if (Superclass != null && Superclass != requestOrigin)
                    result = Superclass.GetStructTemplate(name, this);
            }
            return result;*/
        }

        public PropertyTemplate GetEnumTemplate(string name, TemplateRetrieval requestOrigin = null)
        {
            PropertyTemplate result;
            EnumTemplate temp;
            EnumTemplates.TryGetValue(name, out temp);
            result = temp;

            if (result != null)
            {
                return result;
            }
            else
            {
                if (Superclass != null && Superclass != requestOrigin)
                    result = Superclass.GetEnumTemplate(name, this);
            }
            return result;
        }

        public List<StructTemplate> GetAllStructTemplates()
        {
            return StructTemplates.Values.ToList();
        }

        public List<EnumTemplate> GetAllEnumTemplates()
        {
            return EnumTemplates.Values.ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TemplateStruct represents a model of a single struct definition.
//These have an array of TemplateProperty subproperties, representing each field in the struct.
//There's also a series of Struct templates, since Structs in UE3 can have other Structs defined inside them.

namespace USerialEditor
{
    public class StructTemplate: PropertyTemplate,
                                 TemplateStorage,
                                 TemplateRetrieval,
                                 ExtendedTemplate
    {
        Dictionary<String, StructTemplate> StructTemplates;
        //Dictionary<String, TemplateProperty> EnumTemplates;   //Whoops, structs can't actually have enum definitions

        //TemplateRetrieval TemplateSuper;

        string superStructName;
        StructTemplate superStruct;

        public StructTemplate(String Name, String PropertyType, string Subtype = ""):
            base(Name, PropertyType, Subtype)
        {
            StructTemplates = new Dictionary<String, StructTemplate>(StringComparer.OrdinalIgnoreCase);
            superStructName = "";
            //EnumTemplates = new Dictionary<String, TemplateProperty>();
        }

        public void SetSuperStructName(String name)
        {
            superStructName = name;
        }

        public string GetSuperStructName()
        {
            return superStructName;
        }

        public void SetSuperTemplate(PropertyTemplate superTemplate)
        {
            if(superTemplate != null && superTemplate is StructTemplate)
            {
                this.superStruct = (StructTemplate)superTemplate;
            }
            
        }

        public PropertyTemplate GetSuperTemplate()
        { 
            return superStruct; 
        }

        public bool AddStructTemplate(string name, PropertyTemplate property)
        {
            if(StructTemplates.ContainsKey(name))
            {
                return false;
            }
            if(!(property is StructTemplate))
            {
                return false;
            }
            StructTemplates.Add(name, (StructTemplate)property);
            ((StructTemplate)property).SetTemplateScope(this);
            return true;
        }

        public bool AddEnumTemplate(string name, PropertyTemplate property) 
        {
            return false;
        }
        //=========================================================================================
        //GetStructTemplate(): Retrieves the relevant struct template for the scope of this struct
        //  definition.
        //  This is a depth-first search. Each struct template, in definition order, is checked against the name,
        //  then a deeper search in the template is run, before moving to the next. This recurses for each
        //  level of the search.
        //  If the above does not produce a result, it searches in the super-template.
        //=========================================================================================
        public PropertyTemplate GetStructTemplate(string name, TemplateRetrieval requestOrigin = null, bool flatSearch = false) 
        {
            if (flatSearch)
            {
                StructTemplate flatSearchStruct = null;
                StructTemplates.TryGetValue(name, out flatSearchStruct);
                return flatSearchStruct;
            }

            PropertyTemplate result = null;
            foreach (var tuple in StructTemplates)
            {
                if (tuple.Value == requestOrigin)
                {
                    continue;
                }
                if (String.Equals(tuple.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    result = tuple.Value;
                    break;
                }
                else
                {
                    if (tuple.Value is TemplateRetrieval)
                    {
                        result = ((TemplateRetrieval)tuple.Value).GetStructTemplate(name, this);
                        if (result != null)
                        {
                            break;
                        }
                    }
                }
            }

            if (result != null)
            {
                return result;
            }

            TemplateRetrieval superTemplate = GetTemplateScope();

            if (superTemplate != null && superTemplate != requestOrigin)
            {
                result = superTemplate.GetStructTemplate(name, this);
            }

            return result;
            /*PropertyTemplate result;
            StructTemplate temp;
            StructTemplates.TryGetValue(name,out temp);
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
                    if(result != null)
                    {
                        break;
                    }
                }
            }
            if(result == null)
            {
                if (TemplateScope != requestOrigin)
                    result = TemplateScope.GetStructTemplate(name, this);
            }
            return result;*/
        }
        //=========================================================================================
        //GetEnumTemplate(): Returns the requested Enum template. Structs don't have enum
        //  definitions, so this just passes the request up the chain.
        //=========================================================================================
        public PropertyTemplate GetEnumTemplate(string name, TemplateRetrieval requestOrigin = null) 
        {
            if(TemplateScope != null)
            {
                return TemplateScope.GetEnumTemplate(name, this);
            }
            return null;
        }

        public List<StructTemplate> GetAllStructTemplates()
        {
            return StructTemplates.Values.ToList();
        }

        public List<EnumTemplate> GetAllEnumTemplates()
        {
            return new List<EnumTemplate>();
        }
    }
}

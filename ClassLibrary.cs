using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class ClassLibrary
    {
        Dictionary<string, ClassTemplate> classTemplates;
        Dictionary<string, List<ClassTemplate>> partialClassTemplates;
        public ClassLibrary() 
        {
            classTemplates = new Dictionary<string, ClassTemplate>(StringComparer.OrdinalIgnoreCase);
            partialClassTemplates = new Dictionary<string, List<ClassTemplate>>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddPartialClassTemplate(string className, ClassTemplate template)
        {
            if(!partialClassTemplates.ContainsKey(className))
            {
                List<ClassTemplate> templateList = new List<ClassTemplate>();
                templateList.Add(template);
                partialClassTemplates.Add(className, templateList);
            }
            else
            {
                partialClassTemplates[className].Add(template);
            }
        }

        public bool AddClassTemplate(string name, ClassTemplate template) 
        {
            if(classTemplates.ContainsKey(name)) 
            {
                return false;
            }
            classTemplates.Add(name, template);
            return true;
        }

        public ClassTemplate GetClassTemplate(string name) 
        {
            if(classTemplates.ContainsKey(name)) 
            {
                return classTemplates[name];
            }
            else 
            { 
                return null; 
            }
        }

        public void ReplaceClassTemplate(string name, ClassTemplate newTemplate) 
        {
            if(classTemplates.ContainsKey(name))
            {
                classTemplates.Remove(name);
            }
            classTemplates.Add(name, newTemplate);
        }

        public bool RemoveClassTemplate(string name) 
        {
            if(classTemplates.ContainsKey(name))
            {
                classTemplates.Remove(name);
                return true;
            }
            return false;
        }

        public void ComposePartialClasses()
        {
            foreach(var className in partialClassTemplates.Keys)
            {
               /* if(!classTemplates.ContainsKey(className))
                {
                    continue;
                }*/
                ClassTemplate baseClass;
                if(!classTemplates.TryGetValue(className, out baseClass))
                {
                    baseClass = new ClassTemplate();
                    baseClass.SetName(className);

                    AddClassTemplate(className, baseClass);
                }
                List<ClassTemplate> partials = partialClassTemplates[className];
                foreach(var partialTemplate in partials)
                {
                    foreach (var tempStruct in partialTemplate.GetAllStructTemplates())
                    {
                        baseClass.AddStructTemplate(tempStruct.GetSubtype(), tempStruct);
                    }
                    foreach (var tempEnum in partialTemplate.GetAllEnumTemplates())
                    {
                        baseClass.AddEnumTemplate(tempEnum.GetSubtype(), tempEnum);
                    }
                    foreach (var subProperty in partialTemplate.SubProperties)
                    {
                        baseClass.AddSubProperty(subProperty);
                    }
                    if(partialTemplate.GetSuperclassName() != "")
                    {
                        baseClass.SetSuperclassName(partialTemplate.GetSuperclassName());
                    }
                }
            }
        }

        public void LinkClasses()
        {
            foreach(ClassTemplate template in classTemplates.Values)
            {
                ClassTemplate super = GetClassTemplate(template.GetSuperclassName());
                if(super != null)
                {
                    template.SetSuperTemplate(super);
                }
                List<string> interfaces = template.GetInterfaceNames();
                foreach(string interfaceName in interfaces) 
                {
                    ClassTemplate newInterface = GetClassTemplate(interfaceName);
                    if(newInterface != null)
                    {
                        template.AddInterface(newInterface);
                    }

                }
            }
        }

        public void LinkStructs()
        {
            foreach(ClassTemplate classTemplate in classTemplates.Values)
            {
                foreach(StructTemplate structTemplate in classTemplate.GetAllStructTemplates())
                {
                    LinkStructsHelper(classTemplate, structTemplate);
                }
            }
        }

        public void LinkStructsHelper(ClassTemplate classTemplate, StructTemplate structTemplate)
        {
            foreach(StructTemplate subTemplate in structTemplate.GetAllStructTemplates())
            {
                LinkStructsHelper(classTemplate, subTemplate);
            }
            StructTemplate superTemplate = (StructTemplate)structTemplate.GetTemplateScope().GetStructTemplate(structTemplate.GetSuperStructName(), null, true);
            if(superTemplate == null)
            {
                ClassTemplate currentClass = classTemplate;
                superTemplate = (StructTemplate)classTemplate.GetStructTemplate(structTemplate.GetSuperStructName(), null, true);
                while (superTemplate == null && currentClass.GetSuperTemplate() != null)
                {

                    currentClass = (ClassTemplate)currentClass.GetSuperTemplate();
                    superTemplate = (StructTemplate)classTemplate.GetStructTemplate(structTemplate.GetSuperStructName(), null, true);
                }
            }
            structTemplate.SetSuperTemplate(superTemplate);
        }

        public List<string> GetClassNames()
        {
            return classTemplates.Keys.ToList();
        }

        public void ResolveCustomTypes()
        {
            foreach (ClassTemplate template in classTemplates.Values)
            {
                ResolveCustomTypesHelper(template);
            }
        }

        private void ResolveCustomTypesHelper(PropertyTemplate template)
        {
            foreach(var subProperty in template.SubProperties)
            {
                if(subProperty is PropertyTemplate) 
                {
                    ResolveCustomTypesHelper((PropertyTemplate)subProperty);
                }
            }
            if(template is TemplateRetrieval)
            {
                foreach(var structTemplate in ((TemplateRetrieval)template).GetAllStructTemplates())
                {
                    ResolveCustomTypesHelper(structTemplate);
                }
            }
            //In case we're changing loaded classes, some types may be invalidated
           /* if (template.GetPropertyType() == "StructProperty" && template.GetTemplateScope().GetStructTemplate(template.GetSubtype()) == null)
            {
                template.SetPropertyType("%UNDEFINED%");
            }
            else if (template.GetPropertyType() == "ByteProperty" && template.GetSubtype().Trim() != "" && template.GetTemplateScope().GetStructTemplate(template.GetSubtype()) == null)
            {
                template.SetPropertyType("%UNDEFINED%");
            }
            else if (template.GetPropertyType() == "ObjectProperty" && GetClassTemplate(template.GetSubtype()) == null)
            {
                template.SetPropertyType("%UNDEFINED%");
            }*/
            //If it's an unknown type, try and associate it with a registered type
            if (template.GetPropertyType() == "%UNDEFINED%")
            {
                if (template.GetTemplateScope().GetStructTemplate(template.GetSubtype()) != null)
                {
                    template.SetPropertyType("StructProperty");
                }
                else if (template.GetTemplateScope().GetEnumTemplate(template.GetSubtype()) != null)
                {
                    template.SetPropertyType("ByteProperty");
                }
                else if (GetClassTemplate(template.GetSubtype()) != null)
                {
                    template.SetPropertyType("ObjectProperty");
                }
            }
            
        }
    }
}

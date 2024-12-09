using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
//using USerialEditor.PropertyClasses;

namespace USerialEditor
{
    public class PropertyFactory
    {
        public delegate UProperty PropertyConstructor(String Name, String PropertyType, ByteString Value, String Subtype = "");
        Dictionary<String, PropertyConstructor> Properties;

        ClassLibrary classLibrary;

        public PropertyFactory(ClassLibrary classLibrary) 
        {
            Properties = new Dictionary<String, PropertyConstructor>();
            this.classLibrary = classLibrary;
        }

        public void InitDefaultPropertyConstructors()
        {
            AddPropertyConstructor("ArrayProperty", ArrayProperty.Instantiate);
            AddPropertyConstructor("BoolProperty", BoolProperty.Instantiate);
            AddPropertyConstructor("ByteProperty", ByteProperty.Instantiate);
            AddPropertyConstructor("ClassProperty", ClassProperty.Instantiate);
            AddPropertyConstructor("ClassRefProperty", ClassReferenceProperty.Instantiate);
            AddPropertyConstructor("FloatProperty", FloatProperty.Instantiate);
            AddPropertyConstructor("IntProperty", IntProperty.Instantiate);
            AddPropertyConstructor("NameProperty", NameProperty.Instantiate);
            AddPropertyConstructor("ObjectProperty", ObjectReferenceProperty.Instantiate);
            AddPropertyConstructor("StrProperty", StringProperty.Instantiate);
            AddPropertyConstructor("StructProperty", StructProperty.Instantiate);
            AddPropertyConstructor("StaticArrayProperty", StaticArrayProperty.Instantiate);

        }

        public void NormalizeType (string inType, out string outType, out string outSubtype, TemplateRetrieval templateScope = null)
        {
            inType = inType.Trim();
            string fullType = inType;
            string typeResult = "";
            string subtypeResult = "";

            int openBracketIndex = inType.IndexOf("<");
            if (openBracketIndex != -1)
            {
                int closeBracketIndex = inType.LastIndexOf(">");
                if (closeBracketIndex == -1)
                {
                    outType = inType.Substring(0,openBracketIndex - 1);
                    outSubtype = "";
                    return;
                }
                subtypeResult = inType.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
                inType = inType.Substring(0, openBracketIndex);
            }

            if (String.Equals(inType, "byte", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "ByteProperty";
            }
            else if (String.Equals(inType, "int", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "IntProperty";
            }
            else if (String.Equals(inType, "bool", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "BoolProperty";
            }
            else if (String.Equals(inType, "float", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "FloatProperty";
            }
            else if (String.Equals(inType, "string", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "StrProperty";
            }
            else if (String.Equals(inType, "name", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "NameProperty";
            }
            else if (String.Equals(inType, "array", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "ArrayProperty";
            }
            else if (String.Equals(inType, "class", StringComparison.OrdinalIgnoreCase))
            {
                typeResult = "ClassRefProperty";
            }
            else
            {
                if (templateScope != null)
                {
                    if (templateScope.GetEnumTemplate(inType) != null)
                    {
                        typeResult = "ByteProperty";
                        subtypeResult = inType;
                    }
                    else if (templateScope.GetStructTemplate(inType) != null)
                    {
                        typeResult = "StructProperty";
                        subtypeResult = inType;
                    }
                    else if (classLibrary.GetClassTemplate(inType) != null)
                    {
                        typeResult = "ObjectProperty";
                        subtypeResult = inType;
                    }
                    else
                    {
                        typeResult = "%UNDEFINED%";
                        subtypeResult = fullType;
                    }
                }
                else
                {
                    typeResult = "%UNDEFINED%";
                    subtypeResult = fullType;
                }
            }
            outType = typeResult;
            outSubtype = subtypeResult;
        }

        public UProperty BuildProperty(string ClassName, string Name, string PropertyType, ByteString Value, string Subtype = "") 
        {
            PropertyConstructor propcon;
            UProperty result;
            propcon = GetPropertyConstructor(ClassName);
            if(propcon == null) 
            {
                propcon = UndefinedProperty.Instantiate;
            }
            result = propcon(Name, PropertyType, Value, Subtype);
            return result;
        }

        public PropertyConstructor GetPropertyConstructor(String Name)
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
        }

        public UProperty BuildStruct(StructTemplate template)
        {
            UProperty result;
            if (template == null)
            {
                result = new UndefinedProperty();
                result.Init("", "StructProperty", null, "");
                return result;
            }
            result = IterateTemplate(template, template);
            return result;
        }

        public UProperty BuildClass(ClassTemplate template)
        {
            UProperty result;
            if (template == null)
            {
                result = new UndefinedProperty();
                result.Init("", "ClassProperty", null, "");
                return result;
            }
            result = IterateTemplate(template, template); 
            return result;
        }

        private UProperty IterateTemplate(PropertyTemplate prop, TemplateRetrieval scope, int iterations = 0) //Recursive worker function for building empty properties from templates.
        {
            UProperty result;
            if (iterations >= 80)
            {
                result = new UndefinedProperty();
                result.Init("ERROR END OF TREE", "", new ByteString());
                return result;
            }
            if (prop == null)
            {
                return null;
            }
            
            if (!(prop is StructTemplate) && prop.GetPropertyType() == "StructProperty")
            {
                PropertyTemplate template = null;
                if (prop.GetDefinitionTemplate() != null)
                {
                    template = prop.GetDefinitionTemplate();
                }
                else
                {
                    template = scope.GetStructTemplate(prop.GetSubtype());
                }
                result = BuildStruct((StructTemplate)template);
                if(result != null)
                {
                    result.SetName(prop.GetName());
                }
            }
            else
            {
                result = BuildProperty(prop.GetPropertyType(), prop.GetName(), prop.GetPropertyType(), new ByteString(), prop.GetSubtype());
                result.SetStaticArrayIndex(prop.GetStaticArrayIndex());
                result.SetSpecifiers(prop.GetSpecifiers());
            }
            List<string> subPropertyNames = new List<string>();
            foreach(PropertyTemplate subPropertyTemplate in prop.SubProperties)
            {
                UProperty subProperty = IterateTemplate(subPropertyTemplate, scope, iterations + 1);
                if(subProperty != null)
                    subProperty.SetTemplateScope(scope);
                result.AddSubProperty(subProperty);
                subPropertyNames.Add(subPropertyTemplate.GetName());
            }

            if(prop is ExtendedTemplate)
            {
                PropertyTemplate superTemplate = ((ExtendedTemplate)prop).GetSuperTemplate();
                while(superTemplate != null) 
                {
                    List<UProperty> addedProperties = new List<UProperty>();
                    foreach(PropertyTemplate subPropertyTemplate in superTemplate.SubProperties)
                    {
                        if (subPropertyNames.Contains(subPropertyTemplate.GetName(), StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        UProperty subProperty = IterateTemplate(subPropertyTemplate, scope, iterations + 1);
                        if (subProperty != null)
                        {
                            subProperty.SetTemplateScope(superTemplate.GetTemplateScope());
                            subPropertyNames.Add(subPropertyTemplate.GetName());
                            addedProperties.Add(subProperty);
                        }
                            
                    }
                    if(prop is ClassTemplate)
                    {
                        PropertyGroup newGroup = new PropertyGroup();
                        newGroup.SetName("{"+superTemplate.GetName()+"}");
                        foreach(var addProperty in addedProperties)
                        {
                            newGroup.AddSubProperty(addProperty);
                        }
                        result.SubProperties.Insert(0,newGroup);
                    }
                    else
                    {
                        foreach (var addProperty in addedProperties)
                        {
                            result.AddSubProperty(addProperty);
                        }
                    }
                    if(superTemplate is ExtendedTemplate) 
                    {
                        superTemplate = ((ExtendedTemplate)superTemplate).GetSuperTemplate();
                    }
                    else
                    {
                        superTemplate = null;
                    }
                }
            }
            if(result != null)
                result.SetTemplateScope(scope);
            return result;
        }

        public UProperty ArrayBuildSubproperty(ArrayProperty array)
        {
            if (array == null || array.GetSubtype() == "")
            {
                return null;
            }
            UProperty result = null;
            string subtype = array.GetSubtype();
            string newType = "";
            string newSubtype = "";
            NormalizeType(subtype, out newType, out newSubtype, array.GetTemplateScope());
            if(newType == "StructProperty")
            {
                PropertyTemplate definitionTemplate = array.GetDefinitionTemplate();
                if (definitionTemplate != null && definitionTemplate is StructTemplate)
                {
                    result = BuildStruct((StructTemplate)definitionTemplate);
                }
                else
                {
                    result = BuildStruct((StructTemplate)array.GetTemplateScope().GetStructTemplate(newSubtype));
                }
            }
            else
            {
                result = BuildProperty(newType, "", newType, new ByteString(), newSubtype);
            }
            result.SetTemplateScope(array.GetTemplateScope());
            return result;
        }

        public void ArrayAppendSubproperty(ArrayProperty array)
        {
            UProperty newSubProperty = ArrayBuildSubproperty(array);
            array.AddSubProperty(newSubProperty);
        }

        public void ArrayInsertSubproperty(ArrayProperty array, int index)
        {
            UProperty newSubProperty = ArrayBuildSubproperty(array);
            array.InsertSubProperty(index, newSubProperty);
        }

        public void ArrayRemoveSubproperty(ArrayProperty array, int index)
        {
            array.RemoveSubProperty(index);
        }
    }
}

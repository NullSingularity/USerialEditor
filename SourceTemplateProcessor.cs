using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class SourceTemplateProcessor
    {
        PropertyFactory propertyFactory;
        public SourceTemplateProcessor(PropertyFactory propertyFactory) 
        { 
            this.propertyFactory = propertyFactory;
        }

        public ClassTemplate ProcessClassTemplate(ref List<List<string>> statements)
        {
            ClassTemplate classTemplate = new ClassTemplate();
            List<string> statement;
            bool classStatementFound = false;
            string className = "";
            string superclassName = "";
            
            for (int i = 0; i < statements.Count; i++) 
            {
                statement = statements[i];
                for (int j = 0; j < statement.Count; j++)
                {
                    if (j == 0)
                    {
                        if(String.Equals(statement[j], "class", StringComparison.OrdinalIgnoreCase))
                        {
                            if (j + 1 < statement.Count)
                                className = statement[j + 1];
                            classStatementFound = true;
                        }
                        else if (String.Equals(statement[j], "partial", StringComparison.OrdinalIgnoreCase))
                        {
                            if(j + 2 >= statement.Count)
                            {
                                continue;
                            }
                            if(String.Equals(statement[j + 1], "class", StringComparison.OrdinalIgnoreCase))
                            {
                                className = statement[j + 2];
                                classStatementFound = true;
                                classTemplate.SetPartialTemplate(true);
                            }
                        }
                        else if(String.Equals(statement[j], "interface", StringComparison.OrdinalIgnoreCase))
                        {
                            return null;
                        }
                    }
                    if (classStatementFound)
                    {
                        if (String.Equals(statement[j], "extends", StringComparison.OrdinalIgnoreCase))
                        {
                            superclassName = statement[j + 1];
                        }
                        else if (String.Equals(statement[j], "implements", StringComparison.OrdinalIgnoreCase))
                        {
                            if (statement[j + 1][0] == '(' && statement[j + 1][statement[j + 1].Length - 1] == ')')
                            {
                                string interfaceStatement = statement[j + 1].Substring(1, statement[j + 1].Length - 2);
                                string[] split = interfaceStatement.Split(',');
                                for (int k = 0; k < split.GetLength(0); k++)
                                {
                                    classTemplate.AddInterfaceName(split[k].Trim());
                                }
                            }
                        }
                    }
                }
                if(!classStatementFound)
                {
                    ProcessTemplate(ref statement, classTemplate);
                }
                classStatementFound = false;
            }
            classTemplate.SetName(className);
            classTemplate.SetPropertyType("ClassProperty");
            classTemplate.SetSuperclassName(superclassName);
            return classTemplate;
        }

        public void ProcessTemplate(ref List<string> statement, PropertyTemplate parentTemplate)
        {
            int templateType = -1; //Template classification. -1 = undefined, 0 = Not supported, 1 = variable declaration, 2 = struct definition, 3 = enum definition, 4= struct/var statement, 5= enum/var statement
            List<string> varSpecifiers = new List<string>();
            List<string> structSpecifiers = new List<string>();
            string varType = "";
            string varSubtype = "";
            List<string> varNames = new List<string>();
            List<int> varSizes = new List<int>();
            string newTypeName = "";
            string newTypeSuper = "";
            EnumTemplate newEnumTemplate = null;
            StructTemplate newStructTemplate = null;
            bool typeDefFound = false;

           // List<PropertyTemplate> outProperties = new List<PropertyTemplate>();

            for(int i = 0; i < statement.Count; i++)
            {
                if(i == 0)      //Initial classification always begins with the first term for statements we care about - functions don't follow this convention
                {
                    if (String.Equals(statement[i], "var", StringComparison.OrdinalIgnoreCase))//statement[i] == "var")
                    {
                        templateType = 1;
                    }
                    else if (String.Equals(statement[i], "struct", StringComparison.OrdinalIgnoreCase))//statement[i] == "struct")
                    {
                        templateType = 2;
                    }
                    else if (String.Equals(statement[i], "enum", StringComparison.OrdinalIgnoreCase))//statement[i] == "enum")
                    {
                        templateType = 3;
                    }
                    else
                    {
                        templateType = 0;
                        break;
                    }
                }
                else if (i > 0)
                {
                    if (statement[i] == ";")
                    {
                        break;
                    }
                    if (templateType == 1)   //var statement
                    {
                        if (varType == "")
                        {
                            if (IsVariableSpecifier(statement[i]) ||
                                (statement[i][0] == '(' && statement[i][statement[i].Length - 1] == ')') ||
                                (statement[i][0] == '{' && statement[i][statement[i].Length - 1] == '}'))
                            {
                                varSpecifiers.Add(statement[i]);
                            }
                            else
                            {
                                varType = statement[i];
                                if (String.Equals(varType, "struct", StringComparison.OrdinalIgnoreCase))
                                {
                                    templateType = 4;   //Shift to var struct
                                }
                                else if (String.Equals(varType, "enum", StringComparison.OrdinalIgnoreCase))
                                {
                                    templateType = 5;   //Shift to var enum
                                }
                                else if (String.Equals(varType, "array", StringComparison.OrdinalIgnoreCase) || String.Equals(varType, "class", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (statement[i + 1][0] == '<')
                                    {
                                        varSubtype = statement[i + 1].Substring(1, statement[i + 1].Length - 2);
                                        i++;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (statement[i][0] == '<')
                            {
                                continue;
                            }
                            varNames.Add(statement[i]);
                            if (statement[i + 1][0] == '[')
                            {
                                int size;
                                if (int.TryParse(statement[i + 1].Substring(1, statement[i + 1].Length - 2), out size))
                                {
                                    varSizes.Add(size);
                                }
                                else
                                {
                                    varSizes.Add(1);
                                }
                                i++;
                            }
                            else
                            {
                                varSizes.Add(1);
                            }
                        }
                    }
                    else if (templateType == 2 || templateType == 4)  //Structs and var structs
                    {
                        if (newTypeName == "")
                        {
                            if (IsVariableSpecifier(statement[i]) || IsStructSpecifier(statement[i]) || statement[i][0] == '{')
                            {
                                structSpecifiers.Add(statement[i]);
                            }
                            else
                            {
                                newTypeName = statement[i];
                            }
                        }
                        else
                        {
                            if(!typeDefFound)
                            {
                                if(String.Equals(statement[i], "extends", StringComparison.OrdinalIgnoreCase))
                                {
                                    newTypeSuper = statement[i + 1];
                                }
                                else if (statement[i][0] == '{')
                                {
                                    if (parentTemplate is TemplateStorage)
                                    {
                                        typeDefFound = true;
                                        newStructTemplate = ProcessStructTemplate(newTypeName, structSpecifiers, statement[i], newTypeSuper, parentTemplate);
                                    }
                                }
                            }
                            else if (templateType == 4)
                            {
                                varNames.Add(statement[i]);
                                if (statement[i + 1][0] == '[')
                                {
                                    int size;
                                    if (int.TryParse(statement[i + 1].Substring(1, statement[i + 1].Length - 2), out size))
                                    {
                                        varSizes.Add(size);
                                    }
                                    else
                                    {
                                        varSizes.Add(1);
                                    }
                                    i++;
                                }
                                else
                                {
                                    varSizes.Add(1);
                                }
                            }
                        }
                    }
                    else if (templateType == 3 || templateType == 5) //Enums and var enums
                    {
                        if (newTypeName == "")
                        {
                            newTypeName = statement[i];
                        }
                        else if (statement[i][0] == '{')
                        {
                            if (parentTemplate is TemplateStorage)
                            {
                                newEnumTemplate = ProcessEnumTemplate(newTypeName, statement[i]);
                                if (newEnumTemplate != null)
                                {
                                    ((TemplateStorage)parentTemplate).AddEnumTemplate(newTypeName, newEnumTemplate);
                                }
                            }
                        }
                        else if(templateType == 5)
                        {
                            varNames.Add(statement[i]);
                            if (statement[i + 1][0] == '[')
                            {
                                int size;
                                if (int.TryParse(statement[i + 1].Substring(1, statement[i + 1].Length - 2), out size))
                                {
                                    varSizes.Add(size);
                                }
                                else
                                {
                                    varSizes.Add(1);
                                }
                                i++;
                            }
                            else
                            {
                                varSizes.Add(1);
                            }
                        }

                    }
                }

                
            }
            if(templateType == 1 || templateType == 4 || templateType == 5) //Build and assign variable properties
            {
                if(templateType == 4)
                {
                    varType = "StructProperty";
                    varSubtype = newTypeName;
                }
                else if (templateType == 5)
                {
                    varType = "ByteProperty";
                    varSubtype = newTypeName;
                }
                else  //Convert types to internal types
                {
                    if(String.Equals(varType, "byte", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "ByteProperty";
                    }
                    else if(String.Equals(varType, "int", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "IntProperty";
                    }
                    else if (String.Equals(varType, "bool", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "BoolProperty";
                    }
                    else if (String.Equals(varType, "float", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "FloatProperty";
                    }
                    else if (String.Equals(varType, "string", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "StrProperty";
                    }
                    else if (String.Equals(varType, "name", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "NameProperty";
                    }
                    else if (String.Equals(varType, "array", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "ArrayProperty";
                    }
                    else if (String.Equals(varType, "class", StringComparison.OrdinalIgnoreCase))
                    {
                        varType = "ClassRefProperty";
                    }
                    else    //Unknown or custom type. Custom types, unless defined earlier in the class, must be associated in a second pass. 
                    {
                        if(parentTemplate is TemplateRetrieval)
                        {
                            if(((TemplateRetrieval)parentTemplate).GetStructTemplate(varType) != null)
                            {
                                varSubtype = varType;
                                varType = "StructProperty";
                            }
                            else if(((TemplateRetrieval)parentTemplate).GetEnumTemplate(varType) != null)
                            {
                                varSubtype = varType;
                                varType = "ByteProperty";
                            }
                            else
                            {
                                varSubtype = varType;
                                varType = "%UNDEFINED%";
                            }
                        }
                        else
                        {
                            varSubtype = varType;
                            varType = "%UNDEFINED%";
                        }
                    }
                }
                for(int i = 0; i < varNames.Count; i++)     //Iterate through each variable declared in this statement
                {
                    List<PropertyTemplate> properties = new List<PropertyTemplate>();
                    for(int j = 0; j < varSizes[i]; j++)        //Create as many properties as specified by the static array notation [] if applicable
                    {
                        PropertyTemplate newProperty = new PropertyTemplate();
                        newProperty.SetName(varNames[i]);
                        newProperty.SetPropertyType(varType);
                        newProperty.SetSubtype(varSubtype);
                        newProperty.SetSpecifiers(varSpecifiers);
                        newProperty.SetStaticArrayIndex(j);
                        if (newStructTemplate != null)
                        {
                            newProperty.SetDefinitionTemplate(newStructTemplate);
                        }
                        else if (newEnumTemplate != null)
                        {
                            newProperty.SetDefinitionTemplate(newEnumTemplate);
                        }

                        if (parentTemplate is TemplateRetrieval)
                        {
                            newProperty.SetTemplateScope((TemplateRetrieval)parentTemplate);
                        }
                        properties.Add(newProperty);
                    }
                    if (varSizes[i] > 1)    //If we made more than 1 property, make a StaticArray template to hold them
                    {
                        PropertyTemplate staticArray;
                        staticArray = new PropertyTemplate();
                        staticArray.SetName(varNames[i]);
                        staticArray.SetPropertyType("StaticArrayProperty");
                        staticArray.SetSubtype("");
                        staticArray.SetSpecifiers(varSpecifiers);
                        if (parentTemplate is TemplateRetrieval)
                        {
                            staticArray.SetTemplateScope((TemplateRetrieval)parentTemplate);
                        }
                        foreach(var property in properties)
                        {
                            staticArray.AddSubProperty(property);
                        }
                        parentTemplate.AddSubProperty(staticArray);
                    }
                    else if (varSizes[i] == 1)  //Otherwise, just add it directly to the parent template
                    {
                        parentTemplate.AddSubProperty(properties[0]);
                    }
                }
            }
        }

        public StructTemplate ProcessStructTemplate(string name, List<string> specifiers, string templateText, string superTemplate, PropertyTemplate parentTemplate) 
        {
            StructTemplate newStructTemplate = new StructTemplate("", "StructProperty", name);
            SourceStatementParser parser = new SourceStatementParser();
            List<List<string>> statements;
            templateText = templateText.Substring(1, templateText.Length - 2);

            if (parentTemplate is TemplateRetrieval)
                newStructTemplate.SetTemplateScope((TemplateRetrieval)parentTemplate);

            newStructTemplate.SetSpecifiers(specifiers);
            newStructTemplate.SetSuperStructName(superTemplate);

            parser.Parse(templateText);
            statements = parser.GetStatements();
            if(statements != null)
            {
                for(int i = 0; i < statements.Count; i++)
                {
                    var statement = statements[i];
                    ProcessTemplate(ref statement, newStructTemplate);
                }
            }
            if(parentTemplate is TemplateStorage)
            {
                ((TemplateStorage)parentTemplate).AddStructTemplate(name, newStructTemplate);
            }
            return newStructTemplate;

        }

        public EnumTemplate ProcessEnumTemplate(string name, string templateText)
        {
            EnumTemplate newEnumTemplate = new EnumTemplate("", name);
            templateText = templateText.Substring(1, templateText.Length - 2);
            string[] temp = templateText.Split(',');
            if (temp != null)
            {
                for (int i = 0; i < temp.Length; i++)
                {
                    newEnumTemplate.AddEnumOption(temp[i].Trim());
                }
            }
            return newEnumTemplate;
        }

        public bool IsVariableSpecifier(string text)
        {
            /*• config 
            • globalconfig
            • localized
            • const
            • private 
            • protected 
            • privatewrite 
            • protectedwrite 
            • repnotify 
            • deprecated 
            • instanced 
            • databinding 
            • editoronly 
            • notforconsole 
            • editconst 
            • editfixedsize 
            • editinline 
            • editinlineuse 
            • noclear 
            • interp 
            • input 
            • transient 
            • duplicatetransient 
            • noimport  
            • native 
            • export 
            • noexport 
            • nontransactional 
            • pointer
            • init 
            • repretry 
            • allowabstract */
            string textLowCase = text.ToLower();
            switch (textLowCase)
            {
                case "config":
                case "globalconfig":
                case "localized":
                case "const":
                case "public":
                case "private":
                case "protected":
                case "privatewrite":
                case "protectedwrite":
                case "repnotify":
                case "deprecated":
                case "instanced":
                case "databinding":
                case "editoronly":
                case "notforconsole":
                case "editconst":
                case "editfixedsize":
                case "editinline":
                case "editinlineuse":
                case "noclear":
                case "interp":
                case "transient":
                case "duplicatetransient":
                case "noimport":
                case "native":
                case "export":
                case "noexport":
                case "nontransactional":
                case "pointer":
                case "init":
                case "repretry":
                case "allowabstract":
                case "bitwise":
                case "crosslevelpassive":
                    return true;
            }
            return false;
        }

        public bool IsStructSpecifier (string text) 
        {
            //atomic
            //atomicwhencooked
            //immutable
            //immutablewhencooked
            //strictconfig
            string textLowCase = text.ToLower();
            switch (textLowCase)
            {
                case "atomic":
                case "atomicwhencooked":
                case "immutable":
                case "immutablewhencooked":
                case "strictconfig":
                    return true;
            }
            return false;
        }

        public int FindTerm(ref List<string> statement, string term, int index = 0)
        {
            for(int i = index; i < statement.Count; i++)
            {
                if (statement[i] == term)
                    return i;
            }
            return -1;
        }
    }
}

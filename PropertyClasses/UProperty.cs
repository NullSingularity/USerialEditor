using System;
using System.Collections.Generic;
using System.Linq;

//UProperty models a single property (i.e. a field, variable, etc) in a UE3 Unrealscript class or struct.
//The format of this class is based off of the format produced by the Hat in Time Engine.BasicSaveObject function,
//which serializes a single object instance and saves it to a file.

//This is meant to cover as many possible properties as possible, but there are plenty of special cases.

namespace USerialEditor
{
    public class UProperty
    {
        protected String Name;  //The name of the property.
        protected String PropertyType;  //The property type. Can be one of several basic types.
        protected String Subtype;   //The "subtype". This is typically blank, but is used for arrays (what they're an array of), structs (what struct definition they're using), and enums (defined as byte properties, but if an enum, what enum definition they're using).
        protected ByteString Value;     //The value. This varies depending on the property, but is generally where the data of the property is, i.e. the state of a bool, or an int, etc.
        protected int StaticArrayIndex; //This is typically 0, but if the property is a static array, this represents what index of the array the property is, since they're stored as identical copies of the main property.

        public List<UProperty> SubProperties;   //If the property has any other properties "inside" it, like an array or a struct, they'll be stored here.

        protected List<string> Specifiers; //Any specifiers attached to the variable declaration.

        protected TemplateRetrieval TemplateScope; //The closest outer layer of template scope. Used for searching struct/enum templates.
                                                   //In StructTemplates, this will be the next struct template up in the chain, or the class template.
                                                   //Everywhere else, this will be the most relevant TemplateRetrieval class.
        protected PropertyTemplate DefinitionTemplate; //If assigned, this will be used for enums/structs instead of searching for the struct/enum definition by name.
        protected bool isModified; //Whether or not this property has been explicitly modified by the user
        public UProperty()
        {
            Value = new ByteString();
            SubProperties = new List<UProperty>();
            Specifiers = new List<string>();
        }

        /*public UProperty(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            this.Name = Name;
            this.PropertyType = PropertyType;
            this.Value = Value;
            this.Subtype = Subtype;
            SubProperties = new List<UProperty>();
        }

        public UProperty(UProperty property)
        {
            this.Name = property.Name;
            this.PropertyType = property.PropertyType;
            this.Value = new ByteString(property.Value);
            this.Subtype = property.Subtype;
            SubProperties = property.SubProperties.ToList();
        }*/
        //=========================================================================================
        //Instantiate(): This function creates and initializes an object instance of the class.
        //  This, along with the Init() function, replaces a standard constructor setup
        //  in order to try and make adding more types of properties easier.
        //  Each class's Instantiate() function is stored as a delegate in the factory.
        //=========================================================================================
        public static UProperty Instantiate(String Name, String PropertyType, ByteString Value, string Subtype = "")
        {
            UProperty instance = new UProperty();
            instance.Init(Name, PropertyType, Value, Subtype);
            return instance;
        }

        //=========================================================================================
        //Init(): This function initializes all of the object's properties. 
        //=========================================================================================
        public virtual void Init(String Name, String PropertyType, ByteString Value, string Subtype = "", int StaticArrayIndex = 0)
        {
            this.Name = Name;
            this.PropertyType = PropertyType;
            if (Value == null)
            {
                this.Value = new ByteString();
            }
            else
            {
                this.Value = Value;
            }
            this.Subtype = Subtype;
            this.SubProperties = new List<UProperty>();
            this.Specifiers = new List<string>();
            this.StaticArrayIndex = StaticArrayIndex;
        }

        //=========================================================================================
        // Overload of Init() for copying an existing property.
        //=========================================================================================
        public virtual void Init(UProperty property)
        {
            this.Name = property.Name;
            this.PropertyType = property.PropertyType;
            this.Value = new ByteString(property.Value);
            this.Subtype = property.Subtype;
            this.StaticArrayIndex = property.StaticArrayIndex;
            this.Specifiers = property.Specifiers.ToList();
            this.SubProperties = property.SubProperties.ToList();
        }

        //=========================================================================================
        //SetTemplateScope(): Sets the TemplateScope for this property.
        //=========================================================================================
        public void SetTemplateScope(TemplateRetrieval TemplateScope)
        {
            this.TemplateScope = TemplateScope;
        }

        public TemplateRetrieval GetTemplateScope()
        {
            return this.TemplateScope;
        }

        //=========================================================================================
        //SetName(): Sets the Name for this property.
        //=========================================================================================
        public void SetName(String Name)
        {
            this.Name = Name;
        }
        //=========================================================================================
        //GetName(): Returns the Name for this property.
        //=========================================================================================
        public String GetName()
        {
            return Name;
        }
        //=========================================================================================
        //SetPropertyType(): Sets the PropertyType for this property.
        //=========================================================================================
        public void SetPropertyType(String PropertyType)
        {
            this.PropertyType = PropertyType;
        }
        //=========================================================================================
        //GetPropertyType(): Returns the PropertyType for this property.
        //=========================================================================================
        public virtual String GetPropertyType()
        {
            return PropertyType;
        }

        public virtual String GetHumanReadableType()
        {
            return "Generic property";
        }

        //=========================================================================================
        //SetSubtype(): Sets the PropertyType for this property.
        //=========================================================================================
        public void SetSubtype(String Subtype)
        {
            this.Subtype = Subtype;
        }
        //=========================================================================================
        //GetSubtype(): Returns the PropertyType for this property.
        //  Defaults to returning "None" rather than a blank string, as this has been observed
        //  in UE3 serialized object saves. However, not all properties even serialize the subtype,
        //  so this might have to be overwritten regardless.
        //=========================================================================================
        public virtual String GetSubtype()
        {
            if(Subtype == null)
            {
                return "";
            }
            return Subtype;
        }

        public virtual void ModifyValue(ByteString Value)
        {
            SetValue(Value);
            isModified = true;
        }

        //=========================================================================================
        //SetValue(): Sets the Value for this property.
        //=========================================================================================
        public void SetValue(ByteString Value)
        {
            this.Value = new ByteString(Value);
        }
        //=========================================================================================
        //GetValue(): Returns the Value for this property.
        //=========================================================================================
        public ByteString GetValue()
        {
            return new ByteString(Value);
        }
        //=========================================================================================
        //AddSubProperty(): Adds a UProperty to this object's SubProperties.
        //=========================================================================================
        public void AddSubProperty(UProperty property)
        {
            this.SubProperties.Add(property);
        }

        public void InsertSubProperty(int index, UProperty property)
        {
            this.SubProperties.Insert(index,property);
        }

        public void RemoveSubProperty(int index)
        {
            this.SubProperties.RemoveAt(index);
        }
        //=========================================================================================
        //SetStaticArrayIndex(): Sets this property's StaticArrayIndex.
        //=========================================================================================
        public void SetStaticArrayIndex(int Index)
        {
            this.StaticArrayIndex = Index;
        }
        //=========================================================================================
        //GetStaticArrayIndex(): Returns this property's StaticArrayIndex.
        //=========================================================================================
        public int GetStaticArrayIndex()
        {
            return this.StaticArrayIndex;
        }

        //=========================================================================================
        //AddSpecifier(): Adds a specifier to this property's Specifiers.
        //=========================================================================================
        public void AddSpecifier(string specifier)
        {
            this.Specifiers.Add(specifier);
        }

        //=========================================================================================
        //SetSpecifier(): Sets this property's set of Specifiers directly.
        //=========================================================================================
        public void SetSpecifiers(List<string> specifiers)
        {
            this.Specifiers.Clear();
            foreach (string specifier in specifiers) 
            {
                this.Specifiers.Add(specifier);
            }
        }

        //=========================================================================================
        //GetSpecifiers(): Returns this property's Specifiers.
        //=========================================================================================
        public List<string> GetSpecifiers()
        {
            return this.Specifiers;
        }

        //=========================================================================================
        //SetDefinitionTemplate(): Sets this property's definition template.
        //=========================================================================================
        public void SetDefinitionTemplate(PropertyTemplate newDefinition)
        {
            DefinitionTemplate = newDefinition;
        }

        //=========================================================================================
        //GetDefinitionTemplate(): Returns this property's definition template.
        //=========================================================================================
        public PropertyTemplate GetDefinitionTemplate()
        {
            return DefinitionTemplate;
        }


        //=========================================================================================
        //Serialize(): Returns a ByteString, containing a serialization of the property in question.
        //  The output must exactly mimic the UE3 object serialization format.
        //  As there are many special cases, this function will nearly always be overwritten,
        //  but this definition remains as a guideline.
        //=========================================================================================
        public virtual ByteString Serialize()
        {
            ByteString bytes = new ByteString();
            bytes.AddByteString(SerializeName());
            bytes.AddByteString(SerializePropertyType());
            bytes.AddByteString(SerializeValue());
            return bytes;
        }

        //=========================================================================================
        //SerializeArray(): This returns a ByteString, containing an "abbreviated" serialization of
        //  the property. This is typically just the value, with no additional metadata. This is
        //  typically used for subproperties in arrays and immutable structs.
        //=========================================================================================
        public virtual ByteString SerializeArray()
        {
            return new ByteString(Value);
        }

        //=========================================================================================
        //SerializeName(): Returns a ByteString containing a formatted serialization of the
        //  property's Name.
        //=========================================================================================
        public virtual ByteString SerializeName()
        {
            ByteString bytes = new ByteString();
            bytes.AddSerialFormatString(Name);
            return bytes;
        }

        //=========================================================================================
        //SerializePropertyType(): Returns a ByteString containing a formatted serialization of the
        //  property's PropertyType.
        //=========================================================================================
        public virtual ByteString SerializePropertyType()
        {
            ByteString bytes = new ByteString();
            bytes.AddSerialFormatString(GetPropertyType());
            return bytes;
        }

        //=========================================================================================
        //SerializeSubtype(): Returns a ByteString containing a formatted serialization of the
        //  property's Subtype.
        //=========================================================================================
        public virtual ByteString SerializeSubtype()
        {
            ByteString bytes = new ByteString();
            //bytes.AddInt(Subtype.Length);
            bytes.AddSerialFormatString(Subtype);
            return bytes;
        }

        //=========================================================================================
        //SerializeValue(): Returns a ByteString containing a formatted serialization of the
        //  property's Value. Notably, the StaticArrayIndex is always included in this place
        //  in the metadata, regardless of if it's used or not.
        //=========================================================================================
        public virtual ByteString SerializeValue() 
        {
            ByteString bytes = new ByteString();
            bytes.AddInt(Value.GetLength());
            bytes.AddInt(StaticArrayIndex);
            bytes.AddByteString(Value);
            return bytes;
        }

        public virtual bool IsModified()
        {
            foreach(var subProperty in SubProperties)
            {
                if (subProperty.IsModified())
                    return true;
            }
            return isModified;
        }

        //This just prints out everything related to the property and all subproperties in a theoretically readable format.
        //It'll be removed before release.
        public virtual String DebugToString()
        {
            String result = "\"" + Name + "\"" + " [" + this.GetType().Name + "]: T:" + GetPropertyType() + " S:" + GetSubtype() + " V:" + Value.ToString() + " I:" + GetStaticArrayIndex();
            if(SubProperties.Count > 0)
            {
                result += "\n{";
                foreach (UProperty u in SubProperties) 
                {
                    result += "\n";
                    result += u.DebugToString();
                }
                result += "\n}";
            }
            return result;
        }
    }
}

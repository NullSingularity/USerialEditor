using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USerialEditor
{
    public partial class PropertyControlStruct : PropertyControl
    {
        protected StructProperty property;
        public PropertyControlStruct(StructProperty property)
        {
            InitializeComponent();
            this.property = property;
            UpdateComponents();
        }

        protected override void UpdateComponents()
        {
            int lastIndex = structListBox.SelectedIndex;
            structListBox.Items.Clear();
            if (property == null)
            {
                return;
            }
            int index = 0;
            foreach (var subProperty in property.SubProperties)
            {
                string name = subProperty.GetName();
                name += " : ";
                string value = "";
                if (subProperty is StringProperty)
                {
                    value += "\"" + ((StringProperty)subProperty).GetStringValue() + "\"";
                }
                else if (subProperty is IntProperty)
                {
                    value += ((IntProperty)subProperty).GetIntegerValue();
                }
                else if (subProperty is FloatProperty)
                {
                    value += ((FloatProperty)subProperty).GetFloatValue();
                }
                else if (subProperty is BoolProperty)
                {
                    if (subProperty.GetValue()[0] > 0)
                    {
                        value += "True";
                    }
                    else
                    {
                        value += "False";
                    }
                }
                else if (subProperty is ByteProperty)
                {
                    if (subProperty.GetSubtype() == "")
                    {
                        value += subProperty.GetValue()[0];
                    }
                    else
                    {
                        value += ((ByteProperty)subProperty).GetEnumerationValue();
                    }
                }
                else
                {
                    value += "[No preview]";
                }
                name += value;
                structListBox.Items.Add(name);
                index++;
            }
            structListBox.SelectedIndex = Math.Min(lastIndex, structListBox.Items.Count - 1);
        }
    }
}

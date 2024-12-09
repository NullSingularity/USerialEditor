using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace USerialEditor
{
    public partial class PropertyControlArray : PropertyControl
    {
        protected ArrayProperty property;
        PropertyFactory factory;
        public PropertyControlArray(ArrayProperty property, PropertyFactory factory)
        {
            InitializeComponent();
            this.property = property;
            this.factory = factory;
            UpdateComponents();
        }

        protected override void UpdateComponents()
        {
            int lastIndex = arrayListBox.SelectedIndex;
            arrayListBox.Items.Clear();
            if(property == null)
            {
                return;
            }
            int index = 0;
            foreach (var subProperty in property.SubProperties)
            {
                string name = subProperty.GetName();
                name += "[" + index + "]: ";
                string value = "";
                if (subProperty is StringProperty)
                {
                    value += "\"" + ((StringProperty)subProperty).GetStringValue() + "\"";
                }
                else if(subProperty is IntProperty)
                {
                    value += ((IntProperty)subProperty).GetIntegerValue();
                }
                else if(subProperty is FloatProperty)
                {
                    value += ((FloatProperty)subProperty).GetFloatValue();
                }
                else if(subProperty is BoolProperty)
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
                else if(subProperty is ByteProperty)
                {
                    if(subProperty.GetSubtype() == "")
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
                arrayListBox.Items.Add(name);
                index++;
            }
            arrayListBox.SelectedIndex = Math.Min(lastIndex, arrayListBox.Items.Count - 1);
        }

        private void PropertyControlArray_Load(object sender, EventArgs e)
        {

        }

        protected override void ModifyPropertyValue()
        {
            property.ModifyValue();
            UpdateComponents();
            OnPropertyModified(new PropertyModifiedEventArgs(property));
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            factory.ArrayAppendSubproperty(property);
            ModifyPropertyValue();
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            factory.ArrayInsertSubproperty(property, arrayListBox.SelectedIndex);
            ModifyPropertyValue();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            factory.ArrayRemoveSubproperty(property, arrayListBox.SelectedIndex);
            ModifyPropertyValue();
        }
    }
}

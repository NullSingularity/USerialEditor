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
    public partial class PropertyControlEnum : PropertyControl
    {
        protected ByteProperty property;
        public PropertyControlEnum(ByteProperty property)
        {
            InitializeComponent();
            this.property = property;
            RefreshComboBox();
            UpdateComponents();
        }

        protected void RefreshComboBox()
        {
            if (property == null)
            {
                return;
            }
            if (property.GetSubtype() == null || property.GetSubtype() == "")
            {
                return;
            }
            comboBox1.Items.Clear();
            List<string> enumOptions = property.GetEnumOptions();
            if (enumOptions == null)
            {
                return;
            }
            foreach (string enumOption in enumOptions)
            {
                comboBox1.Items.Add(enumOption);
            }
        }

        protected override void UpdateComponents()
        {
            int index = property.GetByteValue();
            if(index >= 0 && index < comboBox1.Items.Count && index != comboBox1.SelectedIndex)
            {
                comboBox1.SelectedIndex = property.GetByteValue();
            }
        }

        protected override void ModifyPropertyValue()
        {
            int index = comboBox1.SelectedIndex;
            property.ModifyValue((byte)index);
            UpdateComponents();
            OnPropertyModified(new PropertyModifiedEventArgs(property));
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine("ENUM VAL SELECT CHANGE");
            ModifyPropertyValue();
        }
    }
}

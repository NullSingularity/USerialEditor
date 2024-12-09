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
    public partial class PropertyControlFloat : PropertyControl
    {
        protected FloatProperty property;
        public PropertyControlFloat(FloatProperty property)
        {
            InitializeComponent();
            this.property = property;
            UpdateComponents();
        }

        protected void UpdateComponents()
        {
            if (property != null)
            {
                textBox1.Text = "" + property.GetFloatValue();
            }
        }

        protected override void ModifyPropertyValue()
        {
            try
            {
                float newValue = float.Parse(textBox1.Text);
                property.ModifyValue(newValue);
            }
            catch
            {
                Console.WriteLine("Invalid float input!");
            }
            finally
            {
                UpdateComponents();
                OnPropertyModified(new PropertyModifiedEventArgs(property));
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            ModifyPropertyValue();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ModifyPropertyValue();
            }
        }
    }
}

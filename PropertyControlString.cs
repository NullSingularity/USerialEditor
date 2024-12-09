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
    public partial class PropertyControlString : PropertyControl
    {
        protected StringProperty property;
        public PropertyControlString(StringProperty property)
        {
            InitializeComponent();
            this.property = property;
            UpdateComponents();
        }

        protected override void UpdateComponents()
        {
            if (property != null)
            {
                textBox1.Text = property.GetStringValue();
            }
        }

        protected override void ModifyPropertyValue()
        {
            try
            {
                property.ModifyValue(textBox1.Text);
            }
            catch
            {
                Console.WriteLine("Invalid string input!");
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

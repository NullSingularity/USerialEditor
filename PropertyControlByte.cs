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
    public partial class PropertyControlByte : PropertyControl
    {
        protected ByteProperty property;
        public PropertyControlByte(ByteProperty property)
        {
            InitializeComponent();
            this.property = property;
            UpdateComponents();
        }

        protected override void UpdateComponents()
        {
            if (property != null)
            {
                textBox1.Text = ""+property.GetByteValue();
            }
        }

        protected override void ModifyPropertyValue()
        {
            try
            {
                byte newValue = byte.Parse(textBox1.Text);
                property.ModifyValue(newValue);
            }
            catch
            {
                Console.WriteLine("Invalid byte input!");
            }
            finally
            {
                UpdateComponents();
                OnPropertyModified(new PropertyModifiedEventArgs(property));
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ModifyPropertyValue();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            ModifyPropertyValue();
        }

    }
}

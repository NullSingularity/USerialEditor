using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace USerialEditor
{
    public partial class PropertyControlBoolean : PropertyControl
    {
        protected BoolProperty property;
        public PropertyControlBoolean(BoolProperty property)
        {
            this.property = property;
            if(property == null) 
            {
                foreach(Control control in this.Controls)
                {
                    control.Enabled = false;
                }
            }
            InitializeComponent();
            UpdateComponents();
        }

        protected void UpdateComponents()
        {
            if(property != null)
            {
                checkBox1.Checked = property.GetBooleanValue();
            }
        }

        protected override void ModifyPropertyValue()
        {
            if (property != null)
            {
                property.ModifyValue(checkBox1.Checked);
                UpdateComponents();
            }
            OnPropertyModified(new PropertyModifiedEventArgs(property));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ModifyPropertyValue();
        }
    }
}

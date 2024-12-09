using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


/*TODO Remaining PropertyControls
 * Boolean - DONE
 * Float - DONE
 * Int - DONE
 * String - DONE
 *      Object - DONE
 *      Class - DONE
 *      Name - DONE
 * Byte - DONE
 *      Enum - DONE
 * Array (Dynamic)
 * 
*/

namespace USerialEditor
{
    public partial class PropertyControl : UserControl
    {
        private static readonly object EventPropertyModified;
        public PropertyControl()
        {
            InitializeComponent();
        }

        static PropertyControl()
        {
            EventPropertyModified = new object();
        }

        protected virtual void OnPropertyModified(EventArgs e)
        {
            ((EventHandler)base.Events[EventPropertyModified])?.Invoke(this, e);
        }

        public event EventHandler PropertyModified
        {
            add
            {
                base.Events.AddHandler(EventPropertyModified, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventPropertyModified, value);
            }
        }

        protected virtual void UpdateComponents()
        {

        }

        protected virtual void ModifyPropertyValue()
        {
            
        }


    }
}

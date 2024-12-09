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
    public partial class FormFileDisplay : Form
    {
        public FormFileDisplay(string filename, string text)
        {
            InitializeComponent();
            richTextBox1.Text = text;
            this.Text = filename;
        }

        public void LoadText(string text) 
        {
            richTextBox1.Text = text;
        }

        public void SetFilename(string filename) 
        {
            this.Text = filename;
        }
    }
}

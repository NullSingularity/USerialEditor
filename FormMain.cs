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
    public partial class FormMain : Form
    {
        MainApplicationContext context;
        ClassLibrary library;
        SourceCodeProcessor processor;
        PropertyFactory factory;

        Dictionary<TabPage, UProperty> openObjects;
        public FormMain(MainApplicationContext context, ClassLibrary library, SourceCodeProcessor processor, PropertyFactory factory)
        {
            InitializeComponent();
            this.context = context;
            this.library = library;
            this.processor = processor;
            this.factory = factory;
            openObjects = new Dictionary<TabPage, UProperty>();
            RefreshClasses();
        }

        private void RefreshClasses() 
        {
            List<string> names = library.GetClassNames();
            names.Sort();
            toolStripComboBox1.Items.Clear();
            foreach (string name in names) 
            {
                toolStripComboBox1.Items.Add(name);
            }
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            RefreshClasses();
        }

        private void refreshTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            processor.ReloadClassTemplates();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(toolStripComboBox1.SelectedItem == null)
            {
                return;
            }
            ClassTemplate template = library.GetClassTemplate(toolStripComboBox1.SelectedItem.ToString());
            if(template == null) 
            {
                return;
            }
            UProperty newObject = factory.BuildClass(template);
            if( newObject == null ) 
            {
                return;
            }
            ObjectFileControls newControls = new ObjectFileControls(newObject, library, factory);
            newControls.Dock = DockStyle.Fill;
            TabPage newPage = new TabPage();
            newPage.Text = "Untitled";
            newPage.Controls.Add(newControls);
            //newPage.ContextMenuStrip = new ContextMenuStrip();
            /*ToolStripItem closeButton = new ToolStripButton();
            closeButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            closeButton.Text = "X";
            closeButton.Click += CloseTab;
            newPage.ContextMenuStrip.Items.Add(closeButton);*/
            tabControl1.TabPages.Add(newPage);
            openObjects.Add(newPage, newObject);
        }

        public void CloseTab(object sender, EventArgs e)
        {

        }

        private void manageSourceFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            context.ShowSourceManager();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UProperty currentObject = openObjects[tabControl1.SelectedTab];
            if (currentObject == null)
            {
                return;
            }
            if (!(currentObject is ClassProperty))
            {
                return;
            }


            ClassProperty classProperty = (ClassProperty)currentObject;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Serialized object file (*.sobj)|*.sobj|All Files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;

            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = saveFileDialog.FileName;

                if(SerializedFileWriter.WriteSerializedFile(path, classProperty))
                {
                    Console.WriteLine("File successfully written to: " + path);
                }
            }
        }
    }
}

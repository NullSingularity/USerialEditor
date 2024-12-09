using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USerialEditor
{
    public partial class FormSourceManager : Form
    {
        SourceLoader sourceLoader;
        SourceCodeLibrary sourceCodeLibrary;
        SourceCodeProcessor sourceCodeProcessor;

        public FormSourceManager(SourceLoader sourceLoader, SourceCodeLibrary sourceCodeLibrary, SourceCodeProcessor sourceCodeProcessor)
        {
            InitializeComponent();
            this.sourceLoader = sourceLoader;
            this.sourceCodeLibrary = sourceCodeLibrary;
            this.sourceCodeProcessor = sourceCodeProcessor;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            UpdateList();
        }
        private void UpdateList()
        {
            List<string> sourcePaths = sourceCodeLibrary.GetClassSourceNames();

            string currentSelection = "";
            if (listBox1.SelectedIndex > -1)
            {
                currentSelection = listBox1.SelectedItem.ToString();
            }
            listBox1.Items.Clear();
            foreach (string sourcePath in sourcePaths)
            {
                listBox1.Items.Add(sourcePath);
            }
            if(currentSelection != "")
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if (listBox1.Items[i].ToString() == currentSelection)
                    {
                        listBox1.SelectedIndex = i;
                        break;
                    }
                }
            }
            listBox1.Update();
        }

        /* private void loadSourceFileToolStripMenuItem_Click(object sender, EventArgs e)
         {


         }

         private void loadSourceFileDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
         {


         }*/

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                return;
            }
            SourceSegment segment = sourceCodeLibrary.GetClassSource(listBox1.Items[listBox1.SelectedIndex].ToString());
            if (segment == null)
            { 
                return;
            }
            string source = segment.GetText();
            richTextBox1.Clear();
            richTextBox1.Text = source;
        }

        private void AddFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Unrealscript files (*.uc)|*.uc|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string path in openFileDialog.FileNames)
                {
                    sourceLoader.LoadSourceFile(path);
                }
                UpdateList();
            }
        }

        private void AddDirectory()
        {
            string path;
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;
                sourceLoader.LoadSourceDirectory(path);
                UpdateList();
            }
        }

        private void addSourceFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFile();
        }

        private void addSourceDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddDirectory();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            AddFile();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            AddDirectory();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            sourceCodeProcessor.ReloadClassTemplates();
        }

        private void refreshTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sourceCodeProcessor.ReloadClassTemplates();
        }
    }
}

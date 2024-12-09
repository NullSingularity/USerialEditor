using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USerialEditor
{
    public partial class FormTermDisplay : Form
    {
        FormFileDisplay fileDisplay;
        PropertyFactory propertyFactory;
        public FormTermDisplay(FormFileDisplay fileDisplay, PropertyFactory propertyFactory)
        {
            InitializeComponent();
            this.fileDisplay = fileDisplay;
            this.propertyFactory = propertyFactory;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            string path = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Unrealscript files (*.uc)|*.uc|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                path = ofd.FileName;
              //  LoadAndParse(path);
            }
        }

       /* private void LoadAndParse(string path)
        {
            string data;
            using (StreamReader sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
                sr.Close();
            }

            if (data.Length > 0)
            {
                SourceCodeProcessor parsertest = new SourceCodeProcessor(data, propertyFactory);
                parsertest.ProcessSourceCode();
                List<List<string>> statements = parsertest.GetStatements();
                dataGridTerms.Rows.Clear();
                dataGridTerms.Columns.Clear();
                for (int i = 0; i < statements.Count; i++) 
                {
                    for(int k = dataGridTerms.Columns.Count; k < statements[i].Count; k++)
                    {
                        dataGridTerms.Columns.Add("Row "+k, "Row "+k);
                    }
                    dataGridTerms.Rows.Add();
                    for(int j = 0; j < statements[i].Count; j++)
                    {
                        dataGridTerms[j, i].Value = statements[i][j];
                    }
                }
                string Filename = Path.GetFileName(path);
                label1.Text = "File: " + Filename;
                if(fileDisplay != null && !fileDisplay.IsDisposed)
                {
                    fileDisplay.LoadText(parsertest.GetData());
                    fileDisplay.SetFilename(Filename);
                    fileDisplay.Show();
                }

            }
        }*/

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

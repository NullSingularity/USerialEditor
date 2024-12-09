using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class SourceLoader
    {
        SourceCodeProcessor sourceCodeProcessor;
        SourceCodeLibrary sourceCodeLibrary;
        public SourceLoader(SourceCodeProcessor sourceCodeProcessor, SourceCodeLibrary sourceCodeLibrary) 
        { 
            this.sourceCodeProcessor = sourceCodeProcessor;
            this.sourceCodeLibrary = sourceCodeLibrary;
        }

        public void LoadSourceFile (string path, bool force = false)
        {
            string data;
            if(!File.Exists(path))
                return;
            if (!force && sourceCodeLibrary.GetClassSource(path) != null)
                return;
            
            using (StreamReader sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
                sr.Close();
            }

            if (data.Length > 0)
            {
                if (force)
                {
                    sourceCodeProcessor.ProcessAndUpdateSource(path, data);
                }
                else
                {
                    sourceCodeProcessor.ProcessAndAddSource(path, data);
                }
            }
        }

        public void LoadSourceDirectory (string path, bool force = false)
        {
            if (!Directory.Exists(path))
                return;
            foreach(string file in Directory.GetFiles(path)) 
            {
                if (!File.Exists(file))
                    continue;
                if (!file.EndsWith(".uc"))
                    continue;
                if (!force && sourceCodeLibrary.GetClassSource(file) != null)
                    continue;
                string data;
                using (StreamReader sr = new StreamReader(file))
                {
                    data = sr.ReadToEnd();
                    sr.Close();
                }
                if (data.Length > 0) 
                { 
                    if(force)
                    {
                        sourceCodeProcessor.ProcessAndUpdateSource(file, data);
                    }
                    else
                    {
                        sourceCodeProcessor.ProcessAndAddSource(file, data);
                    }
                }
            }
        }
    }
}

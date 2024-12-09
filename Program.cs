using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USerialEditor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
             Application.EnableVisualStyles();
             Application.SetCompatibleTextRenderingDefault(false);
             Application.Run(new MainApplicationContext());
            //  ByteStringTest();
            
            
          //  Initialize();
        }

       /* static void Initialize()
        {
            TemplateManager templateManager = new TemplateManager();
            PropertyFactory propertyFactory = new PropertyFactory(templateManager);

            InitializePropertyConstructors(propertyFactory);
            AddTestStructs(templateManager);
            TestStructSpawning(propertyFactory);
        }

        static void InitializePropertyConstructors(PropertyFactory pf)
        {
            pf.AddPropertyConstructor("ByteProperty", ByteProperty.Instantiate);
            pf.AddPropertyConstructor("StrProperty", StringProperty.Instantiate);
            pf.AddPropertyConstructor("StructProperty", StructProperty.Instantiate);
        }

        static void AddTestStructs(TemplateManager tm)
        {
            PropertyTemplate root1 = new PropertyTemplate("", "StructProperty", "StructOne");
            root1.AddSubProperty(new PropertyTemplate("MyByteProperty1", "ByteProperty"));
            root1.AddSubProperty(new PropertyTemplate("MyStringProperty1", "StrProperty"));
            root1.AddSubProperty(new PropertyTemplate("MyByteProperty2", "ByteProperty"));

            PropertyTemplate root2 = new PropertyTemplate("", "StructProperty", "StructTwo");
            root2.AddSubProperty(new PropertyTemplate("YourByteProperty", "ByteProperty"));
            root2.AddSubProperty(new PropertyTemplate("YourStringProperty", "StrProperty"));
            root2.AddSubProperty(new PropertyTemplate("SubStructProperty", "StructProperty", "StructOne"));

            tm.AddStructTemplate(root1.GetSubtype(), root1);
            tm.AddStructTemplate(root2.GetSubtype(), root2);

        }

        static void TestStructSpawning(PropertyFactory pm)
        {
            UProperty struct1 = pm.BuildStruct("StructTwo","TestStruct");
            Console.WriteLine(struct1.DebugToString());
        }

        static void ByteStringTest()
        {
            Console.WriteLine("Running ByteString Test");
            ByteString b = new ByteString("Hello World!");
            //b = b.Insert(9, (Byte)69);
           // b += 9999;
           // b += "chungus";
            //b.AddByte(0x12);
            //b.AddByte(0xFF);
            // b.AddString("abcdefghijklmnop");
            //b.AddInt(9999);
            //b.AddByte(0x8C);
            //b.AddByte(0x00);
            //b.AddByte(0x00);
            //b.AddByte(0x00);
           // b = b.Remove(0, 6);
            ByteString c = new ByteString("NaniDesuka");
            b += c;
            b = b.Insert(12, 420);
            Console.WriteLine(b.ToString());
            Console.WriteLine(b.GetString(0, b.GetLength()));
            //Console.WriteLine(b[0]);
           // Console.WriteLine(b[1]);
           // Console.WriteLine(b.GetString(2, 16));
           // Console.WriteLine(b.GetInt(18, 4));

        }*/



       /* static void SourceParserTest(string path)
        {
            string data;
            using(StreamReader sr = new StreamReader(path)) 
            {
                data = sr.ReadToEnd();
                sr.Close();
            }

            if(data.Length > 0)
            {
                SourceCodeParser parsertest = new SourceCodeParser(data);
                parsertest.CleanComments();
                parsertest.ParseTerms();
                parsertest.DebugPrintTerms();
            }
        }*/
    }
}

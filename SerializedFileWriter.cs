using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class SerializedFileWriter
    {
        public SerializedFileWriter() 
        { 
        }

        public static bool WriteSerializedFile(string filePath, UProperty classProperty)
        {
            try
            {
                FileStream fs = File.Open(filePath, FileMode.Create);
                using (BinaryWriter sw = new BinaryWriter(fs))
                {
                    ByteString objectData = classProperty.Serialize();
                    Byte[] dataBytes = objectData.GetBytes(0, objectData.GetLength());
                    sw.Write(dataBytes);
                    sw.Flush();
                    sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR! FILE WRITE FAILED!\n" + ex.Message);
                return false;
            }
        }
    }
}

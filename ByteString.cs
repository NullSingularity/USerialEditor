using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class ByteString
    {
        protected byte[] data;
        protected int length;
        protected int max;

        public byte this[int i]
        {
            get { return data[i]; }
            set { data[i] = value; }
        }

        public static ByteString operator +(ByteString a, String s)
        {
            ByteString b = new ByteString(a);
            b.AddString(s);
            return b;
        }

        public static ByteString operator +(ByteString a, byte bt)
        {
            ByteString b = new ByteString(a);
            b.AddByte(bt);
            return b;
        }

        public static ByteString operator +(ByteString a, ByteString b)
        {
            ByteString c = new ByteString(a);
            c.AddByteString(b);
            return c;
        }

        public static ByteString operator +(ByteString a, int num)
        {
            ByteString b = new ByteString(a);
            b.AddInt(num);
            return b;
        }

        public static ByteString operator +(ByteString a, float num)
        {
            ByteString b = new ByteString(a);
            b.AddFloat(num);
            return b;
        }

        public static ByteString operator +(ByteString a, byte[] bytes)
        {
            ByteString b = new ByteString(a);
            b.AddBytes(bytes);
            return b;
        }

        public void AddString(String s)
        {
            char c;
            for(int i = 0; i < s.Length; i++)
            {
                c = s[i];
                AddByte((byte)c);
            }
        }

        public void AddSerialFormatString(String s)
        {
            if(s.Length > 0)
            {
                AddInt(s.Length + 1);
                AddString(s);
                AddByte(0);
            }
            else
            {
                AddInt(0);
            }
            
        }

        public void AddChar(char c)
        {
            AddByte((byte)c);
        }

        public void AddByte(byte b)
        {
            if(length >= max)
            {
                Expand();
            }
            if (length > max)       //PLACEHOLDER - USE EXCEPTIONS INSTEAD!!!!!!!!!!!!
                return;
            data[length] = b;
            length++;
        }

        public void AddBytes(byte[] b)
        {
            int j = b.GetLength(0);
            for(int i = 0; i < j; i++)
            {
                AddByte(b[i]);
            }
        }

        public void AddByteString(ByteString b)
        {
            for(int i = 0; i < b.length; i++)
            {
                AddByte(b.data[i]);
            }
        }

        public void AddInt(int num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            if(!BitConverter.IsLittleEndian)
            {
                bytes.Reverse();
            }
            for(int i = 0; i < 4; i++)
            {
                AddByte(bytes[i]);
            }
        }

        public void AddFloat(float num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            if (!BitConverter.IsLittleEndian)
            {
                bytes.Reverse();
            }
            for (int i = 0; i < 4; i++)
            {
                AddByte(bytes[i]);
            }
        }


        private void Expand()
        {     
            byte[] newdata = new byte[max * 2];
            for(int i = 0; i < length; i++)
            {
                newdata[i] = data[i];
            }
            data = newdata;
            max *= 2;
        }

        public int GetLength()
        {
            return length;
        }

        public int GetMax()
        {
            return max;
        }


        public override String ToString()
        {
            String result = "";
            for(int i = 0; i < length; i++)
            {
                result += data[i];
                if(i < length - 1)
                {
                    result += " ";
                }
            }
            return result;
        }

        public String GetString(int start, int len)
        {
            if (length == 0 || len == 0)
            {
                return "";
            }

            if (len < 0   //PLACEHOLDER - USE PROPER EXCEPTIONS INSTEAD!!!!!!!!!!!!
                || start < 0
                || start >= length
                || start+len > length)
            {
                throw new IndexOutOfRangeException();
            }

            String result = "";

            for (int i = start; i < start+len; i++)
            {
                result += (char)data[i];
            }
            return result;
        }

        public byte[] GetBytes(int start, int len)
        {
            if (len <= 0    //PLACEHOLDER - USE PROPER EXCEPTIONS INSTEAD!!!!!!!!!!!!
                || start < 0
                || start >= length
                || start + len > length)
            {
                throw new IndexOutOfRangeException();
            }

            byte[] result = new byte[len];
            int j = 0;

            for(int i = start; i < start + len; i++)
            {
                result[j] = data[i];
                j++;
            }
            return result;
        }

        public ByteString GetSegment(int start, int len)
        {
            if (len <= 0   //PLACEHOLDER - USE PROPER EXCEPTIONS INSTEAD!!!!!!!!!!!!
                || start < 0
                || start >= length
                || start+len >= length)
            {
                throw new IndexOutOfRangeException();
            }

            ByteString b = new ByteString();
            for(int i = start; i < start+len; i++)
            {
                b.AddByte(data[i]);
            }
            return b;
        }

        public int GetInt(int start, int len)
        {
            if (len <= 0   //PLACEHOLDER - USE PROPER EXCEPTIONS INSTEAD!!!!!!!!!!!!
                || start < 0
                || start >= length
                || start + len > length)
            {
                throw new IndexOutOfRangeException();
            }
            int result = 0;
            byte[] bytes = GetBytes(start, len);
            if (!BitConverter.IsLittleEndian)
            {
                bytes.Reverse();
            }
            result = BitConverter.ToInt32(bytes, 0);
            return result;
        }

        public float GetFloat(int start, int len)
        {
            if (len <= 0   //PLACEHOLDER - USE PROPER EXCEPTIONS INSTEAD!!!!!!!!!!!!
                || start < 0
                || start >= length
                || start + len > length)
            {
                throw new IndexOutOfRangeException();
            }
            float result = 0;
            byte[] bytes = GetBytes(start, len);
            if (!BitConverter.IsLittleEndian)
            {
                bytes.Reverse();
            }
            result = BitConverter.ToSingle(bytes, 0);
            return result;
        }

        public ByteString Insert(int index, Byte value)
        {
            if (index < 0 || index > length)
            {
                throw new IndexOutOfRangeException();
            }
            ByteString result;
            if (index == 0)
            {
                result = new ByteString();
            }
            else
            {
                result = GetSegment(0, index);
            }

            result.AddByte(value);
            for (int i = index; i < length; i++)
            {
                result.AddByte(data[i]);
            }

            return result;
        }

        public ByteString Insert(int index, string value)
        {
            if (index < 0 || index > length)
            {
                throw new IndexOutOfRangeException();
            }
            ByteString result;
            if (index == 0)
            {
                result = new ByteString();
            }
            else
            {
                result = GetSegment(0, index);
            }

            result.AddString(value);
            for (int i = index; i < length; i++)
            {
                result.AddByte(data[i]);
            }

            return result;
        }

        public ByteString Insert(int index, Byte[] value)
        {
            if (index < 0 || index > length)
            {
                throw new IndexOutOfRangeException();
            }
            ByteString result;
            if (index == 0)
            {
                result = new ByteString();
            }
            else
            {
                result = GetSegment(0, index);
            }

            result.AddBytes(value);
            for (int i = index; i < length; i++)
            {
                result.AddByte(data[i]);
            }

            return result;
        }

        public ByteString Insert(int index, int value)
        {
            if (index < 0 || index > length)
            {
                throw new IndexOutOfRangeException();
            }
            ByteString result;
            if (index == 0)
            {
                result = new ByteString();
            }
            else
            {
                result = GetSegment(0, index);
            }

            result.AddInt(value);
            for (int i = index; i < length; i++)
            {
                result.AddByte(data[i]);
            }

            return result;
        }

        public ByteString Insert(int index, float value)
        {
            if (index < 0 || index > length)
            {
                throw new IndexOutOfRangeException();
            }
            ByteString result;
            if (index == 0)
            {
                result = new ByteString();
            }
            else
            {
                result = GetSegment(0, index);
            }

            result.AddFloat(value);
            for (int i = index; i < length; i++)
            {
                result.AddByte(data[i]);
            }

            return result;
        }

        public ByteString Insert(int index, ByteString value)
        {
            if (index < 0 || index > length)
            {
                throw new IndexOutOfRangeException();
            }
            ByteString result;
            if (index == 0)
            {
                result = new ByteString();
            }
            else
            {
                result = GetSegment(0, index);
            }

            result.AddByteString(value);
            for (int i = index; i < length; i++)
            {
                result.AddByte(data[i]);
            }

            return result;
        }

        public ByteString Remove(int index, int len)
        {
            ByteString result;
            if (index >= length
               || index < 0
               || index + len > length)
            {
                throw new IndexOutOfRangeException();
            }

            if(len == 0)
            {
                return new ByteString(this);
            }

            if (index == 0)
            {
                result = new ByteString();
            }
            else
            {
                result = GetSegment(0, index);
            }

            for (int i = index+len; i < length; i++)
            {
                result.AddByte(data[i]);
            }

            return result;
        }

        public ByteString()
        {
            data = new byte[128];
            length = 0;
            max = 128;
        }

        public ByteString(ByteString other)
        {
            data = new byte[other.max];
            length = other.length;
            max = other.max;
            for(int i = 0; i < other.max; i++)
            {
                data[i] = other.data[i];
            }
        }

        public ByteString(String s)
        {
            data = new byte[128];
            length = 0;
            max = 128;
            AddString(s);
        }

        public ByteString(byte[] b)
        {
            data = new byte[128];
            length = 0;
            max = 128;
            AddBytes(b);
        }

        public ByteString(int i)
        {
            data = new byte[128];
            length = 0;
            max = 128;
            AddInt(i);
        }

        public ByteString(float f)
        {
            data = new byte[128];
            length = 0;
            max = 128;
            AddFloat(f);
        }
    }
}

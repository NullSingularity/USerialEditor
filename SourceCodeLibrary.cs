using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class SourceCodeLibrary
    {
        Dictionary<string, SourceSegment> classSourceSegments;
        public SourceCodeLibrary() 
        { 
            classSourceSegments = new Dictionary<string, SourceSegment>();
        }

        public bool AddClassSource(string name, SourceSegment source)
        {
            if(classSourceSegments.ContainsKey(name)) 
            {
                return false;
            }
            else
            {
                classSourceSegments.Add(name, source);
                return true;
            }
        }

        public void ReplaceClassSource(string name, SourceSegment source) 
        {
            if (classSourceSegments.ContainsKey(name)) 
            { 
                classSourceSegments.Remove(name);
            }
            classSourceSegments.Add(name, source);
        }

        public bool RemoveClassSource(string name)
        {
            if(classSourceSegments.ContainsKey(name))
            {
                classSourceSegments.Remove(name);
                return true;
            }
            return false;
        }

        public SourceSegment GetClassSource(string name) 
        { 
            if(classSourceSegments.ContainsKey(name)) 
            { 
                return classSourceSegments[name];
            }
            else
            {
                return null;
            }
        }

        public List<string> GetClassSourceNames()
        {
            return classSourceSegments.Keys.ToList();
        }


    }
}

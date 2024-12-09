using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  This class represents an optional segment of text in the processed sourcecode tree produced by the SourceSegment family of classes.

namespace USerialEditor
{
    public class SourceSegmentOptional: SourceSegment
    {
        bool enabled;

        public SourceSegmentOptional(SourceSegment parent, string source, bool enabled)
        {
            this.enabled = enabled;
            parentNode = parent;
            ProcessSource(source);
        }

        public override int Length()
        {
            if(!enabled)
                return 0;
            return GetSegmentsLength();
        }

        public override string GetText()
        {
            if(!enabled)
            {
                return "";
            }
            string result = "";
            foreach (var segment in segments)
            {
                result += segment.GetText();
            }
            return result;
        }

        public void SetEnabled(bool enabled) 
        {
            this.enabled = enabled;
        }
    }
}

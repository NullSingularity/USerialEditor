using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  This class represents a non-optional segment of text in the processed sourcecode tree produced by the SourceSegment family of classes.

namespace USerialEditor
{
    public class SourceSegmentRequired: SourceSegment
    {
        string text;

        public SourceSegmentRequired(SourceSegment parent, string text)
        {
            this.text = text;
            this.parentNode = parent;
        }

        public override int Length()
        {
            return text.Length;
        }

        public override string GetText()
        {
            return text;
        }

        protected override void ProcessSource(string source)
        {
            return;
        }

        public override void DebugPrintStructure(int tier = 0)
        {
            string spacing = "";
            for (int i = 0; i < tier; i++)
            {
                spacing += "\t";
            }
            if(text.Length > 15)
            {
                Console.WriteLine(spacing+"\"" + text.Substring(0, 15) + "...\"");
            }
            else
            {
                Console.WriteLine(spacing+"\"" + text + "\"");
            }
        }
    }
}

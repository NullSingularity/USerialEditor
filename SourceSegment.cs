using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class, along with its Optional/Required subclasses, process given Hat in Time UE3 Unrealscript source code into a tree structure, segmenting it into optional and non-optional segments.
//This is meant to process the `if/`else/etc segments in the source code that are resolved during compile time, resulting in segments of the text
//  being included or excluded based on set conditions. For the time being, this program will not be resolving the actual conditions given with the segments.
//This will produce a tree of text segments, some of which can be turned "on" or "off" at the user's discretion,
//and which can be reformed into a complete version of the original text (with selected segments present or absent) at any time.

namespace USerialEditor
{
    public class SourceSegment
    {
        protected List<SourceSegment> segments;
        protected SourceSegment parentNode;
        //protected int textOffset;

        public SourceSegment()
        {
            segments = new List<SourceSegment>();
        }
        public SourceSegment(SourceSegment parent, string source) 
        {
            segments = new List<SourceSegment>();
            //textOffset = offset;
            ProcessSource(source);     
        }

        public virtual int Length()
        {
            return GetSegmentsLength();
        }

        protected virtual int GetSegmentsLength()
        {
            int result = 0;
            foreach (var segment in segments)
            {
                result += segment.Length();
            }
            return result;
        }

        public virtual string GetText()
        {
            string result = "";
            foreach (var segment in segments)
            {
                result += segment.GetText();
            }
            return result;
        }

        protected virtual void ProcessSource(string source) 
        {
            int index = 0;          //Search start index for the current loop.
            int nestingLevel = 0;   //How many levels of nested segments we're currently in.
                                    //1 indicates an outer segment, anything higher is inner segments.
            int segmentStart = -1;  //Outer segment beginning, encompassing all inner segments
            int lastSegmentEnd = 0; //End of last outer segment
            bool branching = false; //If last segment ended in `else or `elseif we go straight into another segment without searching
            while(index < source.Length)
            {
                int segStartA = -1; //Starting index of the segment starting term
                int segStartB = -1; //Ending index of the segment starting term
                //int segStartType = -1; //Type of the segment starting term
                int segEndA = -1; //Starting index of the segment ending term
                int segEndB = -1; //Ending index of the segment ending term
                int segEndType = -1; //Type of the segment ending term
                ProcessGetSegmentStart(ref source, index, out segStartA, out segStartB);    //Get the next starting term.
                
                if((segStartA == -1 && nestingLevel <= 0) && !branching)  //No optional segments remaining, add the rest of the file as non-optional text
                {
                    string substring = source.Substring(lastSegmentEnd, source.Length - lastSegmentEnd);
                    segments.Add(new SourceSegmentRequired(this, substring));
                    break;
                }

                /*string segStartText = ""; //Debug variable
                if (segStartA > -1 && segStartB > -1)   
                {
                    segStartText = source.Substring(segStartA, (segStartB + 1) - segStartA);
                }*/

                if (nestingLevel < 1)        //Start of outer segment
                {
                    if(!branching)  //If we're not branching, move to the end of the next starting term and add everything before it as non-optional
                    {
                        string substring = source.Substring(lastSegmentEnd, segStartA - lastSegmentEnd);
                        segments.Add(new SourceSegmentRequired(this, substring));
                        index = segStartB + 1;
                    }
                    // Start the outer segment. We'll be trying to get to the end of this to section it off and give it to another SourceSegment to process
                    //  the internal text, in case of nested segments.
                    segmentStart = index;
                    nestingLevel++;
                    branching = false;
                }
                else
                {
                    ProcessGetSegmentEnd(ref source, index, out segEndA, out segEndB, out segEndType);  //Get the next ending term.

                    if (segEndA == -1)
                        break;  //Failure state

                    //string segEndText = source.Substring(segEndA, (segEndB + 1) - segEndA);   //Debug variable

                    if (((segStartA > -1) && segStartA < segEndA) || branching) //Start of nested inner segment
                    {
                        nestingLevel++;
                        if(!branching)  //If we're branching, we're already where we need to be. Otherwise, move to the end of the next starting term.
                        index = segStartB + 1;
                        branching = false;
                    }
                    else  //End of inner (or outer) segment
                    {
                        if(segEndType == 2 || segEndType == 3)  //If the segment ends with `else or `elseif, we branch,
                                                                //going directly into another segment without searching for the next starting term.
                        {
                            branching = true;
                        }
                        nestingLevel--;
                        index = segEndB + 1;
                    }
                    
                    if(nestingLevel == 0)   //End of outer segment
                    {
                        string substring = source.Substring(segmentStart, segEndA - segmentStart);
                        segments.Add(new SourceSegmentOptional(this, substring, false));
                        if(segEndType == 1)     //If this segment ends in `endif, we've concluded the if statement.
                                                //If it's `else or `elseif we're branching and we've already been moved to the necessary index.
                        {
                            index = segEndB + 1;
                            lastSegmentEnd = index; //Mark where this segment ended so we can catch the intervening text.
                        }
                    }
                        
                }
                

            }
            
        }

        private void ProcessGetSegmentStart(ref string source, int index, out int start, out int end) //Gets the starting/ending index of the next segment-starting term (currently just `if), including parentheses
        {
            int segStart = source.IndexOf("`if", index);
            if (segStart > -1)
            {
                start = segStart;
                end = FindEndingBracket(ref source, segStart, '(', ')');
            }
            else
            {
                start = -1;
                end = -1;
            }
        }

        private void ProcessGetSegmentEnd(ref string source, int index, out int start, out int end, out int type) //Gets the index of the next segment-terminating term (`endif, `else, `elseif)
        {
            int termStart = source.IndexOf("`endif", index);
            int termEnd = termStart + 5;
            int termType = 0;
            if (termStart > -1) 
            {
                termType = 1;
            }
            int elseStart = source.IndexOf("`else", index);
            
            if (elseStart > -1)
            {
                if ( termStart < 0 || elseStart < termStart)
                {
                    termStart = elseStart;
                    termEnd = termStart + 4;
                    termType = 2;

                }
            }
            int elseIfStart = source.IndexOf("`elseif", index);
            if (elseIfStart > -1)
            {
                if ( termStart < 0 || elseIfStart < termStart)
                {
                    termStart = elseIfStart;
                    termEnd = FindEndingBracket(ref source, termStart, '(', ')');
                    termType = 3;
                }
            }

            if(termStart < 0)
            {
                start = -1;
                end = -1;
                type = -1;
            }
            else
            {
                start = termStart;
                end = termEnd;
                type = termType;
            }
        }

        public int FindEndingBracket(ref string text, int start, char startChar, char endChar)
        {
            int nestingLevel = 0;
            int endIndex = -1;
            //char startChar = text[start];
            for(int i = start; i < text.Length; i++)
            {
                if (text[i] == startChar)
                {
                    nestingLevel++;
                }
                else if (text[i] == endChar) 
                {
                    nestingLevel--;
                    if (nestingLevel == 0)
                    {
                        endIndex = i;
                        break;
                    }
                }
                
            }
            return endIndex;
        }

        public void GetOptionalSegments(ref List<SourceSegmentOptional> result)
        {
            if(this is SourceSegmentOptional)
            {
                result.Add((SourceSegmentOptional)this);
            }
            foreach (var segment in segments)
            {
                if(segment is SourceSegmentOptional)
                {
                    segment.GetOptionalSegments(ref result);
                }
            }
        }

        public virtual int GetOffset()
        {
            if(parentNode == null)
            {
                return 0;
            }
            return parentNode.GetSegmentOffset(this);
        }

        protected virtual int GetSegmentOffset(SourceSegment segment)
        {
            if (segment == null || !segments.Contains(segment))
            {
                return 0;
            }
            int offset = 0;
            foreach(var s in segments)
            {
                if (s == segment)
                    break;
                offset += segment.Length();
            }
            return GetOffset() + offset;
        }

        public virtual void DebugPrintStructure(int tier = 0)
        {
            string spacing = "";
            for(int i = 0; i < tier; i++)
            {
                spacing += "\t";
            }
            Console.WriteLine(spacing + "(" + this.GetType().Name + ")" + " {");
            foreach (var s in segments)
            {
                s.DebugPrintStructure(tier+1);
            }
            Console.WriteLine(spacing + "}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USerialEditor
{
    public class SourceCodeProcessor
    {
        protected SourceCodeLibrary sourceLibrary;
        protected SourceTemplateProcessor templateProcessor;
        protected ClassLibrary classLibrary;


        public SourceCodeProcessor(SourceCodeLibrary sourceLibrary, SourceTemplateProcessor templateProcessor, ClassLibrary classLibrary)
        {
            this.sourceLibrary = sourceLibrary;
            this.templateProcessor = templateProcessor;
            this.classLibrary = classLibrary;
        }



        /*public void ProcessSourceCode(string data)
        {
            this.data = data;
            ProcessSourceCode();
        }*/

        public bool ProcessAndAddSource(string path, string source)
        {
            if(sourceLibrary.GetClassSource(path) != null) 
            {
                return false;
            }
            string cleanedSource = CleanComments(source);
            SourceSegment segmentedSource = new SourceSegment(null, cleanedSource);

            sourceLibrary.AddClassSource(path, segmentedSource);
            return true;
        }

        public void ProcessAndUpdateSource(string path, string source) 
        {
            string cleanedSource = CleanComments(source);
            SourceSegment segmentedSource = new SourceSegment(null, cleanedSource);

            sourceLibrary.ReplaceClassSource(path, segmentedSource);
        }

        public bool ProcessAndAddTemplate(string path) 
        {
            SourceSegment sourceSegment = sourceLibrary.GetClassSource(path);
            if(sourceSegment == null) 
            { 
                return false;
            }
            string sourceCode = sourceSegment.GetText();
            if(sourceCode == null) 
            {
                return false;
            }
            SourceStatementParser parser = new SourceStatementParser(sourceCode);
            parser.Parse();
            List<List<string>> statements = parser.GetStatements();
            if(statements == null || statements.Count == 0)
            {
                return false;
            }
            ClassTemplate classTemplate = templateProcessor.ProcessClassTemplate(ref statements);
            if(classTemplate == null)
            {
                return false;
            }
            if(classTemplate.IsPartialTemplate())
            {
                classLibrary.AddPartialClassTemplate(classTemplate.GetName(), classTemplate);
                return true;
            }
            return classLibrary.AddClassTemplate(classTemplate.GetName(), classTemplate);
        }

        public bool ProcessAndUpdateTemplate(string path) 
        {
            SourceSegment sourceSegment = sourceLibrary.GetClassSource(path);
            if (sourceSegment == null)
            {
                return false;
            }
            string sourceCode = sourceSegment.GetText();
            if (sourceCode == null)
            {
                return false;
            }
            SourceStatementParser parser = new SourceStatementParser(sourceCode);
            parser.Parse();
            List<List<string>> statements = parser.GetStatements();
            if (statements == null || statements.Count == 0)
            {
                return false;
            }
            ClassTemplate classTemplate = templateProcessor.ProcessClassTemplate(ref statements);
            if (classTemplate == null)
            {
                return false;
            }
            if (classTemplate.IsPartialTemplate())
            {
                classLibrary.AddPartialClassTemplate(classTemplate.GetName(), classTemplate);
                return true;
            }
            classLibrary.ReplaceClassTemplate(classTemplate.GetName(), classTemplate);
            return true;
        }

        public void ReloadClassTemplates()
        {
            List<string> paths = sourceLibrary.GetClassSourceNames();
            foreach (string path in paths) 
            {
                ProcessAndAddTemplate(path);
            }
            classLibrary.ComposePartialClasses();
            classLibrary.LinkClasses();
            classLibrary.LinkStructs();
            classLibrary.ResolveCustomTypes();
        }

      //  public void Link

        /*public void ProcessSourceCode()
        {
            CleanComments();

            ProcessOptionalSegments();
            
            SourceStatementParser statementParser = new SourceStatementParser(data);
            statementParser.Parse();
            statements = statementParser.GetStatements();

            SourceTemplateProcessor templateProcessor = new SourceTemplateProcessor(propertyFactory);
            ClassTemplate classTemplate = templateProcessor.ProcessClassTemplate(ref statements);

            
            UProperty testClass = propertyFactory.BuildClass(classTemplate);
            Console.WriteLine("Wow! Donezo!");
        }*/

        /*public void ProcessOptionalSegments()
        {
            segmentedDataTree = new SourceSegment(null, data);
            //segmentedDataTree.DebugPrintStructure();
            data = segmentedDataTree.GetText();
            segmentedDataTree.GetOptionalSegments(ref optionalSegments);
            //Console.WriteLine(optionalSegments.Count);
        }*/



        /*public void ParseTerms()
        {
            while(currentByte < data.Length)
            {
                string term = GetNextTerm();
                if(term != "")
                    terms.Add(term);
            }
            //DebugPrintTerms();
        }

        public void BuildStatements()
        {
            int index = 0;
            int statementStart = -1;
            int statementEnd = -1;
            string startTerm = "";
            List<string> newStatement = new List<string>();
            while (index < terms.Count)
            {
                if (statementStart < 0 && terms[index] != "\r" && terms[index] != "\n")
                {
                    statementStart = index;
                    startTerm = terms[statementStart];
                }
              
                if (startTerm == "class"
                    || startTerm == "interface"
                    || startTerm == "var"
                    || startTerm == "enum"
                    || startTerm == "struct")
                    //|| startTerm == "native")
                {
                    if (terms[index] == ";")
                    {
                        statementEnd = index;
                        for (int i = statementStart; i <= statementEnd; i++)
                        {
                            if (terms[i] != "\r" && terms[i] != "\n")
                            newStatement.Add(terms[i]);
                        }
                        statements.Add(newStatement);
                        statementStart = -1;
                        statementEnd = -1;
                        newStatement = new List<string>();
                        startTerm = "";
                    }
                }
                else if(startTerm == "`define")
                {
                    if (terms[index] == "\r" || terms[index] == "\n")
                    {
                        statementEnd = index;
                        for (int i = statementStart; i <= statementEnd; i++)
                        {
                            if (terms[i] != "\r" && terms[i] != "\n")
                                newStatement.Add(terms[i]);
                        }
                        statements.Add(newStatement);
                        statementStart = -1;
                        statementEnd = -1;
                        newStatement = new List<string>();
                        startTerm = "";
                    }
                }
                else if(startTerm != "") 
                {
                    if ((terms[index].First() == '{' && !(index + 1 < terms.Count && terms[index+1] == ";"))
                        || terms[index] == ";")// && terms[index].Last() == '}')
                    {
                        statementEnd = index;
                        for (int i = statementStart; i <= statementEnd; i++)
                        {
                            if (terms[i] != "\r" && terms[i] != "\n")
                                newStatement.Add(terms[i]);
                        }
                        statements.Add(newStatement);
                        statementStart = -1;
                        statementEnd = -1;
                        newStatement = new List<string>();
                        startTerm = "";
                    }
                }
                index++;
            }
            

        }*/

       /* public void DebugPrintTerms()
        {
            Console.WriteLine("PARSED TERMS:\n");
            for (int i = 0; i < terms.Count; i++)
            {
                Console.WriteLine("|||terms["+i+"]|||");
                Console.WriteLine("\""+terms[i]+"\"");
            }
        }

        public bool IsWhitespace(char character)
        {
            if(character == ' '
                || character == '\t'
                || character == '\n'
                || character == '\r') 
            {
                return true;
            }
            return false;
        }

        public bool IsBracketOpener(char character) 
        {
            if(character == '['
                || character == '<' 
                || character == '('
                || character == '{')
            {
                return true;
            }
            return false;
        }

        public bool IsClosingBracketTo(char character, char opener)
        {
            if((character == ']' && opener == '[')
                || (character == '}' && opener == '{')
                || (character == ')' && opener == '(')
                || (character == '>' && opener == '<'))
            { 
                return true;
            }
            return false;
        }*/

        public string CleanComments(string input)
        {
            string data = input;
            int removeStart = 0;
            int removeEnd = 0;

            int index = 0;

            char currentChar = ' ';
            char prevChar = ' ';

            int commentType = 0; //0 = No comment, 1 = Line comment, 2 = Block comment

            while(index < data.Length)
            {
                prevChar = currentChar;
                currentChar = data[index];
                if(commentType == 0)
                {
                    if(prevChar == '/' && currentChar == '/') //New line comment
                    {
                        commentType = 1;
                        removeStart = index - 1;
                    }
                    else if(prevChar == '/' && currentChar == '*') //New block comment
                    {
                        commentType = 2;
                        removeStart = index - 1;
                    }
                }
                else if(commentType == 1) //Line comments
                {
                    if(currentChar == '\n' || currentChar == '\r' || index == data.Length - 1)
                    {
                        removeEnd = index + 1;
                        data = data.Remove(removeStart, removeEnd - removeStart);
                        index -= (removeEnd - removeStart - 2);
                        commentType = 0;
                        continue;
                    }
                }
                else if(commentType == 2) //Block comments
                {
                    if(prevChar == '*' && currentChar == '/')
                    {
                        removeEnd = index + 1;
                        data = data.Remove(removeStart, removeEnd - removeStart);
                        index -= (removeEnd - removeStart - 1);
                        commentType = 0;
                        continue;
                    }
                }
                index++;
            }
            return data;

        }

       /* public string GetNextTerm()
        {
            string result = "";
            int termStart = -1;
            int termEnd = -1;
            int bracketLevel = 0;
            char bracketType = '-';
            char currentChar;
            int index = currentByte;
            while(index < data.Length)
            {
                currentChar = (char)data[index];
                if (termStart < 0)
                {
                    if (currentChar == ';')
                    {
                        currentByte = index + 1;
                        return ";";
                    }
                    else if(currentChar == '\n')
                    {
                        currentByte = index + 1;
                        return "\n";
                    }
                    else if(currentChar == '\r')
                    {
                        currentByte = index + 1;
                        return "\r";
                    }
                    if (!IsWhitespace(currentChar) && currentChar != ',')
                    {
                        termStart = index;
                    }
                    if (IsBracketOpener(currentChar))
                    {
                        bracketType = currentChar;
                        bracketLevel++;
                    }
                }
                else
                {
                    if (IsBracketOpener(currentChar) && bracketLevel > 0)
                    {
                        if (currentChar == bracketType)
                        {
                            bracketLevel++;
                        }
                    }
                    else if (bracketLevel > 0 && IsClosingBracketTo(currentChar,bracketType))
                    {
                        bracketLevel--;
                        if(bracketLevel <= 0)
                        {
                            termEnd = index;
                            break;
                        }
                    }
                    if(bracketLevel <= 0)
                    {
                        if(currentChar == ','
                            || currentChar == ';'
                            || IsWhitespace(currentChar)
                            || IsBracketOpener(currentChar))
                        {
                            termEnd = index - 1;
                            break;
                        }
                    }
                }
                termEnd = index++;
            }
            if(termStart > -1 && termEnd > -1 && termEnd >= termStart)
            {
                result = data.ToString().Substring(termStart, termEnd - termStart + 1);
            }
            else
            {
                result = "";
            }
            currentByte = termEnd + 1;
            return result;
        }*/

       /* public List<string> GetTerms()
        {
            return terms;
        }
       */
       /* public List<List<string>> GetStatements()
        {
            return statements;
        }
       
        public string GetData()
        {
            return data;
        }*/
    }
}

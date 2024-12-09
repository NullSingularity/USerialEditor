using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//SourceStatementParser, originally part of SourceCodeParser, performs the function of parsing given source code into terms, then into discrete statements
//  that can then be interpreted by other classes and used to build the class model.

namespace USerialEditor
{
    public class SourceStatementParser
    {
        string data;
        List<string> terms;             //Each "term" that has been parsed from the data, in order. 
                                        //Terms will generally be individual words (delineated by whitespace, or punctuation such as commas) , or portions of the text encapsulated in (), [], <>, or {}.
        List<List<string>> statements;  //Collections of terms, such that each forms an individual statement - such as a variable declaration or a struct definition.
        int currentByte;        //Persistent index for the GetNextTerm() method

        public SourceStatementParser()
        {
            data = string.Empty;
            terms = new List<string>();
            statements = new List<List<string>>();
        }
        public SourceStatementParser(string data)
        {
            this.data = data;
            terms = new List<string>();
            statements = new List<List<string>>();
        }

        public void Parse(string data)
        {
            this.data = data;
            this.terms = new List<string>();
            this.statements = new List<List<string>>();
            Parse();
        }

        public void Parse()
        {
            ParseTerms();
            BuildStatements();
        }

        public void ParseTerms()
        {
            currentByte = 0;
            while (currentByte < data.Length)
            {
                string term = GetNextTerm();
                if (term != "")
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
                else if (startTerm == "`define")
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
                else if (startTerm != "")
                {
                    if ((terms[index].First() == '{' && !(index + 1 < terms.Count && terms[index + 1] == ";"))
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




        }
        public string GetNextTerm()
        {
            string result = "";
            int termStart = -1;
            int termEnd = -1;
            int bracketLevel = 0;
            char bracketType = '-';
            char currentChar;
            int index = currentByte;
            while (index < data.Length)
            {
                currentChar = (char)data[index];
                if (termStart < 0)
                {
                    if (currentChar == ';')
                    {
                        currentByte = index + 1;
                        return ";";
                    }
                    else if (currentChar == '\n')
                    {
                        currentByte = index + 1;
                        return "\n";
                    }
                    else if (currentChar == '\r')
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
                    else if (bracketLevel > 0 && IsClosingBracketTo(currentChar, bracketType))
                    {
                        bracketLevel--;
                        if (bracketLevel <= 0)
                        {
                            termEnd = index;
                            break;
                        }
                    }
                    if (bracketLevel <= 0)
                    {
                        if (currentChar == ','
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
            if (termStart > -1 && termEnd > -1 && termEnd >= termStart)
            {
                result = data.ToString().Substring(termStart, termEnd - termStart + 1);
            }
            else
            {
                result = "";
            }
            currentByte = termEnd + 1;
            return result;
        }

        public List<string> GetTerms()
        {
            return terms;
        }

        public List<List<string>> GetStatements()
        {
            return statements;
        }

        public string GetData()
        {
            return data;
        }

        public void DebugPrintTerms()
        {
            Console.WriteLine("PARSED TERMS:\n");
            for (int i = 0; i < terms.Count; i++)
            {
                Console.WriteLine("|||terms[" + i + "]|||");
                Console.WriteLine("\"" + terms[i] + "\"");
            }
        }

        public bool IsWhitespace(char character)
        {
            if (character == ' '
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
            if (character == '['
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
            if ((character == ']' && opener == '[')
                || (character == '}' && opener == '{')
                || (character == ')' && opener == '(')
                || (character == '>' && opener == '<'))
            {
                return true;
            }
            return false;
        }
    }
}

/*
Sqlmake http://code.google.com/p/sqlmake/
Copyright © 2010-2012 Mitja Golouh 
  
This file is part of Sqlmake.

Sqlmake is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as 
published by the Free Software Foundation, either version 3 of 
the License, or (at your option) any later version.

Sqlmake is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Sqlmake.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using SQLMake.Util;
using SQLMake.Parser;
using SQLMake.Dao;

namespace SQLMake
{

// End of file 
// Raised when end of input stream is reached
class EOFException : System.Exception { } 

// End of SQL*Plus execution block (single command)
// Every sql*plus script is divided into execution blocks: 
//   - SQL Plus command
//   - SQL command
//   - PLSQL command
//   - wrapped PLSQL command
//   - SQL Plus start command
// Block detection is happening on the fly. Reading a fresh block starts as Command = Unknonwn
// When enough tokens are read it automatically recognises content and switches to one of predefined command types.
// When end of the block is reached EOB exception is raised
// Different blocks end in a different way, eg. PLSQL block ends with "/"
class EOBException : System.Exception { } 


// While autodetecting block content some of the SQL syntax rule are checked
// Any syntax anomaly detected can raise an exception
// This behaviour is controlled by setting: RaiseExceptionWhenSyntaxErrorFoundFlag
class SyntaxErrorFoundException : System.Exception { } // Unknown command found during autodetection of block type

public enum CommandTypes
{
    Unknown = 0,
    SqlPlus = 1,
    Sql = 2,
    Plsql = 3,
    PeekMode = 4,
    WrappedPlsql = 5,
    SqlPlusStart = 6,
}

// Describes scanner state
// Lots of logic depends on this 
public enum AutodetectMode
{
    Normal = 0,
    Detour = 1,
    ObjectName = 2,
    SecondaryObjectName = 3,
    ObjectNameList = 4,
    CommandNameList = 5,
    SecondaryObjectNameList = 6
}

public class SQLPlusScanner
{
    private TextReader inputReader;  // source text reader (sql script)
    private string inputReaderDesc;  // desc of input text eg. filename (used when logging messages, so correlation between error and to correct filename can be made)
    private string currLineBuffer;   // buffer with unparsed text of the current line (used internally)
    public int currColIndex;         // last read character position eg 0=no character read, 1=first character read .. (used internally)
    public int currLineIndex;        // line number that is currently in the currLine buffer (used internally)
    public int currLineLength;       // length of the current line buffer (used internally)

    public CommandTypes prevBlockType;
    public CommandObject currCommand;       // Metadata about current command block

    // --> we should move thise inside CommandObject
    public StringBuilder prevBlockText;     // unparsed/unmodified text of previous command block
    public StringBuilder currBlockText;     // unparsed/unmodified text of current command block
    public int blockStartLine;              // zero based start of the current command block   

    public int tokenCountWithoutComments;   // current number of tokens in execution block, excluding comments
    private int commentCount;                // current number of stand alone comments 
                                             // relevant only in non sqlplus mode

    public string token;                    // current token eg. keyword, literal
    public TokenTypes tokenType;            // eg. Unknown, Identifier, Literal, Integer,  Number,  LineComment, ...
    public int tokenStartLineIndex;         // Zero based line number where token starts
    public int tokenStartColIndex;          // Zero based col number where token starts

    public string sqlPlusKeyword;           // extension to token; sqlplus keywords in sql scripts can be abberivated
                                            // eg. token = A or AP or APP and sqlPlusKeyword is automatically populated 
                                            // with correct sql plus keyword APPEND
    private string prevToken;               // previous token (used internally)


    // For SQL*Plus variable substitution
    private const char substChar = '&';
    private bool substitutionDone = false;     // internal variable, when line is read into buffer it is set to false, after substitution is done this flag changes to true
    private bool skipSubstitution = false;     // internal variable, changes to true depending on what kind of script segment we are reading
                                               // we should probably remove this variable as it conflicts sql*plus compatibilty
    public  bool disableSubstitution = !Settings.getSqlPlusVariableSubstitutionFlag(false); // manually disable substitution

    // There are some subtle differences between sqlplus and sqlmake eg. if you put two select statements on the same line
    // select * from dual; select * from dual;
    // Sqlmake while process it as two command blocks, while sqlplus while raise an error
    // This is just a placeholder, it is still awaiting implementation
    public bool SQLPlusCompatible = false;

    //
    private AutodetectMode autodetectMode;
    private int detourTaken;
    private int detourStart;
    private string detourPath;
 
    Hashtable sqlPlusKeywords = new Hashtable();   // list of known sql*plus keywords

    private string listSeparator; // used in Autodetect List mode
    private Dictionary<String,String> listTerminatorTokenHashSet; // used in Autodetect List mode

    public void raiseSyntaxErrorFoundException(string errorText)
    {
        currCommand.syntaxErrorFound = true;
        if (Settings.getRaiseExceptionWhenSyntaxErrorFoundFlag(true)) throw new SyntaxErrorFoundException();
    }

    public string getModeDesc()
    {
        switch (currCommand.cmdType)
        {
            case CommandTypes.Unknown: return "Unknown command";
            case CommandTypes.SqlPlus: return "SQLPlus command";
            case CommandTypes.Sql: return "SQL command";
            case CommandTypes.Plsql: return "PLSQL block";
            case CommandTypes.WrappedPlsql: return "Wrapped PLSQL block";
            case CommandTypes.SqlPlusStart: return "SQLPlus start another script command";                
            default: return "Unknown command"; 
        }
    }

    
    public SQLPlusScanner(TextReader inputReader, string inputReaderDesc)
    {
        SQLMakeConstants.initializeSqkPlusKeywords(sqlPlusKeywords);
        this.inputReader = inputReader;
        this.inputReaderDesc = inputReaderDesc;
        currLineLength = -1;
        currColIndex = 0;
        currLineIndex = -1;
        tokenStartColIndex = -1;

        currBlockText = new StringBuilder();
        prevBlockText = new StringBuilder();
        currCommand = new CommandObject();

        resetBlockType();
    }

    public void resetBlockType()
    {
        prevBlockType = currCommand.cmdType;
        prevBlockText = currBlockText;
        currCommand.cmdType = CommandTypes.Unknown;
        currBlockText = new StringBuilder();

        prevToken = "";
        token = "";

        currCommand.action = "";
        currCommand.baseCmdName = "";
        currCommand.cmdName = "";
        currCommand.objectName = "";
        currCommand.alterType = "";
        currCommand.secondaryObjectName = "";
        currCommand.secondaryCmdName = "";
        currCommand.syntaxErrorFound = false;

        tokenCountWithoutComments = 0;
        blockStartLine = -1;

        autodetectMode = AutodetectMode.Normal;
        detourTaken = 0;
        detourStart = -1;
        detourPath = "";
    }


    public void nextTokenWillBeObjectName()
    {
        autodetectMode = AutodetectMode.ObjectName;
    }

    public void nextTokenWillBeObjectNameList()
    {
        autodetectMode = AutodetectMode.ObjectNameList;
        listSeparator = ",";
        listTerminatorTokenHashSet = new Dictionary<string,string>();
    }

    public void nextTokenWillBeSecondaryObjectNameList()
    {
        autodetectMode = AutodetectMode.SecondaryObjectNameList;
        listSeparator = ",";
        listTerminatorTokenHashSet = new Dictionary<string,string>();
    }

    
    public void nextTokenWillBeCommandNameList()
    {
        nextTokenWillBeCommandNameList(",");
    }

    public void nextTokenWillBeCommandNameList(string listSeparator)
    {
        autodetectMode = AutodetectMode.CommandNameList;
        this.listSeparator = listSeparator;
        listTerminatorTokenHashSet = new Dictionary<string,string>();
    }


    public void nextTokenWillBeSecondaryObjectName()
    {
        autodetectMode = AutodetectMode.SecondaryObjectName;
        listSeparator = ",";
        listTerminatorTokenHashSet = new Dictionary<string,string>();
    }

    public void setCurrCmdType(CommandTypes newCmdType)
    {
        currCommand.cmdType = newCmdType;
        //System.Console.WriteLine("currCommand.cmdType = {0}", currCommand.cmdType);
    }

    public void setBaseCmdName(string newBaseCmdName)
    {
        currCommand.baseCmdName = newBaseCmdName;
        //System.Console.WriteLine("currCommand.baseCmdName = {0}", newBaseCmdName);
    }

    
    public void appendCurrCmdName(string newCmdName)
    {
        if (currCommand.cmdName == "") currCommand.cmdName = newCmdName;
        else currCommand.cmdName = currCommand.cmdName + " " + newCmdName;
        //System.Console.WriteLine("currCommand.cmdName = {0}", currCommand.cmdName);
    }


    public void appendCurrCmdNameAsCsvList(string newCmdName)
    {
        if (currCommand.cmdName != "") currCommand.cmdName += ",";
        currCommand.cmdName += newCmdName;
    }

    public void appendSecondaryObject(string name)
    {
        if (currCommand.secondaryObjectName != "") currCommand.secondaryObjectName += " ";
        currCommand.secondaryObjectName += name;
    }
    
    public void appendSecondaryObjectAsCsvList(string name)
    {
        if (currCommand.secondaryObjectName != "") currCommand.secondaryObjectName += ",";
        currCommand.secondaryObjectName += name;
    }


    public void setCurrCmdTypeAndAppendCmdName(CommandTypes newCmdType, string newCmdName)
    {
        setCurrCmdType(newCmdType);
        appendCurrCmdName(newCmdName);
    }

    public void setObjectName(string newObjectName)
    {
        currCommand.objectName = newObjectName;
    }

    public void appendObjectName(string newObjectName)
    {
        if (currCommand.objectName != "") currCommand.objectName += " ";
        currCommand.objectName += newObjectName;
    }

    private string getValueOfSubstituteVariable(string variable)
    {
        Log.Verbose("scanner", "Found variable {0}", variable);

         // Console.Write("Enter value for variable {0} ({1}, Line no: {2}): ", realVariableName.ToUpper(), inputReaderDesc, currLineIndex);
        return Settings.getSqlplusVariable(variable);
    }

    // Load one line to buffer and decode all substitution variables
    // return false if the source file has reached EOF
    private bool loadBuffer()
    {
        currLineBuffer = inputReader.ReadLine();
        if (currLineBuffer == null) return false;

        currLineIndex++;
        currLineBuffer += '\n';
        currLineLength = currLineBuffer.Length;
        currColIndex = 0; 
        substitutionDone = false;

        return true;
    }

    private void expandSubstitutionVariablesInBuffer()
    {
        string substitutedLine = currLineBuffer.Substring(0, currColIndex);

        for (int i = currColIndex; i < currLineLength; i++)
        {
            if (currLineBuffer[i] == substChar)
            {
                string substVariableName = "";
                while (!(i == currLineLength || 
                         Char.IsWhiteSpace(currLineBuffer[i]) || 
                         currLineBuffer[i] == '.' ||
                         currLineBuffer[i] == ';' ||
                         currLineBuffer[i] == '\'' ||
                         currLineBuffer[i] == ')' ||
                         currLineBuffer[i] == '(' ||
                         currCommand.cmdType == CommandTypes.SqlPlus && currLineBuffer[i] == '-' && currLineBuffer[i + 1] == '\n')
                      )
                {
                    substVariableName += currLineBuffer[i];
                    i++;
                }
                substitutedLine += getValueOfSubstituteVariable(substVariableName);
                if (!(i == currLineLength || currLineBuffer[i] == '.')) substitutedLine += currLineBuffer[i]; 
            }
            else substitutedLine = substitutedLine + currLineBuffer[i];
        }

        currLineBuffer = substitutedLine;
        currLineLength = currLineBuffer.Length;
        substitutionDone = true;
    }

    // peek into buffered line, does not move current column index
    // LL is Look ahead Level - peek character at position currColIndex + LL
    private char peekChar(int LL)
    {
        System.Diagnostics.Debug.Assert((LL >= -1 && LL <= 2), "Expected input for LL is between -1 and 2");

        // check if currentLine was loaded
        if (currLineBuffer == null) loadBuffer();
        if (currLineBuffer == null) return '\n';

        // loadbuffer returns false if it reaches end of file
        // but we ignore it due to the fact we are using peek
        // see hack with \n
     
        // we load only one line using peek
        // it is possible that large LL goes over this line
        // but currently there is no business logic to implement this issue

        // TODO : Check if condition is correct
        // HACK : Can something go wrong with \n ?
        if (currColIndex + LL >= currLineLength || currColIndex + LL < 0) return '\n';


        // if we step on substitution character, call expand susbtitution variables
        if (currLineBuffer[currColIndex + LL] == substChar && currCommand.cmdType != CommandTypes.Unknown && !substitutionDone && !skipSubstitution && !disableSubstitution)
        {
            expandSubstitutionVariablesInBuffer();
        }
        
        return currLineBuffer[currColIndex + LL];
    }

    // Peek current char in buffer
    private char peekChar()
    {
        return peekChar(0);
    }

    private char getChar(bool ignoreEOB)
    {
        // empty buffer or current column index at the end of current buffer
        if (currLineBuffer == null || currColIndex >= currLineLength) 
        { 
            if (!loadBuffer()) throw new EOFException(); 
        }

        // if we step on substitution character, call expand susbtitution variables
        if (currLineBuffer[currColIndex] == substChar && currCommand.cmdType != CommandTypes.Unknown && !substitutionDone && !skipSubstitution && !disableSubstitution)
        {
            expandSubstitutionVariablesInBuffer();
        }

        // locate EOB marker
        // handle slash '/', which marks end of block (except in SQLPlus block type)
        if (currCommand.cmdType != CommandTypes.SqlPlus &&
            currLineBuffer[currColIndex] == '/' &&
            currLineBuffer.Trim() == "/" &&
            !ignoreEOB) 
        {
            if (tokenCountWithoutComments > 0)
            {
                skipSubstitution = false;
                ++currColIndex;
                throw new EOBException();
            }
            // slash indicating repeat of last command on the stack
            // we report it as sqlplus keyword RUN
            else
            {
                skipSubstitution = false;
                ++currColIndex;
                setCurrCmdType(CommandTypes.SqlPlus);
                token = "RUN";
                tokenType = TokenTypes.Identifier;
                tokenCountWithoutComments = 1;
                sqlPlusKeyword = "RUN";
                currBlockText.Append("/");
                currCommand.action = "RUN";
                throw new EOBException();
            }
        }

        // locate EOB marker
        if (!ignoreEOB)
        {
            // handle semicolumn ';', which marks end of block 
            if (currCommand.cmdType == CommandTypes.Sql && currLineBuffer[currColIndex] == ';') { skipSubstitution = false; ++currColIndex; throw new EOBException(); }
            if ((currCommand.cmdType == CommandTypes.SqlPlus || currCommand.cmdType == CommandTypes.SqlPlusStart) && currLineBuffer[currColIndex] == ';' && peekChar(1) == '\n') { skipSubstitution = false; ++currColIndex; throw new EOBException(); }
            // handle '-' , which marks continuation of current sqlplus command in next line
            if ((currCommand.cmdType == CommandTypes.SqlPlus || currCommand.cmdType == CommandTypes.SqlPlusStart) && currLineBuffer[currColIndex] == '-' && peekChar(1) == '\n') { if (!loadBuffer()) throw new EOFException(); currBlockText.Append("-\n"); return getChar(); }
            // handle new line which marks EOB of current sqlplus command 
            if ((currCommand.cmdType == CommandTypes.SqlPlus || currCommand.cmdType == CommandTypes.SqlPlusStart) && currLineBuffer[currColIndex] == '\n') { skipSubstitution = false; throw new EOBException(); }
        }

        char result = currLineBuffer[currColIndex++];
        if (currColIndex >= currLineLength) loadBuffer();
        currBlockText.Append(result);
        return result;
    }

    private char getChar()
    {
        return getChar(false);
    }
    
    public string getLine()
    {
        if (currColIndex >= currLineLength) if (!loadBuffer()) throw new EOFException();

        if (currCommand.cmdType != CommandTypes.SqlPlus &&
            currLineBuffer[currColIndex] == '/' &&
            currLineBuffer.Trim() == "/") 
        {
            skipSubstitution = false;
            ++currColIndex;
            throw new EOBException();
        }

        currBlockText.Append(currLineBuffer.Substring(currColIndex));
        int rememberCurrColIndex = currColIndex;
        currColIndex = currLineLength; // this triggers load buffer on next call to getXYZ
        return currLineBuffer.Substring(rememberCurrColIndex);
    }
    
    public string getLineFromStartOfLastToken(){
        System.Diagnostics.Debug.Assert(currLineIndex == tokenStartLineIndex, "Token spans over more than one line");
        if (currColIndex >= currLineLength) if (!loadBuffer()) throw new EOFException();

        if (currCommand.cmdType != CommandTypes.SqlPlus &&
            currLineBuffer[currColIndex] == '/' &&
            currLineBuffer.Trim() == "/")
        {
            skipSubstitution = false;
            ++currColIndex;
            throw new EOBException();
        }

        currBlockText.Append(currLineBuffer.Substring(currColIndex));
        currColIndex = currLineLength; // this triggers load buffer on next call to getXYZ
        
        return currLineBuffer.Substring(tokenStartColIndex);
    }


    // helper subroutine to read qouted identifier 
    private void getQuotedIdentifier()
    {
        System.Diagnostics.Debug.Assert(peekChar()=='\"', "Expected peekChar = \"");
        try
        {
            do { token = token + getChar(); } while (peekChar() != '\"');
            token = token + getChar();
        }
        catch (EOFException)
        {
            Log.Warning("scanner", "Incorrectly terminated quoted identifier in {0} starting from line [{1}, {2}] until EOF", inputReaderDesc, tokenStartLineIndex + 1, tokenStartColIndex + 1);
        }
        catch (EOBException)
        {
            Log.Warning("scanner", "Incorrectly terminated quoted identifier in {0} starting from line [{1}, {2}] until EOB at [{3}, {4}]", inputReaderDesc, tokenStartLineIndex + 1, tokenStartColIndex + 1, currLineIndex + 1, currColIndex);
        }
    }

    // helper subroutine to read nonqouted identifier 
    private void getNonQuotedIdentifier()
    {
        System.Diagnostics.Debug.Assert(Char.IsLetter(peekChar()), "Expected peekChar should be character");
        String t = "";
        try
        {
            do { t = t + getChar(); }
            while (Char.IsLetter(peekChar()) ||
                   Char.IsNumber(peekChar()) ||
                   peekChar() == '$' ||
                   peekChar() == '_' ||
                   peekChar() == '#');
        }
        catch (EOFException)
        {
        }
        catch (EOBException)
        {
        }
        token = token + t.ToUpper();
    }

    // helper subroutine to read nonqouted identifier allowing dot (to read filename for example)
    private void getNonQuotedIdentifierAllowDot()
    {
        System.Diagnostics.Debug.Assert(Char.IsLetter(peekChar()), "Expected peekChar should be character");
        String t = "";
        try
        {
            do { t = t + getChar(); }
            while (Char.IsLetter(peekChar()) ||
                   Char.IsNumber(peekChar()) ||
                   peekChar() == '$' ||
                   peekChar() == '_' ||
                   peekChar() == '#' ||
                   peekChar() == '.');
        }
        catch (EOFException)
        {
        }
        catch (EOBException)
        {
        }
        token = token + t.ToUpper();
    }

    public void readNextBlock()
    {
        try
        {
            while (true) { get(); }
        }
        catch (EOFException)
        {
        }
        catch (EOBException)
        {
        }
    }

    public string get()
    {
        prevToken = token;
        token = "";
        tokenType = TokenTypes.NotAvailable;
    
        // Locate the startof the first keyword
        while (Char.IsWhiteSpace(peekChar()))
        {
            getChar();
        }

        tokenStartColIndex = currColIndex;
        tokenStartLineIndex = currLineIndex;
        if (blockStartLine == -1) blockStartLine = currLineIndex;
        if (currColIndex >= currLineLength) { throw new EOFException(); }


        // wrapped mode takes whole block as a token until EOB
        //********************************************************
        if (currCommand.cmdType == CommandTypes.WrappedPlsql)
        {
            // We have to use stringBuilder, because string is too slow
            StringBuilder sb = new StringBuilder();
            try
            {
                do sb.Append(getLine());
                while (true);
            }
            catch (EOBException)
            {
                token = sb.ToString();
                currColIndex = 0;
            }

            // if we dont change mode back, we end up in endless loop
            if (currCommand.baseCmdName == "TYPE") setCurrCmdType(CommandTypes.Sql);
            else setCurrCmdType(CommandTypes.Plsql);

            tokenType = TokenTypes.WrappedCode;
            token = token.TrimEnd();
            tokenCountWithoutComments++;
            return token;
        }

        // with sqlplus START everything until the EOB is returned as single token
        //********************************************************
        if (currCommand.cmdType == CommandTypes.SqlPlusStart || currCommand.cmdType == CommandTypes.SqlPlus && currCommand.action == "SPOOL")
        {
            try
            {
                do token = token + getChar();
                while (peekChar()!= ' '); 
            }
            catch (EOFException)
            {
            }
            catch (EOBException)
            {
            }

            tokenType = TokenTypes.Identifier;
            tokenCountWithoutComments++;
            return token;        
        }

        // with sqlplus DOC 
        //********************************************************
        if (currCommand.cmdType == CommandTypes.SqlPlus &&
            currCommand.action == "DOC")
        {
            while (currLineBuffer.Trim() != "#")
            {
                token = token + getChar(true);
            }
            getChar();
            tokenType = TokenTypes.DocComment;
        }

        // Quoted literal
        //********************************************************
        if (peekChar() == '\'' ||
            Char.ToUpper(peekChar()) == 'N' && peekChar(1) == '\'' 
           )
        { 
            try
            {
                // literal using national characterset
                if (Char.ToUpper(peekChar()) == 'N')
                {
                    token = token + getChar();
                }
                token = token + getChar();

                while ((peekChar() != '\'' || peekChar() == '\'' && peekChar(1) == '\'') )
                {                
                    // two single quotation marks
                    if (peekChar() == '\'') 
                    {
                        token = token + getChar();
                    }
                    token = token + getChar(true);                             
                }
                token = token + getChar();

            }
            catch (EOFException)
            {
                // TODO Tole bi moralo narediti reraise exception
                Log.Warning("scanner", "Incorrectly terminated literal in {0} starting from line [{1}, {2}] until EOF", inputReaderDesc, tokenStartLineIndex + 1, tokenStartColIndex + 1);
            }
            catch (EOBException)
            {
                // TODO Tole bi moralo narediti reraise exception
                Log.Warning("scanner", "Incorrectly terminated literal in {0} starting from line [{1}, {2}] until EOB at [{3}, {4}]", inputReaderDesc, tokenStartLineIndex + 1, tokenStartColIndex + 1, currLineIndex + 1, currColIndex);
            }
            finally
            {
                tokenType = TokenTypes.Literal;
            }
        }

        // Quoted literal with quota delimiter
        //********************************************************
        else if (Char.ToUpper(peekChar()) == 'Q' && peekChar(1) == '\'' ||
            Char.ToUpper(peekChar()) == 'N' && peekChar(1) == 'Q' && peekChar(2) == '\''
           )
        { 
            // using national characterset
            if (Char.ToUpper(peekChar()) == 'N')
            {
                token = token + getChar();
            }

            token = token + getChar(); // read 'Q'
            token = token + getChar(); // read single quote
            char quoteDelimiter = getChar(); // reade quote delimiter
            token = token + quoteDelimiter;

            while (!(peekChar() == quoteDelimiter && peekChar(1) == '\''))
            {
                token = token + getChar();
            }
            token = token + getChar() + getChar();

            tokenType = TokenTypes.Literal;            
        }

        // Precompiler directives start with $
        else if (Char.IsLetter(peekChar()) || peekChar() == '\"' || peekChar() == '$')
        {
            // Identifier can be [schema.]object_name[.part][@database_link]

            if (peekChar() == '\"') getQuotedIdentifier();
            else getNonQuotedIdentifier();

            if (peekChar() == '.')
            {
                token = token + getChar();
                if (peekChar() == '\"') getQuotedIdentifier();
                else getNonQuotedIdentifier();
            }

            if (peekChar() == '.')
            {
                token = token + getChar();
                if (peekChar() == '\"') getQuotedIdentifier();
                else getNonQuotedIdentifier();
            }

            if (peekChar() == '@')
            {
                token = token + getChar();
                if (peekChar() == '\"') getQuotedIdentifier();
                else getNonQuotedIdentifierAllowDot();
            }

            tokenType = TokenTypes.Identifier;
        }

        // Number
        //********************************************************
        else if ((peekChar() == '+' || peekChar() == '-') && (peekChar(1) == '.' && Char.IsNumber(peekChar(2)) || Char.IsNumber(peekChar(1))) ||
                 (peekChar() == '.' && Char.IsNumber(peekChar(1)) || Char.IsNumber(peekChar()))
                )
        {
            bool isInteger = true;
            try
            {
                if (peekChar() == '+' || peekChar() == '-') { token = token + getChar(); }

                while (Char.IsNumber(peekChar()) ) {token = token + getChar(); }
                if (peekChar() == '.') { isInteger = false;  token = token + getChar(); }
                while (Char.IsNumber(peekChar())) { token = token + getChar(); }

                // lets check if we are using scientific notation
                if (Char.ToUpper(peekChar()) == 'E'){ 
                    isInteger = false; 
                    token = token + getChar();
                    string exp = "";
                    if (peekChar() == '+' || peekChar() == '-') { exp = exp + peekChar(); token = token + getChar(); }
                    while (Char.IsNumber(peekChar())) { exp = exp + peekChar(); token = token + getChar(); }
                    if (int.Parse(exp) < -130 || int.Parse(exp) > 125)
                    {
                        Log.Warning("scanner", "The exponent can range from -130 to 125 in {0} starting from line [{1}, {2}] to [{3}, {4}]", inputReaderDesc, tokenStartLineIndex + 1, tokenStartColIndex + 1, currLineIndex + 1, currColIndex);
                    }
                }
                
                // and also check BINARY_FLOAT and BINARY_DOUBLE operation
                if (Char.ToUpper(peekChar()) == 'F' || Char.ToUpper(peekChar()) == 'D')
                {
                    isInteger = false;
                    token = token + getChar();
                }
            }
            catch (EOFException)
            {
                
            }
            catch (EOBException)
            {

            }
            finally
            {
                if (isInteger)
                {
                    tokenType = TokenTypes.Integer;
                }
                else
                {
                    tokenType = TokenTypes.Number;
                }
            }
        }

            
        // LINE_COMMENT
        //********************************************************
        else if (peekChar() == '-' && peekChar(1) == '-')
        { 
            token = token + getChar() + getChar();
            try
            {
                while (peekChar() != '\n')
                {
                    token = token + getChar(true);
                }
            }
            catch (EOBException) { }
            finally
            {
                tokenType = TokenTypes.LineComment;
            }
        }

        // BLOCK_COMMENT
        //********************************************************
        else if (peekChar() == '/' && peekChar(1) == '*')
        { 
            try
            {
                token = token + getChar() + getChar();
                while (!(peekChar() == '*' && peekChar(1) == '/'))
                {
                    token = token + getChar(true);
                }
                token = token + getChar() + getChar();
            }
            catch (EOFException)
            {
                Log.Warning("scanner", "Incorrectly terminated block comment in {0} starting from line [{1}, {2}] until EOF", inputReaderDesc, tokenStartLineIndex + 1, tokenStartColIndex + 1);
            }
            finally
            {
                tokenType = TokenTypes.BlockComment;
            }       
        }

        else if (peekChar() == ';')
        {
            token = token + getChar();
            tokenType = TokenTypes.Semi;
        }
        else if (peekChar() == ':' && peekChar(1) == '=')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.AssignmentEQ;
        }
        else if (peekChar() == ':')
        {
            token = token + getChar();
            tokenType = TokenTypes.Colon;
        }
        else if (peekChar() == '.' && peekChar(1) == '.')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.DoubleDot;
        }
        else if (peekChar() == '.')
        {
            token = token + getChar();
            tokenType = TokenTypes.Dot;
        }
        else if (peekChar() == ',')
        {
            token = token + getChar();
            tokenType = TokenTypes.Comma;
        }
        else if (peekChar() == '*')
        {
            token = token + getChar();
            tokenType = TokenTypes.Asteriks;
        }
        else if (peekChar() == '@')
        {
            token = token + getChar();
            tokenType = TokenTypes.AtSign;

            // Some more work to extract @@
            if (token == "@" && peekChar() == '@')
            {
                token = token + getChar();
                tokenType = TokenTypes.DoubleAtSign;
            }

        }
        else if (peekChar() == '(' && peekChar(1) == '*' && peekChar(2) == ')')
        {
            token = token + getChar()+ getChar()+ getChar();
            tokenType = TokenTypes.OuterJoinOracleSyntax;
        }
        else if (peekChar() == '(')
        {
            token = token + getChar();
            tokenType = TokenTypes.OpenParenthesis;
        }
        else if (peekChar() == ')')
        {
            token = token + getChar();
            tokenType = TokenTypes.CloseParenthesis;
        }
        else if (peekChar() == '+')
        {
            token = token + getChar();
            tokenType = TokenTypes.Plus;
        }
        else if (peekChar() == '-')
        {
            token = token + getChar();
            tokenType = TokenTypes.Minus;
        }
        else if (peekChar() == '/')
        {
            token = token + getChar();
            tokenType = TokenTypes.Divide;
        }
        else if (peekChar() == '=' && peekChar(1) == '>')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.PassParameterByName;
        }
        else if (peekChar() == '=')
        {
            token = token + getChar();
            tokenType = TokenTypes.EQ;
        }
        else if (peekChar() == '%')
        {
            token = token + getChar();
            tokenType = TokenTypes.Percentage;
        }
        else if (peekChar() == '|' && peekChar(1) == '|')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.Concat;
        }
        else if (peekChar() == '|')
        {
            token = token + getChar();
            tokenType = TokenTypes.VerticalBar;
        }
        else if (peekChar() == '<' && peekChar(1) == '<')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.OpenLabele;
        }
        else if (peekChar() == '>' && peekChar(1) == '>')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.CloseLabele;
        }
        else if (peekChar() == '<' && peekChar(1) == '>' ||
                 peekChar() == '!' && peekChar(1) == '=' ||
                 peekChar() == '^' && peekChar(1) == '>' 
                )
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.NotEQ;
        }
        else if (peekChar() == '<' && peekChar(1) == '=')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.LessThanOrEqual;
        }
        else if (peekChar() == '<' )
        {
            token = token + getChar();
            tokenType = TokenTypes.LessThan;
        }
        else if (peekChar() == '>' && peekChar(1) == '=')
        {
            token = token + getChar() + getChar();
            tokenType = TokenTypes.GreaterThanOrEqual;
        }
        else if (peekChar() == '>')
        {
            token = token + getChar();
            tokenType = TokenTypes.GreaterThan;
        }
        else if (disableSubstitution && peekChar() == '&')
        {
            token = token + getChar();
            try
            {
                while (!(Char.IsWhiteSpace(peekChar()) || peekChar() == '.' || peekChar() == ';' || currCommand.cmdType == CommandTypes.SqlPlus && peekChar() == '-' && peekChar(1) == '\n'))
                {
                    token = token + getChar();
                }
            }
            finally
            {
                tokenType = TokenTypes.SubstitutionVariableIndicator;
            }
        }
        else
        {
            // Locate the start of the next token by skipping until next whitespace
            while (!Char.IsWhiteSpace(peekChar())) { token = token + getChar(); }
            tokenType = TokenTypes.Unknown;

            Log.Warning("scanner", "Unknown token \"{5}\" in {0} starting from line [{1}, {2}] until EOB at [{3}, {4}]", inputReaderDesc, tokenStartLineIndex + 1, tokenStartColIndex + 1, currLineIndex + 1, currColIndex, token);
        }
        
        autodetectBlockContent();

        // After mode selection we treat REM comment from sqlplus
        if (currCommand.cmdType == CommandTypes.SqlPlus && tokenCountWithoutComments == 0 && sqlPlusKeyword == "REMARK")
        {
            tokenType = TokenTypes.SqlPlusRemark;
            skipSubstitution = true;
            do { token = token + getChar(); } while (true); // we will bail out on end of block exception
        }
        // After mode selection we treat PROMPT from sqlplus
        if (currCommand.cmdType == CommandTypes.SqlPlus && tokenCountWithoutComments == 0 && sqlPlusKeyword == "PROMPT")
        {
            tokenType = TokenTypes.SqlPlusPrompt;
            do token = token + getChar(); while (peekChar() != '\n'); // we will bail jus before end of block exception
        }



        // Different code to comply with SQLPlus compatibility
        if (SQLPlusCompatible)
        {
            if (currCommand.cmdType != CommandTypes.PeekMode && tokenType != TokenTypes.BlockComment && tokenType != TokenTypes.LineComment) tokenCountWithoutComments++;
            if (tokenType == TokenTypes.BlockComment || tokenType == TokenTypes.LineComment) commentCount++;
        }
        else
        {
            if (currCommand.cmdType != CommandTypes.PeekMode) tokenCountWithoutComments++;
        }

        //System.Console.WriteLine(token);

        return token;
    }

    private void checkForAndBeginDetour(int detourStartsAtToken, CommandTypes cmdTypeBefore, String actionBefore, String detourStartsWithToken, String detourUniqueName)
    {
        int tokenCountWithoutCommentsAndDetour = tokenCountWithoutComments - detourTaken;
        string upperToken = token.ToUpper();

        if (tokenCountWithoutCommentsAndDetour == detourStartsAtToken &&
            currCommand.cmdType == cmdTypeBefore &&
            currCommand.action == actionBefore &&
            upperToken == detourStartsWithToken)
        {
            autodetectMode = AutodetectMode.Detour;
            detourStart = tokenCountWithoutComments;
            detourPath = detourUniqueName;
        }
    }

    private bool handleDetourStep(String detourName, int detourStepSeq, String whichTokenIsExpected, bool endOfDetour)
    {
        if (detourPath == detourName)
        {
            int detourTokenCountWithoutComments = tokenCountWithoutComments - detourStart + 1;

            if (detourTokenCountWithoutComments == detourStepSeq)
            {
                string upperToken = token.ToUpper();

                if (whichTokenIsExpected != "" && upperToken != whichTokenIsExpected) 
                {
                    raiseSyntaxErrorFoundException(whichTokenIsExpected + " expected");
                    autodetectMode = AutodetectMode.Normal;
                    detourStart = -1;
                    return false;
                }

                if (endOfDetour)
                {
                    autodetectMode = AutodetectMode.Normal;
                    detourStart = -1;
                    detourTaken = detourTaken + detourStepSeq;
                }

                return true;
            }
        }

        return false;
    }

    private bool handleDetourStep(String detourName, int detourStepSeq, String[] whichTokensAreExpected, bool endOfDetour)
    {
        if (detourPath == detourName)
        {
            int detourTokenCountWithoutComments = tokenCountWithoutComments - detourStart + 1;

            if (detourTokenCountWithoutComments == detourStepSeq)
            {

                if (whichTokensAreExpected.Length != 0)
                {
                    string upperToken = token.ToUpper();
                    string tokenFound = Array.Find(whichTokensAreExpected, s => s.Equals(upperToken));

                    if (tokenFound == "")
                    {
                        raiseSyntaxErrorFoundException(string.Join(",", whichTokensAreExpected) + " expected");
                        autodetectMode = AutodetectMode.Normal;
                        detourStart = -1;
                        return false;
                    }
                }

                if (endOfDetour)
                {
                    autodetectMode = AutodetectMode.Normal;
                    detourStart = -1;
                    detourTaken = detourTaken + detourStepSeq;
                }

                return true;
            }
        }

        return false;
    }


    private void checkForAndBeginDetour(int detourStartsAtToken, CommandTypes cmdTypeBefore, String actionBefore, String baseCmdNameBefore, String detourStartsWithToken, String detourUniqueName)
    {
        int tokenCountWithoutCommentsAndDetour = tokenCountWithoutComments - detourTaken;
        string upperToken = token.ToUpper();

        if (tokenCountWithoutCommentsAndDetour == detourStartsAtToken &&
            currCommand.cmdType == cmdTypeBefore &&
            currCommand.action == actionBefore &&
            currCommand.baseCmdName == baseCmdNameBefore &&
            upperToken == detourStartsWithToken)
        {
            autodetectMode = AutodetectMode.Detour;
            detourStart = tokenCountWithoutComments;
            detourPath = detourUniqueName;
        }
    }

    private void autodetectBlockContent()
    {
        sqlPlusKeyword = "";
        string upperToken = token.ToUpper();
        int tokenCountWithoutCommentsAndDetour = tokenCountWithoutComments - detourTaken;

        // System.Console.WriteLine("---------------------------------------------------------------------");
        // System.Console.WriteLine("tokenCountWithoutComments={0} currCommand.action={1} upperToken={2}", tokenCountWithoutComments, currCommand.action, upperToken);
        // System.Console.WriteLine("tokenCountWithoutCommentsAndDetour == {0}, detourTaken = {1}", tokenCountWithoutCommentsAndDetour, detourTaken);
        // System.Console.WriteLine("autodetectMode = {1}, currCommand.cmdType = {0}", currCommand.cmdType, autodetectMode);
        if (autodetectMode == AutodetectMode.Normal || autodetectMode == AutodetectMode.ObjectName)
        {
            // detour detection
            //////////////////////////////////////////////////////////////////////////////////////////////

            // CREATE [OR REPLACE] ...
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "OR", "OR REPLACE");

            // UPDATE [ONLY] (....) ....
            checkForAndBeginDetour(1, CommandTypes.Sql, "UPDATE", "ONLY", "UPDATE ONLY");

            // UPDATE [(]SELECT EMPNO FROM EMP) ....
            checkForAndBeginDetour(1, CommandTypes.Sql, "UPDATE", "(", "UPDATE SUBQUERY");

            // INSERT INTO (....) ....
            checkForAndBeginDetour(1, CommandTypes.Sql, "INSERT", "INTO", "INSERT INTO");

            // INSERT INTO (SUBQUERY) ....
            checkForAndBeginDetour(1, CommandTypes.Sql, "INSERT", "(", "INSERT SUBQUERY");

            // DELETE FROM (....) ....
            checkForAndBeginDetour(1, CommandTypes.Sql, "DELETE", "FROM", "DELETE FROM");

            // DELETE (SUBQUERY) ....
            checkForAndBeginDetour(1, CommandTypes.Sql, "DELETE", "(", "DELETE SUBQUERY");

            // DELETE ONLY ....
            checkForAndBeginDetour(1, CommandTypes.Sql, "DELETE", "ONLY", "DELETE ONLY");
          
            // CREATE [GLOBAL TEMPORARY] TABLE
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "GLOBAL", "GLOBAL TEMPORARY");
            
            // CREATE [PUBLIC] SYNONYM, DATABASE LINK
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "PUBLIC", "PUBLIC");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "REPLACE", "PUBLIC", "PUBLIC");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE OR REPLACE", "PUBLIC", "PUBLIC");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "DROP", "PUBLIC", "PUBLIC");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "ALTER", "PUBLIC", "PUBLIC");

            // CREATE [PRIVATE] OUTLINE
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "PRIVATE", "PRIVATE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "REPLACE", "PRIVATE", "PRIVATE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE OR REPLACE", "PRIVATE", "PRIVATE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "ALTER", "PRIVATE", "PRIVATE");

            // CREATE [SHARED] PUBLIC DATABASE LINK
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "SHARED", "SHARED");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "ALTER", "SHARED", "SHARED");


            // CREATE BIGFILE TABLESPACE
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "BIGFILE", "BIGFILE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "UNDO", "UNDO");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "TEMPORARY", "TEMPORARY");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "SMALLFILE", "SMALLFILE");

            // CREATE UNIQUE INDEX
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "UNIQUE", "UNIQUE");

            // CREATE BITMAP INDEX
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "BITMAP", "BITMAP");

            // CREATE EDITIONING VIEW
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "EDITIONING", "EDITIONING");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "REPLACE", "EDITIONING", "EDITIONING");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE OR REPLACE", "EDITIONING", "EDITIONING");

            // CREATE FORCE VIEW
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "FORCE", "FORCE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "REPLACE", "FORCE", "FORCE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE OR REPLACE", "FORCE", "FORCE");

            // CREATE NO FORCE VIEW
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "NO", "NO FORCE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "REPLACE", "NO", "NO FORCE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE OR REPLACE", "NO", "NO FORCE");

            // CREATE OR REPLACE (AND (RESOLVE | COMPILE)) JAVA
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "AND", "AND RESOLVE COMPILE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "REPLACE", "AND", "AND RESOLVE COMPILE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE OR REPLACE", "AND", "AND RESOLVE COMPILE");

            // CREATE OR REPLACE NOFORCE JAVA
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE", "NOFORCE", "NOFORCE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "REPLACE", "NOFORCE", "NOFORCE");
            checkForAndBeginDetour(1, CommandTypes.Unknown, "CREATE OR REPLACE", "NOFORCE", "NOFORCE");

            // CREATE FLASHBACK ARCHIVE DEFAULT ...
            checkForAndBeginDetour(3, CommandTypes.Sql, "CREATE", "FLASHBACK ARCHIVE", "DEFAULT", "DEFAULT");

            // CREATE TYPE TEST OID 'xyz' ...
            checkForAndBeginDetour(3, CommandTypes.Sql, "CREATE", "TYPE", "OID", "OID");
            checkForAndBeginDetour(3, CommandTypes.Sql, "REPLACE", "TYPE", "OID", "OID");
            checkForAndBeginDetour(3, CommandTypes.Sql, "CREATE OR REPLACE", "TYPE", "OID", "OID");

            // CREATE TYPE TEST TIMESTAMP 'xyz' ...
            checkForAndBeginDetour(3, CommandTypes.Sql, "CREATE", "TYPE", "TIMESTAMP", "TIMESTAMP");
            checkForAndBeginDetour(3, CommandTypes.Sql, "REPLACE", "TYPE", "TIMESTAMP", "TIMESTAMP");
            checkForAndBeginDetour(3, CommandTypes.Sql, "CREATE OR REPLACE", "TYPE", "TIMESTAMP", "TIMESTAMP");

            // FLASHBACK STANDBY DATABASE TO TIMESTAMP SYSDATE-1;
            checkForAndBeginDetour(1, CommandTypes.Sql, "FLASHBACK", "STANDBY", "STANDBY");

            // REVOKE ... ON [DIRECTORY|EDITION|MINING MODEL|JAVA SOURCE|JAVA RESOURCE] schema.object
            if (currCommand.action == "REVOKE" &&
                autodetectMode == AutodetectMode.ObjectName &&
                (upperToken == "JAVA" || upperToken == "MINING" || upperToken == "EDITION" || upperToken == "DIRECTORY"))
            {
                autodetectMode = AutodetectMode.Detour;
                detourStart = tokenCountWithoutComments;
                detourPath = "REVOKE_OBJECT_CLAUSE";
            }
        
        }

        if (autodetectMode == AutodetectMode.ObjectNameList)
        {
            if (listTerminatorTokenHashSet.ContainsKey(token.ToUpper())) autodetectMode = AutodetectMode.Normal;
            else
            {
                if (currCommand.objectName == "") detourTaken = detourTaken - 1;

                if (token == listSeparator) { appendObjectName(token); detourTaken = detourTaken + 1; }
                else { appendObjectName(token); detourTaken = detourTaken + 1; }
            }
        }

        if (autodetectMode == AutodetectMode.SecondaryObjectNameList)
        {
            if (listTerminatorTokenHashSet.ContainsKey(token.ToUpper())) autodetectMode = AutodetectMode.Normal;
            else
            {
                if (currCommand.secondaryObjectName == "") detourTaken = detourTaken - 1;

                if (token == listSeparator) { appendSecondaryObject(token); detourTaken = detourTaken + 1; }
                else { appendSecondaryObject(token); detourTaken = detourTaken + 1; }
            }
        }

        if (autodetectMode == AutodetectMode.CommandNameList)
        {
            if (listTerminatorTokenHashSet.ContainsKey(token.ToUpper())) autodetectMode = AutodetectMode.Normal;
            else
            {
                if (currCommand.cmdName == "") detourTaken = detourTaken - 1; 
                
                if (token == listSeparator) { appendCurrCmdName(token); detourTaken = detourTaken + 1; }
                else { appendCurrCmdName(token); detourTaken = detourTaken + 1; }
            }
        }
        
        if (autodetectMode == AutodetectMode.Detour)
        {
            // System.Console.WriteLine("detourStart == {0}", detourStart);
            int detourTokenCountWithoutComments = tokenCountWithoutComments - detourStart + 1;

            // CREATE --> OR REPLACE <--
            // Affects CommandObject.Action
            handleDetourStep("OR REPLACE", 1, "OR", false);
            if (handleDetourStep("OR REPLACE", 2, "REPLACE", true)) currCommand.action = currCommand.action + " " + detourPath;

            // UPDATE ONLY (....) ....
            handleDetourStep("UPDATE ONLY", 1, "ONLY", true);

            // UPDATE (SELECT EMPNO FROM EMP) ....
            handleDetourStep("UPDATE SUBQUERY", 1, "(", true);

            // INSERT INTO (....) ....
            handleDetourStep("INSERT INTO", 1, "INTO", true);

            // INSERT INTO (SELECT EMPNO FROM EMP) ....
            handleDetourStep("INSERT SUBQUERY", 1, "(", true);

            // DELETE FROM (....) ....
            handleDetourStep("DELETE FROM", 1, "FROM", true);

            // DELETE (SUBQUERY) ....
            handleDetourStep("DELETE SUBQUERY", 1, "(", true);

            // DELETE ONLY ....
            handleDetourStep("DELETE ONLY", 1, "ONLY", false);
            handleDetourStep("DELETE ONLY", 2, "(", true);
 
            // CREATE --> GLOBAL TEMPORARY <-- TABLE
            // Affects CommandObject.Action
            handleDetourStep("GLOBAL TEMPORARY", 1, "GLOBAL", false);
            if (handleDetourStep("GLOBAL TEMPORARY", 2, "TEMPORARY", true)) appendCurrCmdName(detourPath);

            // CREATE --> PUBLIC <-- SYNONYM || DATABASE LINK
            // Affects CommandObject.CmdName
            if (handleDetourStep("PUBLIC", 1, "PUBLIC", true)) appendCurrCmdName(detourPath);

            // CREATE --> PRIVATE <-- OUTLINE
            // Affects CommandObject.CmdName
            if (handleDetourStep("PRIVATE", 1, "PRIVATE", true)) appendCurrCmdName(detourPath);

            // CREATE --> SHARED <-- PUBLIC DATABASE LINK
            // Affects CommandObject.CmdName
            if (handleDetourStep("SHARED", 1, "SHARED", true)) appendCurrCmdName(detourPath);

            // CREATE --> BIGFILE <--
            // Affects CommandObject.CmdName
            if (handleDetourStep("BIGFILE", 1, "BIGFILE", true)) appendCurrCmdName(detourPath);
            if (handleDetourStep("TEMPORARY", 1, "TEMPORARY", true)) appendCurrCmdName(detourPath);
            if (handleDetourStep("UNDO", 1, "UNDO", true)) appendCurrCmdName(detourPath);
            if (handleDetourStep("SMALLFILE", 1, "SMALLFILE", true)) appendCurrCmdName(detourPath);

            // CREATE --> BITMAP <--
            // Affects CommandObject.CmdName
            if (handleDetourStep("BITMAP", 1, "BITMAP", true)) appendCurrCmdName(detourPath);

            // CREATE --> UNIQUE <--
            // Affects CommandObject.CmdName
            if (handleDetourStep("UNIQUE", 1, "UNIQUE", true)) appendCurrCmdName(detourPath);

            // CREATE --> EDITIONING <--
            // Affects CommandObject.CmdName
            if (handleDetourStep("EDITIONING", 1, "EDITIONING", true)) appendCurrCmdName(detourPath);

            // CREATE --> FORCE <--
            // Affects CommandObject.CmdName
            if (handleDetourStep("FORCE", 1, "FORCE", true)) appendCurrCmdName(detourPath);

            // CREATE --> NO FORCE <-- VIEW
            // Affects CommandObject.Action
            handleDetourStep("NO FORCE", 1, "NO", false);
            if (handleDetourStep("NO FORCE", 2, "FORCE", true)) appendCurrCmdName(detourPath);

            // CREATE --> AND RESOLVE | COMPILE <-- JAVA ...
            // Affects CommandObject.CmdName
            handleDetourStep("AND RESOLVE COMPILE", 1, "AND", false);
            handleDetourStep("AND RESOLVE COMPILE", 2, new String[2] {"RESOLVE", "COMPILE"}, true);

            // CREATE --> AND RESOLVE | COMPILE <-- JAVA ...
            // Affects CommandObject.CmdName
            handleDetourStep("NOFORCE", 1, "NOFORCE", true);

            // CREATE FLASHBACK ARCHIVE --> DEFAULT <-- ...
            // Affects CommandObject.CmdName
            if (handleDetourStep("DEFAULT", 1, "DEFAULT", true)) nextTokenWillBeObjectName();

            // CREATE TYPE xyz --> OID 'aaaaaaaa' <-- ...
            // Affects CommandObject.CmdName
            handleDetourStep("OID", 1, "OID", false);
            handleDetourStep("OID", 2, "", true);

            // CREATE TYPE xyz --> TIMESTAMP 'aaaaaaaa' <-- ...
            // Affects CommandObject.CmdName
            handleDetourStep("TIMESTAMP", 1, "TIMESTAMP", false);
            handleDetourStep("TIMESTAMP", 2, "", true);

            // FLASHBACK --> STANDBY <-- DATABASE TO TIMESTAMP SYSDATE-1;
            if (handleDetourStep("STANDBY", 1, "STANDBY", true)) appendCurrCmdName(detourPath);


            if (handleDetourStep("REVOKE_OBJECT_CLAUSE", 1, new String[4] { "JAVA", "MINING", "EDITION", "DIRECTORY"}, false)) 
                if (upperToken == "EDITION" || upperToken == "DIRECTORY") { nextTokenWillBeObjectName(); detourStart = -1; }
            if (handleDetourStep("REVOKE_OBJECT_CLAUSE", 2, new String[3] { "MODEL", "SOURCE", "RESOURCE" }, true)) nextTokenWillBeObjectName();
            

        }
        else if (autodetectMode == AutodetectMode.ObjectName)
        {
            setObjectName(token);
            autodetectMode = AutodetectMode.Normal;
        }
        else if (autodetectMode == AutodetectMode.SecondaryObjectName)
        {
            currCommand.secondaryObjectName = token;
            autodetectMode = AutodetectMode.Normal;
        }
        else if (autodetectMode == AutodetectMode.ObjectNameList || autodetectMode == AutodetectMode.CommandNameList || autodetectMode == AutodetectMode.SecondaryObjectNameList)
        {
            // do nothing here, just skiping no detour path of ELSE statement
        }
        else
        {
            //System.Console.WriteLine("No detour");
            if (tokenCountWithoutCommentsAndDetour == 0)
            {
                // First check if it is SQLPlus reserved word
                //****************************************************
                if (sqlPlusKeywords.ContainsKey(upperToken))
                {

                    setCurrCmdType(CommandTypes.SqlPlus);
                    sqlPlusKeyword = sqlPlusKeywords[upperToken].ToString();
                    currCommand.action = sqlPlusKeyword;

                    // With @, @ and start, lexer uses space as separator between literals
                    if (sqlPlusKeyword == "@" || sqlPlusKeyword == "@@" || sqlPlusKeyword == "START")
                    {
                        setCurrCmdType(CommandTypes.SqlPlusStart);
                    }

                }
                // The ARCHIVE LOG command in SQL*PLUS
                else if (upperToken == "ARCHIVE")
                {
                    setCurrCmdTypeAndAppendCmdName(CommandTypes.SqlPlus, "ARCHIVE");
                }
                // SET command is used both as a SQLPlus reserved word
                // and SQL reserved word
                //****************************************************
                else if (upperToken == "SET")
                {
                    // lets stay in unknown mode, could be SQL or SQLPlus
                }
                // Anonymous PL/SQL block
                //****************************************************
                else if (upperToken == "BEGIN" || upperToken == "DECLARE")
                {
                    setCurrCmdTypeAndAppendCmdName(CommandTypes.Plsql, "ANONYMOUS");
                }
                else if (upperToken == "CREATE")
                {
                    // lets stay in unknown mode, could be SQL or PLSQL 
                }

                    // Single SQL command
                //****************************************************
                else if (upperToken == "ANALYZE" ||
                         upperToken == "COMMENT" ||
                         upperToken == "SELECT" ||
                         upperToken == "WITH" ||
                         upperToken == "INSERT" ||
                         upperToken == "UPDATE" ||
                         upperToken == "DELETE" ||
                         upperToken == "COMMIT" ||
                         upperToken == "ROLLBACK" ||
                         upperToken == "GRANT" ||
                         upperToken == "REVOKE" ||
                         upperToken == "AUDIT" ||
                         upperToken == "NOAUDIT" ||
                         upperToken == "RENAME" ||
                         upperToken == "TRUNCATE" ||
                         upperToken == "CALL" ||
                         upperToken == "EXPLAIN" ||
                         upperToken == "LOCK" ||
                         upperToken == "MERGE" ||
                         upperToken == "FLASHBACK" ||
                         upperToken == "PURGE")
                {
                    setCurrCmdType(CommandTypes.Sql);
                }
                else if (upperToken == "SAVEPOINT")
                {
                    setCurrCmdType(CommandTypes.Sql);
                    nextTokenWillBeObjectName();
                }
                else if (upperToken == "ASSOCIATE")
                {
                    setCurrCmdType(CommandTypes.Sql); currCommand.action = "ASSOCIATE STATISTICS";
                }
                else if (upperToken == "DISASSOCIATE")
                {
                    setCurrCmdType(CommandTypes.Sql); currCommand.action = "DISASSOCIATE STATISTICS";
                }
                else if (upperToken == "ALTER" ||
                         upperToken == "DROP")
                {
                }
                // This condition is skipped in SQLPlus compatibility mode
                // With comment do not modify current mode !!
                else if (SQLPlusCompatible &&
                         (tokenType == TokenTypes.BlockComment || tokenType == TokenTypes.LineComment))
                {
                }
                // We treat unknown command as SQLPlusCommand
                else
                {
                    setCurrCmdType(CommandTypes.SqlPlus);
                }

                if (tokenType == TokenTypes.BlockComment) currCommand.action = "Block comment";
                else if (tokenType == TokenTypes.LineComment) currCommand.action = "Line comment";
                else if (currCommand.cmdType == CommandTypes.Sql && upperToken == "WITH") currCommand.action = "SELECT";
                else if (currCommand.cmdType != CommandTypes.SqlPlus && currCommand.action == "") currCommand.action = upperToken;

                if (upperToken == "CALL") nextTokenWillBeObjectName();
                if (upperToken == "GRANT") { nextTokenWillBeCommandNameList(); listTerminatorTokenHashSet.Add("TO", ""); listTerminatorTokenHashSet.Add("ON", ""); }
                if (upperToken == "REVOKE") { nextTokenWillBeCommandNameList(); listTerminatorTokenHashSet.Add("FROM", ""); listTerminatorTokenHashSet.Add("ON", ""); }
            }

            // we have already read the first token 
            // and are currently reading the second one
            // check if this can change the mode
            if (tokenCountWithoutCommentsAndDetour == 1)
            {
                if (currCommand.action == "ALTER" || currCommand.action == "DROP" || currCommand.action == "ANALYZE" || currCommand.action == "LOCK" || currCommand.action == "TRUNCATE" || currCommand.action == "FLASHBACK")
                {
                    if (upperToken == "DATABASE") { setCurrCmdType(CommandTypes.Sql); appendCurrCmdName(upperToken); setBaseCmdName(upperToken); }
                    else if (upperToken == "PACKAGE") setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken);
                    else if (upperToken == "TYPE") { appendCurrCmdName(upperToken); }
                    else if (upperToken == "FLASHBACK") setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken);
                    else if (upperToken == "MATERIALIZED") { setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "PROCEDURE" || upperToken == "FUNCTION" || upperToken == "TRIGGER")
                    {
                        setCurrCmdType(CommandTypes.Sql);
                        appendCurrCmdName(upperToken);
                        setBaseCmdName(upperToken);
                        nextTokenWillBeObjectName();
                    }
                    else if (upperToken == "ROLLBACK") { setBaseCmdName("ROLLBACK SEGMENT"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "RESOURCE") { setBaseCmdName("RESOURCE COST"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "RESTORE") { setBaseCmdName("RESTORE POINT"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "JAVA") { setBaseCmdName("JAVA"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "SESSION") { setBaseCmdName("SESSION"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "SYSTEM") { setBaseCmdName("SYSTEM"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (currCommand.action == "LOCK" && upperToken == "TABLE")
                    {
                        appendCurrCmdName(upperToken);
                        setBaseCmdName(upperToken);
                        nextTokenWillBeObjectNameList();
                        listTerminatorTokenHashSet.Add("IN", "");
                    }
                    else
                    {
                        setCurrCmdType(CommandTypes.Sql);
                        appendCurrCmdName(upperToken);
                        setBaseCmdName(upperToken);
                        nextTokenWillBeObjectName();
                    }
                }

                if (currCommand.cmdType == CommandTypes.Unknown && currCommand.action == "SET")
                {
                    switch (upperToken)
                    {
                        case "CONSTRAINT": setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); break;
                        case "CONSTRAINTS": setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); break;
                        case "ROLE": setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); break;
                        case "TRANSACTION": setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); break;
                        default: setCurrCmdTypeAndAppendCmdName(CommandTypes.SqlPlus, upperToken); break;
                    }
                }
                if (currCommand.cmdType == CommandTypes.Unknown && (currCommand.action == "CREATE" || currCommand.action == "REPLACE" || currCommand.action == "CREATE OR REPLACE"))
                {
                    if (upperToken == "FUNCTION" ||
                        upperToken == "LIBRARY" ||
                        upperToken == "PROCEDURE" ||
                        upperToken == "TRIGGER"
                        )
                    {
                        setBaseCmdName(upperToken);
                        setCurrCmdTypeAndAppendCmdName(CommandTypes.Plsql, upperToken);
                        nextTokenWillBeObjectName();
                    }
                    else if (upperToken == "PACKAGE") setCurrCmdTypeAndAppendCmdName(CommandTypes.Plsql, upperToken);
                    else if (upperToken == "JAVA") setCurrCmdTypeAndAppendCmdName(CommandTypes.Plsql, upperToken);
                    else if (upperToken == "FLASHBACK") setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken);
                    else if (upperToken == "ROLLBACK") { setBaseCmdName("ROLLBACK SEGMENT"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "RESTORE") { setBaseCmdName("RESTORE POINT"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "SCHEMA") { setBaseCmdName("SCHEMA"); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "TYPE") { appendCurrCmdName(upperToken); }
                    else if (upperToken == "MATERIALIZED") { setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else if (upperToken == "DATABASE") { setBaseCmdName(upperToken); setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken); }
                    else
                    {
                        setBaseCmdName(upperToken);
                        setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, upperToken);
                        nextTokenWillBeObjectName();
                    }
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "INSERT")
                {
                    if (upperToken == "ALL") currCommand.objectName = "[MULTI TABLE]";
                    else if (upperToken == "FIRST") currCommand.objectName = "[MULTI TABLE]";
                    else if (upperToken == "SELECT") currCommand.objectName = "[SUBQUERY]";
                    else if (upperToken == "TABLE") currCommand.objectName = "[TABLE COLLECTION]";
                    else currCommand.objectName = upperToken;
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "UPDATE")
                {
                    if (upperToken == "SELECT") currCommand.objectName = "[SUBQUERY]";
                    else if (upperToken == "TABLE") currCommand.objectName = "[TABLE COLLECTION]";
                    else currCommand.objectName = upperToken;
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "DELETE")
                {
                    if (upperToken == "SELECT") currCommand.objectName = "[SUBQUERY]";
                    else if (upperToken == "TABLE") currCommand.objectName = "[TABLE COLLECTION]";
                    else currCommand.objectName = upperToken;
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for ASSOCIATE STATISTICS
                if (currCommand.cmdType == CommandTypes.Sql && (currCommand.action == "ASSOCIATE STATISTICS" ||currCommand.action == "DISASSOCIATE STATISTICS"))
                {

                    if (upperToken != "STATISTICS") raiseSyntaxErrorFoundException("STATISTICS expected");
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "PURGE")
                {
                    appendCurrCmdName(upperToken);
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "RENAME")
                {
                    currCommand.objectName = upperToken;
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "EXPLAIN")
                {
                    if (upperToken != "PLAN") raiseSyntaxErrorFoundException("PLAN expected");
                    currCommand.action += " " + upperToken;
                }

            
            }

            if (tokenCountWithoutCommentsAndDetour == 2)
            {
                // we have no token look ahead, so we have to compensate using some extra IFs
                // for TYPE BODY
                if (currCommand.cmdName == "TYPE")
                {
                    if (upperToken == "BODY") 
                    { 
                        setBaseCmdName("TYPE BODY"); 
                        if (currCommand.action == "ALTER" || currCommand.action == "DROP") setCurrCmdTypeAndAppendCmdName(CommandTypes.Sql, "BODY");
                        else setCurrCmdTypeAndAppendCmdName(CommandTypes.Plsql, "BODY"); 
                        nextTokenWillBeObjectName(); 
                    }
                    else 
                    { 
                        setBaseCmdName("TYPE"); 
                        setCurrCmdType(CommandTypes.Sql); 
                        setObjectName(token); 
                    }
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for PACKAGE BODY
                if (currCommand.cmdName == "PACKAGE")
                {
                    if (upperToken == "BODY") 
                    { 
                        setBaseCmdName("PACKAGE BODY"); 
                        appendCurrCmdName("BODY"); 
                        nextTokenWillBeObjectName(); 
                    }
                    else 
                    { 
                        setBaseCmdName("PACKAGE"); 
                        setObjectName(token); 
                    }
                }
                
                // we have no token look ahead, so we have to compensate using some extra IFs
                // for ROLLBACK SEGMENT
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.baseCmdName == "ROLLBACK SEGMENT")
                {
                    if (upperToken == "SEGMENT") { appendCurrCmdName("SEGMENT"); nextTokenWillBeObjectName(); }
                    else { raiseSyntaxErrorFoundException("SEGMENT expected"); }
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for RESOURCE COST
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.baseCmdName == "RESOURCE COST")
                {
                    if (upperToken == "COST") { appendCurrCmdName("COST");}
                    else { raiseSyntaxErrorFoundException("COST expected"); }
                }


                // we have no token look ahead, so we have to compensate using some extra IFs
                // for RESTORE POINT
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.baseCmdName == "RESTORE POINT")
                {
                    if (upperToken == "POINT") { appendCurrCmdName("POINT"); nextTokenWillBeObjectName(); }
                    else { raiseSyntaxErrorFoundException("POINT expected"); }
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for RESTORE POINT
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.baseCmdName == "SCHEMA")
                {
                    if (upperToken == "AUTHORIZATION") { nextTokenWillBeObjectName(); }
                    else { raiseSyntaxErrorFoundException("AUTHORIZATION expected"); }
                }


                
                // we have no token look ahead, so we have to compensate using some extra IFs
                // to distinguishe between DATABASE and DATABASE LINK
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.baseCmdName == "DATABASE")
                {
                    if (upperToken == "LINK") { setBaseCmdName("DATABASE LINK"); appendCurrCmdName("LINK"); nextTokenWillBeObjectName(); }
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for FLASHBACK ARCHIVE
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.cmdName == "FLASHBACK")
                {
                    if (upperToken == "ARCHIVE") { setBaseCmdName("FLASHBACK ARCHIVE"); appendCurrCmdName("ARCHIVE"); nextTokenWillBeObjectName(); }
                    else { raiseSyntaxErrorFoundException("ARCHIVE expected"); }
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for MATERIALIZED VIEW (LOG)
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.cmdName == "MATERIALIZED")
                {
                    if (upperToken == "VIEW") { appendCurrCmdName(upperToken); }
                    else { raiseSyntaxErrorFoundException("VIEW expected"); }
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for DROP JAVA (SOURCE || CLASS || RESOURCE) xyz
                if ((currCommand.action == "DROP" || currCommand.action == "ALTER") && currCommand.baseCmdName == "JAVA")
                {
                    if (upperToken == "SOURCE" || upperToken == "CLASS" || upperToken == "RESOURCE") { appendCurrCmdName(upperToken); nextTokenWillBeObjectName(); }
                    else { raiseSyntaxErrorFoundException("SOURCE|CLASS|RESOURCE expected"); }
                }

                
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "COMMENT")
                {
                    appendCurrCmdName(upperToken);
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for ASSOCIATE STATISTICS
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "ASSOCIATE STATISTICS")
                {
                    if (upperToken != "WITH") raiseSyntaxErrorFoundException("WITH expected");
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for DISASSOCIATE STATISTICS
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "DISASSOCIATE STATISTICS")
                {
                    if (upperToken != "FROM") raiseSyntaxErrorFoundException("FROM expected");
                }

                // GRANT SELECT, UPDATE ---> ON <---- SOME_TABLE TO  
                // GRANT SELECT ANY TABLE, DROP ANY TABLE ---> TO <---- USER1, USER2  
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "GRANT")
                {
                    if (upperToken == "ON") nextTokenWillBeObjectName();
                    else if (upperToken == "TO") { nextTokenWillBeSecondaryObjectNameList(); listTerminatorTokenHashSet.Add("WITH", ""); }
                    else raiseSyntaxErrorFoundException("ON|TO expected");
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "REVOKE")
                {
                    if (upperToken == "ON") nextTokenWillBeObjectName();
                    else if (upperToken == "FROM") { nextTokenWillBeSecondaryObjectNameList(); listTerminatorTokenHashSet.Add("CASCADE", ""); listTerminatorTokenHashSet.Add("FORCE", ""); }
                    else raiseSyntaxErrorFoundException("ON|FROM expected");
                }


            }

            if (tokenCountWithoutCommentsAndDetour == 3)
            {

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for MATERIALIZED VIEW (LOG)
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.cmdName == "MATERIALIZED VIEW")
                {
                    if (upperToken == "LOG") { setBaseCmdName("MATERIALIZED VIEW LOG"); appendCurrCmdName(upperToken); }
                    else { setObjectName(token); setBaseCmdName("MATERIALIZED VIEW"); }
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.baseCmdName == "SYNONYM" && upperToken == "FOR")
                {
                    nextTokenWillBeSecondaryObjectName();
                }

                if ((currCommand.cmdType == CommandTypes.Sql || currCommand.cmdType == CommandTypes.Plsql) &&
                    (currCommand.baseCmdName == "TYPE" || currCommand.baseCmdName == "PROCEDURE" || currCommand.baseCmdName == "FUNCTION" || currCommand.baseCmdName == "PACKAGE" || currCommand.baseCmdName == "LIBRARY") && 
                    upperToken == "WRAPPED")
                {
                    setCurrCmdType(CommandTypes.WrappedPlsql);
                }


                
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "ALTER" && currCommand.cmdName == "TABLE")
                {
                    currCommand.alterType = upperToken;
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for ASSOCIATE STATISTICS
                if (currCommand.cmdType == CommandTypes.Sql && (currCommand.action == "ASSOCIATE STATISTICS" || currCommand.action == "DISASSOCIATE STATISTICS"))
                {
                    setBaseCmdName(upperToken);
                    currCommand.cmdName = upperToken;
                    nextTokenWillBeObjectNameList();
                    listTerminatorTokenHashSet.Add("USING", "");
                    listTerminatorTokenHashSet.Add("DEFAULT", "");
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "COMMENT")
                {
                    if (currCommand.cmdName == "MATERIALIZED" || currCommand.cmdName == "MINING") { appendCurrCmdName(upperToken); currCommand.baseCmdName = currCommand.cmdName; }
                    else
                    {
                            if (currCommand.cmdName == "COLUMN")
                            {
                                // split table.column when comment on column ...
                                string[] table_column = token.Split('.');
                                if (table_column.Length == 2)
                                {
                                    setObjectName(table_column[0]);
                                    currCommand.secondaryObjectName = table_column[1];
                                }
                                else if (table_column.Length == 3)
                                {
                                    setObjectName(table_column[0] + "." + table_column[1]);
                                    currCommand.secondaryObjectName = table_column[2];
                                }
                                else raiseSyntaxErrorFoundException("Column is not identified by 2|3 parts");
                            }
                            else setObjectName(token);
                     }        

                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "RENAME")
                {
                    currCommand.secondaryObjectName = upperToken;
                }
            }

            if (tokenCountWithoutCommentsAndDetour == 4)
            {
                if (currCommand.cmdType == CommandTypes.Plsql &&
                    (currCommand.baseCmdName == "TYPE BODY" || currCommand.baseCmdName == "PACKAGE BODY") &&
                    upperToken == "WRAPPED")
                {
                    setCurrCmdType(CommandTypes.WrappedPlsql);
                }

                // we have no token look ahead, so we have to compensate using some extra IFs
                // for MATERIALIZED VIEW (LOG) ON
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.cmdName == "MATERIALIZED VIEW LOG")
                {
                    if (upperToken == "ON") nextTokenWillBeSecondaryObjectName();
                    else { raiseSyntaxErrorFoundException("ON expected"); }
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "ALTER" && currCommand.cmdName == "TABLE" && currCommand.alterType == "ADD" && upperToken == "CONSTRAINT")
                {
                    currCommand.alterType += " " + upperToken;
                }
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "COMMENT")
                {
                    if (currCommand.cmdName == "MATERIALIZED VIEW" || currCommand.cmdName == "MINING MODEL") setObjectName(token);
                }


                // GRANT SELECT, UPDATE  ON  SOME_TABLE ---> TO  <---- somebody
                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "GRANT")
                {
                    if (upperToken == "TO") { nextTokenWillBeSecondaryObjectNameList(); listTerminatorTokenHashSet.Add("WITH", ""); }
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "REVOKE")
                {
                    if (upperToken == "FROM") {nextTokenWillBeSecondaryObjectNameList(); listTerminatorTokenHashSet.Add("CASCADE", ""); listTerminatorTokenHashSet.Add("FORCE", ""); }
                }
            }


            if (tokenCountWithoutCommentsAndDetour == 5)
            {
                if (currCommand.cmdType == CommandTypes.Sql &&
                    currCommand.action == "ALTER" &&
                    currCommand.cmdName == "TABLE" &&
                    currCommand.alterType == "ADD CONSTRAINT")
                {
                    currCommand.secondaryObjectName = token;
                }

                if (currCommand.cmdType == CommandTypes.Sql && currCommand.action == "ALTER" && currCommand.cmdName == "TABLE" && currCommand.alterType == "ADD" && upperToken == "CONSTRAINT")
                {
                    currCommand.alterType += " " + upperToken;
                }

            }

            if (tokenCountWithoutCommentsAndDetour == 6)
            {
                if (currCommand.cmdType == CommandTypes.Sql &&
                    currCommand.action == "ALTER" &&
                    currCommand.cmdName == "TABLE" &&
                    currCommand.alterType == "ADD CONSTRAINT" &&
                    currCommand.secondaryObjectName != "")
                {
                    currCommand.secondaryCmdName = token;
                }

                if (currCommand.cmdType == CommandTypes.Sql &&
                    currCommand.action == "ALTER" &&
                    currCommand.cmdName == "TABLE" &&
                    currCommand.alterType == "ADD CONSTRAINT" &&
                    currCommand.secondaryObjectName == "")
                {
                    currCommand.secondaryObjectName = token;
                }
            }

            if (tokenCountWithoutCommentsAndDetour == 7)
            {
                if (currCommand.cmdType == CommandTypes.Sql &&
                    currCommand.action == "ALTER" &&
                    currCommand.cmdName == "TABLE" &&
                    currCommand.alterType == "ADD CONSTRAINT" &&
                    currCommand.secondaryCmdName == "")
                {
                    currCommand.secondaryCmdName = token;
                }
            }
        }
        
        // this is to fish out table name on which index is created
        if (currCommand.cmdType == CommandTypes.Sql &&
            currCommand.action == "CREATE" &&
            (currCommand.cmdName == "INDEX" || currCommand.cmdName == "UNIQUE INDEX") &&
            prevToken == "ON" &&
            currCommand.secondaryObjectName == "")
        {
            currCommand.secondaryObjectName = token;
        }

    }
    
    public string getSkipComments()
    {
        string token = get();

        // skip any possible comments
        while (tokenType == TokenTypes.LineComment ||
               tokenType == TokenTypes.BlockComment)
        {
            token = get();
        }

        return token;
    }
}
}

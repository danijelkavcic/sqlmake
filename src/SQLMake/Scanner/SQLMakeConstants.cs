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

using System.Collections;
namespace SQLMake.Parser
{
     public enum TokenTypes
    {
        NotAvailable,
        Unknown,
        Identifier,
        Literal,
        Integer,
        Number,
        LineComment,
        BlockComment,
        DocComment, // DOC SQLPlus keyword
        WrappedCode,
        Semi,
        Colon,
        DoubleDot,
        Dot,
        Comma,
        Asteriks,
        AtSign,
        DoubleAtSign,
        OpenParenthesis,
        CloseParenthesis,
        Plus,
        Minus,
        Divide,
        PassParameterByName, // =>
        AssignmentEQ, // :=
        EQ, // =
        NotEQ,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Percentage,
        Concat, // ||
        VerticalBar, // |
        OpenLabele, // <<
        CloseLabele, // >>
        SubstitutionVariableIndicator,
        OuterJoinOracleSyntax, // (*)
        SqlPlusRemark,
        SqlPlusPrompt
    }


    class SQLMakeConstants

    {
       
        static public void initializeSqkPlusKeywords(Hashtable sqlPlusKeywords)
        {
            sqlPlusKeywords.Clear();
            sqlPlusKeywords.Add("@", "@");
            sqlPlusKeywords.Add("@@", "@@");
            sqlPlusKeywords.Add("ACC", "ACCEPT");
            sqlPlusKeywords.Add("ACCE", "ACCEPT");
            sqlPlusKeywords.Add("ACCEP", "ACCEPT");
            sqlPlusKeywords.Add("ACCEPT", "ACCEPT");
            sqlPlusKeywords.Add("A", "APPEND");
            sqlPlusKeywords.Add("AP", "APPEND");
            sqlPlusKeywords.Add("APP", "APPEND");
            sqlPlusKeywords.Add("APPE", "APPEND");
            sqlPlusKeywords.Add("APPEN", "APPEND");
            sqlPlusKeywords.Add("APPEND", "APPEND");
            sqlPlusKeywords.Add("ATTRIBUTE", "ATTRIBUTE");
            sqlPlusKeywords.Add("BRE", "BREAK");
            sqlPlusKeywords.Add("BREA", "BREAK");
            sqlPlusKeywords.Add("BREAK", "BREAK");
            sqlPlusKeywords.Add("BTI", "BTITLE");
            sqlPlusKeywords.Add("BTIT", "BTITLE");
            sqlPlusKeywords.Add("BTITL", "BTITLE");
            sqlPlusKeywords.Add("BTITLE", "BTITLE");
            sqlPlusKeywords.Add("C", "CHANGE");
            sqlPlusKeywords.Add("CH", "CHANGE");
            sqlPlusKeywords.Add("CHA", "CHANGE");
            sqlPlusKeywords.Add("CHAN", "CHANGE");
            sqlPlusKeywords.Add("CHANG", "CHANGE");
            sqlPlusKeywords.Add("CHANGE", "CHANGE");
            sqlPlusKeywords.Add("CL", "CLEAR");
            sqlPlusKeywords.Add("CLE", "CLEAR");
            sqlPlusKeywords.Add("CLEA", "CLEAR");
            sqlPlusKeywords.Add("CLEAR", "CLEAR");
            sqlPlusKeywords.Add("COL", "COLUMN");
            sqlPlusKeywords.Add("COLU", "COLUMN");
            sqlPlusKeywords.Add("COLUM", "COLUMN");
            sqlPlusKeywords.Add("COLUMN", "COLUMN");
            sqlPlusKeywords.Add("COMP", "COMPUTE");
            sqlPlusKeywords.Add("COMPU", "COMPUTE");
            sqlPlusKeywords.Add("COMPUT", "COMPUTE");
            sqlPlusKeywords.Add("COMPUTE", "COMPUTE");
            sqlPlusKeywords.Add("CONN", "CONNECT");
            sqlPlusKeywords.Add("CONNE", "CONNECT");
            sqlPlusKeywords.Add("CONNEC", "CONNECT");
            sqlPlusKeywords.Add("CONNECT", "CONNECT");
            sqlPlusKeywords.Add("COPY", "COPY");
            sqlPlusKeywords.Add("DEF", "DEFINE");
            sqlPlusKeywords.Add("DEFI", "DEFINE");
            sqlPlusKeywords.Add("DEFIN", "DEFINE");
            sqlPlusKeywords.Add("DEFINE", "DEFINE");
            sqlPlusKeywords.Add("DEL", "DEL");
            sqlPlusKeywords.Add("DESC", "DESCRIBE");
            sqlPlusKeywords.Add("DESCR", "DESCRIBE");
            sqlPlusKeywords.Add("DESCRI", "DESCRIBE");
            sqlPlusKeywords.Add("DESCRIB", "DESCRIBE");
            sqlPlusKeywords.Add("DESCRIBE", "DESCRIBE");
            sqlPlusKeywords.Add("DISC", "DISCONNECT");
            sqlPlusKeywords.Add("DISCO", "DISCONNECT");
            sqlPlusKeywords.Add("DISCON", "DISCONNECT");
            sqlPlusKeywords.Add("DISCONN", "DISCONNECT");
            sqlPlusKeywords.Add("DISCONNE", "DISCONNECT");
            sqlPlusKeywords.Add("DISCONNEC", "DISCONNECT");
            sqlPlusKeywords.Add("DISCONNECT", "DISCONNECT");
            sqlPlusKeywords.Add("DOC", "DOC");
            sqlPlusKeywords.Add("ED", "EDIT");
            sqlPlusKeywords.Add("EDI", "EDIT");
            sqlPlusKeywords.Add("EDIT", "EDIT");
            sqlPlusKeywords.Add("EXEC", "EXECUTE");
            sqlPlusKeywords.Add("EXECU", "EXECUTE");
            sqlPlusKeywords.Add("EXECUT", "EXECUTE");
            sqlPlusKeywords.Add("EXECUTE", "EXECUTE");
            sqlPlusKeywords.Add("EXIT", "EXIT");
            sqlPlusKeywords.Add("GET", "GET");
            sqlPlusKeywords.Add("HELP", "HELP");
            sqlPlusKeywords.Add("HO", "HOST");
            sqlPlusKeywords.Add("HOS", "HOST");
            sqlPlusKeywords.Add("HOST", "HOST");
            sqlPlusKeywords.Add("I", "INPUT");
            sqlPlusKeywords.Add("IN", "INPUT");
            sqlPlusKeywords.Add("INP", "INPUT");
            sqlPlusKeywords.Add("INPU", "INPUT");
            sqlPlusKeywords.Add("INPUT", "INPUT");
            sqlPlusKeywords.Add("L", "LIST");
            sqlPlusKeywords.Add("LI", "LIST");
            sqlPlusKeywords.Add("LIS", "LIST");
            sqlPlusKeywords.Add("LIST", "LIST");
            sqlPlusKeywords.Add("PASSW", "PASSWORD");
            sqlPlusKeywords.Add("PASSWO", "PASSWORD");
            sqlPlusKeywords.Add("PASSWOR", "PASSWORD");
            sqlPlusKeywords.Add("PASSWORD", "PASSWORD");
            sqlPlusKeywords.Add("PAU", "PAUSE");
            sqlPlusKeywords.Add("PAUS", "PAUSE");
            sqlPlusKeywords.Add("PAUSE", "PAUSE");
            sqlPlusKeywords.Add("PRI", "PRINT");
            sqlPlusKeywords.Add("PRIN", "PRINT");
            sqlPlusKeywords.Add("PRINT", "PRINT");
            sqlPlusKeywords.Add("PRO", "PROMPT");
            sqlPlusKeywords.Add("PROM", "PROMPT");
            sqlPlusKeywords.Add("PROMP", "PROMPT");
            sqlPlusKeywords.Add("PROMPT", "PROMPT");
            sqlPlusKeywords.Add("QUIT", "QUIT");
            sqlPlusKeywords.Add("RECOVER", "RECOVER");
            sqlPlusKeywords.Add("REM", "REMARK");
            sqlPlusKeywords.Add("REMA", "REMARK");
            sqlPlusKeywords.Add("REMAR", "REMARK");
            sqlPlusKeywords.Add("REMARK", "REMARK");
            sqlPlusKeywords.Add("REPF", "REPFOOTER");
            sqlPlusKeywords.Add("REPFO", "REPFOOTER");
            sqlPlusKeywords.Add("REPFOO", "REPFOOTER");
            sqlPlusKeywords.Add("REPFOOT", "REPFOOTER");
            sqlPlusKeywords.Add("REPFOOTE", "REPFOOTER");
            sqlPlusKeywords.Add("REPFOOTER", "REPFOOTER");
            sqlPlusKeywords.Add("REPH", "REPHEADER");
            sqlPlusKeywords.Add("REPHE", "REPHEADER");
            sqlPlusKeywords.Add("REPHEA", "REPHEADER");
            sqlPlusKeywords.Add("REPHEAD", "REPHEADER");
            sqlPlusKeywords.Add("REPHEADE", "REPHEADER");
            sqlPlusKeywords.Add("REPHEADER", "REPHEADER");
            sqlPlusKeywords.Add("R", "RUN");
            sqlPlusKeywords.Add("RU", "RUN");
            sqlPlusKeywords.Add("RUN", "RUN");
            sqlPlusKeywords.Add("SAV", "SAVE");
            sqlPlusKeywords.Add("SAVE", "SAVE");
            sqlPlusKeywords.Add("SHO", "SHOW");
            sqlPlusKeywords.Add("SHOW", "SHOW");
            sqlPlusKeywords.Add("SHUTDOWN", "SHUTDOWN");
            sqlPlusKeywords.Add("SPO", "SPOOL");
            sqlPlusKeywords.Add("SPOO", "SPOOL");
            sqlPlusKeywords.Add("SPOOL", "SPOOL");
            sqlPlusKeywords.Add("STA", "START");
            sqlPlusKeywords.Add("STAR", "START");
            sqlPlusKeywords.Add("START", "START");
            sqlPlusKeywords.Add("STARTUP", "STARTUP");
            sqlPlusKeywords.Add("STORE", "STORE");
            sqlPlusKeywords.Add("TIMI", "TIMING");
            sqlPlusKeywords.Add("TIMIN", "TIMING");
            sqlPlusKeywords.Add("TIMING", "TIMING");
            sqlPlusKeywords.Add("TTI", "TTITLE");
            sqlPlusKeywords.Add("TTIT", "TTITLE");
            sqlPlusKeywords.Add("TTITL", "TTITLE");
            sqlPlusKeywords.Add("TTITLE", "TTITLE");
            sqlPlusKeywords.Add("UNDEF", "UNDEFINE");
            sqlPlusKeywords.Add("UNDEFI", "UNDEFINE");
            sqlPlusKeywords.Add("UNDEFIN", "UNDEFINE");
            sqlPlusKeywords.Add("UNDEFINE", "UNDEFINE");
            sqlPlusKeywords.Add("VAR", "VARIABLE");
            sqlPlusKeywords.Add("VARI", "VARIABLE");
            sqlPlusKeywords.Add("VARIA", "VARIABLE");
            sqlPlusKeywords.Add("VARIAB", "VARIABLE");
            sqlPlusKeywords.Add("VARIABL", "VARIABLE");
            sqlPlusKeywords.Add("VARIABLE", "VARIABLE");
            sqlPlusKeywords.Add("WHENEVER", "WHENEVER");
            sqlPlusKeywords.Add("XQUERY", "XQUERY");
        }


    /*
        public const short LINECOMMENT = 1;
        public const short BLOCKCOMMENT = 2;
        public const short CMDTERMINATOR = 3;
        public const short BLOCKTERMINATOR = 4;
        public const short TEXT = 5;
        public const short NUMBER = 6;
        public const short TOKEN = 8;
        public const short LITERAL_TOKEN = 9;*/

    }
}

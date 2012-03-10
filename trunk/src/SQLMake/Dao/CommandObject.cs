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

namespace SQLMake.Dao
{
    public class CommandObject
    {
        public CommandTypes cmdType;   // eg. unknownBlockType,, SQLPlusBlockType, SQLBlockType, ...
        public string action;      // eg. CREATE, REPLACE, ALTER etc
        public string cmdName;        // eg. UNIQUE INDEX, GLOBAL TEMPORARY TABLE, PUBLIC SYNONYM etc
        public string baseCmdName;        // eg. INDEX, TABLE, etc
        public string objectName;        //[schema.]name
        public string alterType;         // eg. add constraint
        public string secondaryObjectName; // eg. name of the constraint
        public string secondaryCmdName; // eg. primary, unique, foreign, check

        // public StringBuilder currCmdText; // text of current block
        //public int cmdStartLine;
        //public int cmdEndLine;

        public bool syntaxErrorFound; // syntax error found while processing command object
    }
}

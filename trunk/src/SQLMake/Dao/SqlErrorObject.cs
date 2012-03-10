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

namespace SQLMake.Util
{
    class SqlErrorObject
    {

        public int errorSeq;
        public string filename;
        public string sqlShortSummary;
        public int lineNo;
        public string errorMessage;
        public string sqlText;

        public SqlErrorObject(int errorSeq,
                         string filename,
                         string sqlShortSummary,
                         int lineNo,
                         string errorMessage,
                         string sqlText) 
        {
            this.errorSeq = errorSeq;
            this.filename = filename;
            this.sqlShortSummary = sqlShortSummary;
            this.lineNo = lineNo;
            this.errorMessage = errorMessage;
            this.sqlText = sqlText;
        }
    }
}

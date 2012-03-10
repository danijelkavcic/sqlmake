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
    class StringUtils
    {

        public static string TrimExtraBlanksAtEndOfTheLine(string text)
        {
            //view_text can contain additional blanks which makes it difficult to compare with file source
            //we loop through each line and trim the extra blanks
            StringBuilder sb = new StringBuilder();
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                if (line == "") sb.Append("\n");
                else { sb.Append(line.TrimEnd()); sb.Append("\n"); }
            }
            
            // Example of plsql file, why this was added
            // Blank lines between end of stored procedure and block terminator
            // ...
            // end;
            //
            //
            // /
            return sb.ToString().TrimEnd();;
        }
    }
}

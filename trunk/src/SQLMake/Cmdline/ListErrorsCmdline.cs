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
using SQLMake.Util;
using SQLMake.InstallFromScripts;

namespace SQLMake.Cmdline
{
    class ListErrorsCmdline
    {
        public static void ExecuteTask()
        {
            OracleSql db = new OracleSql();
            db.OpenMetaConnection(Settings.getUserId(true));

            List<SqlErrorObject> errorList = RegistryTable.listErrors(db);
            foreach (SqlErrorObject sqlErrorObject in errorList)
            {
                Console.WriteLine("");
                Console.WriteLine("PROMPT {0}", sqlErrorObject.sqlShortSummary);
                Console.WriteLine("REM Sequence id: {0}", sqlErrorObject.errorSeq);
                Console.WriteLine("REM {0}, Line no: {1}", sqlErrorObject.filename, sqlErrorObject.lineNo);
                Console.WriteLine("REM {0}", sqlErrorObject.errorMessage);
                Console.WriteLine(sqlErrorObject.sqlText);
                Console.WriteLine("/");
            }
        }
    }
}

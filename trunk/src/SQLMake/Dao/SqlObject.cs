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

namespace SQLMake.InstallFromScripts
{
    class SqlObject : IComparable
    {
        public string commandType; //Sqlplus, sql, plsql
        public string objectName;
        public string secondaryObjectName;
        public string action; //CREATE, DROP, ALTER ...
        public string objectType;
        public string filename;
        public int seqInFile;
        public int lineStart;
        public int lineEnd;
        public string sqlText;
        public bool isInstalled = false;

        public SqlObject(string commandType,
                         string objectName,
                         string secondaryObjectName,
                         string action,
                         string objectType,
                         string filename,
                         int seqInFile,
                         int lineStart,
                         int lineEnd,
                         string sqlText) 
        {
            this.commandType = commandType;
            this.objectName = objectName;
            this.secondaryObjectName = secondaryObjectName;
            this.action = action;
            this.objectType = objectType;
            this.filename = filename;
            this.seqInFile = seqInFile;
            this.lineStart = lineStart;
            this.lineEnd = lineEnd;
            this.sqlText = sqlText;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is SqlObject)
            {
                SqlObject otherSqlObject = (SqlObject)obj;
                if (this.objectName == otherSqlObject.objectName)
                    return this.secondaryObjectName.CompareTo(otherSqlObject.secondaryObjectName);
                else
                    return this.objectName.CompareTo(otherSqlObject.objectName);
            }
            else
            {
                throw new ArgumentException("object is not a PlsqlObject");
            }
        }

        #endregion
    }
}

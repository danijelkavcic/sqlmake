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
using System.IO;
using SQLMake.Util;
using System.Collections;

namespace SQLMake.PLSQLCode
{
    class GrantObject : IComparable
    {
        public string grantType; //system, object
        public string objectClause; //null, directory, java source, java resource
        public string grantee;
        public string tableName;
        public string grantor;
        public string privilege;
        public string adminOption;
        public string grantable;
        public string hierarchy;

        public string filename;
        public bool existsInSchema;
        public bool different = false;

        public string getAdminOption()
        {
            if (adminOption == "YES") return " WITH ADMIN OPTION";
            else return "";
        }


        
        public GrantObject(string p_grantType, string p_objectClause, string p_grantee, string p_tableName, string p_grantor, string p_privilege, string p_adminOption, string p_grantable, string p_hierarchy)
        {
            grantType = p_grantType.ToUpper().Trim();
            objectClause = p_objectClause.ToUpper().Trim();
            grantee = p_grantee.ToUpper().Trim();
            tableName = p_tableName.ToUpper().Trim();
            grantor = p_grantor.ToUpper().Trim();
            privilege = p_privilege.ToUpper().Trim();
            adminOption = p_adminOption.ToUpper().Trim();
            grantable = p_grantable.ToUpper().Trim();
            hierarchy = p_hierarchy.ToUpper().Trim();                        
            existsInSchema = true;
        }

        public GrantObject(string p_filename, string p_grantType, string p_objectClause, string p_grantee, string p_tableName, string p_grantor, string p_privilege, string p_adminOption, string p_grantable, string p_hierarchy)
        {
            filename = p_filename.Trim();
            grantType = p_grantType.ToUpper().Trim();
            objectClause = p_objectClause.ToUpper().Trim();
            grantee = p_grantee.ToUpper().Trim();
            tableName = p_tableName.ToUpper().Trim();
            grantor = p_grantor.ToUpper().Trim();
            privilege = p_privilege.ToUpper().Trim();
            adminOption = p_adminOption.ToUpper().Trim();
            grantable = p_grantable.ToUpper().Trim();
            hierarchy = p_hierarchy.ToUpper().Trim();                        

            existsInSchema = false;
        }

        public int CompareTo(object obj)
        {
            if (obj is GrantObject)
            {
                GrantObject otherFileItem = (GrantObject)obj;

                if (this.grantee != otherFileItem.grantee) return this.grantee.CompareTo(otherFileItem.grantee);
                if (this.tableName != otherFileItem.tableName) return this.tableName.CompareTo(otherFileItem.tableName);
                if (this.grantor != otherFileItem.grantor) return this.grantor.CompareTo(otherFileItem.grantor);
                if (this.privilege != otherFileItem.privilege) return this.privilege.CompareTo(otherFileItem.privilege);
                if (this.grantable != otherFileItem.grantable) return this.grantable.CompareTo(otherFileItem.grantable);
                if (this.hierarchy != otherFileItem.hierarchy) return this.tableName.CompareTo(otherFileItem.hierarchy);

                return 0;
            }
            else
            {
                throw new ArgumentException("object is not a GrantObject");
            }
        }

        // identifies 
        public override bool Equals(object obj)
        {
            GrantObject otherFileItem = (GrantObject)obj;

            if (this.tableName != otherFileItem.tableName) return false;
            // if (this.grantor != otherFileItem.grantor) return false;
            if (this.privilege != otherFileItem.privilege) return false;
            if (this.grantee != otherFileItem.grantee) return false;

            return true;
        }

        public override string ToString()
        {
            return this.grantType + ',' + this.objectClause + ',' + this.grantee + ',' + this.tableName + ',' + this.grantor + ',' + this.privilege + ',' + this.adminOption + ',' + this.grantable + ',' + this.hierarchy;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }

}

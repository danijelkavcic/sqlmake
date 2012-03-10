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

namespace SQLMake.PLSQLCode
{
    class SynonymObject : IComparable
    {
        public string synonymSchema;
        public string synonymName;
        public string synonymTarget;
        public bool isPublic;

        public string filename;
        public bool existsInSchema;
        public bool different = false;

        public SynonymObject(string p_synonymSchema, string p_synonymName, string p_synonymTarget, bool p_isPublic)
        {
            synonymSchema = p_synonymSchema.ToUpper().Trim();
            synonymName = p_synonymName.ToUpper().Trim();
            synonymTarget = p_synonymTarget.ToUpper().Trim();
            isPublic = p_isPublic;

            existsInSchema = true;
        }

        public SynonymObject(string p_filename, string p_synonymSchema, string p_synonymName, string p_synonymTarget, bool p_isPublic)
        {
            filename = p_filename.Trim();
            synonymSchema = p_synonymSchema.ToUpper().Trim();
            synonymName = p_synonymName.ToUpper().Trim();
            synonymTarget = p_synonymTarget.ToUpper().Trim();
            isPublic = p_isPublic;

            existsInSchema = false;
        }

        public int CompareTo(object obj)
        {
            if (obj is SynonymObject)
            {
                SynonymObject otherFileItem = (SynonymObject)obj;

                if (this.synonymSchema != otherFileItem.synonymSchema) return this.synonymSchema.CompareTo(otherFileItem.synonymSchema);
                if (this.synonymName != otherFileItem.synonymName) return this.synonymName.CompareTo(otherFileItem.synonymName);
                if (this.synonymTarget != otherFileItem.synonymTarget) return this.synonymTarget.CompareTo(otherFileItem.synonymTarget);
                if (this.isPublic != otherFileItem.isPublic) return this.isPublic.CompareTo(otherFileItem.isPublic);

                return 0;
            }
            else
            {
                throw new ArgumentException("object is not a SynonymObject");
            }
        }

        // identifies 
        public override bool Equals(object obj)
        {
            SynonymObject otherFileItem = (SynonymObject)obj;

            if (this.synonymName != otherFileItem.synonymName) return false;
            if (this.synonymTarget != otherFileItem.synonymTarget) return false;
            if (this.isPublic != otherFileItem.isPublic) return false;

            return true;
        }

        public override string ToString()
        {
            return this.synonymSchema + ',' + this.synonymName + ',' + this.synonymTarget + ',' + this.isPublic;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

    }
}

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
using System.Collections.Specialized;

namespace SQLMake.PLSQLCode
{
    class PlsqlObject : IComparable
    {
        public string objectName;
        public string objectType;
        public string filename;
        public bool   existsInSchema;
        public string file_md5;
        public string plsql_md5;
        public string plsqlText;
        public bool   filenameDifferent; // Filename is different from plsql objectname

        private long fileSize;
        private DateTime fileCreated;
        private DateTime fileModified;

        public PlsqlObject(string p_objectname, string p_objecttype, int p_objectId, DateTime p_objectCreated, DateTime p_objectLastDdlTime)
        {
            objectName = p_objectname.ToUpper();
            objectType = p_objecttype.ToUpper();
            existsInSchema = true;
            filenameDifferent = false;
        }

        public PlsqlObject(string p_filename, string p_objectname, string p_objecttype, string p_plsqlText)
        {
            filename = p_filename;
            objectName = p_objectname.ToUpper();
            objectType = p_objecttype.ToUpper();
            plsqlText = p_plsqlText;

            existsInSchema = false;

            // read file attributes
            FileInfo fi = new FileInfo(p_filename);

            fileSize = fi.Length;
            fileCreated = fi.CreationTime;
            fileModified = fi.LastWriteTime;


            // Check if filename is different from plsql object name inside
            string filenameObjectType = "";
            switch (Path.GetExtension(p_filename).ToUpper())
            {
                case ".PKS": filenameObjectType = "PACKAGE"; break;
                case ".PKB": filenameObjectType = "PACKAGE BODY"; break;
                case ".SPC": filenameObjectType = "PACKAGE"; break;
                case ".BDY": filenameObjectType = "PACKAGE BODY"; break;
                case ".PRC": filenameObjectType = "PROCEDURE"; break;
                case ".FNC": filenameObjectType = "FUNCTION"; break;
                case ".TRG": filenameObjectType = "TRIGGER"; break;
                case ".TPS": filenameObjectType = "TYPE"; break;
                case ".TPB": filenameObjectType = "TYPE BODY"; break;
            }

            filenameDifferent = false;
            if (Path.GetFileNameWithoutExtension(p_filename).ToUpper() != objectName ||
                objectType != filenameObjectType
                )
            {
                filenameDifferent = true;
                Log.Verbose("PlsqlObject", "Content {0} {1} does not match the name of the file {2}", objectType, objectName, p_filename);
            }
        }

/*        
        public PlsqlObject(string p_filename)
        {
            filename = p_filename;

            existsInSchema = false;
            switch (Path.GetExtension(p_filename).ToUpper())
            {
                case ".PKS": objectType = "PACKAGE"; break;
                case ".PKB": objectType = "PACKAGE BODY"; break;
                case ".SPC": objectType = "PACKAGE"; break;
                case ".BDY": objectType = "PACKAGE BODY"; break;
                case ".PRC": objectType = "PROCEDURE"; break;
                case ".FNC": objectType = "FUNCTION"; break;
                case ".TRG": objectType = "TRIGGER"; break;
                case ".TPS": objectType = "TYPE"; break;
                case ".TPB": objectType = "TYPE BODY"; break;
            }

            // read file attributes
            FileInfo fi = new FileInfo(p_filename);

            fileSize = fi.Length;
            fileCreated = fi.CreationTime;
            fileModified = fi.LastWriteTime;


            // Check if filename is different from plsql object name inside
            string plsqlObjectType = "";
            objectName = PlsqlMake.getObjectNameFromFile(p_filename, ref plsqlObjectType).ToUpper();
            filenameDifferent = false;
            if (Path.GetFileNameWithoutExtension(p_filename).ToUpper() != objectName ||
                objectType != plsqlObjectType
                )
            {
                filenameDifferent = true;
                objectType = plsqlObjectType;
                Log.Warnning("PlsqlObject", "Content {0} {1} does not match the name of the file {2}", objectType, objectName, p_filename);
            }
        }
*/
        public string getExtFromObjectType()
        {
            switch (objectType)
            {
                case "PACKAGE": return ".PKS";
                case "PACKAGE BODY": return ".PKB";
                case "PROCEDURE": return ".PRC";
                case "FUNCTION": return ".FNC";
                case "TRIGGER": return ".TRG";
                case "TYPE": return ".TPS";
                case "TYPE BODY": return ".TPB";
            }
            return ".UNKNOWN";
        }

        public int CompareTo(object obj)
        {
            if (obj is PlsqlObject)
            {
                PlsqlObject otherFileItem = (PlsqlObject)obj;
                if (this.objectType == otherFileItem.objectType)
                {
                    return this.objectName.CompareTo(otherFileItem.objectName);
                }
                else
                {
                    Settings.getInstallOrder(true);
                    return Settings.getInstallOrderSeq(this.objectType) - Settings.getInstallOrderSeq(otherFileItem.objectType);
                }
                
            }
            else
            {
                throw new ArgumentException("object is not a PlsqlObject");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is PlsqlObject)
            {
                PlsqlObject otherFileItem = (PlsqlObject)obj;
                if (this.objectType == otherFileItem.objectType) return this.objectName.Equals(otherFileItem.objectName);
                else return false;
            }
            else
            {
                throw new ArgumentException("object is not a PlsqlObject");
            }
        }

        public override int GetHashCode()
        {
            return this.objectName.GetHashCode();
        }
    }

}

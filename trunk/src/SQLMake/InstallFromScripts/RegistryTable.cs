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
using System.Data.OracleClient;
using SQLMake.Util;

namespace SQLMake.InstallFromScripts
{
    /* Creates or upgrades setup table to current version */
    class RegistryTable
    {
        static int sqlmakeRegistryTableVersion = 2;
        static string sqlmakeRegistryTableName = "sqlmake";
        static string ddl_sqlmakeRegistryComment = "comment on table " + sqlmakeRegistryTableName + " is 'Sqlmake registry table (version=" + sqlmakeRegistryTableVersion + ")'";

        static string ddl_sqlmakeRegistryTable =
            "create table " + sqlmakeRegistryTableName + " (" +
            "  path     varchar2(250) not null," +
            "  name     varchar2(30)  not null," +
            "  value    varchar2(30)  not null," +
            "  sdate_i  date          not null," +
            "  suser_i  varchar2(100) not null," +
            "  sdate_u  date          null," +
            "  suser_u  varchar2(100) null," +
            "  value_v1 varchar2(800) null," +
            "  value_v2 varchar2(800) null," +
            "  value_c1 clob          null," +
            "  value_c2 clob          null," +
            "  value_n1 number        null," +
            "  value_n2 number        null)";

        static string ddl_sqlmakeRegistryPk =
          "ALTER TABLE " + sqlmakeRegistryTableName + " " +
          "ADD CONSTRAINT " + sqlmakeRegistryTableName + "_pk PRIMARY KEY (path, name, value)";

        static string insertInitialDatamodelRow =
          "insert into " + sqlmakeRegistryTableName + " (path, name, value, sdate_i, suser_i)" +
          "values ('VERSION', 'INSTALL', '0', sysdate, '" + Environment.UserName + "@" + Environment.MachineName + "')";

        static string insertInitialPlsqlRow =
          "insert into " + sqlmakeRegistryTableName + " (path, name, value, sdate_i, suser_i)" +
          "values ('BUILD', 'NUMBER', '-1', sysdate, '" + Environment.UserName + "@" + Environment.MachineName + "')";

        // compare for eg. 1.2.3.4 to 4.6
        public static int compareVersion(string a, string b)
        {
            string[] aVer = a.Split('.');
            string[] bVer = b.Split('.');

            int index = 0;
            foreach (string num in aVer)
            {
                //check if bVer is out of bounds
                if (bVer.Length < index + 1) return 1;

                int aNum;
                if (!int.TryParse(aVer[index], out aNum))
                {
                    Log.Error("compareVersion", "Version string {0} must be made of numbers only", a);
                    Log.ExitError("");
                }
                int bNum;
                if (!int.TryParse(bVer[index], out bNum))
                {
                    Log.Error("compareVersion", "Version string {0} must be made of numbers only", b);
                    Log.ExitError("");
                }

                if (aNum > bNum) return 1;
                else if (aNum < bNum) return -1;

                index++;
            }

            // check if bVer is longer
            if (bVer.Length > index) return -1;

            // both strings are equal 
            return 0;
        }
        
        static int getRegistryTableVersion(OracleConnection conn)
        {
            //check if registry table is already created in schema
            string cmdQuery = "select count(*) from user_tables where table_name = :name";
            OracleCommand cmd = new OracleCommand(cmdQuery, conn);
            OracleParameter Param1 = cmd.Parameters.Add("name", OracleType.VarChar);
            Param1.Direction = System.Data.ParameterDirection.Input;
            Param1.Value = sqlmakeRegistryTableName.ToUpper();
            Int32 tableCount = Convert.ToInt32(cmd.ExecuteScalar());

            // Clean up
            cmd.Dispose();        
            
            // registry table does not exist, bail out
            if (tableCount == 0) return -1;

            //get table version from table comment
            cmdQuery = "select comments from user_tab_comments where table_name = :name";
            cmd = new OracleCommand(cmdQuery, conn);
            Param1 = cmd.Parameters.Add("name", OracleType.VarChar);
            Param1.Direction = System.Data.ParameterDirection.Input;
            Param1.Value = sqlmakeRegistryTableName.ToUpper();
            string tableComment = (string)cmd.ExecuteScalar();

            // Clean up
            cmd.Dispose();
            int tableVersion = int.Parse(tableComment.Substring(tableComment.IndexOf('=')+1, tableComment.IndexOf(')') - tableComment.IndexOf('=') - 1));
            return tableVersion;            
        }

        void installRegistryTables(OracleSql db)
        {
            db.Prompt("Creating sqlmake registry table ...");
            db.Exec(ddl_sqlmakeRegistryTable, "Install setup tables", "Create registry table");
            db.Exec(ddl_sqlmakeRegistryPk, "Install setup tables", "Create registry table primary key");
            db.Exec(ddl_sqlmakeRegistryComment, "Install setup tables", "Create comment on registry table");
            db.Exec(insertInitialDatamodelRow, "Install setup tables", "Insert initial datamodel row");
            db.Exec(insertInitialPlsqlRow, "Install setup tables", "Insert initial plsql row");
            db.Exec("commit", "Install setup tables", "Commit");
        }

        static void upgradeRegistryTables(OracleSql db)
        {
            int tableVersion = getRegistryTableVersion(db.GetMetaConnection());
            if (tableVersion < 2)
            {
                db.Exec(insertInitialPlsqlRow, "Upgrade setup tables", "Insert initial plsql row");
                db.Exec(ddl_sqlmakeRegistryComment, "Upgrade setup tables", "Create comment on registry table");
                db.Exec("commit", "Upgrade setup tables", "Commit");
            }
        }

        public void setDatamodelVersion(OracleSql db, string version)
        {
            db.Exec("update " + sqlmakeRegistryTableName + " " +
                    "set value = " + version + 
                    ",sdate_u = sysdate" +
                    ",suser_u = '" + Environment.UserName + "@" + Environment.MachineName + "' " +
                    "where path = 'VERSION' and name = 'INSTALL'", "Write setup tables", "New datamodel version");
            db.Exec("commit", "Write setup tables", "Commit");
        }

        public void setBuildNumber(OracleSql db, string version)
        {
            db.Prompt("Setting datamodel version to " + version + "...");
            db.Exec("update " + sqlmakeRegistryTableName + " " +
                    "set value = " + version +
                    ",sdate_u = sysdate" +
                    ",suser_u = '" + Environment.UserName + "@" + Environment.MachineName + "' " +
                    "where path = 'BUILD' and name = 'NUMBER'", "Write setup tables", "New plsql version");
            db.Exec("commit", "Write setup tables", "Commit");
        }

        public void addNewUpgrade(OracleSql db, string version, int error_count, string scriptName)
        {
            db.Prompt("Setting datamodel version to " + version + "...");
            db.Exec(
                "insert into " + sqlmakeRegistryTableName + " (path, name, value, sdate_i, suser_i, value_v1)" +
                "values ('VERSION', 'UPGRADE', '" + version + "', sysdate, '" + Environment.UserName + "@" + Environment.MachineName + "' " + ", '" + scriptName + "')",
                "Write setup tables", "Version upgrade");
            db.Exec("commit", "Write setup tables", "Commit");
        }

        // Datamodel version can be 1.2.3.4
        // Therefore order by is complicated to correctly sort by each part of version number
        public static string getDatamodelVersion(OracleSql db)
        {
            string datamodelVersion = "-1";
            if (db.GetMetaConnection() != null)
            {
                string cmdQuery = "select value\n" +
                "from\n" +
                "(\n" +
                "select value from " + sqlmakeRegistryTableName + " where path = 'VERSION'\n" +
                "order by\n" +
                "       to_number(substr(value,1,decode(instr(value,'.',1,1),0,length(value),instr(value,'.',1,1)-1))) desc nulls last,\n" +
                "        to_number(decode(instr(value,'.',1,1),0,null,substr(value,instr(value,'.',1,1)+1,decode(instr(value,'.',1,2),0,length(value),instr(value,'.',1,2)-instr(value,'.',1,1)-1)))) desc nulls last,\n" +
                "        to_number(decode(instr(value,'.',1,2),0,null,substr(value,instr(value,'.',1,2)+1,decode(instr(value,'.',1,3),0,length(value),instr(value,'.',1,3)-instr(value,'.',1,2)-1)))) desc nulls last,\n" +
                "        to_number(decode(instr(value,'.',1,3),0,null,substr(value,instr(value,'.',1,3)+1,decode(instr(value,'.',1,4),0,length(value),instr(value,'.',1,4)-instr(value,'.',1,3)-1)))) desc nulls last,\n" +
                "        to_number(decode(instr(value,'.',1,4),0,null,substr(value,instr(value,'.',1,4)+1,decode(instr(value,'.',1,5),0,length(value),instr(value,'.',1,5)-instr(value,'.',1,4)-1)))) desc nulls last,\n" +
                "        to_number(decode(instr(value,'.',1,5),0,null,substr(value,instr(value,'.',1,5)+1,decode(instr(value,'.',1,6),0,length(value),instr(value,'.',1,6)-instr(value,'.',1,5)-1)))) desc nulls last\n" +
                ")\n" +
                " where rownum = 1";

                OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
                datamodelVersion = Convert.ToString(cmd.ExecuteScalar());
                cmd.Dispose();                        
            }
            return datamodelVersion;
        }

        public static string getBuildNumber(OracleSql db)
        {
            string buildNumber = "-1";
            if (db.GetMetaConnection() != null)
            {
                string cmdQuery = "select to_number(value) value from " + sqlmakeRegistryTableName + " where path = 'BUILD' and name = 'NUMBER'";
                OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
                buildNumber = Convert.ToString(cmd.ExecuteScalar());
                cmd.Dispose();
                if (buildNumber == "") buildNumber = "-1";
            }
            return buildNumber;
        }

        public static int getErrorCount(OracleSql db, string version)
        {
            Int32 error_count = 0;
            if (db.GetMetaConnection() != null)
            {
                string cmdQuery = "select count(*) from " + sqlmakeRegistryTableName + " where path = 'ERRORS." + version + "'";
                OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
                error_count = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            return error_count;
        }

        public static int getClearedCount(OracleSql db, string version)
        {
            Int32 error_count = 0;
            if (db.GetMetaConnection() != null)
            {
                string cmdQuery = "select count(*) from " + sqlmakeRegistryTableName + " where path = 'CLEARED." + version + "'";
                OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
                error_count = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            return error_count;
        }


        public static void clearErrors(OracleSql db)
        {
            string version = getDatamodelVersion(db);
            Int32 error_count = 0;
            if (db.GetMetaConnection() != null)
            {
                string cmdQuery = "update " + sqlmakeRegistryTableName + " set path = 'CLEARED." + version + "', sdate_u=sysdate, suser_u='" + Environment.UserName + "@" + Environment.MachineName  + "' where path = 'ERRORS." + version + "'";
                OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
                error_count = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            if (error_count == 0) { Log.ExitError("No errors to clear"); }
            else Log.Info("clearErrors", "{0} error(s) cleared", error_count);
        }

        public static void clearError(OracleSql db, int seqNo)
        {
            string version = getDatamodelVersion(db);
            Int32 error_count = 0;
            if (db.GetMetaConnection() != null)
            {
                string cmdQuery = "update " + sqlmakeRegistryTableName + " set path = 'CLEARED." + version + "', sdate_u=sysdate, suser_u='" + Environment.UserName + "@" + Environment.MachineName + "' where path = 'ERRORS." + version + "' and value = " + seqNo;
                OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
                error_count = cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            if (error_count == 0) { Log.ExitError("No error to clear"); }
            else Log.Info("clearError", "Error with sequence number {0} cleared", seqNo);
        }        

        public void addError(OracleSql db, string version, int sequenceNo, string scriptName, int lineNo, string errm, string sqlText, string sqlShortSummary)
        {
            string cmdInsert = "insert into " + sqlmakeRegistryTableName + " (path, name, value, sdate_i, suser_i, value_v1, value_v2, value_n1, value_c1, value_c2) " +
                               "values (:path, 'SEQ', :value, sysdate, :suser_i, :value_v1, :value_v2, :value_n1, :value_c1, :value_c2)";
            OracleCommand cmd = new OracleCommand(cmdInsert, db.GetMetaConnection());
            cmd.Parameters.AddWithValue("path", "ERRORS."+version.ToString());
            cmd.Parameters.AddWithValue("value", sequenceNo.ToString());
            cmd.Parameters.AddWithValue("suser_i", Environment.UserName + "@" + Environment.MachineName);
            cmd.Parameters.AddWithValue("value_v1", scriptName);
            cmd.Parameters.AddWithValue("value_v2", sqlShortSummary);
            cmd.Parameters.AddWithValue("value_n1", lineNo);
            cmd.Parameters.AddWithValue("value_c1", errm.Trim());
            cmd.Parameters.AddWithValue("value_c2", sqlText.Trim());

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            //db.Exec(
            //    "insert into " + sqlmakeRegistryTableName + " (path, name, value, sdate_i, suser_i, value_v1, value_n1, value_c1)" +
            //    String.Format("values ('ERRORS.{0}', 'ERROR', '{4}', sysdate, '{2}', '{3}', {5}, '{6}')", 
            //    version, errm.Substring(0, 9), Environment.UserName + "@" + Environment.MachineName, scriptName, sequenceNo, lineNo, errm.TrimEnd()),
            //    "Write setup tables", "Add error");
            //db.Exec("commit", "Write setup tables", "Commit");
        }

        public void checkRegistryTables(OracleSql db)
        {
            int tableVersion = -1;
            if (db.GetMetaConnection() != null) tableVersion = getRegistryTableVersion(db.GetMetaConnection());
            if (tableVersion == -1) installRegistryTables(db);
            if (tableVersion > sqlmakeRegistryTableVersion)
            {
                Log.Warning("checkRegistryTables", "You are using an old version of sqlmake executable.");
                Log.Warning("checkRegistryTables", "Target schema was installed with a newer verions.");
                Log.ExitError("Please upgrade to a newer version.");
            }
            if (tableVersion > sqlmakeRegistryTableVersion) upgradeRegistryTables(db);
        }

        public static List<SqlErrorObject> listErrors(OracleSql db)
        {
            string version = getDatamodelVersion(db);
            string cmdQuery = "select * from sqlmake where path = 'ERRORS."+version+"' order by to_number(value)";
            OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
            OracleDataReader r = cmd.ExecuteReader();

            List<SqlErrorObject> sqlList = new List<SqlErrorObject>();

            while (r.Read())
            {
                sqlList.Add(new SqlErrorObject(int.Parse(r.GetString(2)), r.GetString(7), r.GetString(8), r.GetInt32(11), r.GetString(9), r.GetString(10)));                   
            }
            
            return sqlList;
        }
        
        public static void status(OracleSql db)
        {
            // check for registry table
            int regTableVersion = getRegistryTableVersion(db.GetMetaConnection());
            if (regTableVersion == -1)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Log.Warning("status", "Target schema is not using sqlmake tool");
                Log.Warning("status", "No status available");
                Console.ResetColor();
                Log.ExitError("");
            }

            string dbModelVersion = getDatamodelVersion(db);
            string dbBuildNumber = getBuildNumber(db);
            int errorCount = getErrorCount(db, dbModelVersion);
            int clearedCount = getClearedCount(db, dbModelVersion);
            List<string> invalidObjects = db.GetInvalidObjectsList();

            if (dbModelVersion == "-1") Log.Warning("status", "Current data model version   : Unknown");
            else if (dbModelVersion == "0") Log.Warning("status", "Current data model version   : Last installation procedure was terminted");
            else Log.Info("status", "Current data model version   : {0}", dbModelVersion);

            //if (dbBuildNumber == "-1") Log.Warning("status", "Current build number         : Unknown");
            //else if (dbBuildNumber == "0") Log.Warning("status", "Current build number         : Last build deployment was terminted");
            //else Log.Info("status", "Current build number         : {0}", dbBuildNumber);

            Log.Error("status", "Errors during last deployment: {0}", errorCount);
            if (clearedCount > 0) Log.Info("status", "Errors cleared after install : {0}", clearedCount);
            Log.Error("status", "Invalid object(s) found      : {0}", invalidObjects.Count);
            foreach (string invalidObject in invalidObjects) Log.Error("status", "   "+invalidObject);

            if (errorCount > 0 || invalidObjects.Count > 0)
            {
                Log.ExitError("Schema deployment is invalid");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Log.Info("status", "Installation successful");
                Console.ResetColor();
            }
        }
    }
}

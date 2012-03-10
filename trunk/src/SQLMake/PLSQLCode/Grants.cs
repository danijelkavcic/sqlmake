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
using SQLMake.InstallFromScripts;
using System.Data.OracleClient;
using System.Collections;
using SQLMake.Util;
using System.Text.RegularExpressions;
using System.IO;

namespace SQLMake.PLSQLCode
{
    class Grants
    {
        static public string appendDotIfNotEmpty(string text)
        {
            if (text == "") return text;
            return text + ".";
        }
        static public string replaceWithSandbox(string schemaName)
        {
            if (schemaName == "") return "";
            if (Settings.getSandboxPattern(false) == "") return schemaName;
            return Settings.getSandboxPattern(false).Replace("%", schemaName);
        }

        static public ArrayList parseGrantSql(string p_sql, string p_filename)
        {
            SQLPlusScanner scanner = new SQLPlusScanner(new StringReader(p_sql), "parseGrantSql");
            string currGrantType = "";
            string currObjectClause = "";
            string currGrantor = "";
            string currTableName = "";
            string currAdminOption = "NO";
            string currGrantable = "NO";
            string currHierarchy = "NO";
            bool complete = false;
            StringBuilder grantPrivileges = new StringBuilder();
            StringBuilder grantees = new StringBuilder();

            try
            {
                string currentToken = scanner.getSkipComments();
                if (currentToken != "GRANT") throw new ArgumentException("Sql does not contain grant statement");

                while (true)
                {
                    currentToken = scanner.getSkipComments();
                    if (currentToken == "ON" || currentToken == "TO") break;
                    if (currentToken != "," && grantPrivileges.Length > 0) grantPrivileges.Append(" ");
                    grantPrivileges.Append(currentToken);
                }

                if (grantPrivileges.ToString().Trim() == "")
                    Log.Warning("GrantObject", "Incorrect grant, was expecting grant privileges: {0}", p_sql);

                if (currentToken == "ON") currGrantType = "OBJECT";
                else currGrantType = "SYSTEM";

                // read objectName
                if (currGrantType == "OBJECT")
                {
                    currentToken = scanner.getSkipComments();
                    if (currentToken == "DIRECTORY")
                    {
                        currObjectClause = currentToken;
                        currentToken = scanner.getSkipComments();
                    }
                    else if (currentToken == "JAVA")
                    {
                        currObjectClause = currentToken;
                        currentToken = scanner.getSkipComments();
                        if (currentToken != "SOURCE" || currentToken != "RESOURCE")
                            SQLMake.Util.Log.Warning("GrantObject", "Incorrect grant, was expecting keyword SOURCE|RESOURCE: {0}", p_sql);
                        currObjectClause += " " + currentToken;
                        currentToken = scanner.getSkipComments();
                    }

                    if (currentToken.IndexOf('.') > -1)
                    {
                        currGrantor = currentToken.Split('.')[0];
                        currTableName = currentToken.Split('.')[1];
                    }
                    else
                    {
                        currTableName = currentToken;
                    }
                    currentToken = scanner.getSkipComments();
                }

                if (currentToken != "TO")
                    Log.Warning("GrantObject", "Incorrect grant, was expecting keyword TO: {0}", p_sql);

                // read grantees part
                while (true)
                {
                    currentToken = scanner.getSkipComments();
                    grantees.Append(currentToken);
                    complete = true;
                    currentToken = scanner.getSkipComments();
                    if (currentToken != ",") break;
                    else grantees.Append(currentToken);
                }

                // read with admin clause
                if (currGrantType == "SYSTEM")
                {
                    currentToken += " " + scanner.getSkipComments() + " " + scanner.getSkipComments();
                    if (currentToken == "WITH ADMIN OPTION") currAdminOption = "YES";
                    else Log.Warning("GrantObject", "Incorrect grant, was expecting keyword WITH ADMIN OPTION: {0}", p_sql);
                }
                else
                {
                    currentToken += " " + scanner.getSkipComments() + " " + scanner.getSkipComments();
                    if (currentToken == "WITH GRANT OPTION") currGrantable = "YES";
                    else if (currentToken == "WITH HIERARCHY OPTION") currHierarchy = "YES";
                    else Log.Warning("GrantObject", "Incorrect grant, was expecting keyword WITH GRANT OPTION|WITH HIERARCHY OPTION: {0}", p_sql);

                    currentToken = scanner.getSkipComments();
                    currentToken += " " + scanner.getSkipComments() + " " + scanner.getSkipComments();
                    if (currentToken == "WITH GRANT OPTION") currGrantable = "YES";
                    else if (currentToken == "WITH HIERARCHY OPTION") currHierarchy = "YES";
                    else Log.Warning("GrantObject", "Incorrect grant, was expecting keyword WITH GRANT OPTION|WITH HIERARCHY OPTION: {0}", p_sql);
                }

                currentToken = scanner.getSkipComments();
                Log.Warning("GrantObject", "Incorrect grant, keywords found after expected end of grant clause: {0}", p_sql);
            }
            catch (EOFException)
            {
                if (!complete) Log.Warning("GrantObject", "Incorrect grant, premature termination of sql statement: {0}", p_sql);
            }
            catch (EOBException)
            {
                if (!complete) Log.Warning("GrantObject", "Incorrect grant, premature termination of sql statement: {0}", p_sql);
            }


            ArrayList resultSet = new ArrayList();
            if (complete)
            {
                foreach (string grantPrivilege in grantPrivileges.ToString().Split(','))
                {
                    foreach (string grantee in grantees.ToString().Split(','))
                    {
                        GrantObject grantObject = new GrantObject(p_filename, currGrantType, currObjectClause, replaceWithSandbox(grantee.Trim()), currTableName, replaceWithSandbox(currGrantor.Trim()), grantPrivilege, currAdminOption, currGrantable, currHierarchy);
                        // Console.WriteLine(grantObject.ToString());
                        resultSet.Add(grantObject);
                    }
                }
            }
            return resultSet;

        }

        static public ArrayList buildObjectGrantsList(OracleConnection conn, string p_object)
        {
            ArrayList grantsList = new ArrayList();
            // Read all entries from user_tab_privs_made
            string cmdQuery = "select grantee, table_name, grantor, privilege, grantable, hierarchy from user_tab_privs_made where table_name = :p_object";
            OracleCommand cmd = new OracleCommand(cmdQuery, conn);
            OracleParameter Param2 = cmd.Parameters.Add("p_object", OracleType.VarChar);
            Param2.Direction = System.Data.ParameterDirection.Input;
            Param2.Value = p_object;

            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                GrantObject currObject = new GrantObject("OBJECT", "", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), "NO", reader.GetString(4), reader.GetString(5));
                grantsList.Add(currObject);
            }
            return grantsList;
        }

        static public void executeGrantList(OracleSql db, ArrayList grantsList)
        {
            foreach (GrantObject obj in grantsList)
            {
                if (obj.grantType == "OBJECT")
                    db.Exec(String.Format("grant {0} on {3} to {1} {2}", obj.privilege, obj.grantee, obj.getAdminOption(), obj.tableName), "grants", "executeGrantList");
                else
                    Log.Warning("grants", "Non object grants currently not suported in executeGrantList: {0}", obj.ToString());
            }
        }

        
        static public ArrayList buildGrantsList(string p_dir, OracleConnection conn)
        {
            Log.Info("grants", "Processing scripts ... {0}", p_dir);
            // First read all files from the file system
            string configSqlFileList = Settings.getSqlFileList(true);
            string configIgnoreDirList = Settings.getIgnoreDirList(true);
            string configIgnoreFileList = Settings.getIgnoreFileList(true);

            Log.Verbose("grants", "Searching for sql scripts");
            string[] files = FolderSearch.Search(p_dir, true, configSqlFileList, configIgnoreDirList, configIgnoreFileList);

            ArrayList fileList = new ArrayList();
            // each file can contain 0 or more blocks
            foreach (string filename in files)
            {
                ArrayList sqlCommandList = new ArrayList(SqlScript.Load(filename));
                foreach (SqlObject sqlCommand in sqlCommandList)
                {
                    if (sqlCommand.commandType == "SQL" && sqlCommand.action == "GRANT") 
                        fileList.AddRange(parseGrantSql(sqlCommand.sqlText, filename));
                }
            }

            // TODO: Check for duplicate grants

            Console.WriteLine("Processing database catalog ...");
            // Read all entries from user_tab_privs_made
            // Carefull about recycle bin objects (filter using cross check on user objects view)
            string cmdQuery = "select grantee, table_name, grantor, privilege, grantable, hierarchy from user_tab_privs_made where exists (select 1 from user_objects where table_name = object_name)";
            OracleCommand cmd = new OracleCommand(cmdQuery, conn);
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                GrantObject currObject = new GrantObject("OBJECT", "", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), "NO", reader.GetString(4), reader.GetString(5));
                int existingObjectIndex = fileList.IndexOf(currObject);

                // This object does not exist in arrayList
                if (existingObjectIndex == -1) fileList.Add(currObject);

                // because of duplicates loop through all occurences
                while (existingObjectIndex >= 0)
                {
                    GrantObject existingFileItem = (GrantObject)fileList[existingObjectIndex];
                    existingFileItem.existsInSchema = true;
                    existingObjectIndex = fileList.IndexOf(currObject, existingObjectIndex + 1);
                }
            }

            // Read all entries from user_sys_privs
            cmdQuery = "select username, privilege, admin_option from user_sys_privs";
            bool objectFound;
            cmd = new OracleCommand(cmdQuery, conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                GrantObject currObject = new GrantObject("SYSTEM", "", reader.GetString(0), "", "", reader.GetString(1), reader.GetString(2), "NO", "NO");
                int existingObjectIndex = fileList.IndexOf(currObject);
                objectFound = false;

                // This object does not exist in arrayList
                if (existingObjectIndex == -1 && !objectFound) fileList.Add(currObject);

                while (existingObjectIndex >= 0)
                {
                    objectFound = true;
                    ((GrantObject)fileList[existingObjectIndex]).existsInSchema = true;
                    ((GrantObject)fileList[existingObjectIndex]).different = !((GrantObject)fileList[existingObjectIndex]).Equals(currObject);
                    existingObjectIndex = fileList.IndexOf(currObject, existingObjectIndex + 1);
                }
            }


            return fileList;
        }


        static public void RunSystemGrants(OracleSql db, string p_dir)
        {
            Log.Info("grants", "Loading grants command list");
            ArrayList list = buildGrantsList(p_dir, db.GetMetaConnection());
            Log.Info("grants", "Sorting command list");
            list.Sort();

            Log.Info("grants", "Reconcile system grants");
            Console.WriteLine("Reconciling system grants");
            bool missingIndicator = false;
            string sqlGrant = "";
            int errCount = 0;
            
            foreach (GrantObject obj in list)
            {
                if (obj.grantType == "SYSTEM")
                {
                    if (obj.filename != null && obj.existsInSchema == false)
                    {
                        missingIndicator = true;
                        sqlGrant = String.Format("grant {0} to {1} {2}", obj.privilege, obj.grantee, obj.getAdminOption());
                        errCount += db.Exec(sqlGrant, "grants", "grant system privileege");
                    }
                    if (obj.filename == null && obj.existsInSchema == true)
                    {
                        missingIndicator = true;
                        sqlGrant = String.Format("revoke {0} from {1}", obj.privilege, obj.grantee);
                        errCount += db.Exec(sqlGrant, "grants", "revoke system privileege");
                    }
                    if (obj.different)
                    {
                        missingIndicator = true;
                        sqlGrant = String.Format("revoke {0} from {1}", obj.privilege, obj.grantee);
                        errCount += db.Exec(sqlGrant, "grants", "revoke system privileege");

                        sqlGrant = String.Format("grant {0} to {1} {2}", obj.privilege, obj.grantee, obj.getAdminOption());
                        errCount += db.Exec(sqlGrant, "grants", "grant system privileege");
                    }

                }
            }
            if (!missingIndicator) Console.WriteLine("    No changes necessary");
            if (errCount > 0)
            {
                Log.Error("grants", "Error(s) found while reconciling system grants");
                Log.ExitError("System grants reconcile failed");
            }
        }

        static public void RunObjectGrants(OracleSql db, string p_dir)
        {
            Log.Verbose("grants", "Loading grants command list");
            ArrayList list = buildGrantsList(p_dir, db.GetMetaConnection());
            Log.Verbose("grants", "Sorting command list");
            list.Sort();

            Log.Info("grants", "Reconciling object grants");
            bool missingIndicator = false;
            string sqlGrant = "";
            int errCount = 0;

            foreach (GrantObject obj in list)
            {
                if (obj.grantType == "OBJECT")
                {
                    if (obj.filename != null && obj.existsInSchema == false)
                    {
                        missingIndicator = true;
                        sqlGrant = String.Format("grant {0} on {4}{1} to {2}{3}", obj.privilege, obj.tableName, obj.grantee, obj.getAdminOption(), appendDotIfNotEmpty(obj.grantor));
                        errCount += db.Exec(sqlGrant, "grants", "grant privileege");
                    }
                    if (obj.filename == null && obj.existsInSchema == true)
                    {
                        missingIndicator = true;
                        sqlGrant = String.Format("revoke {0} on {1} from {2}", obj.privilege, obj.tableName, obj.grantee);
                        errCount += db.Exec(sqlGrant, "grants", "revoke privileege");

                    }
                    if (obj.different)
                    {
                        missingIndicator = true;
                        //Console.WriteLine("    revoke {0} on {1} from {2};", obj.privilege, obj.tableName, obj.grantee);
                        //Console.WriteLine("    grant {0} on {1} to {2}{3};", obj.privilege, obj.tableName, obj.grantee, obj.getAdminOption());

                        sqlGrant = String.Format("revoke {0} on {1} from {2}", obj.privilege, obj.tableName, obj.grantee);
                        errCount += db.Exec(sqlGrant, "grants", "revoke privileege");

                        sqlGrant = String.Format("grant {0} on {1} to {2}{3}", obj.privilege, obj.tableName, obj.grantee, obj.getAdminOption());
                        errCount += db.Exec(sqlGrant, "grants", "grant privileege");
                    }

                }
            }
            if (!missingIndicator) Console.WriteLine("    No changes necessary");

            if (errCount > 0)
            {
                Log.Error("grants", "Error(s) found while reconciling object grants");
                Log.ExitError("Grants reconcile failed");
            }
        }


    }
}

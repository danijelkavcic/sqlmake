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
using System.Collections;
using System.Data.OracleClient;
using SQLMake.Util;
using System.IO;
using SQLMake.InstallFromScripts;

namespace SQLMake.PLSQLCode
{
    class Synonym
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

        static public SynonymObject parseSynonymSql(string p_sql, string p_filename)
        {
            SQLPlusScanner scanner = new SQLPlusScanner(new StringReader(p_sql), "parseSynonymSql");
            string synonymSchema = "";
            string synonymName = "";
            string synonymTarget = "";
            bool isPublic = false;

            bool complete = false;

            try
            {
                string currentToken = scanner.getSkipComments();
                if (currentToken == "CREATE") currentToken = scanner.getSkipComments();
                if (currentToken == "OR")
                {
                    currentToken = scanner.getSkipComments();
                    if (currentToken != "REPLACE") throw new ArgumentException("REPLACE expected after CREATE OR tokens");
                    currentToken = scanner.getSkipComments();
                }

                isPublic = false;
                if (currentToken == "PUBLIC")
                {
                    isPublic = true;
                    currentToken = scanner.getSkipComments();
                }

                if (currentToken != "SYNONYM") throw new ArgumentException("Sql does not contain SYNONYM statement");
                currentToken = scanner.getSkipComments();

                // expecting schema.synonymName
                if (currentToken.IndexOf('.') > -1)
                {
                    synonymSchema = currentToken.Split('.')[0];
                    synonymName = currentToken.Split('.')[1];
                }
                else
                {
                    synonymSchema = "";
                    synonymName = currentToken;
                }

                currentToken = scanner.getSkipComments();
                // expecting FOR
                if (currentToken != "FOR") throw new ArgumentException("FOR statement expected in synonym statement");

                // target schema: schema.object@db_link
                // we have to convert schema to sandbox name
                currentToken = scanner.getSkipComments();
                if (currentToken.IndexOf('.') > -1)
                {
                    synonymTarget = replaceWithSandbox(currentToken.Split('.')[0].Trim()) + "." + currentToken.Split('.')[1];
                }
                else
                {
                    synonymTarget = currentToken;
                }


                complete = true;

                // this should throw exception as we should be at the end
                currentToken = scanner.getSkipComments();
            }
            catch (EOFException)
            {
                if (!complete) Log.Warning("SynonymObject", "Incorrect synonym, premature termination of sql statement: {0}", p_sql);
            }
            catch (EOBException)
            {
                if (!complete) Log.Warning("GrantObject", "Incorrect synonym, premature termination of sql statement: {0}", p_sql);
            }

            return new SynonymObject(p_filename, synonymSchema, synonymName, synonymTarget, isPublic);
        }

        static public ArrayList buildSynonymsList(string p_dir, OracleConnection conn)
        {
            Console.WriteLine("Processing scripts ... {0}", p_dir);
            // First read all files from the file system
            string configSqlFileList = Settings.getSqlFileList(true);
            string configIgnoreDirList = Settings.getIgnoreDirList(true);
            string configIgnoreFileList = Settings.getIgnoreFileList(true);

            Log.Info("grants", "Searching for sql scripts");
            string[] files = FolderSearch.Search(p_dir, true, configSqlFileList, configIgnoreDirList, configIgnoreFileList);

            ArrayList fileList = new ArrayList();
            // each file can contain 0 or more blocks
            foreach (string filename in files)
            {
                ArrayList sqlCommandList = new ArrayList(SqlScript.Load(filename));
                foreach (SqlObject sqlCommand in sqlCommandList)
                {
                    if (sqlCommand.commandType == "SQL" && sqlCommand.objectType == "SYNONYM")
                        fileList.Add(parseSynonymSql(sqlCommand.sqlText, filename));
                }
            }

            // TODO: Check for duplicate synonyms

            Console.WriteLine("Processing database catalog ...");
            // Read all entries from user_tab_privs_made
            string cmdQuery = "select user, synonym_name, table_owner||'.'||table_name||nvl2(db_link,'@','')||db_link targetName from user_synonyms";
            OracleCommand cmd = new OracleCommand(cmdQuery, conn);
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                SynonymObject currObject = new SynonymObject(reader.GetString(0), reader.GetString(1), reader.GetString(2), false);
                int existingObjectIndex = fileList.IndexOf(currObject);

                // This object does not exist in arrayList
                if (existingObjectIndex == -1) fileList.Add(currObject);

                // because of duplicates loop through all occurences
                while (existingObjectIndex >= 0)
                {
                    SynonymObject existingFileItem = (SynonymObject)fileList[existingObjectIndex];
                    existingFileItem.existsInSchema = true;
                    existingObjectIndex = fileList.IndexOf(currObject, existingObjectIndex + 1);
                }
            }

            return fileList;
        }


        static public void RunPrivateSynonyms(OracleSql db, string p_dir)
        {
            Log.Info("synonym", "Loading synonyms command list");
            ArrayList list = buildSynonymsList(p_dir, db.GetMetaConnection());
            Log.Info("synonym", "Sorting command list");
            list.Sort();

            Log.Info("synonym", "Reconciling synonyms");
            Console.WriteLine("Reconciling synonyms:");
            bool missingIndicator = false;
            string sqlSynonym = "";
            int errCount = 0;

            foreach (SynonymObject obj in list)
            {
                if (obj.filename != null && obj.existsInSchema == false)
                {
                    missingIndicator = true;
                    sqlSynonym = String.Format("create or replace synonym {0} for {1}", obj.synonymName, obj.synonymTarget);
                    errCount += db.Exec(sqlSynonym, "synonym", "create or replace synonym");
                }
                if (obj.filename == null && obj.existsInSchema == true)
                {
                    missingIndicator = true;
                    sqlSynonym = String.Format("drop synonym {0}", obj.synonymName);
                    errCount += db.Exec(sqlSynonym, "synonym", "drop synonym");

                }
                if (obj.different)
                {
                    missingIndicator = true;
                    sqlSynonym = String.Format("create or replace synonym {0} for {1}", obj.synonymName, obj.synonymTarget);
                    errCount += db.Exec(sqlSynonym, "synonym", "create or replace synonym");
                }
            }
            if (!missingIndicator) Console.WriteLine("    No changes necessary");

            if (errCount > 0)
            {
                Log.Error("synonym", "Error(s) found while reconciling synonyms");
                Log.ExitError("Synonyms reconcile failed");
            }
        }

    }
}

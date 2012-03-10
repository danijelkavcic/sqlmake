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
using System.Data.OracleClient;
using System.Collections;
using System.Data;
using SQLMake.Util;
using SQLMake.InstallFromScripts;
//using Oracle.DataAccess.Client; 


namespace SQLMake.PLSQLCode
{   
    class PlsqlMake
    {
        public static int getDependantTypesCount(string oracleTypeName, OracleSql db)
        {
            
            Int32 dependantTypesCount = 0;
            if (db.GetMetaConnection() != null)
            {
                string cmdQuery = "select count(*) from user_dependencies where referenced_name = :referenced_name and type = 'TYPE'";
                OracleCommand cmd = new OracleCommand(cmdQuery, db.GetMetaConnection());
                cmd.Parameters.AddWithValue("referenced_name", oracleTypeName);
                dependantTypesCount = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
            }
            return dependantTypesCount;
        }
        
        static public ArrayList buildFileList(string p_dir, OracleConnection conn)
        {
            Log.Info("plsqlmake", "  Processing scripts in directory {0} ...", p_dir);
            // First read all files from the file system
            string configPlsqlFileList = Settings.getPlsqlFileList(true);
            string configIgnoreDirList = Settings.getIgnoreDirList(true);
            string configIgnoreFileList = Settings.getIgnoreFileList(true);

            Log.Verbose("plsqlmake", "Searching for plsql scripts");
            string[] files = FolderSearch.Search(p_dir, true, configPlsqlFileList, configIgnoreDirList, configIgnoreFileList);

            ArrayList fileList = new ArrayList();
            // each file can contain 0 or more PLSQL blocks
            // anonymous PL/SQL blocks are skipped
            foreach (string filename in files)
            {
                ArrayList sqlCommandList = new ArrayList(SqlScript.Load(filename));
                foreach (SqlObject sqlCommand in sqlCommandList)
                {
                    if (sqlCommand.commandType == "PLSQL" && !sqlCommand.objectType.ToUpper().Equals("ANONYMOUS")) fileList.Add(new PlsqlObject(filename, sqlCommand.objectName, sqlCommand.objectType, sqlCommand.sqlText));
                    if (sqlCommand.commandType == "SQL" && sqlCommand.objectType == "VIEW") fileList.Add(new PlsqlObject(filename, sqlCommand.objectName, sqlCommand.objectType, sqlCommand.sqlText));
                }
            }

            // TODO: check for duplicates on objectName, objectType
            Log.Verbose("plsqlmake", "Check for duplicates on objectName, objectType");
            fileList.Sort();
            PlsqlObject prevObj = null;
            int duplicateObjectsCounter = 0;
            foreach (PlsqlObject obj in fileList)
            {
                if (prevObj != null &&
                    prevObj.objectName == obj.objectName &&
                    prevObj.objectType == obj.objectType)
                {
                    Log.Error("plsqlmake", "Duplicate object {0} {1} found on filesystem\n     Files: {2}, {3}", obj.objectType, obj.objectName, obj.filename, prevObj.filename);
                    duplicateObjectsCounter++;
                }
                prevObj = obj;
            }
            if (duplicateObjectsCounter>0) 
            {
                Log.ExitError(String.Format("{0} duplicate object(s) found", duplicateObjectsCounter)); 
            }


            Log.Info("plsqlmake", "  Processing database catalog ...");
            // Read all entries from user_objects
            string cmdQuery = "select object_name, object_type, object_id, created, last_ddl_time from user_objects where object_type in ('PACKAGE', 'PACKAGE BODY', 'PROCEDURE', 'FUNCTION', 'TRIGGER', 'TYPE', 'TYPE BODY','VIEW')";
            OracleCommand cmd = new OracleCommand(cmdQuery, conn);
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PlsqlObject currObject = new PlsqlObject (reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetDateTime(3), reader.GetDateTime(4));
                int existingObjectIndex = fileList.IndexOf(currObject);
                
                if (existingObjectIndex >= 0)
                {
                    ((PlsqlObject)fileList[existingObjectIndex]).existsInSchema = true;
                }

                // This object does not exist in arrayList
                if (existingObjectIndex == -1) fileList.Add(currObject);
            }

            Log.Verbose("plsqlmake", "Final command list sort");
            fileList.Sort();

            return fileList;
        }
        
        static public string getObjectNameFromFile(string fileName, ref string plsqlObjectType)
        {
            StreamReader r = new StreamReader(fileName, Encoding.GetEncoding(Settings.getEncoding(false)));
            SQLPlusScanner scanner = new SQLPlusScanner(r, fileName);

            String token = null;
            CommandTypes mode = CommandTypes.Unknown;
            do
            {
                try
                {
                    token = scanner.get();
                    mode = scanner.currCommand.cmdType;
                }
                // this is needed to filter out any SQLPlus commands like PROMPT
                // or line&block comments
                catch (EOBException)
                {
                    mode = scanner.currCommand.cmdType;
                    scanner.resetBlockType();
                }
            } while (mode == CommandTypes.SqlPlus);

            // This is cheap but perhaps not exactly correct
            // way to skip "create or replace"
            while (token.ToUpper() != "FUNCTION" &&
                   token.ToUpper() != "LIBRARY" &&
                   token.ToUpper() != "PACKAGE" &&
                   token.ToUpper() != "PROCEDURE" &&
                   token.ToUpper() != "TRIGGER" &&
                   token.ToUpper() != "TYPE" &&
                   token.ToUpper() != "VIEW")
            {
                token = scanner.get();
            }
            plsqlObjectType = token.ToUpper();

            token = scanner.getSkipComments();
            
            if ((plsqlObjectType == "PACKAGE" || plsqlObjectType == "TYPE") 
                && token.ToUpper() == "BODY")
            {
                plsqlObjectType += " BODY";
                token = scanner.getSkipComments();
            }

            // now the token contains plsql object name
            string plsqlObjectName = token;

            r.Close();
            r.Dispose();

            return plsqlObjectName;
        }

        static public string extractPlsql(string text, string fileName)
        {
            StringReader r = new StringReader(text);
            SQLPlusScanner scanner = new SQLPlusScanner(r, fileName);

            String token = null;
            CommandTypes mode = CommandTypes.Unknown;
            do
            {
                try
                {
                    token = scanner.get();
                    mode = scanner.currCommand.cmdType;
                }
                // this is needed to filter out any SQLPlus commands like PROMPT
                // or line&block comments
                catch (EOBException)
                {
                    mode = scanner.currCommand.cmdType;
                    scanner.resetBlockType();
                }
            } while (mode == CommandTypes.SqlPlus);

            // Console.WriteLine(token);
            while (token.ToUpper() != "FUNCTION" &&
                   token.ToUpper() != "LIBRARY" &&
                   token.ToUpper() != "PACKAGE" &&
                   token.ToUpper() != "PROCEDURE" &&
                   token.ToUpper() != "TRIGGER" &&
                   token.ToUpper() != "TYPE" &&
                   token.ToUpper() != "VIEW")
            {
                token = scanner.get();
                // Console.WriteLine(token);
            }

            // with views we use only select statement starting after keyword AS
            if (token.ToUpper() == "VIEW")
            {
                while (token.ToUpper() != "AS" )
                {
                    token = scanner.get();
                }
                token = scanner.get();
            }

            StringBuilder sb = new StringBuilder();
            string currLine = null;

            try
            {
                currLine = scanner.getLineFromStartOfLastToken();
                while (currLine != null)
                {
                    if (sb.Length > 0) sb.Append("\n");
                    sb.Append(currLine.TrimEnd(null));
                    currLine = scanner.getLine();
                }
            }
            catch (EOFException)
            {
            }
            // Example of plsql file, why this was added
            // Blank lines between end of stored procedure and block terminator
            // ...
            // end;
            //
            //
            // /

            // text += "\n";
            r.Close();

            //string fileName1 = @"c:\temp\" + Path.GetFileName(fileName) + ".sqlmake";
            //System.IO.File.WriteAllText(fileName1, text);

            return sb.ToString().TrimEnd();
        }
        
        
        static public string getPLSQLFromFile(string fileName)
        {
            // Console.WriteLine("Reading " + fileName + "...");
            // string fileName = @"G:\ucg\Database\croo\plsql\BL_ATRIBUTFIZICKOGLICA.pkb";
            StreamReader r = new StreamReader(fileName, Encoding.GetEncoding(Settings.getEncoding(false)));
            SQLPlusScanner scanner = new SQLPlusScanner(r, fileName);

            String token = null;
            CommandTypes mode = CommandTypes.Unknown;
            do
            {
                try
                {
                    token = scanner.get();
                    mode = scanner.currCommand.cmdType;
                }
                // this is needed to filter out any SQLPlus commands like PROMPT
                // or line&block comments
                catch (EOBException)
                {
                    mode = scanner.currCommand.cmdType;
                    scanner.resetBlockType();
                }
            } while (mode == CommandTypes.SqlPlus);

            // Console.WriteLine(token);
            while (token.ToUpper() != "FUNCTION" &&
                   token.ToUpper() != "LIBRARY" &&
                   token.ToUpper() != "PACKAGE" &&
                   token.ToUpper() != "PROCEDURE" &&
                   token.ToUpper() != "TRIGGER" &&
                   token.ToUpper() != "TYPE" &&
                   token.ToUpper() != "VIEW")
            {
                token = scanner.get();
                // Console.WriteLine(token);
            }

            StringBuilder sb = new StringBuilder();
            string currLine = null;

            try
            {
                currLine = scanner.getLineFromStartOfLastToken();
                while (currLine != null)
                {
                    if (sb.Length > 0) sb.Append("\n");
                    sb.Append(currLine.TrimEnd(null));
                    currLine = scanner.getLine();
                }
            }
            catch (EOBException)
            {
            }
            // Example of plsql file, why this was added
            // Blank lines between end of stored procedure and block terminator
            // ...
            // end;
            //
            //
            // /

            // text += "\n";
            r.Close();
          
            //string fileName1 = @"c:\temp\" + Path.GetFileName(fileName) + ".sqlmake";
            //System.IO.File.WriteAllText(fileName1, text);

            return sb.ToString().TrimEnd();
        }

        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            //Encoding.Default.GetBytes

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        static public void Run(string p_conn, string p_dir, bool debugFlag, bool syncToFilesystem, bool syncToDb, string outputFile)
        {
            OracleSql db = new OracleSql();
            db.OpenMetaConnection(p_conn);
            if (outputFile == "") db.OpenConnection(p_conn);
            else db.SpoolOn(outputFile);
            Run(db, p_dir, debugFlag, syncToFilesystem, syncToDb, outputFile);
            // Close and Dispose OracleConnection object
            db.Close();
        }

        static public void Run(OracleSql db, string p_dir, bool debugFlag, bool syncToFilesystem, bool syncToDb, string outputFile)
        {
            Log.Info("plsqlmake", "Loading plsql command list");
            ArrayList list = buildFileList(p_dir, db.GetMetaConnection());

            Log.Info("plsqlmake", "");
            Log.Info("plsqlmake", "PL/SQL differences"); 
            Log.Info("plsqlmake", "  Object only on filesystem");
            bool missingIndicator = false;
            foreach (PlsqlObject obj in list)
            {
                if (obj.filename != null && obj.existsInSchema == false)
                {
                    //Console.WriteLine("  {0} {1} ({2})", obj.name.ToLower(), obj.filetype, obj.filename);
                    missingIndicator = true;
                    if (!syncToFilesystem && !syncToDb)
                        Log.Info("plsqlmake", "    @{2}", obj.objectName.ToLower(), obj.objectType, obj.filename);

                    if (syncToFilesystem)
                    {
                        Log.Info("plsqlmake", "    Deleting {0}", obj.filename);
                        FileInfo fi = new FileInfo(obj.filename);
                        fi.MoveTo(obj.filename + ".bak");
                    }

                    if (syncToDb)
                    {
                        Log.Info("plsqlmake", "    Creating {0} {1}", obj.objectType, obj.objectName);
                        if (obj.objectType != "VIEW") db.Exec("create or replace " + extractPlsql(obj.plsqlText, obj.filename), "syncToDb", "Create " + obj.objectType + " " + obj.objectName);
                        else db.Exec("create or replace " + obj.objectType + " " + obj.objectName + " as\n" + extractPlsql(obj.plsqlText, obj.filename), "syncToDb", "Create " + obj.objectType + " " + obj.objectName);
                    }
                }
            }
            if (!missingIndicator) Log.Info("plsqlmake", "    None");

            Log.Info("plsqlmake", "  Object only in DB");
            bool extraIndicator = false;
            foreach (PlsqlObject obj in list)
            {
                if (obj.filename == null && obj.existsInSchema == true)
                {
                    extraIndicator = true;

                    if (!syncToFilesystem && !syncToDb)
                        Log.Info("plsqlmake", "    {0} {1}", obj.objectName.ToLower(), obj.objectType);

                    if (syncToFilesystem)
                    {
                        Log.Info("plsqlmake", "    Added {0}\\{1}{2}", p_dir, obj.objectName.ToLower(), obj.getExtFromObjectType().ToLower());
                        System.IO.File.WriteAllText(p_dir + "\\" + obj.objectName.ToLower() + obj.getExtFromObjectType().ToLower(), "create or replace " + db.getPLSQLfromDB(obj.objectName, obj.objectType) + "\n/", Encoding.Default);
                    }

                    if (syncToDb)
                    {
                        Log.Info("plsqlmake", "    Droping {0} {1}", obj.objectType, obj.objectName);
                        try
                        {
                            if (obj.objectType == "TYPE" && getDependantTypesCount(obj.objectName, db) > 0)
                            {
                                db.Exec("drop " + obj.objectType + " " + obj.objectName + " force", "syncToDb", "Drop " + obj.objectType + " " + obj.objectName);
                            }
                            else db.ExecUnmanaged("drop " + obj.objectType + " " + obj.objectName, "syncToDb", "Drop " + obj.objectType + " " + obj.objectName);
                        }
                        catch (OracleException e)
                        {
                            // handle ORA-04043: object {objectName} does not exist
                            // when package is droped, package body is automatically droped
                            // resulting in ORA-04043
                            if (!(e.Code == 2303 || e.Code == 4043 || e.Code == 4042))
                            {
                                Log.Warning("oracleSql", "Error when executing SQL command\r\n{0}\r\n{1}", e.Message, "Drop " + obj.objectType + " " + obj.objectName);
                            }
                        }
                    }
                }
            }
            if (!extraIndicator) Log.Info("plsqlmake", "    None");

            Log.Info("plsqlmake", "  Different objects");
            bool diffIndicator = false;
            foreach (PlsqlObject obj in list)
            {
                if (obj.filename != null && obj.existsInSchema == true)
                {
                    obj.file_md5 = getMd5Hash(extractPlsql(obj.plsqlText, obj.filename));
                    string dbText = db.getPLSQLfromDB(obj.objectName, obj.objectType);
                     obj.plsql_md5 = getMd5Hash(dbText);
                    //Console.WriteLine("       File hash: {0}  Db hash: {1}", obj.file_md5, obj.plsql_md5);
                    if (obj.file_md5 != obj.plsql_md5)
                    {
                        diffIndicator = true;
                        
                        if (!syncToFilesystem && !syncToDb)
                            Log.Info("plsqlmake", "    {0} {1} ({2})", obj.objectName.ToLower(), obj.objectType, obj.filename);

                        if (syncToFilesystem)
                        {
                            Log.Info("plsqlmake", "    Modified {0}", obj.filename);
                            FileInfo fi = new FileInfo(obj.filename);
                            FileInfo fiBak = new FileInfo(obj.filename + ".bak");
                            fiBak.Delete();
                            fi.MoveTo(obj.filename + ".bak");
                            System.IO.File.WriteAllText(obj.filename, "create or replace " + dbText + "\n/", Encoding.Default);
                        }

                        if (syncToDb)
                        {
                            ArrayList grantsList = new ArrayList();

                            db.Prompt(String.Format("    Modified {0} {1}", obj.objectType, obj.objectName));
                            if (obj.objectType == "TYPE" && getDependantTypesCount(obj.objectName, db) > 0)
                            {
                                /*foreach (PlsqlObject typeBody in list)
                                {
                                    if (typeBody.objectType == "TYPE BODY" && typeBody.objectName = obj.objectName)
                                    {
                                        typeBody.
                                    }
                                }*/
                                grantsList = Grants.buildObjectGrantsList(db.GetMetaConnection(), obj.objectName);
                                db.Exec("drop " + obj.objectType + " " + obj.objectName + " force", "syncToDb", "Drop " + obj.objectType + " " + obj.objectName);
                            }
                            try
                            {
                                if (obj.objectType != "VIEW") db.ExecUnmanaged("create or replace " + extractPlsql(obj.plsqlText, obj.filename), "syncToDb", "Replace " + obj.objectType + " " + obj.objectName);
                                else db.ExecUnmanaged("create or replace " + obj.objectType + " " + obj.objectName + " as\n" + extractPlsql(obj.plsqlText, obj.filename), "syncToDb", "Replace " + obj.objectType + " " + obj.objectName);
                                Grants.executeGrantList(db, grantsList);
                            }
                            catch (OracleException e)
                            {
                                // handle ORA-02303: cannot drop or replace a type with type or table dependents
                                // we check for type dependency but it is not working when type is referenced via private synony
                                // in another schema
                                if (e.Code != 2303)
                                {
                                    string lastErrm = e.Message;
                                    Log.Warning("oracleSql", "Error when executing SQL command\r\n{0}\r\n{1}", e.Message, "create or replace " + extractPlsql(obj.plsqlText, obj.filename));
                                }
                                
                                // retry operation
                                grantsList = Grants.buildObjectGrantsList(db.GetMetaConnection(), obj.objectName);
                                db.Exec("drop " + obj.objectType + " " + obj.objectName + " force", "syncToDb", "Drop " + obj.objectType + " " + obj.objectName);
                                db.Exec("create or replace " + extractPlsql(obj.plsqlText, obj.filename), "syncToDb", "Replace " + obj.objectType + " " + obj.objectName);
                            }
                            if (obj.objectType == "TYPE")
                            {
                                Grants.executeGrantList(db, grantsList);
                            }
                        }

                        if (debugFlag)
                        {
                            Log.Debug("plsqlMake","Write db and filesystem PLSQL text to file:");
                            Log.Debug("plsqlMake", "  {0}", obj.objectName + " " + obj.objectType + ".file.log");
                            Log.Debug("plsqlMake", "  {0}", obj.objectName + " " + obj.objectType + ".db.log");
                            Log.Debug("plsqlMake", "  File hash: {0}  Db hash: {1}", obj.file_md5, obj.plsql_md5);
                            System.IO.File.WriteAllText(obj.objectName + " " + obj.objectType + ".file.log", extractPlsql(obj.plsqlText, obj.filename), Encoding.Default);
                            System.IO.File.WriteAllText(obj.objectName + " " + obj.objectType + ".db.log", dbText, Encoding.Default);
                        }

                    }
//                    if (getMd5Hash(getPLSQLfromDB(obj.objectName, obj.objectType, con)) != obj.plsql_md5)
//                    {
//                        System.IO.File.WriteAllText(obj.objectName + " " + obj.objectType + ".db.log", dbText, Encoding.Default);
//                        Console.WriteLine(obj.objectName + " " + obj.objectType + "       File hash: {0}  Db hash: {1}", obj.file_md5, getPLSQLfromDBCryptoAPI(obj.objectName, obj.objectType, con));
//                    }                

                }
            }
            if (!diffIndicator) Log.Info("plsqlmake", "    None");

            int equalObjectsCount = 0;
            foreach (PlsqlObject obj in list)
            {
                if (obj.filename != null && obj.existsInSchema == true)
                {
                    if (obj.file_md5 == obj.plsql_md5) equalObjectsCount++;
                }
            }
            Log.Info("plsqlmake", "  Equal objects count:");
            Log.Info("plsqlmake", "    {0} object(s)", equalObjectsCount);

            if (syncToDb)
            {
                bool failedInstall = false;

                int invalidCount = db.RecompileInvalidObjects();
                if (invalidCount > 0)
                {
                    failedInstall = true;
                    Log.Error("sync2db", "{0} invalid object(s) found after recompilation", invalidCount);
                    foreach (string objectName in db.GetInvalidObjectsList())
                        Log.Error("sync2db", "  {0}", objectName);
                }

                if (failedInstall)
                {
                    db.Close();
                    Log.ExitError("Sync plsql to database failed");
                }
            }
        }

    }
}

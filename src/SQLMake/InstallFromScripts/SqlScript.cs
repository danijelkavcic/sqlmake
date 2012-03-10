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
using System.Collections;
using SQLMake.Util;

namespace SQLMake.InstallFromScripts
{
    class SqlScript
    {
        public static int RunLogErrors(OracleSql db, string filename, string datamodelVersion, RegistryTable registryTable)
        {
            Log.Info("SqlScript", "Execute script {0}", filename);
            int errCount = 0;

            ArrayList cmdList = new ArrayList(SqlScript.Load(filename));
            foreach (SqlObject cmd in cmdList)
            {
                if (cmd.commandType == "SQL" || cmd.commandType == "PLSQL")
                {
                    db.Comment(String.Format("  [Line {0} to {1}] {2}", cmd.lineStart, cmd.lineEnd, cmd.filename));
                    string installingWhat = "";
                    if (cmd.secondaryObjectName == "") installingWhat = String.Format("  {0} {1} {2}...", cmd.action.ToLower(), cmd.objectType.ToLower(), cmd.objectName.ToLower());
                    else installingWhat = String.Format("  {0} {1} {2} on {3}...", cmd.action.ToLower(), cmd.objectType.ToLower(), cmd.secondaryObjectName.ToLower(), cmd.objectName.ToLower());
                    db.Prompt(installingWhat);

                    int errCode = db.Exec(cmd.sqlText, "sqlScript", "run");
                    if (errCode != 0)
                    {
                        errCount++;
                        registryTable.addError(db, datamodelVersion, cmd.seqInFile, cmd.filename, cmd.lineStart, db.lastErrm, cmd.sqlText, installingWhat);
                    }
                }
            }

            return errCount;
        }

        
        public static int Run(OracleSql db, string filename)
        {
            Log.Info("SqlScript", "Execute script {0}", filename);
            int errCount = 0;
            ArrayList cmdList = new ArrayList(SqlScript.Load(filename));
            foreach (SqlObject cmd in cmdList)
            {
                if (cmd.commandType == "SQL" || cmd.commandType == "PLSQL")
                {
                    db.Comment(String.Format("[Line {0} to {1}] {2}", cmd.lineStart, cmd.lineEnd, cmd.filename));
                    string installingWhat = "";
                    if (cmd.secondaryObjectName == "") installingWhat = String.Format("{0} {1} {2}...", cmd.action.ToLower(), cmd.objectType.ToLower(), cmd.objectName.ToLower());
                    else installingWhat = String.Format("{0} {1} {2} on {3}...", cmd.action.ToLower(), cmd.objectType.ToLower(), cmd.secondaryObjectName.ToLower(), cmd.objectName.ToLower());
                    db.Prompt(installingWhat);

                    int errCode = db.Exec(cmd.sqlText, "sqlScript", "run");
                    if (errCode != 0) errCount++;
                }
            }

            return errCount;
        }

        public static Object[] Load(string filename)
        {
            int sqlModeCount = 0;
            int plsqlModeCount = 0;
            int sqlPlusModeCount = 0;
            int unknownModeCount = 0;
            int seqInFile = 0;
            string commandType = "";
            string objectType = "";
            string action = "";

            StreamReader r = new StreamReader(filename, Encoding.GetEncoding(Settings.getEncoding(false)));
            SQLPlusScanner scanner = new SQLPlusScanner(r, filename);

            ArrayList sqlObjectList = new ArrayList();

            try
            {
                do
                {
                    try
                    {
                        //Console.WriteLine(filename);
                        do { 
                            scanner.get();
                            if ((scanner.currCommand.cmdType == CommandTypes.Plsql ||
                                scanner.currCommand.cmdType == CommandTypes.WrappedPlsql)
                                && scanner.currCommand.objectName != "") break;
                           } while (true);

                        if (scanner.currCommand.cmdType == CommandTypes.Plsql ||
                            scanner.currCommand.cmdType == CommandTypes.WrappedPlsql)
                        {
                            do { scanner.getLine(); } while (true);
                        }

                    }
                    catch (EOBException)
                    {
                        commandType = "Unknown";
                        seqInFile++;
                        action = scanner.currCommand.action; // CREATE, ALTER, DROP, INSERT, ...
                        objectType = scanner.currCommand.cmdName; //TABLE, PACKAGE, TRIGGER, INDEX, ...

                        switch (scanner.currCommand.cmdType)
                        {
                            case CommandTypes.Sql:
                                sqlModeCount++;
                                commandType = "SQL";
                                if (action == "ALTER" && objectType == "TABLE" && scanner.currCommand.alterType == "ADD CONSTRAINT")
                                {
                                    action = "CREATE";
                                    objectType = scanner.currCommand.secondaryCmdName;
                                    if (objectType != "CHECK") objectType += " KEY";
                                }
                                if (action == "COMMENT")
                                {
                                    action = "CREATE";
                                    objectType = "COMMENT";
                                }

                                //Console.WriteLine("[{6},{7}] action {0}, currSubMode {1}, primaryObject {2}, alterType {3}, secondaryObject {4}, secondaryObjectType {5}", scanner.action, scanner.currSubMode, scanner.primaryObject, scanner.alterType, scanner.secondaryObject, scanner.secondaryObjectType, scanner.blockStartLine+1, scanner.currLineIndex+1);
                                //Console.WriteLine(scanner.currBlock.ToString().Trim());
                                break;
                            case CommandTypes.Plsql:
                                commandType = "PLSQL";
                                plsqlModeCount++;
                                //Console.WriteLine("[{6},{7}] action {0}, currSubMode {1}, primaryObject {2}, alterType {3}, secondaryObject {4}, secondaryObjectType {5}", scanner.action, scanner.currSubMode, scanner.primaryObject, scanner.alterType, scanner.secondaryObject, scanner.secondaryObjectType, scanner.blockStartLine+1, scanner.currLineIndex+1);
                                break;
                            case CommandTypes.WrappedPlsql:
                                commandType = "PLSQL";
                                plsqlModeCount++;
                                //Console.WriteLine("[{6},{7}] action {0}, currSubMode {1}, primaryObject {2}, alterType {3}, secondaryObject {4}, secondaryObjectType {5}", scanner.action, scanner.currSubMode, scanner.primaryObject, scanner.alterType, scanner.secondaryObject, scanner.secondaryObjectType, scanner.blockStartLine+1, scanner.currLineIndex+1);
                                break;
                            case CommandTypes.SqlPlus:
                                commandType = "SQLPlus";
                                //Console.WriteLine("action {0}, currSubMode {1}, primaryObject {2}, alterType {3}, secondaryObject {4}, secondaryObjectType {5}", scanner.action, scanner.currSubMode, scanner.primaryObject, scanner.alterType, scanner.secondaryObject, scanner.secondaryObjectType);
                                sqlPlusModeCount++;
                                break;
                            case CommandTypes.SqlPlusStart:
                                commandType = "SQLPlus";
                                //Console.WriteLine("action {0}, currSubMode {1}, primaryObject {2}, alterType {3}, secondaryObject {4}, secondaryObjectType {5}", scanner.action, scanner.currSubMode, scanner.primaryObject, scanner.alterType, scanner.secondaryObject, scanner.secondaryObjectType);
                                sqlPlusModeCount++;
                                break;
                            case CommandTypes.Unknown:
                                commandType = "Unknown";
                                //Console.WriteLine("Unknown BLOCK found in file {0} from line {1} to {2}", filename, scanner.blockStartLine + 1, scanner.currLineIndex + 1);
                                //Console.WriteLine(scanner.getCurrentLine());
                                unknownModeCount++;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        sqlObjectList.Add(new SqlObject(commandType,
                                                        scanner.currCommand.objectName,
                                                        scanner.currCommand.secondaryObjectName,
                                                        action,
                                                        objectType,
                                                        filename,
                                                        seqInFile,
                                                        scanner.blockStartLine + 1,
                                                        scanner.currLineIndex + 1,
                                                        scanner.currBlockText.ToString().Trim()));
                        scanner.resetBlockType();
                    }
                } while (true);
            }
            catch (EOFException)
            {
                if (scanner.currCommand.cmdType != CommandTypes.Unknown)
                {
                    Log.ExitError(String.Format("Last {0} in file {1} not correctly terminated", scanner.getModeDesc(), filename));
                }
            }

            //foreach (SqlObject s in sqlObjectList)
            //{
            //    Console.WriteLine("[{0},{1}] action {2}, objectType {3}, objectName {4}, secondaryObjectName {5}", s.lineStart, s.lineEnd, s.action, s.objectType, s.objectName, s.secondaryObjectName);
            //}

            return sqlObjectList.ToArray();
        }
    }
}

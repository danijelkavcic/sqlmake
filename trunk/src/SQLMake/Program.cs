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
using System.Text;
using SQLMake.PLSQLCode;
using SQLMake.Util;
using System.IO;
using SQLMake.Crawler;
using SQLMake.Statistics;
using SQLMake.InstallFromScripts;
using System.Diagnostics;
using System.Collections;
using SQLMake.Cmdline;
using SQLMake.Parser;

namespace SQLMake
{
    class Program
    {
        static private DateTime start;

        static string checkSettingIsSet(string value, string desc)
        {
            if (value == "") { Log.Error("program", "Parameter {0} is missing", desc); Program.Exit(1, "SQLMake is unable to start"); }
            return value;
        }

        static string getNextArg(ref int argIndex, string[] args, string paramName)
        {
            if (argIndex > args.Length - 1) { Log.Error("program", "Expected command line parameter [" + paramName + "]"); Program.Exit(1, "SQLMake is unable to start"); }
            return args[argIndex++];
        }

        static string peekNextArg(int argIndex, string[] args, string paramName)
        {
            if (argIndex > args.Length - 1) { return ""; } 
            return args[argIndex];
        }

        public static void Exit(int exitCode, string reason)
        {
            Log.Verbose("program", "Sqlmake finished with an error");
            Console.ForegroundColor = ConsoleColor.Red;
            Log.Error("program", reason);
            Console.ResetColor();

            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            Log.Info("program", "Elapsed time " + duration.TotalSeconds + " sec");

            Environment.Exit(exitCode);
        }

        static void Main(string[] args)
        {
            start = DateTime.Now;

            Console.WriteLine("SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com");
            Console.WriteLine("Revision 10");

            if (args.Length == 0)
            {
                Help();
                Environment.Exit(0);
            }

            Log.Verbose("program", "Log started -------------------------------------------------------");

            int argIndex = 0;
            bool somethingFound = true;

            // First read swithces
            do
            {
                somethingFound = true;
                switch (peekNextArg(argIndex, args, "Optional parameters").ToUpper())
                {
                    case "/D":
                        argIndex++;
                        Settings.setDebugFlag(true);
                        break;                        
                    case "/R":
                        argIndex++;
                        Settings.setRecurseFlag(true);
                        break;
                    case "/DB":
                        argIndex++;
                        Settings.setTargetIsDbFlag(true);
                        Settings.setTargetIsFilesystemFlag(false);
                        break;
                    case "/FS":
                        argIndex++;
                        Settings.setTargetIsDbFlag(false);
                        Settings.setTargetIsFilesystemFlag(true);
                        break;
                    default:
                        somethingFound = false;
                        break;
                }
            } while (somethingFound && (argIndex <= args.Length - 1));

            if (Settings.getTargetIsDbFlag(false) && Settings.getTargetIsFilesystemFlag(false))
            {
                throw new ArgumentException("Switches /DB and /FS can not be used at same time");
            }

            OracleSql db;

            // Then read action
            string actionName = getNextArg(ref argIndex, args, "Action");

            //And finally read key/value pairs
            do
            {
                string param = peekNextArg(argIndex, args, "Key=Value");
                if (param != "")
                {
                    somethingFound = true;
                    string keyName;
                    string keyValue;
                    keyName = ""; keyValue = "";
                    int eqSignPosition = param.IndexOf('=');
                    if (eqSignPosition > 0)
                    {
                        keyName = param.Substring(0, eqSignPosition).ToUpper();
                        keyValue = param.Substring(eqSignPosition + 1);

                        switch (keyName.ToUpper())
                        {
                            case "SPOOL":
                                argIndex++;
                                Settings.setSpoolOutput(keyValue);
                                break;
                            case "OUTSCRIPT":
                                argIndex++;
                                Settings.setSpoolOutput(keyValue);
                                break;
                            case "VAR":
                                argIndex++;
                                string varParameter = getNextArg(ref argIndex, args, "SQL*Plus variable name=value");
                                string varName = varParameter.Substring(0, varParameter.IndexOf('='));
                                string varValue = varParameter.Substring(varParameter.IndexOf('=') + 1);
                                Settings.addSqlplusVariable(varName, varValue);
                                // Console.WriteLine("{0}={1}", varName, varValue);
                                break;
                            case "CONFIG":
                                argIndex++;
                                Settings.setConfig(keyValue);
                                if (!File.Exists(Settings.getConfig(true)))
                                {
                                    Log.Error("program", "Config file does not exist: {0}", Settings.getConfig(true));
                                    Program.Exit(1, "SQLMake is unable to start");
                                }
                                break;
                            case "USERID":
                                argIndex++;
                                Settings.setUserId(OracleSql.convertConnectionString2NetSyntax(keyValue));
                                break;
                            case "SANDBOXPATTERN":
                                argIndex++;
                                Settings.setSandboxPattern(keyValue);
                                break;
                            case "SCRIPTS": // this is old alias for srcscriptsdir
                                Console.WriteLine("This is deprecated parameter. Use srcscriptsdir instead.");
                                argIndex++;
                                Settings.setSourceScriptsDir(keyValue);
                                if (!Directory.Exists(Settings.getSourceScriptsDir(true)))
                                {
                                    Log.Error("program", "Source scripts directory does not exist: {0}", Settings.getSourceScriptsDir(true));
                                    Program.Exit(1, "SQLMake is unable to start");
                                }
                                break;
                            case "SRCSCRIPTSDIR":
                                argIndex++;
                                Settings.setSourceScriptsDir(keyValue);
                                if (!Directory.Exists(Settings.getSourceScriptsDir(true)))
                                {
                                    Log.Error("program", "Source scripts directory does not exist: {0}", Settings.getSourceScriptsDir(true));
                                    Program.Exit(1, "SQLMake is unable to start");
                                }
                                break;
                            case "SCRIPT":
                                argIndex++;
                                Settings.setScript(keyValue);
                                if (!File.Exists(Settings.getScript(true)))
                                {
                                    Log.Error("program", "SQL script does not exist: {0}", Settings.getScript(true));
                                    Program.Exit(1, "SQLMake is unable to start");
                                }
                                break;
                            case "UPGRADETO":
                                argIndex++;
                                Settings.setUpgradeTo(keyValue);
                                break;
                            default:
                                Log.Error("program", "Unexpected input parameter {0}", param);
                                Program.Exit(1, "SQLMake is unable to start");
                                break;
                        }
                    }
                    else break;

                }
            } while (somethingFound && (argIndex <= args.Length - 1));

            Settings.loadSettings();

            switch (actionName.ToUpper())
            {
                case "-CHECKCONNECTION":
                    Console.WriteLine("Action: Check Oracle DB connection");
                    Console.WriteLine();

                    CheckConnection.Check(Settings.getUserId(true));

                    break;


                case "-PLSQL":
                    if (!Settings.getTargetIsDbFlag(false) && !Settings.getTargetIsFilesystemFlag(false))
                    {
                        Console.WriteLine("Action: List PLSQL differences between database schema and scripts");
                        Console.WriteLine();
                        PlsqlMake.Run(Settings.getUserId(true), Settings.getSourceScriptsDir(true), Settings.getDebugFlag(true), false, false, "");
                    }

                    if (Settings.getTargetIsDbFlag(false) && !Settings.getTargetIsFilesystemFlag(false))
                    {
                        Console.WriteLine("Action: Sync PL/SQL differences to database");
                        Console.WriteLine();
                        PlsqlMake.Run(Settings.getUserId(true), Settings.getSourceScriptsDir(true), false, false, true, Settings.getSpoolOutput(false));
                    }

                    if (!Settings.getTargetIsDbFlag(false) && Settings.getTargetIsFilesystemFlag(false))
                    {
                        Console.WriteLine("Action: Sync PL/SQL differences to filesystem");
                        Console.WriteLine();
                        PlsqlMake.Run(Settings.getUserId(true), Settings.getSourceScriptsDir(true), false, true, false, "");
                    }

                    break;
                
                case "-SCANNER":
                    Console.WriteLine("Action: SQLPlus Scanner");
                    Console.WriteLine();
                    RunScanner(Settings.getScript(true));
                    break;
                case "-INSTALL":
                    Console.WriteLine("Action: Installs schema based on install scripts");
                    Console.WriteLine();
                    Install.Go(Settings.getUserId(false), Settings.getSpoolOutput(false), Settings.getSourceScriptsDir(true), Settings.getRecurseFlag(true));
                    break;
                case "-UPGRADE":
                    Console.WriteLine("Action: Upgrades schema based on upgrade scripts");
                    Console.WriteLine();
                    Upgrade.Go(Settings.getUserId(true), Settings.getSpoolOutput(false), Settings.getSourceScriptsDir(true), Settings.getRecurseFlag(true), Settings.getUpgradeTo(false));
                    break;
                case "-STATUS":
                    Console.WriteLine("Action: Prints target schema status");
                    Console.WriteLine();

                    db = new OracleSql();
                    db.OpenMetaConnection(Settings.getUserId(true));
                    RegistryTable.status(db);
                    break;
                case "-LIST_ERRORS":
                    Console.WriteLine("Action: Lists all errors from last install");
                    Console.WriteLine();
                    ListErrorsCmdline.ExecuteTask();
                                        
                    break;
                case "-CLEAR_ERRORS":
                    Console.WriteLine("Action: Clear all errors from last install");
                    Console.WriteLine();

                    db = new OracleSql();
                    db.OpenMetaConnection(Settings.getUserId(true));
                    RegistryTable.clearErrors(db);
                    break;
                case "-CLEAR_ERROR":
                    Console.WriteLine("Action: Clear error with [sequenceNumber] from last install");
                    Console.WriteLine();

                    db = new OracleSql();
                    db.OpenMetaConnection(Settings.getUserId(true));
                    RegistryTable.clearError(db, int.Parse(getNextArg(ref argIndex, args, "Error sequence number")));
                    break;
                case "-CRAWL":
                    Console.WriteLine("Action: Crawl and print links");
                    Console.WriteLine();
                    Crawl.Run("START", Settings.getScript(true), Path.GetDirectoryName(Settings.getScript(true)),0);
                    break;
                case "-STATS":
                    Console.WriteLine("Action: Prints stats");
                    Console.WriteLine();
                    //Stats.Print(args[argIndex + 1], includeList, SearchOption.AllDirectories);
                    Stats.BasicStats(Settings.getSourceScriptsDir(true), SearchOption.AllDirectories);
                    break;
                case "-LIST_CHANGES":
                    Console.WriteLine("Action: List all changes of an database object");
                    Console.WriteLine();

                    ListChanges(getNextArg(ref argIndex, args, "Database object name").ToUpper());
                    break;

                case "-GRANTS":
                    db = new OracleSql();
                    db.OpenMetaConnection(Settings.getUserId(true));
                    if (Settings.getTargetIsDbFlag(false)) db.OpenConnection(Settings.getUserId(true));
                    db.setEcho(EchoFlag.on);
                    Grants.RunObjectGrants(db, Settings.getSourceScriptsDir(true));
                    break;

                case "-SYSGRANTS":
                    db = new OracleSql();
                    db.OpenMetaConnection(Settings.getUserId(true));
                    if (Settings.getTargetIsDbFlag(false)) db.OpenConnection(Settings.getUserId(true));
                    db.setEcho(EchoFlag.on);
                    Grants.RunSystemGrants(db, Settings.getSourceScriptsDir(true));
                    break;

                case "-SYNONYMS":
                    db = new OracleSql();
                    db.OpenMetaConnection(Settings.getUserId(true));
                    if (Settings.getTargetIsDbFlag(false)) db.OpenConnection(Settings.getUserId(true));
                    db.setEcho(EchoFlag.on);
                    Synonym.RunPrivateSynonyms(db, Settings.getSourceScriptsDir(true));
                    break;

                case "-TEST":
                    Console.WriteLine("Action: Test");
                    Console.WriteLine();
                    Console.WriteLine(VersionStringManipulation.extractVersionStringFromTextualFile(Settings.getSourceScriptsDir(true), Settings.getRecurseFlag(true), Settings.getDatamodelVersionFilename(true), Settings.getDatamodelVersionSearchPattern(true), Settings.getDatamodelVersionIdDefinition(true))) ;
                    break;

                default:
                    Log.Error("program", "Unknown action: {0}", actionName);
                    Program.Exit(1, "SQLMake is unable to start");
                    break;
            }

            Log.Verbose("program", "Sqlmake finished successfuly");

            DateTime end = DateTime.Now;
            TimeSpan duration = end - start;
            Log.Info("program", "Elapsed time " + duration.TotalSeconds + " sec");
        }

        static void Help()
        {
            Console.WriteLine();
            Console.WriteLine("sqlmake [/switch] -command [key=value]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  C1. checkConnection userid" + Environment.NewLine +
                              "      Checks if connection can be established to userId" + Environment.NewLine);
            Console.WriteLine("  C2. [/db|/fs] plsql userid srcscriptsdir" + Environment.NewLine +
                              "      Checks if PLSQL code in scripts is in sync with db schema" + Environment.NewLine);
            Console.WriteLine("  C3. [/db] upgrade userid srcscriptsdir [testrun] [upgradeTo]" + Environment.NewLine +
                              "      Upgrades database schema based on upgrade scripts" + Environment.NewLine);
            Console.WriteLine("  C4. install userid|outscript srcscriptsdir" + Environment.NewLine +
                              "      Installs schema based on install scripts" + Environment.NewLine);
            Console.WriteLine("  C5. status userid" + Environment.NewLine +
                              "      Status summary of target schema" + Environment.NewLine);
            Console.WriteLine("  C6. list_errors userid" + Environment.NewLine +
                              "      Lists errors from last install" + Environment.NewLine);
            Console.WriteLine("  C7. clear_errors userid [comment]" + Environment.NewLine +
                              "      Clear all errors from last install" + Environment.NewLine);
            Console.WriteLine("  C8. clear_error userid seqno [comment]" + Environment.NewLine +
                              "       Clear error with sequenceNumber from last install" + Environment.NewLine);
            Console.WriteLine("  C9. stats srcscriptsdir" + Environment.NewLine +
                              "       Prints basic stats about sql scripts on filesystem" + Environment.NewLine);
            Console.WriteLine("  C10. list_changes srcscriptsdir databaseObjectName" + Environment.NewLine +
                              "       List all changes of an database object in upgrade scripts" + Environment.NewLine);
            Console.WriteLine("  C11. [/db] grants userid srcscriptsdir [sandboxPattern]" + Environment.NewLine +
                              "       Reconcile object grants" + Environment.NewLine);
            Console.WriteLine("  C12. [/db] synonyms userid srcscriptsdir [sandboxPattern]" + Environment.NewLine +
                              "       Reconcile private synonyms" + Environment.NewLine);

            Console.WriteLine();
            Console.WriteLine("Keys:");
            Console.WriteLine("  K1. userid (database connection string, username/password@dbname)");
            Console.WriteLine("  K2. srcscriptsdir (root folder with database scripts, default value current dir)");
            Console.WriteLine("  K3. config (path to config file, default value is srcscriptsdir)");
            Console.WriteLine("  K4. spool (path to output script file)");
            Console.WriteLine("  K5. var (Sql*plus variable)");
            Console.WriteLine("  K6. sandboxPattern (pattern like %_beta1, where '%' is substituted with schema name)");
            Console.WriteLine("  K7. upgradeTo (install or upgrade only to upgrade script with version <= upgradeTo)");
            Console.WriteLine();
            Console.WriteLine("Switches:");
            Console.WriteLine("  S1. d (enable debug mode)");
            Console.WriteLine("  S2. r (recurse into scripts folder)");
            Console.WriteLine("  S3. db (execute sql statements in target schema defined by userid)");
            Console.WriteLine("  S4. fs (write sql statements to filesystem)");
            Console.WriteLine();
        }

        static void ListChanges(string objectName)
        {
            // find upgrade folder (should be just one)
            string configUpgradeFolderName = Settings.getUpgradeFolderName(true);
            string configIgnoreDirList = Settings.getIgnoreDirList(true);
            string configIgnoreFileList = Settings.getIgnoreFileList(true);
            string[] upgradeFolderList = Directory.GetDirectories(Settings.getSourceScriptsDir(true), configUpgradeFolderName, SearchOption.AllDirectories);

            if (upgradeFolderList.Length == 0)
            {
                Log.Error("upgrade", "Upgrade directory not found");
                Program.Exit(1, "Upgrade failed");
            }

            // make a list of all upgrade scripts
            ArrayList upgradeScriptList = new ArrayList();
            // there can be more than one upgrade folder 
            foreach (string folder in upgradeFolderList)
            {
                // Console.WriteLine("Upgrade folder {0}", folder);
                upgradeScriptList.AddRange(FolderSearch.Search(folder, true, "*.sql", configIgnoreDirList, configIgnoreFileList));
            }

            // sort in ascending order
            upgradeScriptList.Sort(new UpgradeScriptnameComparer());

            foreach (string upgradeScript in upgradeScriptList)
            {
                // Console.WriteLine(upgradeScript);
                ArrayList sqlCommandList = new ArrayList(SqlScript.Load(upgradeScript));
                foreach (SqlObject sqlCommand in sqlCommandList)
                {
                    if (sqlCommand.objectName.ToUpper() == objectName)
                    {
                        Console.WriteLine("-- Source: {0} ", sqlCommand.filename);
                        Console.WriteLine("-- From line {0} to {1}", sqlCommand.lineStart, sqlCommand.lineEnd);
                        Console.WriteLine(sqlCommand.sqlText);
                        Console.WriteLine("/");
                    }
                }
            }

        }


        static void RunScanner(string fileName)
        {
            StreamReader r = new StreamReader(fileName, Encoding.GetEncoding(Settings.getEncoding(false)));
             SQLPlusScanner scanner = new SQLPlusScanner(r, fileName);
             while (true)
             {
                 try
                 {
                     scanner.get();
                     Console.WriteLine("Line [" + (scanner.tokenStartLineIndex + 1) + "," + (scanner.currLineIndex + 1) + "] Col [" + (scanner.tokenStartColIndex + 1) + "," + scanner.currColIndex + "] " + scanner.tokenType + ":" + scanner.token);
                 }
                 catch (EOBException)
                 {
                     if (scanner.tokenType != TokenTypes.NotAvailable)
                     {
                         Console.WriteLine("Line [" + (scanner.tokenStartLineIndex + 1) + "," + (scanner.currLineIndex + 1) + "] Col [" + (scanner.tokenStartColIndex + 1) + "," + scanner.currColIndex + "] " + scanner.tokenType + ":" + scanner.token);
                     }
                     Console.WriteLine("* {0} * {1} * {2} * {3} * {4} * {5} *", scanner.getModeDesc(), scanner.currCommand.action, scanner.currCommand.cmdName, scanner.currCommand.objectName, scanner.currCommand.secondaryCmdName, scanner.currCommand.secondaryObjectName);
                     Console.WriteLine(scanner.currBlockText.ToString().Trim());
                     Console.WriteLine("*** EOB ***\n");
                     scanner.resetBlockType();
                 }
                 catch (EOFException)
                 {
                     if (scanner.tokenType != TokenTypes.NotAvailable)
                     {
                         Console.WriteLine("Line [" + (scanner.tokenStartLineIndex + 1) + "," + (scanner.currLineIndex + 1) + "] Col [" + (scanner.tokenStartColIndex + 1) + "," + scanner.currColIndex + "] " + scanner.tokenType + ":" + scanner.token);
                     }
                     if (scanner.tokenType != TokenTypes.NotAvailable)
                     {
                         Console.WriteLine("* {0} * {1} * {2} * {3} * {4} * {5} *", scanner.getModeDesc(), scanner.currCommand.action, scanner.currCommand.cmdName, scanner.currCommand.objectName, scanner.currCommand.secondaryCmdName, scanner.currCommand.secondaryObjectName);
                     }
                     else
                     {
                         Console.WriteLine("*** EOF ***");
                     }
                     break;
                 }
             }
        }
    }
}

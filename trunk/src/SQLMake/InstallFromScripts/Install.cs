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
using SQLMake.Util;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace SQLMake.InstallFromScripts
{
    class Install
    {
        public static void Go(string connectString, string outputScript, string sourceScriptsDir, bool recurseFlag)
        {
            Log.Verbose("install", "Install start");

            OracleSql db = new OracleSql();
            RegistryTable registryTable = new RegistryTable();
            Settings.overrideSqlPlusVariableSubstitutionFlag(false);

            if (connectString != "")
            {
                // Open connections
                if (connectString != "")
                {
                    db.OpenMetaConnection(connectString);
                    db.OpenConnection(connectString);
                }

                Log.Verbose("install", "Check registry table");
                // Check if registry files are already created
                registryTable.checkRegistryTables(db);

                string currDatamodelVersion = RegistryTable.getDatamodelVersion(db);
                if (currDatamodelVersion == "-1")
                {
                    db.Close();
                    Log.Error("install", "Target schema is installed but does not contain version information", currDatamodelVersion);
                    Log.Error("install", "Only manual upgrade is possible");
                    Log.ExitError("Installation failed");
                }

                if (RegistryTable.compareVersion(currDatamodelVersion, "0") == 1)
                {
                    db.Close();
                    Log.Error("install", "Target schema already contains version {0} of datamodel", currDatamodelVersion);
                    Log.Error("install", "Use upgrade instead of install");
                    Log.ExitError("Installation failed");
                }

            }
            else if (outputScript != "")
                db.SpoolOn(outputScript);
            else
            {
                Log.Error("install", "Either connect string or output script must be specified");
                Log.ExitError("Install did not start");
            }


            Log.Info("install", "Searching for sql scripts ...");
            Log.Verbose("install", "Allowed file types {0}", Settings.getSqlFileList(true));
            Log.Verbose("install", "Ignore directories {0}", Settings.getIgnoreDirList(true));
            Log.Verbose("install", "Ignore files {0}", Settings.getIgnoreFileList(true));
            ArrayList sqlCommandList = new ArrayList();
            string[] fileList = FolderSearch.Search(sourceScriptsDir, recurseFlag, Settings.getSqlFileList(true), Settings.getIgnoreDirList(true), Settings.getIgnoreFileList(true));

            Log.Info("install", "Loading sql commands from scripts ... ");
            //Load SQL commands from scripts
            foreach (string scriptname in fileList)
            {
                Log.Verbose("install", "Loading script {0}", scriptname);
                sqlCommandList.AddRange(SqlScript.Load(scriptname));
            }
            Log.Verbose("install", "Sort loaded sql commands");
            sqlCommandList.Sort();


            //Extract datamodel version
            Log.Verbose("Install", "Looking for datamodel version...");
            string datamodelVersion = "-1";

            if (Settings.getDatamodelVersionLocation(true) == "FILE")
            {
                Log.Verbose("Install", "Extracting datamodel version from file {0}", Settings.getDatamodelVersionFilename(true));
                datamodelVersion = VersionStringManipulation.extractVersionStringFromTextualFile(sourceScriptsDir, recurseFlag, Settings.getDatamodelVersionFilename(true), Settings.getDatamodelVersionSearchPattern(true), Settings.getDatamodelVersionIdDefinition(true));
            }

            if (Settings.getDatamodelVersionLocation(true) == "DIRECTORY")
            {
                Log.Verbose("Install", "Extracting datamodel version from directory name {0}", sourceScriptsDir);
                datamodelVersion = VersionStringManipulation.extractVersionStringFromDirectoryName(sourceScriptsDir, Settings.getDatamodelVersionSearchPattern(true), Settings.getDatamodelVersionIdDefinition(true));
            }

            if (datamodelVersion == "-1") Log.Warning("Install", "  Target datamodel version not found. Setting version to -1.");

            //foreach (SqlObject s in sqlCommandList)
            //{
            //    Console.WriteLine("{6} [{0},{1}] action {2}, objectType {3}, objectName {4}, secondaryObjectName {5}", s.lineStart, s.lineEnd, s.action, s.objectType, s.objectName, s.secondaryObjectName, s.filename);
            //}

            Log.Info("install", "Executing SQL commands in predefined order...");
            // execute in predefined order
            string installOrder = Settings.getInstallOrder(true);
            int sqlCount = 0;
            int cmdSequence = 0;
            int errCount = 0;
            foreach (string objectType in installOrder.Split(','))
            {
                sqlCount = 0;
                Log.Verbose("install", "Looking for SQL commands where type is {0}", objectType);
                foreach (SqlObject s in sqlCommandList)
                {
                    if (s.commandType != "SQLPlus" && 
                        (
                        ((s.action.Trim() == "CREATE" || s.action == "CREATE OR REPLACE") && s.objectType == objectType.Trim()) ||
                        ((s.action.Trim() == "GRANT" || s.action.Trim() == "INSERT") && s.action == objectType.Trim())
                        )
                        )
                    {
                        if (sqlCount == 0) db.Prompt("=========== " + objectType + " ======================================");
                        db.Comment(String.Format("[Line {0} to {1}] {2}", s.lineStart, s.lineEnd, s.filename));
                        string installingWhat = "";
                        if (s.secondaryObjectName == "") installingWhat = String.Format("{0} {1} {2}", s.action.ToLower(), s.objectType.ToLower(), s.objectName.ToLower());
                        else installingWhat = String.Format("{0} {1} {2} on {3}", s.action.ToLower(), s.objectType.ToLower(), s.secondaryObjectName.ToLower(), s.objectName.ToLower());
                        db.Prompt(installingWhat + "...");
                        cmdSequence++;
                        int sqlCode = db.Exec(s.sqlText, "install", "");
                        if (sqlCode != 0)
                        {
                            errCount++;
                            registryTable.addError(db, datamodelVersion, cmdSequence, s.filename, s.lineStart, db.lastErrm, s.sqlText, installingWhat);
                        }
                        s.isInstalled = true;
                        sqlCount++;
                    }
                }
                Log.Verbose("install", "{0} found", sqlCount);
            }

            Log.Verbose("install", "Check for leftover commands");
            // check if any of sqlObjects were left unatended
            // we can safely ignore: COMMIT
            bool lefotverFlag = false;
            foreach (SqlObject s in sqlCommandList)
            {
                if (s.commandType != "SQLPlus" && !s.isInstalled && s.action.Trim() != "COMMIT")
                {
                    lefotverFlag = true;
                    string installingWhat = "";
                    if (s.secondaryObjectName == "") installingWhat = String.Format("Action={0} ObjectType={1} ObjectName={2}...", s.action.ToLower(), s.objectType.ToLower(), s.objectName.ToLower());
                    else installingWhat = String.Format("Action={0} ObjectType={1} {2} on {3}...", s.action.ToLower(), s.objectType.ToLower(), s.secondaryObjectName.ToLower(), s.objectName.ToLower());
                    Log.Warning("install", "Leftover: [Line {0} to {1}] {2}", s.lineStart, s.lineEnd, s.filename);
                    Log.Warning("install", installingWhat);
                }
            }

            // check for errors during install
            Log.Verbose("install", "Check for erros during install");
            bool failedInstall = false;

            if (errCount > 0)
            {
                failedInstall = true;
                Log.Error("install", "{0} error(s) occured during install", errCount);
            }
            if (lefotverFlag)
            {
                failedInstall = true;
                Log.Error("install", "There are lefover SQL commands that did not get installed. See sqlmake log file for details");
            }

            /*
            Log.Info("install", "Recompile invalid objects");
            int invalidCount = db.RecompileInvalidObjects();
            if ( invalidCount > 0)
            {
                failedInstall = true;
                Log.Error("install", "{0} invalid object(s) found after recompilation", invalidCount);
                Console.WriteLine("{0} invalid object(s) found after recompilation", invalidCount);
            }
            */

            Log.Info("install", "");
            Log.Verbose("install", "Set datamodel version in registry table");
            if (datamodelVersion == "0")
            {
                failedInstall = true;
                registryTable.setDatamodelVersion(db, "-1");
                Log.Error("install", "Unknown version of datamodel installed");
            }
            else
            {
                registryTable.setDatamodelVersion(db, datamodelVersion);
                Log.Info("install", "Version {0} of datamodel installed", datamodelVersion);

            }
            db.Close();

            // Done
            if (failedInstall)
            {
                Log.ExitError("Installation failed");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Log.Info("install", "Installation successful");
                Console.ResetColor();
            }

        }
    }
}
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
using SQLMake.PLSQLCode;
using System.Globalization;

namespace SQLMake.InstallFromScripts
{
    public class UpgradeScriptnameComparer : IComparer
    {
        #region IComparer Members

        public static string extractUpgradeScriptVersion(string scriptName)
        {
            return Path.GetFileName(scriptName).Split('_')[0];
        }

        public int Compare(object x, object y)
        {
            if (!(x is String)) throw new ArgumentException("object x is not String");
            if (!(y is String)) throw new ArgumentException("object y is not String");

            // Console.WriteLine((string)x + "," + (string)y);
            return RegistryTable.compareVersion(extractUpgradeScriptVersion((string)x), 
                                   extractUpgradeScriptVersion((string)y));
        }

        #endregion
    }

    class Upgrade
    {
        public static void Go(string conn, string outputFile, string scriptFolder, bool recurseFlag, string upgradeTo)
        {
            // find upgrade folder (should be just one)
            string configUpgradeFolderName = Settings.getUpgradeFolderName(true);
            string configIgnoreDirList = Settings.getIgnoreDirList(true);
            string configIgnoreFileList = Settings.getIgnoreFileList(true);
            string[] upgradeFolderList = Directory.GetDirectories(scriptFolder, configUpgradeFolderName, SearchOption.AllDirectories);

            if (upgradeFolderList.Length == 0)
            {
                Log.Error("upgrade", "Upgrade directory not found");
                Log.ExitError("Upgrade failed");
            }

            // make a list of all upgrade scripts
            ArrayList upgradeScriptList = new ArrayList();
            // there can be more than one upgrade folder 
            foreach (string folder in upgradeFolderList)
            {
                ArrayList unfilteredUpgradeScriptList = new ArrayList();
                unfilteredUpgradeScriptList.AddRange(FolderSearch.Search(folder, true, "*.sql", configIgnoreDirList, configIgnoreFileList));
                // remove all scripts > upgradeTo
                if (upgradeTo != "")
                {
                    foreach (string upgradeScript in unfilteredUpgradeScriptList)
                    {
                        if (RegistryTable.compareVersion(UpgradeScriptnameComparer.extractUpgradeScriptVersion(upgradeScript), upgradeTo) <= 0)
                            upgradeScriptList.Add(upgradeScript);
                    }
                }
                else
                {
                    upgradeScriptList.AddRange(unfilteredUpgradeScriptList);
                }

            }


            
            // sort in ascending order
            upgradeScriptList.Sort(new UpgradeScriptnameComparer());

            string maxDatamodelVersion = UpgradeScriptnameComparer.extractUpgradeScriptVersion((string)upgradeScriptList[upgradeScriptList.Count - 1]);
            //Console.WriteLine(maxDatamodelVersion);

            Log.Verbose("upgrade", "Open connection to database");
            // Open connections
            if (conn == "")
            {
                Log.Error("upgrade", "Upgrade can not be done in offline mode");
                Log.ExitError("Upgrade failed");
            }
            OracleSql db = new OracleSql();
            db.OpenMetaConnection(conn);
            if (outputFile == "") db.OpenConnection(conn);
            else db.SpoolOn(outputFile);
            Log.Verbose("upgrade", "DB object successfuly created...");

            Log.Verbose("upgrade", "Extract version info from sqlmake registry");
            // Extract current datamodel version
            RegistryTable registryTable = new RegistryTable();
            string currDatamodelVersion = RegistryTable.getDatamodelVersion(db);
            int currErrCount = RegistryTable.getErrorCount(db, currDatamodelVersion);
            Log.Verbose("upgrade", "Current schema datamodel version is {0}", currDatamodelVersion);

            if (currErrCount > 0)
            {
                Log.Error("upgrade", "Previous install/upgrade finished with {0} error(s)", currErrCount);
                // TODO: force command line switch to force upgrade even if errors are found
                Log.ExitError("Upgrade failed");
            }
            
            if (RegistryTable.compareVersion(maxDatamodelVersion, currDatamodelVersion) == -1)
            {
                Log.Error("upgrade", "Datamodel is newer then scripts on filesystem. No upgrade is necessary");
                Log.ExitError("Skipping upgrade script deployment");
            }

            if (RegistryTable.compareVersion(maxDatamodelVersion, currDatamodelVersion) == 1)
            {
                Log.Info("upgrade", "Upgrading data model ...");
                foreach (string upgradeScript in upgradeScriptList)
                {
                    string scriptVersion = UpgradeScriptnameComparer.extractUpgradeScriptVersion(upgradeScript);
                    if (RegistryTable.compareVersion(currDatamodelVersion, scriptVersion) == -1)
                    {
                        Log.Info("upgrade", "");
                        int errCount = SqlScript.RunLogErrors(db, upgradeScript, scriptVersion, registryTable);
                        currDatamodelVersion = scriptVersion;
                        registryTable.addNewUpgrade(db, currDatamodelVersion, errCount, upgradeScript);

                        if (errCount > 0)
                        {
                            Log.Error("upgrade", "Errors found during upgrade to version {0}", currDatamodelVersion);
                            Log.ExitError("Upgrade failed");
                        }
                        else
                        {
                            Log.Info("upgrade", "Installation of upgrade script {0} successful", Path.GetFileName(upgradeScript));
                        }
                    }
                }
                Log.Info("upgrade", "");
                Console.ForegroundColor = ConsoleColor.Green;
                Log.Info("upgrade", "All upgrade scripts applied");
                Console.ResetColor();
            }
            else
            {
                Log.Info("upgrade", "Datamodel is up to date. No upgrade is necessary");
            }
        }
    }
}

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
using System.IO;
using System.Configuration;
using System.Collections.Specialized;

namespace SQLMake.Util
{
    // We have two different kind of settings comming from command line:
    // - key  (eg. -key=value)
    // - switch (eg. -switchName)
    //
    // All settings you can pass from command line can also be set from config file
    // Config file additionally supports other key like settings, not supported from command line
    //
    // Settings are loaded in predefined order:
    // - ...
    // Once set, setting can not be changed. So for example if you define setting in command line and config file
    // the one listed higher is used 
    class Settings : ConfigurationSection
    {
        // Config file name that sqlmake looks for by default
        static private string defaultConfigName = "sqlmake.config";
        
        static string checkSetting(bool isValueSet, string value, string desc, bool required)
        {
            if (!isValueSet)
            {
                if (required) 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log.Error("settings", "Parameter {0} is not defined", desc);
                    Console.ResetColor();
                    Log.ExitError(""); 
                    return "";
                }
                else return "";
            }
            else return value;
        }

        static bool checkSetting(bool isValueSet, bool value, string desc, bool required)
        {
            if (!isValueSet)
            {
                if (required)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log.Error("settings", "Parameter {0} is not defined", desc);
                    Console.ResetColor();
                    Log.ExitError("");
                    return false;
                }
                else return false;
            }
            else return value;
        }

        static void writeSetting(ref bool isValueSet, ref string setting, string value, string desc)
        {
            if (!isValueSet && value != null) 
            {
                setting = value;
                isValueSet = true;
                Log.Verbose("Settings", "Setting {0} = {1}", desc, value);
            }
        }

        static void writeSetting(ref bool isValueSet, ref bool setting, bool value, string desc)
        {
            if (!isValueSet)
            {
                setting = value;
                isValueSet = true;
                Log.Verbose("Settings", "Setting {0} = {1}", desc, value.ToString());
            }
        }

        // KEYS
        static private bool isConfigSet = false;
        static private string config = "";
        static public string getConfig(bool required) { return checkSetting(isConfigSet, config, "Config", required); }
        static public void setConfig(string value) { writeSetting(ref isConfigSet, ref config, value, "Config"); }

        static private bool isUserIdSet = false;
        static private string userId = "";
        static public string getUserId(bool required) { return checkSetting(isUserIdSet, userId, "UserId", required); }
        static public void setUserId(string value) { writeSetting(ref isUserIdSet, ref userId, value, "UserId"); }

        static private bool isSandboxPatternSet = false;
        static private string sandboxPattern = "";
        static public string getSandboxPattern(bool required) { return checkSetting(isSandboxPatternSet, sandboxPattern, "SandboxPattern", required); }
        static public void setSandboxPattern(string value) { writeSetting(ref isSandboxPatternSet, ref sandboxPattern, value, "SandboxPattern"); }

        static private bool isSourceScriptsDirSet = false;
        static private string sourceScriptsDir = "";
        static public string getSourceScriptsDir(bool required) { return checkSetting(isSourceScriptsDirSet, sourceScriptsDir, "Scripts", required); }
        static public void setSourceScriptsDir(string value) { writeSetting(ref isSourceScriptsDirSet, ref sourceScriptsDir, value, "Scripts"); }

        static private bool isScriptSet = false;
        static private string script = "";
        static public string getScript(bool required) { return checkSetting(isScriptSet, script, "Script", required); }
        static public void setScript(string value) { writeSetting(ref isScriptSet, ref script, value, "Script"); }

        static private bool isSpoolOutput = false;
        static private string spoolOutput = "";
        static public string getSpoolOutput(bool required) { return checkSetting(isSpoolOutput, spoolOutput, "SpoolOutput", required); }
        static public void setSpoolOutput(string value) { writeSetting(ref isSpoolOutput, ref spoolOutput, value, "SpoolOutput"); }

        static private bool isUpgradeTo = false;
        static private string upgradeTo = "";
        static public string getUpgradeTo(bool required) { return checkSetting(isUpgradeTo, upgradeTo, "UpgradeTo", required); }
        static public void setUpgradeTo(string value) { writeSetting(ref isUpgradeTo, ref upgradeTo, value, "UpgradeTo"); }
        

        static private Hashtable sqlplusVariables = new Hashtable();

        // switches
        static private bool isDebugFlagSet = false;
        static private bool debugFlag = false;
        static public bool getDebugFlag(bool required) { return checkSetting(isDebugFlagSet, debugFlag, "DebugFlag", required); }
        static public void setDebugFlag(bool value) { writeSetting(ref isDebugFlagSet, ref debugFlag, value, "DebugFlag"); }

        static private bool isRecurseFlagSet = false;
        static private bool recurseFlag = false;
        static public bool getRecurseFlag(bool required) { return checkSetting(isRecurseFlagSet, recurseFlag, "RecurseFlag", required); }
        static public void setRecurseFlag(bool value) { writeSetting(ref isRecurseFlagSet, ref recurseFlag, value, "RecurseFlag"); }

        static private bool isTargetIsDbFlagSet = false;
        static private bool targetIsDbFlag = false;
        static public bool getTargetIsDbFlag(bool required) { return checkSetting(isTargetIsDbFlagSet, targetIsDbFlag, "TargetIsDbFlag", required); }
        static public void setTargetIsDbFlag(bool value) { writeSetting(ref isTargetIsDbFlagSet, ref targetIsDbFlag, value, "TargetIsDbFlag"); }

        static private bool isTargetIsFilesystemFlagSet = false;
        static private bool targetIsFilesystemFlag = false;
        static public bool getTargetIsFilesystemFlag(bool required) { return checkSetting(isTargetIsFilesystemFlagSet, targetIsFilesystemFlag, "TargetIsFilesystemFlag", required); }
        static public void setTargetIsFilesystemFlag(bool value) { writeSetting(ref isTargetIsFilesystemFlagSet, ref targetIsFilesystemFlag, value, "TargetIsFilesystemFlag"); }

        static private bool isSqlPlusSubstitutionFlagSet = false;
        static private bool SqlPlusSubstitutionFlag = false;
        static public bool getSqlPlusVariableSubstitutionFlag(bool required) { return checkSetting(isSqlPlusSubstitutionFlagSet, SqlPlusSubstitutionFlag, "SQLPlusVariableSubstitutionFlag", required); }
        static public void setSqlPlusVariableSubstitutionFlag(bool value) { writeSetting(ref isSqlPlusSubstitutionFlagSet, ref SqlPlusSubstitutionFlag, value, "SQLPlusVariableSubstitutionFlag"); }
        static public void overrideSqlPlusVariableSubstitutionFlag(bool value) { isSqlPlusSubstitutionFlagSet = false; setSqlPlusVariableSubstitutionFlag(value); }

        static private bool isRaiseExceptionWhenSyntaxErrorFoundFlagSet = false;
        static private bool RaiseExceptionWhenSyntaxErrorFoundFlag = false;
        static public bool getRaiseExceptionWhenSyntaxErrorFoundFlag(bool required) { return checkSetting(isRaiseExceptionWhenSyntaxErrorFoundFlagSet, RaiseExceptionWhenSyntaxErrorFoundFlag, "RaiseExceptionWhenSyntaxErrorFoundFlag", required); }
        static public void setRaiseExceptionWhenSyntaxErrorFoundFlag(bool value) { writeSetting(ref isRaiseExceptionWhenSyntaxErrorFoundFlagSet, ref RaiseExceptionWhenSyntaxErrorFoundFlag, value, "RaiseExceptionWhenSyntaxErrorFoundFlag"); }



        //settings
        static private bool isSqlFileListSet = false;
        static private string sqlFileList = "";
        static public string getSqlFileList(bool required) { return checkSetting(isSqlFileListSet, sqlFileList, "SqlFileList", required); }
        static public void setSqlFileList(string value) { writeSetting(ref isSqlFileListSet, ref sqlFileList, value, "SqlFileList"); }

        static private bool isPlsqlFileListSet = false;
        static private string plsqlFileList = "";
        static public string getPlsqlFileList(bool required) { return checkSetting(isPlsqlFileListSet, plsqlFileList, "PlsqlFileList", required); }
        static public void setPlsqlFileList(string value) { writeSetting(ref isPlsqlFileListSet, ref plsqlFileList, value, "PlsqlFileList"); }

        static private bool isIgnoreDirListSet = false;
        static private string ignoreDirList = "";
        // automatically append upgrade folder in ignore dir list
        static public string getIgnoreDirList(bool required) { return checkSetting(isIgnoreDirListSet, ignoreDirList + ",^" + getUpgradeFolderName(false) + "$", "IgnoreDirList", required); }
        static public void setIgnoreDirList(string value) { writeSetting(ref isIgnoreDirListSet, ref ignoreDirList, value, "IgnoreDirList"); }

        static private bool isIgnoreFileListSet = false;
        static private string ignoreFileList = "";
        static public string getIgnoreFileList(bool required) { return checkSetting(isIgnoreFileListSet, ignoreFileList, "IgnoreFileList", required); }
        static public void setIgnoreFileList(string value) { writeSetting(ref isIgnoreFileListSet, ref ignoreFileList, value, "IgnoreFileList"); }

        static private bool isInstallOrderSet = false;
        static private string installOrder = "";
        static private StringDictionary installOrderSd = new StringDictionary();
        static public string getInstallOrder(bool required) { return checkSetting(isInstallOrderSet, installOrder, "InstallOrder", required); }
        static public int getInstallOrderSeq(string objectType) { return int.Parse(installOrderSd[objectType]); }
        static public void setInstallOrder(string value) 
        { 
            writeSetting(ref isInstallOrderSet, ref installOrder, value, "InstallOrder");

            if (value != null)
            {
                int seq = 0;
                installOrderSd.Clear();
                foreach (string objectType in value.Split(','))
                {
                    installOrderSd.Add(objectType.Trim(), seq.ToString());
                    seq++;
                }
            }
        }

        static private bool isDatamodelVersionLocation = false;
        static private string datamodelVersionLocation = "";
        static public string getDatamodelVersionLocation(bool required) { return checkSetting(isDatamodelVersionLocation, datamodelVersionLocation, "DatamodelVersionLocation", required); }
        static public void setDatamodelVersionLocation(string value) { writeSetting(ref isDatamodelVersionLocation, ref datamodelVersionLocation, value.ToUpper(), "DatamodelVersionLocation"); }

        
        static private bool isDatamodelVersionFilenameSet = false;
        static private string datamodelVersionFilename = "";
        static public string getDatamodelVersionFilename(bool required) { return checkSetting(isDatamodelVersionFilenameSet, datamodelVersionFilename, "DatamodelVersionFilename", required); }
        static public void setDatamodelVersionFilename(string value) { writeSetting(ref isDatamodelVersionFilenameSet, ref datamodelVersionFilename, value, "DatamodelVersionFilename"); }

        static private bool isDatamodelVersionSearchPatternSet = false;
        static private string datamodelVersionSearchPattern = "";
        static public string getDatamodelVersionSearchPattern(bool required) { return checkSetting(isDatamodelVersionSearchPatternSet, datamodelVersionSearchPattern, "DatamodelVersionSearchPattern", required); }
        static public void setDatamodelVersionSearchPattern(string value) { writeSetting(ref isDatamodelVersionSearchPatternSet, ref datamodelVersionSearchPattern, value, "DatamodelVersionSearchPattern"); }

        static private bool isDatamodelVersionIdDefinitionSet = false;
        static private string datamodelVersionIdDefinition = "";
        static public string getDatamodelVersionIdDefinition(bool required) { return checkSetting(isDatamodelVersionIdDefinitionSet, datamodelVersionIdDefinition, "DatamodelVersionIdDefinition", required); }
        static public void setDatamodelVersionIdDefinition(string value) { writeSetting(ref isDatamodelVersionIdDefinitionSet, ref datamodelVersionIdDefinition, value, "DatamodelVersionIdDefinition"); }

        static private bool isUpgradeFolderNameSet = false;
        static private string upgradeFolderName = "";
        static public string getUpgradeFolderName(bool required) { return checkSetting(isUpgradeFolderNameSet, upgradeFolderName, "UpgradeFolderName", required); }
        static public void setUpgradeFolderName(string value) { writeSetting(ref isUpgradeFolderNameSet, ref upgradeFolderName, value, "UpgradeFolderName"); }

        static private bool isEncodingSet = false;
        static private string encoding = Encoding.Default.CodePage.ToString();
        static public string getEncoding(bool required) { return checkSetting(isEncodingSet, encoding, "Encoding", required); }
        static public void setEncoding(string value) { writeSetting(ref isEncodingSet, ref encoding, value, "Encoding"); }


        public static void addSqlplusVariable(string key, string value)
        {
            // variable name is defined as &variable_name or &&variable_name
            string realVariableName = key.TrimStart('&').ToUpper();

            if (sqlplusVariables.ContainsKey(realVariableName))
            {
                sqlplusVariables.Remove(realVariableName);
            }
            sqlplusVariables.Add(realVariableName, value);
        }

        public static string getSqlplusVariable(string key)
        {
            // variable name is defined as &variable_name or &&variable_name
            string realVariableName = key.TrimStart('&').ToUpper();

            if (Settings.sqlplusVariables.ContainsKey(realVariableName))
            {
                return (string)Settings.sqlplusVariables[realVariableName];
            }
            else
            {
                Console.Write("Enter value for variable {0}", realVariableName.ToUpper());
                string varValue = Console.ReadLine();
                Settings.sqlplusVariables.Add(realVariableName.ToUpper(), varValue);
                return varValue;
            }
        }


        public static void loadSettings()
        {
            string settingsFile = "";

            //first check config parameter
            if (config != "")
            {
                settingsFile = config;
                Log.Info("Settings", "Loading settings from user defined config {0}", settingsFile);
                //Console.WriteLine("Loading settings from user defined config {0}", settingsFile);
            }

            //then check scripts folder
            else if (sourceScriptsDir != "" && File.Exists(sourceScriptsDir + @"\" + defaultConfigName))
            {
                settingsFile = sourceScriptsDir + @"\" + defaultConfigName;
                Log.Info("Settings", "Loading settings from config in scripts folder {0}", settingsFile);
                //Console.WriteLine("Loading settings from config in scripts folder {0}", settingsFile);
            }

            //then check current folder
            else if (File.Exists(Directory.GetCurrentDirectory() + @"\" + defaultConfigName))
            {
                settingsFile = Directory.GetCurrentDirectory() + @"\" + defaultConfigName;
                Log.Info("Settings", "Loading settings from config in current folder {0}", settingsFile);
                //Console.WriteLine("Loading settings from config in current folder {0}", settingsFile);
            }

            //finally use default values
            else
            {
                settingsFile = "";
                Log.Info("Settings", "No config found. Using default settings");
                setRaiseExceptionWhenSyntaxErrorFoundFlag(false);
            }

            if (settingsFile == "")
            {
                // keys
                setUserId(ConfigurationManager.AppSettings.Get("UserId"));
                setSandboxPattern(ConfigurationManager.AppSettings["SandboxPattern"]);
                setSourceScriptsDir(ConfigurationManager.AppSettings["Scripts"]);
                setScript(ConfigurationManager.AppSettings["Script"]);
                setSpoolOutput(ConfigurationManager.AppSettings["Spool"]);
                setUpgradeTo(ConfigurationManager.AppSettings["UpgradeTo"]);

                // switches
                if (ConfigurationManager.AppSettings["ExecuteFlag"] != null)
                {
                    setTargetIsDbFlag(ConfigurationManager.AppSettings["ExecuteFlag"].ToUpper() == "TRUE");
                }
                
                if (ConfigurationManager.AppSettings["FilesystemFlag"] != null)
                {
                    setTargetIsFilesystemFlag(ConfigurationManager.AppSettings["FilesystemFlag"].ToUpper() == "TRUE");
                }
                
                if (ConfigurationManager.AppSettings["DebugFlag"] != null)
                {
                    setDebugFlag(ConfigurationManager.AppSettings["DebugFlag"].ToUpper() == "TRUE");
                }

                if (ConfigurationManager.AppSettings["RecurseFlag"] != null)
                {
                    setRecurseFlag(ConfigurationManager.AppSettings["RecurseFlag"].ToUpper() == "TRUE");
                }

                if (ConfigurationManager.AppSettings["SQLPlusVariableSubstitutionFlag"] != null)
                {
                    setSqlPlusVariableSubstitutionFlag(ConfigurationManager.AppSettings["SQLPlusVariableSubstitutionFlag"].ToUpper() == "TRUE");
                }

                if (ConfigurationManager.AppSettings["RaiseExceptionWhenSyntaxErrorFoundFlag"] != null)
                {
                    setRaiseExceptionWhenSyntaxErrorFoundFlag(ConfigurationManager.AppSettings["RaiseExceptionWhenSyntaxErrorFoundFlag"].ToUpper() == "TRUE");
                }


                //settings
                setSqlFileList(ConfigurationManager.AppSettings["SqlFileList"]);
                setPlsqlFileList(ConfigurationManager.AppSettings["PlsqlFileList"]);
                setIgnoreDirList(ConfigurationManager.AppSettings["IgnoreDirList"]);
                setIgnoreFileList(ConfigurationManager.AppSettings["IgnoreFileList"]);
                setInstallOrder(ConfigurationManager.AppSettings["InstallOrder"]);
                setDatamodelVersionLocation(ConfigurationManager.AppSettings["DatamodelVersionLocation"]);
                setDatamodelVersionFilename(ConfigurationManager.AppSettings["DatamodelVersionFilename"]);
                setDatamodelVersionSearchPattern(ConfigurationManager.AppSettings["DatamodelVersionSearchPattern"]);
                setDatamodelVersionIdDefinition(ConfigurationManager.AppSettings["DatamodelVersionIdDefinition"]);
                setUpgradeFolderName(ConfigurationManager.AppSettings["UpgradeFolderName"]);
                setEncoding(Encoding.Default.WebName);
            }
            else
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = settingsFile;
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                // keys
                if (configuration.AppSettings.Settings["UserId"] != null) setUserId(configuration.AppSettings.Settings["UserId"].Value);
                if (configuration.AppSettings.Settings["SandboxPattern"] != null) setSandboxPattern(configuration.AppSettings.Settings["SandboxPattern"].Value);
                if (configuration.AppSettings.Settings["Scripts"] != null) setSourceScriptsDir(configuration.AppSettings.Settings["Scripts"].Value);
                if (configuration.AppSettings.Settings["Script"] != null) setScript(configuration.AppSettings.Settings["Script"].Value);
                if (configuration.AppSettings.Settings["Spool"] != null) setSpoolOutput(configuration.AppSettings.Settings["Spool"].Value);
                if (configuration.AppSettings.Settings["UpgradeTo"] != null) setUpgradeTo(configuration.AppSettings.Settings["UpgradeTo"].Value);
                //Console.WriteLine(configuration.AppSettings.Settings["Variable"].Value);

                // switches
                if (configuration.AppSettings.Settings["DebugFlag"] != null) setDebugFlag(configuration.AppSettings.Settings["DebugFlag"].Value.ToUpper() == "TRUE");
                if (configuration.AppSettings.Settings["RecurseFlag"] != null) setRecurseFlag(configuration.AppSettings.Settings["RecurseFlag"].Value.ToUpper() == "TRUE");
                if (configuration.AppSettings.Settings["ExecuteFlag"] != null) setTargetIsDbFlag(configuration.AppSettings.Settings["ExecuteFlag"].Value.ToUpper() == "TRUE");
                if (configuration.AppSettings.Settings["FilesystemFlag"] != null) setTargetIsFilesystemFlag(configuration.AppSettings.Settings["FilesystemFlag"].Value.ToUpper() == "TRUE");
                if (configuration.AppSettings.Settings["SQLPlusVariableSubstitutionFlag"] != null) setSqlPlusVariableSubstitutionFlag(configuration.AppSettings.Settings["SQLPlusVariableSubstitutionFlag"].Value.ToUpper() == "TRUE");
                if (configuration.AppSettings.Settings["RaiseExceptionWhenSyntaxErrorFoundFlag"] != null) setRaiseExceptionWhenSyntaxErrorFoundFlag(configuration.AppSettings.Settings["RaiseExceptionWhenSyntaxErrorFoundFlag"].Value.ToUpper() == "TRUE");
                
                //settings
                if (configuration.AppSettings.Settings["SqlFileList"] != null) setSqlFileList(configuration.AppSettings.Settings["SqlFileList"].Value);
                if (configuration.AppSettings.Settings["PlsqlFileList"] != null) setPlsqlFileList(configuration.AppSettings.Settings["PlsqlFileList"].Value);
                if (configuration.AppSettings.Settings["IgnoreDirList"] != null) setIgnoreDirList(configuration.AppSettings.Settings["IgnoreDirList"].Value);
                if (configuration.AppSettings.Settings["IgnoreFileList"] != null) setIgnoreFileList(configuration.AppSettings.Settings["IgnoreFileList"].Value);
                if (configuration.AppSettings.Settings["InstallOrder"] != null) setInstallOrder(configuration.AppSettings.Settings["InstallOrder"].Value);
                if (configuration.AppSettings.Settings["DatamodelVersionLocation"] != null) setDatamodelVersionLocation(configuration.AppSettings.Settings["DatamodelVersionLocation"].Value);
                if (configuration.AppSettings.Settings["DatamodelVersionFilename"] != null) setDatamodelVersionFilename(configuration.AppSettings.Settings["DatamodelVersionFilename"].Value);
                if (configuration.AppSettings.Settings["DatamodelVersionSearchPattern"] != null) setDatamodelVersionSearchPattern(configuration.AppSettings.Settings["DatamodelVersionSearchPattern"].Value);
                if (configuration.AppSettings.Settings["DatamodelVersionIdDefinition"] != null) setDatamodelVersionIdDefinition(configuration.AppSettings.Settings["DatamodelVersionIdDefinition"].Value);
                if (configuration.AppSettings.Settings["UpgradeFolderName"] != null) setUpgradeFolderName(configuration.AppSettings.Settings["UpgradeFolderName"].Value);
                if (configuration.AppSettings.Settings["Encoding"] != null) setEncoding(configuration.AppSettings.Settings["Encoding"].Value);
                else setEncoding(Encoding.Default.HeaderName);

                //config.AppSettings.Settings["SQLFileList"].Value;
            }

            Settings.sqlplusVariables.Add("_O_RELEASE", "102010000");
            Settings.sqlplusVariables.Add("USER", "PRAC");

        }

    }
}

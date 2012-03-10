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
using SQLMake.InstallFromScripts;

namespace SQLMake.Statistics
{
    class Stats
    {
        static public void BasicStats(string p_dir, SearchOption p_recurse_dir)
        {          
            // Read configuration
            string configSqlFileList = SQLMake.Util.Settings.getSqlFileList(true);

            // disable variable substitution as we are just scanning through files
            Settings.overrideSqlPlusVariableSubstitutionFlag(false);

            int dmlModeCount = 0; // insert, update, delete, select
            int ddlModeCount = 0; // all other SQL
            int tabColCommentsModeCount = 0; //
            int plsqlModeCount = 0;
            int sqlPlusModeCount = 0;
            int unknownModeCount = 0;
            int numberOfScripts = 0;

            string[] files = FolderSearch.Search(p_dir, Settings.getRecurseFlag(true), Settings.getSqlFileList(true), Settings.getIgnoreDirList(true), Settings.getIgnoreFileList(true));

            foreach (string filename in files)
            {
                //Console.WriteLine(filename);
                numberOfScripts++;
                StreamReader r = new StreamReader(filename, Encoding.GetEncoding(Settings.getEncoding(false)));
                SQLPlusScanner scanner = new SQLPlusScanner(r, filename);
                

                try
                {
                    do
                    {
                        try
                        {
                            do { scanner.get(); } while (true);
                        }
                        catch (EOBException)
                        {
                            switch (scanner.currCommand.cmdType)
                            {
                                case CommandTypes.Sql:
                                    if (scanner.currCommand.action == "SELECT" ||
                                        scanner.currCommand.action == "INSERT" ||
                                        scanner.currCommand.action == "UPDATE" ||
                                        scanner.currCommand.action == "DELETE") dmlModeCount++;
                                    else if (scanner.currCommand.action == "COMMENT") tabColCommentsModeCount++;
                                    else ddlModeCount++;
                                    break;
                                case CommandTypes.Plsql:                                    
                                    plsqlModeCount++;
                                    break;
                                case CommandTypes.SqlPlus:
                                    sqlPlusModeCount++;
                                    break;
                                case CommandTypes.Unknown:
                                    //Console.WriteLine("Unknown BLOCK found in file {0} from line {1} to {2}", filename, scanner.startLineIndex + 1, scanner.currLineIndex + 1);
                                    //Console.WriteLine(scanner.getCurrentLine());
                                    unknownModeCount++;
                                    break;
                            }
                            //Console.WriteLine("   mode {1}, action {0} {2}, plsql:{3}, sql:{4}, sqlplus:{5}", scanner.action, scanner.getModeDesc(), scanner.currSubMode, plsqlModeCount, ddlModeCount, sqlPlusModeCount);

                            scanner.resetBlockType();
                        }
                    } while (true);
                }
                catch (EOFException)
                {
                    if (scanner.currCommand.cmdType != CommandTypes.Unknown)
                    {
                        Log.Warning("stat", "Last {0} in file {1} not correctly terminated", scanner.getModeDesc(), filename);
                        Console.WriteLine("Last {0} in file {1} not correctly terminated", scanner.getModeDesc(), filename);
                    }
                }
            }
            Console.WriteLine("Number of scripts: {0}", numberOfScripts);
            Console.WriteLine("Number of DDL commands: {0}", ddlModeCount);
            Console.WriteLine("Number of table/column comments: {0}", tabColCommentsModeCount);
            Console.WriteLine("Number of DML commands: {0}", dmlModeCount);
            Console.WriteLine("Number of PLSQL commands: {0}", plsqlModeCount);
            Console.WriteLine("Number of SQLPlus commands: {0}", sqlPlusModeCount);
            Console.WriteLine("Number of Unknown commands: {0}", unknownModeCount);
        }

        static public void Print(string p_dir, string p_include_list, SearchOption p_recurse_dir)
        {
            string[] files = new string[1];

            string[] include_list = p_include_list.Split(',');
            foreach (string filetype in include_list)
            {
                string[] matchedFiles = Directory.GetFiles(p_dir, filetype, p_recurse_dir);
                if (matchedFiles.Length > 0)
                {
                    int startPos = files.Length;
                    if (startPos == 1) startPos = 0; // compensate files array[1] declaration
                    Array.Resize(ref files, startPos + matchedFiles.Length);
                    Array.Copy(matchedFiles, 0, files, startPos, matchedFiles.Length);
                }
            }

            foreach (string filename in files)
            {
                //Console.WriteLine("xxx"+filename);
                StreamReader r = new StreamReader(filename, Encoding.GetEncoding(Settings.getEncoding(false)));
                SQLPlusScanner scanner = new SQLPlusScanner(r, filename);

                try
                {
                    do
                    {
                        try
                        {
                            do { scanner.get(); } while (true);
                        }
                        catch (EOBException)
                        {
                            if (scanner.currCommand.action != "REMARK") Console.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", filename, scanner.getModeDesc(), scanner.currCommand.action, scanner.currCommand.cmdName, scanner.currCommand.objectName, scanner.currCommand.alterType, scanner.currCommand.secondaryCmdName, scanner.currCommand.secondaryObjectName);
                            scanner.resetBlockType();
                        }
                    } while (true);
                }
                catch (EOFException)
                {
                }
            }
        }
    
    
    }
}

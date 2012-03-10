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
using SQLMake.Parser;

namespace SQLMake.Crawler
{
    class Crawl
    {
        // Find all dependencies between sql script files
        // Who is calling who
        /// ///////////////////////////////////////////////////////////////////
        static public void Run(string callerScript, string targetScript, string baseDir, int level)
        {
            Console.WriteLine("REM "+" ".PadLeft(level * 3) + Path.GetFileName(targetScript) + "(" + Path.GetDirectoryName(targetScript) + ")");
            String normalisedTargetScript = "";

            if (File.Exists(targetScript))
                normalisedTargetScript = targetScript;
            else if (File.Exists(targetScript + ".sql"))
                normalisedTargetScript = targetScript + ".sql";
            else
            {
                Log.Error("Crawler", "SP2-0310: unable to open file \"{0}\"", targetScript);
                return;
            }

            StreamReader r = new StreamReader(normalisedTargetScript, Encoding.GetEncoding(Settings.getEncoding(false)));
            SQLPlusScanner scanner = new SQLPlusScanner(r, targetScript);

            String token = null;

            try
            {
                // loop through all SQLPlus, SQL and PL/SQL commands in script
                do
                {
                    try
                    {
                        token = scanner.get();

                        // Is this SQLPlus command one of: start, @, @@
                        // this is indicated by scanner mode SQLPlusStartMode
                        if (scanner.currCommand.cmdType == CommandTypes.SqlPlusStart)
                        {
                            string scannerAction = scanner.currCommand.action;

                            // We have found token start, @ or @@
                            // All text until end of the line is considered as a target
                            String target = null;
                            try
                            {
                                do { target += " " + scanner.get(); } while (true);
                            }
                            catch (EOBException)
                            {
                                scanner.resetBlockType();
                            }

                            if (scannerAction != "@@") Run(targetScript, baseDir + "\\" + target.Trim(), baseDir, level + 1);
                            else Run(targetScript, Path.GetDirectoryName(targetScript) + "\\" + target.Trim(), baseDir, level + 1);
                            

                        }
                        else if (scanner.currCommand.cmdType == CommandTypes.SqlPlus && scanner.currCommand.action == "DEFINE")
                        {
                            string define_variable_name = scanner.get();
                            scanner.get(); // skip equal sign
                            string define_variable_content = scanner.get();

                            // variable content can be optionally enclosed in 'xyz'
                            if (define_variable_content.StartsWith("'") && define_variable_content.EndsWith("'"))
                                define_variable_content = define_variable_content.Substring(1, define_variable_content.Length - 2);

                            Settings.addSqlplusVariable(define_variable_name.ToUpper(), define_variable_content);
                        }
                        // This command is not one of: start, @, @@
                        // Skip it
                        else
                        {
                            try
                            {
                                do { scanner.get(); } while (true);
                            }
                            catch (EOBException)
                            {
                                Console.WriteLine(scanner.currBlockText);
                                scanner.resetBlockType();
                            }
                        }
                    }
                    catch (EOBException)
                    {
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

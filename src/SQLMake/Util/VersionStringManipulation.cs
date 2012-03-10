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
using System.Text.RegularExpressions;
using System.IO;

namespace SQLMake.Util
{
    class VersionStringManipulation
    {
        public static string extractVersionStringFromTextualFile(string dirWithVersionFile, bool recurseFlag, string lookForFilename, string searchPattern, string outputPattern)
        {
                string[] versionFileList = FolderSearch.Search(dirWithVersionFile, recurseFlag, lookForFilename, "", "");

                if (versionFileList.Length == 0)
                {
                    Log.Warning("VersionStringManipulation", "  File with version number {0} not found on path {1}", lookForFilename, dirWithVersionFile);
                }

                if (versionFileList.Length > 1)
                {
                    Log.Warning("VersionStringManipulation", "  Multiple copies of file with version number {0} found on path {1}", lookForFilename, dirWithVersionFile);
                }

                if (versionFileList.Length == 1)
                {
                    Log.Debug("VersionStringManipulation", "  Search pattern is: {0}", searchPattern);
                    Log.Debug("VersionStringManipulation", "  Output pattern is: {0}", outputPattern);
                    Regex regex = new Regex(searchPattern, RegexOptions.Singleline);
                    // Regex regex = new Regex(searchPattern);
                    string fileContent = File.ReadAllText(versionFileList[0]);

                    if (regex.IsMatch(fileContent))
                    {
                        string outputVersionString = regex.Match(fileContent).Result(outputPattern);
                        Log.Debug("VersionStringManipulation", "  Version {0} extracted from file {1}", outputVersionString, lookForFilename);
                        return outputVersionString;
                    }
                    else
                    {
                        Log.Error("VersionStringManipulation", "  Search pattern not found in target file {0}", lookForFilename);
                    }
                }

                return "-1";
        }


        public static string extractVersionStringFromDirectoryName(string path, string searchPattern, string outputPattern)
        {
            // Option is also to search only last directory name in path and not the whole path
            // string[] directories = path.Split(Path.DirectorySeparatorChar);
            // string dirName = directories[directories.Length - 1];

            Regex regex = new Regex(searchPattern);
            if (regex.IsMatch(path))
            {
                string outputVersionString = regex.Replace(path, outputPattern);
                Log.Info("VersionStringManipulation", "  Version {0} extracted from directory name {1}", outputVersionString, path);
                return outputVersionString;
            }

            return "-1";
        }
    
    
    }
}

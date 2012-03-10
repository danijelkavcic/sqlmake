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
using System.Collections;

namespace SQLMake.Util
{
    class FolderSearch
    {
        // inputList contains filenames or folder names
        // regexPatterns is array of regular expression 
        // output list contains no filenames that match any of regexPattern
        static List<string> FilterFilenames(List<string> inputList, string[] regexPatterns)
        {
            List<string> finalList = new List<string>();
            //Console.WriteLine("curr regex filter" + regexPatterns[0]);
            foreach (string currValue in inputList)
            {
                //Console.WriteLine("currValue in filter:" + currValue);
                bool isFiltered = false;
                foreach (string currPattern in regexPatterns)
                {
                    //Console.WriteLine("   currPattern in filter:" + currPattern);
                    if (currPattern.Trim() != "" && Regex.IsMatch(Path.GetFileName(currValue), currPattern.Trim()))
                    {
                        Log.Verbose("util", "Skipped {0} based on pattern {1}", currValue, currPattern.Trim());
                        isFiltered = true;
                        break;
                    }
                }
                if (!isFiltered) finalList.Add(currValue);
            }
            return finalList;
        }

        // Removes all duplicates in the input list
        static List<string> removeDuplicates(List<string> inputList)
        {
            Dictionary<string, int> uniqueStore = new Dictionary<string, int>();
            List<string> finalList = new List<string>();

            foreach (string currValue in inputList)
            {
                if (!uniqueStore.ContainsKey(currValue))
                {
                    uniqueStore.Add(currValue, 0);
                    finalList.Add(currValue);
                }
            }
            return finalList;
        }

        static public string[] Search(string p_dir, bool p_recurse, string allowedFileTypes, string ignoreFolders, string ignoreFiles)
        {
            Log.Verbose("util", "Searching for files in {0} (recursive={1})", p_dir, p_recurse);

            // Console.WriteLine(allowedFileTypes);
            List<string> allFiles = new List<string>();

            // read all files in p_dir matching fileTypes
            string[] allowedFileTypeList = allowedFileTypes.Split(',');
            foreach (string pattern in allowedFileTypeList)
            {
                //Console.WriteLine(pattern);
                allFiles.AddRange( Directory.GetFiles(p_dir, pattern.Trim()));
            }

            allFiles = removeDuplicates(allFiles);
            //Filter unwanted files
            allFiles = FilterFilenames(allFiles, ignoreFiles.Split(','));


            if (p_recurse)
            {
                List<string> allDirs = new List<string>();
                allDirs.AddRange(Directory.GetDirectories(p_dir));
                //Filter unwanted directories
                allDirs = FilterFilenames(allDirs, ignoreFolders.Split(','));

                foreach (string subdir in allDirs)
                {
                    //Console.WriteLine(subdir);
                    allFiles.AddRange(Search(subdir, p_recurse, allowedFileTypes, ignoreFolders, ignoreFiles));
                }
            }

            // foreach (string filename in allFiles) Console.WriteLine("+"+filename);
            return (string[])allFiles.ToArray();
        }

    }
}

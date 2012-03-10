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
using System.Diagnostics;

namespace SQLMake.Util
{
    class Log
    {
        public static void LogRow(string severity, string category, string msg)
        {
            Trace.WriteLine(severity + " " + DateTime.Now.ToString("yyyyMMddhhmmss") + "(" + category + ") " + msg);
            Trace.Flush();
        }

        public static void LogRow(string severity, string category, string fmt, params object[] args)
        {
            Trace.WriteLine(severity + " " + DateTime.Now.ToString("yyyyMMddhhmmss") + "(" + category + ") " + string.Format(fmt, args));
            Trace.Flush();
        }


        public static void Debug(string category, string msg)
        {
            if (Settings.getDebugFlag(true)) LogRow("DEBUG", category, msg);
        }

        public static void Verbose(string category, string msg)
        {
            LogRow("VERBOSE", category, msg);
        }

        public static void Info(string category, string msg)
        {
            LogRow("INFO", category, msg);
            Console.WriteLine(msg);
        }

        public static void Warning(string category, string msg)
        {
            LogRow("WARN", category, msg);
            Console.WriteLine(msg);
        }

        public static void Error(string category, string msg)
        {
            LogRow("ERROR", category, msg);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }

        public static void Debug(string category, string fmt, params object[] args)
        {
            if (Settings.getDebugFlag(true)) LogRow("DEBUG", category, fmt, args);
        }

        public static void Verbose(string category, string fmt, params object[] args)
        {
            LogRow("VERBOSE", category, fmt, args);
        }

        public static void Info(string category, string fmt, params object[] args)
        {
            LogRow("INFO", category, fmt, args);
            Console.WriteLine(fmt, args);
        }

        public static void Error(string category, string fmt, params object[] args)
        {
            LogRow("ERROR", category, fmt, args);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(fmt, args);
            Console.ResetColor();
        }

        public static void Warning(string category, string fmt, params object[] args)
        {
            LogRow("WARN", category, fmt, args);
            Console.WriteLine(fmt, args);
        }

        public static void ExitError(string errText)
        {
            Program.Exit(1, errText);
        }  

    }
}

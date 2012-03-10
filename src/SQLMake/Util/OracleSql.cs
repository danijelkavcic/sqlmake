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
using System.Data.OracleClient;
using System.IO;
using System.Collections.Generic;
using System.Data;

namespace SQLMake.Util
{
    public enum EchoFlag {on, off, shortDesc}

    class OracleSql
    {
        private OracleConnection conn = null;
        private OracleConnection metaConn = null;
        private FileStream spool = null;
        public string lastErrm = null;

        private EchoFlag echoFlag = EchoFlag.off;

        public void setEcho(EchoFlag echo)
        {
            this.echoFlag = echo;
        }

        public static string convertConnectionString2NetSyntax(string connectionString)
        {
            if (connectionString.ToUpper() == "OFFLINE") return "";
            int firstDelimeter = connectionString.IndexOf('/');
            int secondDelimeter = connectionString.IndexOf('@');
            // in case of username@hostname:port/service_name
            if (firstDelimeter > secondDelimeter && secondDelimeter != -1) firstDelimeter = -1;

            string username = "";
            string password = "";
            string database = "";

            if (firstDelimeter!=-1)
            {
                username = connectionString.Substring(0, firstDelimeter);

                if (secondDelimeter!=-1) 
                {
                    password = connectionString.Substring(firstDelimeter+1, secondDelimeter-firstDelimeter-1);
                    database = connectionString.Substring(secondDelimeter+1);
                }

                if (secondDelimeter==-1) password = connectionString.Substring(firstDelimeter+1);
            }

            if (firstDelimeter==-1 && secondDelimeter!=-1)
            {
                username = connectionString.Substring(0, secondDelimeter);
                database = connectionString.Substring(secondDelimeter+1);
            }

            if (firstDelimeter==-1 && secondDelimeter==-1)
            {
                username = connectionString;
            }

            if (username == "") 
            {
                Console.Write("Enter user-name: ");
                username = Console.ReadLine();
            }
            if (password == "") 
            {
                Console.Write("Enter password: ");
                
                ConsoleKeyInfo cki;
                while (true)
                {
                    cki = Console.ReadKey(true);
                    if (cki.Key == ConsoleKey.Enter) break;
                    if (cki.Key == ConsoleKey.Backspace)
                    {
                        if (password.Length > 0)
                        {
                            password = password.Remove(password.Length - 1);
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                            Console.Write(' ');
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        }
                    }
                    else
                    {
                        if (Char.IsLetterOrDigit(cki.KeyChar) || cki.KeyChar == '_')
                        {
                            password = password + cki.KeyChar;
                            Console.Write('*');
                        }
                    }
                }
                Console.WriteLine();
            }
            if (database == "")
            {
                Console.Write("Enter database name: ");
                database = Console.ReadLine();
            }

            // Console.WriteLine("User Id="+username+";Password="+password+";Data Source="+database);
            return "User Id="+username+";Password="+password+";Data Source="+database;
        }

        public string getPLSQLfromDB(string p_objectname, string p_objecttype)
        {
            if (metaConn == null) { throw new Exception("Not connected to Oracle Database"); }
            if (p_objecttype == "VIEW") return getViewDefinitionfromDB(p_objectname);

            string cmdQuery = "select rtrim(text,chr(10)||chr(9)||' ') text from user_source where name = :p_name and type = :p_type order by line";

            OracleCommand cmd = new OracleCommand(cmdQuery, metaConn);
            OracleParameter Param1 = cmd.Parameters.Add("p_name", OracleType.VarChar);
            OracleParameter Param2 = cmd.Parameters.Add("p_type", OracleType.VarChar);

            // For ORacle ODP.NET drivers
            //cmd.FetchSize = 1024 * 1024 * 50;
            //OracleParameter Param1 = cmd.Parameters.Add("p_name", OracleDbType.Varchar2);
            //OracleParameter Param2 = cmd.Parameters.Add("p_type", OracleDbType.Varchar2);

            Param1.Direction = System.Data.ParameterDirection.Input;
            Param2.Direction = System.Data.ParameterDirection.Input;

            Param1.Value = p_objectname;
            Param2.Value = p_objecttype;

            OracleDataReader reader = cmd.ExecuteReader();
            StringBuilder sb = new StringBuilder();
            string plsql_text;

            plsql_text = null;
            while (reader.Read())
            {
                if (reader.IsDBNull(0)) sb.Append("\n");
                else { sb.Append(reader.GetString(0)); sb.Append("\n"); }
            }

            // Example of plsql file, why this was added
            // Blank lines between end of stored procedure and block terminator
            // ...
            // end;
            //
            //
            // /
            plsql_text = sb.ToString().TrimEnd();


            //string fileName1 = "c:\\temp\\"+ p_objectname + p_objecttype + ".sqlmake";
            //System.IO.File.WriteAllText(fileName1, plsql_text);

            return plsql_text;
        }
        
        public string getViewDefinitionfromDB(string p_viewname)
        {
            if (metaConn == null) { throw new Exception("Not connected to Oracle Database"); }

            string cmdQuery = "select text from user_views where view_name = :p_name";

            OracleCommand cmd = new OracleCommand(cmdQuery, metaConn);
            OracleParameter Param1 = cmd.Parameters.Add("p_name", OracleType.VarChar);

            
            // For ORacle ODP.NET drivers
            //cmd.FetchSize = 1024 * 1024 * 50;
            //OracleParameter Param1 = cmd.Parameters.Add("p_name", OracleDbType.Varchar2);
            //OracleParameter Param2 = cmd.Parameters.Add("p_type", OracleDbType.Varchar2);

            Param1.Direction = System.Data.ParameterDirection.Input;
            Param1.Value = p_viewname;

            string view_text = null;
            OracleDataReader reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);


            reader.Read();
            view_text = reader.GetString(0);

            return StringUtils.TrimExtraBlanksAtEndOfTheLine(view_text);
        }

        
        public void OpenConnection(string connectionString)
        {
            if (conn != null) CloseConnection();
            
            conn = new OracleConnection();
            //Console.WriteLine(connectionString);
            conn.ConnectionString = connectionString;
            try
            {
                conn.Open();
            }
            catch (OracleException e)
            {
                Log.Error("oraclesql", e.Message);
                Environment.Exit(e.Code);
            }
            Log.Verbose("oraclesql", "Oracle connection open");
        }

        public void CloseConnection()
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }

        public void OpenMetaConnection(string connectionString)
        {
            if (metaConn != null) CloseMetaConnection();

            metaConn = new OracleConnection();
            // Console.WriteLine(connectionString);
            metaConn.ConnectionString = connectionString;
            try
            {
                metaConn.Open();
            }
            catch (OracleException e)
            {
                Log.Error("oraclesql", e.Message);
                Environment.Exit(e.Code);
            }
            Log.Verbose("oraclesql", "Oracle meta connection open");
        }

        public void CloseMetaConnection()
        {
            metaConn.Close();
            metaConn.Dispose();
            metaConn = null;
        }

        public OracleConnection GetMetaConnection()
        {
            return metaConn;
        }

        
        // add check if input parameter is actually filename
        public void SpoolOn(string filename)
        {
            if (spool != null) SpoolOff();
            if (File.Exists(filename)) File.Delete(filename);
            spool = File.Open(filename, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            Log.Info("oracleSql", "Spooling SQL commands to file {0}", filename);
        }

        public void SpoolOff()
        {
            if (spool == null) return;
            spool.Close();
            spool.Dispose();
            spool = null;
            Log.Info("oracleSql", "Spool file closed");
        }


        public void WritelnSpool(string text)
        {
            string convertedText = text.Replace("\n", "\r\n");
            if (spool != null)
            {
                spool.Write(Encoding.Default.GetBytes(convertedText), 0, Encoding.Default.GetByteCount(convertedText));
                spool.Write(Encoding.Default.GetBytes(Environment.NewLine), 0, Encoding.Default.GetByteCount(Environment.NewLine));
            }
            else
            {
                Log.Info("oraclesql", text);
            }
        }


        public int CountInvalidObjects()
        {
            Int32 invalidCount = 0;

            if (metaConn != null)
            {
                string cmdQuery = "select count(*) from user_objects where status != 'VALID' and generated = 'N' and secondary = 'N' and temporary = 'N'";
                OracleCommand cmd = new OracleCommand(cmdQuery, metaConn);
                invalidCount = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Dispose();
            }

            return invalidCount;
        }

        public List<String> GetInvalidObjectsList()
        {
            List<String> invalidObjectsList = new List<String>();

            if (metaConn != null)
            {
                string cmdQuery = "select object_name||' ('||lower(object_type)||')' invalid_object from user_objects where status != 'VALID' and generated = 'N' and secondary = 'N' and temporary = 'N' order by object_name, object_type";
                OracleCommand cmd = new OracleCommand(cmdQuery, metaConn);
                OracleDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    invalidObjectsList.Add(r.GetString(0));
                }

                cmd.Dispose();
            }

            return invalidObjectsList;
        }        

        public int RecompileInvalidObjects()
        {
            int invalidCountBefore = CountInvalidObjects();
            if (invalidCountBefore == 0) return 0;

            Prompt("Recompiling invalid stored procedures ...");
            Exec("begin dbms_utility.compile_schema( user, compile_all => FALSE, reuse_settings => TRUE ); end;", "oracleSql", "recompile invalid stored procedures");

            int invalidCountAfter = CountInvalidObjects();
            return invalidCountAfter;
        }

        public void ExecUnmanaged(string sqlText, string action, string client_info)
        {
            if (echoFlag == EchoFlag.on)
            {
                if (conn != null) Log.Info("oraclesql", "SQL> {0}", sqlText);
                else Log.Info("oraclesql", "OFFLINE> {0}", sqlText);
            }

            if (spool != null)
            {
                WritelnSpool(sqlText);
                WritelnSpool("/");
            }

            if (conn != null)
            {
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlText;
                cmd.CommandType = System.Data.CommandType.Text;

                Log.Debug("oracleSql", "Executing SQL command\r\n{0}", sqlText);
                cmd.ExecuteNonQuery();
            }
        }
        
        public int Exec(string sqlText, string action, string client_info)
        {
            try
            {
                ExecUnmanaged(sqlText, action, client_info);
                lastErrm = "";
            }
            catch (OracleException e)
            {
                lastErrm = e.Message;
                Log.Error("oracleSql", "Error when executing SQL command\r\n{0}{1}\r\n", e.Message, sqlText);
                return e.ErrorCode;
            }

            return 0;
        }

        public void Prompt(string text)
        {
            if (conn != null)
                foreach (string s in text.Split('\n')) Log.Info("sqlmake", s);

            if (spool != null)
                foreach (string s in text.Split('\n')) WritelnSpool("PROMPT " + s);
        }

        public void Comment(string text)
        {
            if (spool != null)
                foreach (string s in text.Split('\n')) WritelnSpool("REM " + s);
        }

        public void Close()
        {
            Log.Verbose("oracleSql", "oracleSql object closed");
            if (conn != null) { conn.Close(); conn.Dispose(); }
            if (metaConn != null) { metaConn.Close(); metaConn.Dispose(); }
            if (spool != null) { spool.Close(); spool.Dispose(); }
        }
    }
}

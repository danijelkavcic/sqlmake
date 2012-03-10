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
using System.Data.OracleClient;

namespace SQLMake.Util
{
    class CheckConnection
    {
        /// <summary>
        /// Detects if connection to database can be established based on username,
        /// password and database name.
        /// </summary>
        /// <param name="p_datasource">Connection string in the .net form off: "User Id="+username+";Password="+password+";Data Source="+database;</param>
        static public void Check(string p_datasource)
        {
            OracleConnection con = new OracleConnection();
           
            //using connection string attributes to connect to Oracle Database
            con.ConnectionString = p_datasource;
            try
            {
                con.Open();
                Console.WriteLine("Connection succesfull");
                Console.WriteLine("Server version: {0}", con.ServerVersion);
            }
            catch (OracleException e)
            {
                Console.WriteLine("Connection failed");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
        }
    }
}

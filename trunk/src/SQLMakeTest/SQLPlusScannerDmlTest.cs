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
using NUnit.Framework;
using System.IO;
using SQLMake;

namespace SQLMakeTest
{
    class SQLPlusScannerDmlTest
    {
        [Test]
        public void Select1Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Select1"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Select1");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("SELECT", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void Select2Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Select2"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Select2");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("SELECT", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void UpdateSimpleTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("UpdateSimple"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "UpdateSimple");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("UPDATE", scanner.currCommand.action);
            Assert.AreEqual("SH.EMPLOYEES@REMOTE", scanner.currCommand.objectName);
        }

        [Test]
        public void UpdateSubqueryTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("UpdateSubquery"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "UpdateSubquery");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("UPDATE", scanner.currCommand.action);
            Assert.AreEqual("[SUBQUERY]", scanner.currCommand.objectName);
        }

        [Test]
        public void UpdateTableCollectionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("UpdateTableCollection"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "UpdateTableCollection");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("UPDATE", scanner.currCommand.action);
            Assert.AreEqual("[TABLE COLLECTION]", scanner.currCommand.objectName);
        }

        [Test]
        public void InsertSimpleTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("InsertSimple"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "InsertSimple");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("INSERT", scanner.currCommand.action);
            Assert.AreEqual("DEPARTMENTS", scanner.currCommand.objectName);
        }

        [Test]
        public void InsertSubqueryTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("InsertSubquery"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "InsertSubquery");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("INSERT", scanner.currCommand.action);
            Assert.AreEqual("[SUBQUERY]", scanner.currCommand.objectName);
        }

        [Test]
        public void InsertTableCollectionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("InsertTableCollection"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "InsertTableCollection");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("INSERT", scanner.currCommand.action);
            Assert.AreEqual("[TABLE COLLECTION]", scanner.currCommand.objectName);
        }

        [Test]
        public void InsertMultitableTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("InsertMultitable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "InsertMultitable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("INSERT", scanner.currCommand.action);
            Assert.AreEqual("[MULTI TABLE]", scanner.currCommand.objectName);
        }

        [Test]
        public void DeleteSimpleTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DeleteSimple"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DeleteSimple");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("DELETE", scanner.currCommand.action);
            Assert.AreEqual("OE.ORDER", scanner.currCommand.objectName);
        }

        [Test]
        public void DeleteSubqueryTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DeleteSubquery"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DeleteSubquery");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("DELETE", scanner.currCommand.action);
            Assert.AreEqual("[SUBQUERY]", scanner.currCommand.objectName);
        }

        [Test]
        public void DeleteTableCollectionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DeleteTableCollection"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DeleteTableCollection");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("DELETE", scanner.currCommand.action);
            Assert.AreEqual("[TABLE COLLECTION]", scanner.currCommand.objectName);
        }

    }
}

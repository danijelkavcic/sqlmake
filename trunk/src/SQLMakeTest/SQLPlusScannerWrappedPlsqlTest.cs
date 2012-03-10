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
using System.Linq;
using NUnit.Framework;
using System.Text;
using System.IO;
using SQLMake;

namespace SQLMakeTest
{
    [TestFixture]
    class SQLPlusScannerWrappedPlsqlTest
    {
        [Test]
        public void WrappedTypeTest()
        {
        
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("WrappedType"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "WrappedType");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TYPE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("SCHEMA_NAME.TYPE_NAME", scanner.currCommand.objectName);
            Assert.AreEqual(9, scanner.tokenCountWithoutComments);
        }

        [Test]
        public void WrappedPackageBodyTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("WrappedPackageBody"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "WrappedPackageBody");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("PACKAGE BODY", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("ZBLJ", scanner.currCommand.objectName);
            Assert.AreEqual(8, scanner.tokenCountWithoutComments);
        }

        [Test]
        public void WrappedPackageTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("WrappedPackage"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "WrappedPackage");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("PACKAGE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("ZBLJ", scanner.currCommand.objectName);
            Assert.AreEqual(7, scanner.tokenCountWithoutComments);
        }

        [Test]
        public void WrappedLibraryTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("WrappedLibrary"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "WrappedLibrary");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("LIBRARY", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("ZBLJ", scanner.currCommand.objectName);
            Assert.AreEqual(5, scanner.tokenCountWithoutComments);
        }

        [Test]
        public void WrappedFunctionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("WrappedFunction"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "WrappedFunction");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("FUNCTION", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("ZBLJ", scanner.currCommand.objectName);
            Assert.AreEqual(7, scanner.tokenCountWithoutComments);
        }
    }
}

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
using System.Text;
using NUnit.Framework;
using System.IO;
using SQLMake;

namespace SQLMakeTest
{
    class SQLPlusScannerConstraints
    {
        [Test]
        public void AlterTableAddConstraintUniqueTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterTableAddConstraintUnique"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterTableAddConstraintUnique");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("STUDENTS", scanner.currCommand.objectName);
            Assert.AreEqual("ADD CONSTRAINT", scanner.currCommand.alterType);
            Assert.AreEqual("UNIQUE", scanner.currCommand.secondaryCmdName);
            Assert.AreEqual("STUDENT_UK", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void AlterTableAddConstraintPrimaryTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterTableAddConstraintPrimary"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterTableAddConstraintPrimary");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("STUDENTS", scanner.currCommand.objectName);
            Assert.AreEqual("ADD CONSTRAINT", scanner.currCommand.alterType);
            Assert.AreEqual("PRIMARY", scanner.currCommand.secondaryCmdName);
            Assert.AreEqual("STUDENTS_PK", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void AlterTableAddConstraintForeignTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterTableAddConstraintForeign"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterTableAddConstraintForeign");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("STUDENTS", scanner.currCommand.objectName);
            Assert.AreEqual("ADD CONSTRAINT", scanner.currCommand.alterType);
            Assert.AreEqual("FOREIGN", scanner.currCommand.secondaryCmdName);
            Assert.AreEqual("SCHOOL_FK", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void AlterTableAddConstraintCheckTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterTableAddConstraintCheck"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterTableAddConstraintCheck");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("SUPPLIERS", scanner.currCommand.objectName);
            Assert.AreEqual("ADD CONSTRAINT", scanner.currCommand.alterType);
            Assert.AreEqual("CHECK", scanner.currCommand.secondaryCmdName);
            Assert.AreEqual("CHECK_SUPPLIER_NAME", scanner.currCommand.secondaryObjectName);
        }


    }
}

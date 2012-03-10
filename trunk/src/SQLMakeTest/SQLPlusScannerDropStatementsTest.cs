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
    [TestFixture]
    class SQLPlusScannerDropStatementsTest
    {

        [Test]
        public void DropClusterTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropCluster"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropCluster");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CLUSTER", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("PERSONNEL", scanner.currCommand.objectName);
        }

        [Test]
        public void DropContextTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropContext"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropContext");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CONTEXT", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("HR_CONTEXT", scanner.currCommand.objectName);
        }

        [Test]
        public void DropDatabaseTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropDatabase"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropDatabase");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DATABASE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
        }

        [Test]
        public void DropDatabaseLinkTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropDatabaseLink");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("REMOTE", scanner.currCommand.objectName);
        }

        [Test]
        public void DropPublicDatabaseLinkTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropPublicDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropPublicDatabaseLink");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("DATABASE LINK", scanner.currCommand.baseCmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("REMOTE", scanner.currCommand.objectName);
        }

        [Test]
        public void DropDimensionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropDimension"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropDimension");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DIMENSION", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("CUSTOMERS_DIM", scanner.currCommand.objectName);
        }

        [Test]
        public void DropDirectoryTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropDirectory"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropDirectory");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DIRECTORY", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("BFILE_DIR", scanner.currCommand.objectName);
        }

        [Test]
        public void DropDiskgroupTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropDiskgroup"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropDiskgroup");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DISKGROUP", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("DGROUP_01", scanner.currCommand.objectName);
        }

        [Test]
        public void DropEditionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropEdition"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropEdition");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("EDITION", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("TEST", scanner.currCommand.objectName);
        }

        [Test]
        public void DropFlashbackArchiveTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropFlashbackArchive"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropFlashbackArchive");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FLASHBACK ARCHIVE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("TEST", scanner.currCommand.objectName);
        }

        [Test]
        public void DropFunctionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropFunction"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropFunction");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FUNCTION", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("OE.SECONDMAX", scanner.currCommand.objectName);
        }

        [Test]
        public void DropIndexTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropIndex"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropIndex");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("INDEX", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("ORD_CUSTOMER_IX_DEMO", scanner.currCommand.objectName);
        }

        [Test]
        public void DropIndextypeTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropIndextype"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropIndextype");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("INDEXTYPE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("POSITION_INDEXTYPE", scanner.currCommand.objectName);
        }

        [Test]
        public void DropJavaClassTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropJavaClass"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropJavaClass");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("JAVA CLASS", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("\"Agent\"", scanner.currCommand.objectName);
        }

        [Test]
        public void DropJavaSourceTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropJavaSource"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropJavaSource");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("JAVA SOURCE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("\"Agent\"", scanner.currCommand.objectName);
        }

        [Test]
        public void DropJavaResourceTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropJavaResource"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropJavaResource");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("JAVA RESOURCE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("\"Agent\"", scanner.currCommand.objectName);
        }

        [Test]
        public void DropLibraryTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropLibrary"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropLibrary");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("LIBRARY", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("EXT_LIB", scanner.currCommand.objectName);
        }

        [Test]
        public void DropMaterializedViewTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropMaterializedView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropMaterializedView");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("MATERIALIZED VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("EMP_DATA", scanner.currCommand.objectName);
        }

        [Test]
        public void DropMaterializedViewLogTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropMaterializedViewLog"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropMaterializedViewLog");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("MATERIALIZED VIEW LOG", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("OE.CUSTOMERS", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void DropOperatorTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropOperator"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropOperator");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("OPERATOR", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("EQ_OP", scanner.currCommand.objectName);
        }

        [Test]
        public void DropOutlineTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropOutline"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropOutline");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("OUTLINE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("SALARIES", scanner.currCommand.objectName);
        }

        [Test]
        public void DropPackageTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropPackage"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropPackage");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PACKAGE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("EMP_MGMT", scanner.currCommand.objectName);
        }

        [Test]
        public void DropPackageBodyTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropPackageBody"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropPackageBody");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PACKAGE BODY", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("EMP_MGMT", scanner.currCommand.objectName);
        }

        [Test]
        public void DropProcedureTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropProcedure"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropProcedure");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PROCEDURE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("HR.REMOVE_EMP", scanner.currCommand.objectName);
        }

        [Test]
        public void DropProfileTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropProfile"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropProfile");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PROFILE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("APP_USER", scanner.currCommand.objectName);
        }

        [Test]
        public void DropRestorePointTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropRestorePoint"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropRestorePoint");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("RESTORE POINT", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("GOOD_DATA", scanner.currCommand.objectName);
        }

        [Test]
        public void DropRoleTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropRole"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropRole");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ROLE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("DW_MANAGER", scanner.currCommand.objectName);
        }

        [Test]
        public void DropRollbackSegmentTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropRollbackSegment"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropRollbackSegment");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ROLLBACK SEGMENT", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("RBS_ONE", scanner.currCommand.objectName);
        }

        [Test]
        public void DropSequenceTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropSequence"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropSequence");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SEQUENCE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("OE.CUSTOMERS_SEQ", scanner.currCommand.objectName);
        }

        [Test]
        public void DropPublicSynonymTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropPublicSynonym"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropPublicSynonym");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC SYNONYM", scanner.currCommand.cmdName);
            Assert.AreEqual("SYNONYM", scanner.currCommand.baseCmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("CUSTOMERS", scanner.currCommand.objectName);
        }

        [Test]
        public void DropSynonymTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropSynonym"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropSynonym");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SYNONYM", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("CUSTOMERS", scanner.currCommand.objectName);
        }

        [Test]
        public void DropTableTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropTable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("LIST_CUSTOMERS", scanner.currCommand.objectName);
        }

        [Test]
        public void DropTablespaceTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropTablespace"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropTablespace");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLESPACE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("TBS_01", scanner.currCommand.objectName);
        }

        [Test]
        public void DropTriggerTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropTrigger"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropTrigger");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TRIGGER", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("HR.SALARY_CHECK", scanner.currCommand.objectName);
        }

        [Test]
        public void DropTypeTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropType"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropType");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TYPE", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("PERSON_T", scanner.currCommand.objectName);
        }

        [Test]
        public void DropTypeBodyTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropTypeBody"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropTypeBody");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TYPE BODY", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("DATA_TYP1", scanner.currCommand.objectName);
        }

        [Test]
        public void DropUserTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropUser"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropUser");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("USER", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("BEZEL", scanner.currCommand.objectName);
        }

        [Test]
        public void DropViewTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DropView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DropView");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("DROP", scanner.currCommand.action);
            Assert.AreEqual("EMP_VIEW", scanner.currCommand.objectName);
        }

    }
}

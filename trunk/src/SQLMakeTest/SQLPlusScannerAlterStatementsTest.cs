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
using SQLMake;
using System.IO;

namespace SQLMakeTest
{
    [TestFixture]
    class SQLPlusScannerAlterStatementsTest
    {

        [Test]
        public void AlterClusterTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterCluster"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterCluster");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CLUSTER", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("PERSONNEL", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterDatabaseTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterDatabase"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterDatabase");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DATABASE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterDatabaseLink");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_LINK", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterDimensionTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterDimension"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterDimension");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DIMENSION", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("CUSTOMERS_DIM", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterDiskgroupTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterDiskgroup"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterDiskgroup");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DISKGROUP", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("DGROUP_01", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterFlashbackArchiveTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterFlashbackArchive"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterFlashbackArchive");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FLASHBACK ARCHIVE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterFunctionTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterFunction"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterFunction");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FUNCTION", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("SCOTT.TEST_FUNCTION", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterIndexTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterIndex"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterIndex");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("INDEX", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("OE.TEST_IX", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterIndextypeTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterIndextype"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterIndextype");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("INDEXTYPE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_INDEXTYPE", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterJavaClassTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterJavaClass"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterJavaClass");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("JAVA CLASS", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("\"Agent\"", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterJavaSourceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterJavaSource"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterJavaSource");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("JAVA SOURCE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("SCOTT.\"Generated/TESTSTAPI\"", scanner.currCommand.objectName);
        }

        
        [Test]
        public void AlterLibraryTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterLibrary"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterLibrary");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("LIBRARY", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("SCOTT.TEST_EXT_LIB", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterMaterializedViewTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterMaterializedView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterMaterializedView");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("MATERIALIZED VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("SALES_BY_MONTH_BY_STATE", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterMaterializedViewLogTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterMaterializedViewLog"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterMaterializedViewLog");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("MATERIALIZED VIEW LOG", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.objectName);
            Assert.AreEqual("ORDER_ITEMS", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void AlterOperatorTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterOperator"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterOperator");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("OPERATOR", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_OP", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterOutlineTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterOutline"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterOutline");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("OUTLINE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("BLUBERIES", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterPackageTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterPackage"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterPackage");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PACKAGE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_PACKAGE", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterPrivateOutlineTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterPrivateOutline"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterPrivateOutline");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PRIVATE OUTLINE", scanner.currCommand.cmdName);
            Assert.AreEqual("OUTLINE", scanner.currCommand.baseCmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("BLUBERIES", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterProcedureTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterProcedure"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterProcedure");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PROCEDURE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("SCOTT.TEST_PROC", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterProfileTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterProfile"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterProfile");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PROFILE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_PROFILE", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterPublicDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterPublicDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterPublicDatabaseLink");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("DATABASE LINK", scanner.currCommand.baseCmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_LINK", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterPublicOutlineTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterPublicOutline"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterPublicOutline");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC OUTLINE", scanner.currCommand.cmdName);
            Assert.AreEqual("OUTLINE", scanner.currCommand.baseCmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("BLUBERIES", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterResourceCostTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterResourceCost"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterResourceCost");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("RESOURCE COST", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterRoleTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterRole"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterRole");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ROLE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("DW_MANAGER", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterRollbackSegmentTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterRollbackSegment"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterRollbackSegment");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ROLLBACK SEGMENT", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("RBS_ONE", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterSequenceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterSequence"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterSequence");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SEQUENCE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_SEQ", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterSessionTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterSession"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterSession");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SESSION", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterSharedDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterSharedDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterSharedDatabaseLink");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SHARED DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("DATABASE LINK", scanner.currCommand.baseCmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_LINK", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterSharedPublicDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterSharedPublicDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterSharedPublicDatabaseLink");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SHARED PUBLIC DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("DATABASE LINK", scanner.currCommand.baseCmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("TEST_LINK", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterSystemTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterSystem"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterSystem");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SYSTEM", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterTableTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterTable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("STUDENTS", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterTablespaceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterTablespace"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterTablespace");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLESPACE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("DEMO", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterTriggerTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterTrigger"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterTrigger");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TRIGGER", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("JOURNAL_TRG", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterTypeTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterType"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterType");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TYPE", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("PHONE_LIST_TYP", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterUserTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterUser"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterUser");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("USER", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("APP_USER", scanner.currCommand.objectName);
        }

        [Test]
        public void AlterViewTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AlterView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AlterView");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("ALTER", scanner.currCommand.action);
            Assert.AreEqual("DEMO_V", scanner.currCommand.objectName);
        }

    }
}

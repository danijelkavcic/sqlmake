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
    public class SQLPlusScannerCreateStatementsTest
    {
        [Test]
        public void CreateClusterTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateCluster"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateCluster");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CLUSTER", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("PERSONNEL", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateContextTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateContext"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateContext");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CONTEXT", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("HR_CONTEXT", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateControlFileTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateControlFile"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateControlFile");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CONTROLFILE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreateDatabaseTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateDatabase"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateDatabase");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DATABASE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreateDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateDatabaseLink");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreatePublicDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePublicDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePublicDatabaseLink");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreateSharedDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateSharedDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateSharedDatabaseLink");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SHARED DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreateSharedPublicDatabaseLinkTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateSharedPublicDatabaseLink"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateSharedPublicDatabaseLink");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SHARED PUBLIC DATABASE LINK", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreateDimensionTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateDimension"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateDimension");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DIMENSION", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("CUSTOMERS_DIM", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateDirectoryTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateDirectory"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateDirectory");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DIRECTORY", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("BFILE_DIR", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateDiskgroupTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateDiskgroup"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateDiskgroup");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DISKGROUP", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("DGROUP_01", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateEditionTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateEdition"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateEdition");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("EDITION", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("TEST_ED", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateFlashbackArchiveTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateFlashbackArchive"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateFlashbackArchive");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FLASHBACK ARCHIVE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("TEST_ARCHIVE1", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateFunctionTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateFunction"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateFunction");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("FUNCTION", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("GET_BAL", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateIndexTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateIndex"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateIndex");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("INDEX", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("ORD_CUSTOMER_IX", scanner.currCommand.objectName);
            Assert.AreEqual("ORDERS", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void CreateUniqueIndexTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateUniqueIndex"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateUniqueIndex");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("UNIQUE INDEX", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("PROMO_IX", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateBitmapIndexTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateBitmapIndex"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateBitmapIndex");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("BITMAP INDEX", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("PRODUCT_BM_IX", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateIndextypeTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateIndextype"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateIndextype");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("INDEXTYPE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("POSITION_INDEXTYPE", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateJavaTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateJava"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateJava");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("JAVA", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
        }

        [Test]
        public void CreateLibraryTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateLibrary"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateLibrary");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("LIBRARY", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("EXT_LIB", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateMaterializedViewTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateMaterializedView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateMaterializedView");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("MATERIALIZED VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("FOREIGN_CUSTOMERS", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateMaterializedViewLogTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateMaterializedViewLog"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateMaterializedViewLog");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("MATERIALIZED VIEW LOG", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("PRODUCT_INFORMATION", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void CreateOperatorTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateOperator"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateOperator");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("OPERATOR", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("EQ_OP", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateOutlineTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateOutline"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateOutline");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("OUTLINE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("SALARIES", scanner.currCommand.objectName);
        }

        [Test]
        public void CreatePublicOutlineTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePublicOutline"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePublicOutline");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC OUTLINE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("SALARIES", scanner.currCommand.objectName);
        }

        [Test]
        public void CreatePrivateOutlineTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePrivateOutline"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePrivateOutline");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PRIVATE OUTLINE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("SALARIES", scanner.currCommand.objectName);
        }

        [Test]
        public void CreatePackageTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePackage"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePackage");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("PACKAGE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("EMP_MGMT", scanner.currCommand.objectName);
        }

        [Test]
        public void CreatePackageBodyTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePackageBody"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePackageBody");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("PACKAGE BODY", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("EMP_MGMT", scanner.currCommand.objectName);
        }

        [Test]
        public void CreatePfileTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePfile"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePfile");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PFILE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreateProcedureTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateProcedure"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateProcedure");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("PROCEDURE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("REMOVE_EMP", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateProfileTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateProfile"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateProfile");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PROFILE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("NEW_PROFILE", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateRestorePointTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateRestorePoint"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateRestorePoint");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("RESTORE POINT", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("GOOD_DATA", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateRoleTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateRole"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateRole");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ROLE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("DW_MANAGER", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateRollbackSegmentTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateRollbackSegment"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateRollbackSegment");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ROLLBACK SEGMENT", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("RBS_ONE", scanner.currCommand.objectName);
        }

        [Test]
        public void CreatePublicRollbackSegmentTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePublicRollbackSegment"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePublicRollbackSegment");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC ROLLBACK SEGMENT", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("RBS_ONE", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateSchemaTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateSchema"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateSchema");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SCHEMA", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("OE", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateSequenceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateSequence"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateSequence");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SEQUENCE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("CUSTOMERS_SEQ", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateSpfileTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateSpfile"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateSpfile");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SPFILE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
        }

        [Test]
        public void CreateSynonymTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateSynonym"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateSynonym");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SYNONYM", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("OFFICES", scanner.currCommand.objectName);
            Assert.AreEqual("HR.LOCATIONS", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void CreatePublicSynonymTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreatePublicSynonym"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreatePublicSynonym");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PUBLIC SYNONYM", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("OFFICES", scanner.currCommand.objectName);
            Assert.AreEqual("HR.LOCATIONS", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void CreateTableTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateTable");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("EMPLOYEES_DEMO", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateGlobalTemporaryTableTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateGlobalTemporaryTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateGlobalTemporaryTable");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("GLOBAL TEMPORARY TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("TODAY_SALES", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateTablespaceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateTablespace"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateTablespace");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLESPACE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("NORMAL_01", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateBigfileTablespaceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateBigfileTablespace"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateBigfileTablespace");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("BIGFILE TABLESPACE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("BIGTBS_01", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateUndoTablespaceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateUndoTablespace"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateUndoTablespace");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("UNDO TABLESPACE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("UNDOTS1", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateTemporaryTablespaceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateTemporaryTablespace"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateTemporaryTablespace");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TEMPORARY TABLESPACE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("TEMP_DEMO", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateSmallfileTablespaceTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateSmallfileTablespace"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateSmallfileTablespace");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SMALLFILE TABLESPACE", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE", scanner.currCommand.action);
            Assert.AreEqual("TBS_01", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateTriggerTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateTrigger"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateTrigger");

            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("TRIGGER", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("T", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateTypeTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateType"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateType");
            Assert.AreEqual("CREATE", scanner.get());
            Assert.AreEqual("TYPE", scanner.get());
            Assert.AreEqual("TEST", scanner.get());
            Assert.AreEqual("AS", scanner.get());
            Assert.AreEqual("OBJECT", scanner.get());
            Assert.AreEqual("(", scanner.get());
            Assert.AreEqual("A", scanner.get());
            Assert.AreEqual("NUMBER", scanner.get());
            Assert.AreEqual("(", scanner.get());
            Assert.AreEqual("1", scanner.get());
            Assert.AreEqual(")", scanner.get());
            Assert.AreEqual(")", scanner.get());

            try { scanner.get(); Assert.Fail(); }
            catch (EOBException)
            {
                Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
                Assert.AreEqual("TYPE", scanner.currCommand.cmdName);
                Assert.AreEqual("CREATE", scanner.currCommand.action);
            }
        }

        [Test]
        public void CreateTypeBodyTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateTypeBody"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateType");
            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Plsql, scanner.currCommand.cmdType);
            Assert.AreEqual("TYPE BODY", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("TEST", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateViewTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateView");
            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("EMP_VIEW", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateForceViewTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateForceView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateForceView");
            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FORCE VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("EMP_VIEW", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateNoForceViewTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateNoForceView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateNoForceView");
            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("NO FORCE VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("EMP_VIEW", scanner.currCommand.objectName);
        }

        [Test]
        public void CreateEditioningViewTest()
        {
            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CreateEditioningView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CreateEditioningView");
            scanner.readNextBlock();
            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("EDITIONING VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("CREATE OR REPLACE", scanner.currCommand.action);
            Assert.AreEqual("ED_ORDERS_VIEW", scanner.currCommand.objectName);
        }    
    }
}

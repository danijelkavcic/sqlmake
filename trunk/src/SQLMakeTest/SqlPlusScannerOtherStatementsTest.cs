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
    class SQLPlusScannerOtherStatementsTest
    {
        [Test]
        public void AnalyzeClusterTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AnalyzeCluster"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AnalyzeCluster");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CLUSTER", scanner.currCommand.cmdName);
            Assert.AreEqual("ANALYZE", scanner.currCommand.action);
            Assert.AreEqual("TEST_CLUSTER", scanner.currCommand.objectName);
        }

        [Test]
        public void AnalyzeIndexTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AnalyzeIndex"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AnalyzeIndex");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("INDEX", scanner.currCommand.cmdName);
            Assert.AreEqual("ANALYZE", scanner.currCommand.action);
            Assert.AreEqual("TEST_I", scanner.currCommand.objectName);
        }

        [Test]
        public void AnalyzeTableTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AnalyzeTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AnalyzeTable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("ANALYZE", scanner.currCommand.action);
            Assert.AreEqual("HR.ORDERS", scanner.currCommand.objectName);
        }

        [Test]
        public void AssociateStatisticsTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("AssociateStatistics"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "AssociateStatistics");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ASSOCIATE STATISTICS", scanner.currCommand.action);
            Assert.AreEqual("PACKAGES", scanner.currCommand.cmdName);
            Assert.AreEqual("SPECIAL_FUNCTIONS , OTHER_FUNCTIONS , LAST_FUNCTIONS", scanner.currCommand.objectName);
        }

        [Test]
        public void DisassociateStatisticsTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("DisassociateStatistics"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "DisassociateStatistics");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("DISASSOCIATE STATISTICS", scanner.currCommand.action);
            Assert.AreEqual("PACKAGES", scanner.currCommand.cmdName);
            Assert.AreEqual("SPECIAL_FUNCTIONS , OTHER_FUNCTIONS , LAST_FUNCTIONS", scanner.currCommand.objectName);
        }

        [Test]
        public void AuditTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Audit"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Audit");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("AUDIT", scanner.currCommand.action);
        }

        [Test]
        public void NoauditTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Noaudit"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Noaudit");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("NOAUDIT", scanner.currCommand.action);
        }

        [Test]
        public void CommentTableTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CommentTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CommentTable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMENT", scanner.currCommand.action);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("HR.ORDERS", scanner.currCommand.objectName);
        }

        [Test]
        public void CommentOperatorTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CommentOperator"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CommentOperator");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMENT", scanner.currCommand.action);
            Assert.AreEqual("OPERATOR", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST_OPERATOR", scanner.currCommand.objectName);
        }

        [Test]
        public void CommentMiningModelTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CommentMiningModel"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CommentMiningModel");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMENT", scanner.currCommand.action);
            Assert.AreEqual("MINING MODEL", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST_MINING_MODEL", scanner.currCommand.objectName);
        }

        [Test]
        public void CommentMaterializedViewTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CommentMaterializedView"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CommentMaterializedView");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMENT", scanner.currCommand.action);
            Assert.AreEqual("MATERIALIZED VIEW", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST_MV", scanner.currCommand.objectName);
        }

        [Test]
        public void CommentIndextypeTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CommentIndextype"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CommentIndextype");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMENT", scanner.currCommand.action);
            Assert.AreEqual("INDEXTYPE", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST_INDEXTYPE", scanner.currCommand.objectName);
        }

        [Test]
        public void CommentEditionTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CommentEdition"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CommentEdition");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMENT", scanner.currCommand.action);
            Assert.AreEqual("EDITION", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST_EDITION", scanner.currCommand.objectName);
        }

        [Test]
        public void CommentColumnTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("CommentColumn"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "CommentColumn");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMENT", scanner.currCommand.action);
            Assert.AreEqual("COLUMN", scanner.currCommand.cmdName);
            Assert.AreEqual("HR.ORDERS", scanner.currCommand.objectName);
            Assert.AreEqual("NAME", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void CommitTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Commit"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Commit");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("COMMIT", scanner.currCommand.action);
        }

        [Test]
        public void RollbackTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Rollback"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Rollback");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("ROLLBACK", scanner.currCommand.action);
        }

        [Test]
        public void SavepointTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Savepoint"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Savepoint");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SAVEPOINT", scanner.currCommand.action);
            Assert.AreEqual("TEST_SAL", scanner.currCommand.objectName);
        }

        [Test]
        public void FlashbackDatabaseTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("FlashbackDatabase"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "FlashbackDatabase");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FLASHBACK", scanner.currCommand.action);
            Assert.AreEqual("STANDBY DATABASE", scanner.currCommand.cmdName);
        }

        [Test]
        public void FlashbackTableTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("FlashbackTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "FlashbackTable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("FLASHBACK", scanner.currCommand.action);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST", scanner.currCommand.objectName);
        }

        [Test]
        public void RenameTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Rename"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Rename");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("RENAME", scanner.currCommand.action);
            Assert.AreEqual("OLD_NAME", scanner.currCommand.objectName);
            Assert.AreEqual("NEW_NAME", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void TruncateTableTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("TruncateTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "TruncateTable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TRUNCATE", scanner.currCommand.action);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST_TABLE", scanner.currCommand.objectName);
        }

        [Test]
        public void TruncateClusterTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("TruncateCluster"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "TruncateCluster");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("TRUNCATE", scanner.currCommand.action);
            Assert.AreEqual("CLUSTER", scanner.currCommand.cmdName);
            Assert.AreEqual("TEST_CLUSTER", scanner.currCommand.objectName);
        }

        [Test]
        public void SetConstraintsTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("SetConstraints"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "SetConstraints");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("SET", scanner.currCommand.action);
            Assert.AreEqual("CONSTRAINTS", scanner.currCommand.cmdName);
        }

        [Test]
        public void LockTableTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("LockTable"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "LockTable");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("LOCK", scanner.currCommand.action);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("A@REMOTE , B", scanner.currCommand.objectName);
        }

        [Test]
        public void PurgeTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Purge"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Purge");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("PURGE", scanner.currCommand.action);
            Assert.AreEqual("TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void ExplainPlanTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("ExplainPlan"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "ExplainPlan");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("EXPLAIN PLAN", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("", scanner.currCommand.objectName);
        }

        [Test]
        public void CallTest()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Call"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Call");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("CALL", scanner.currCommand.action);
            Assert.AreEqual("", scanner.currCommand.cmdName);
            Assert.AreEqual("EMP_MGMT.REMOVE_DEPT@DBLINK", scanner.currCommand.objectName);
        }

        [Test]
        public void Grant1Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Grant1"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Grant1");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("GRANT", scanner.currCommand.action);
            Assert.AreEqual("SELECT , UPDATE", scanner.currCommand.cmdName);
            Assert.AreEqual("EMP_VIEW", scanner.currCommand.objectName);
            Assert.AreEqual("PUBLIC , SCOTT", scanner.currCommand.secondaryObjectName);
        } 


        [Test]
        public void Grant2Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Grant2"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Grant2");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("GRANT", scanner.currCommand.action);
            Assert.AreEqual("CREATE ANY MATERIALIZED VIEW , ALTER ANY MATERIALIZED VIEW , DROP ANY MATERIALIZED VIEW , QUERY REWRITE , GLOBAL QUERY REWRITE", scanner.currCommand.cmdName);
            Assert.AreEqual("", scanner.currCommand.objectName);
            Assert.AreEqual("DW_MANAGER , DW_USER", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void Revoke1Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Revoke1"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Revoke1");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("REVOKE", scanner.currCommand.action);
            Assert.AreEqual("SELECT , INSERT", scanner.currCommand.cmdName);
            Assert.AreEqual("HR.TABLE1", scanner.currCommand.objectName);
            Assert.AreEqual("USERA , USERC", scanner.currCommand.secondaryObjectName);
        }


        [Test]
        public void Revoke2Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Revoke2"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Revoke2");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("REVOKE", scanner.currCommand.action);
            Assert.AreEqual("ALL", scanner.currCommand.cmdName);
            Assert.AreEqual("ORDERS", scanner.currCommand.objectName);
            Assert.AreEqual("HR", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void Revoke3Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Revoke3"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Revoke3");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("REVOKE", scanner.currCommand.action);
            Assert.AreEqual("CREATE TABLESPACE , CREATE TABLE", scanner.currCommand.cmdName);
            Assert.AreEqual("", scanner.currCommand.objectName);
            Assert.AreEqual("DW_MANAGER , DW_USER", scanner.currCommand.secondaryObjectName);
        }

        [Test]
        public void Revoke4Test()
        {

            StringReader sr = new StringReader(ResourceHelper.GetResourceString("Revoke4"));
            SQLMake.Util.Settings.loadSettings();

            SQLPlusScanner scanner;
            scanner = new SQLPlusScanner(sr, "Revoke4");
            scanner.readNextBlock();

            Assert.AreEqual(CommandTypes.Sql, scanner.currCommand.cmdType);
            Assert.AreEqual("REVOKE", scanner.currCommand.action);
            Assert.AreEqual("ALL", scanner.currCommand.cmdName);
            Assert.AreEqual("\"/2a6f1ff1_LOBCOMPRESSOR\"", scanner.currCommand.objectName);
            Assert.AreEqual("DW_MANAGER", scanner.currCommand.secondaryObjectName);
        }     

    
    }
}

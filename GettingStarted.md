# Hands on tutorial for sqlmake #

## Is sqlmake for you? ##
Sqlmake is a deployment tool for SQL scripts with support for Oracle databases. It is a windows .NET executable and requires Windows OS and .NET run-time. Oracle client is also required because sqlmake interacts with Oracle database using Oracle client DDLs.

Sqlmake was created to support easy configuration management of SQL scripts with support of automatic deployment of schema (modifications). Main sqlmake functionality is deploying data model in a database schema, keeping it up to date with new delta scripts, new versions of PL/SQL packages, privileges and synonyms.

Following goals were set during implementation of the tool:
  * Data model upgrades follow the logic described by Scott W. Ambler
  * Database schema is not only about tables and indexes. PL/SQL is vital part of the database development process and must be equally supported.
  * There must be minimal or no extra work for database developer to prepare SQL scripts
  * Existing database modeling tools, PL/SQL development tools and other must coexist with sqlmake

## How it works? ##
Sqlmake requires SQL scripts to be divided in the following three functional groups:
  * (optional) Data model directory where you keep data model baseline scripts
  * Upgrade directory where you keep sql delta scripts for incremental datamodel upgrade
  * Plsql directory where you keep stored procedures, views and everything else with "CREATE OR REPLACE" SQL syntax

How SQL scripts are organized inside the three groups is up to the user (except for the upgrade delta scripts which must conform to a naming convention). For example: all SQL commands can be in one big sql scripts, every SQL command can be in its own file, sub-directories can be created and so on. This makes it very flexible to support SQL scripts generated from any data modeling tools generator.
Using SQL scripts with sqlmake requires no additional choreography, plain old SQL\*Plus script syntax is used. Sqlmake ignores all SQL\*Plus commands for now as they are not needed with schema deployment process.


Delta upgrade scripts have to be named according to the following naming convention: "version\_scriptdesc.sql".  For example `121_create_table_employees.sql`. Version is an incremental integer value used to determine correct sequence when applying delta scripts.

Based on the functional structure when you start sqlmake you have an option to:
  * Install baseline SQL scripts
  * Upgrade schema with delta SQL scripts or
  * Deploy PL/SQL code, reconcile grants and synonyms
After invocation sqlmake parses SQL scripts and executes SQL commands against an Oracle database. No SQL\*Plus or other external tool like TOAD is required.
  * When installing data model baseline sqlmake automatically parses all SQL scripts in source directory and internally builds a list of all SQL statements. Based on a type of SQL statement and their dependency sqlmake determines the correct order of sql statement execution.
  * When upgrading data model sqlmake automatically determines current schema version and applies only necessary delta scripts
  * When deploying PL/SQL code sqlmake applies only new, modified or deleted PL/SQL code. It does this by doing a diff between PL/SQL code on file system and PL/SQL code in database. If there is no difference nothing is modified in the database schema.

At the end of every data model install/upgrade sqlmake updates schema metadata with current schema version (kept in table SQLMAKE) and reports status info.
## Try it ##
Sqlmake requires Windows, .NET 2.0 runtime (or higher) and an Oracle Client 10g (or higher). To run these examples you need access to an Oracle database.

**Download distribution package and prepare environment**

1. From the downloads page download the distribution zip file.

2. Unzip this in c:\.

3. Open a command prompt cmd.exe in c:\sqlmake

4. Set PATH variable for sqlmake.exe

Set path to sqlmake bin folder
```
C:\sqlmake>set PATH=c:\sqlmake\bin;%PATH%
```

#### Description of examples directory structure ####

Examples directory contains four subdirectories:
  1. day1
  1. day2
  1. day3
  1. playground
It is prepared as a time view of the same database project directory in different points in time (day 1, day 2, day3 ..). 

&lt;BR&gt;


If using version control like subversion it would be like [revision 1](https://code.google.com/p/sqlmake/source/detail?r=1), 2, 3 and HEAD of the database project directory.

### PART I: Datamodel baseline deployment ###
Picture yourself as a new member of sqlmake demo development project. Your first objective is to set up a development environment:

**Deploy schema to a new clean environment**

Having an access to an Oracle database the first step is creating a new database user(s). Every database project should have a script to create database users and grant required privileges. This scripts are usually executed by DBA.

5. Create users SMDEMO and SMDEMO\_APP using scripts

Default tablespace is set to USERS and temporary tablespace TEMP is used.

```
C:\sqlmake>sqlplus system/manager

SQL*Plus: Release 11.1.0.6.0 - Production on Sre Sep 16 21:47:13 2009

Copyright (c) 1982, 2007, Oracle.  All rights reserved.


Connected to:
Oracle Database 11g Enterprise Edition Release 11.1.0.6.0 - Production
With the OLAP option

SQL> set verify off
SQL> @examples\day1\dba\create_users_SMDEMO.sql
==============================================================================
Creating user SMDEMO
==============================================================================
If user already exists than this command fails
All other commands should be without errors
Enter value for user_pwd: smdemo

User created.

Setting default and temporary tablespace

User altered.

Setting tablespace quotas

User altered.

Granting system privileges

Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.


Grant succeeded.

SQL> @examples\day1\dba\create_users_SMDEMO_APP.sql
==============================================================================
Creating user SMDEMO_APP
==============================================================================
If user already exists than this command fails
All other commands should be without errors
Enter value for user_pwd: smdemo_app
old   1: create user &&user_name identified by &&user_pwd
new   1: create user SMDEMO_APP identified by smdemo_app

User created.

Setting default and temporary tablespace
old   1: alter user &&user_name default tablespace users temporary tablespace temp
new   1: alter user SMDEMO_APP default tablespace users temporary tablespace temp

User altered.

Setting tablespace quotas
old   1: alter user &&user_name quota unlimited on users
new   1: alter user SMDEMO_APP quota unlimited on users

User altered.

Granting system privileges
old   1: grant create session to &&user_name
new   1: grant create session to SMDEMO_APP

Grant succeeded.

old   1: grant create synonym to &&user_name
new   1: grant create synonym to SMDEMO_APP

Grant succeeded.

SQL> exit
Disconnected from Oracle Database 11g Enterprise Edition Release 11.1.0.6.0 - Production
With the OLAP option

C:\sqlmake>
```

Now deploy the latest baseline of the datamodel.

> Baseline is used to set up database schema from a scratch. In very early stages of a project baselines are often sufficient to prototype the database model using following cycle: drop existing schema, create a new schema from the baseline. 

&lt;BR&gt;


> After application development takes off dropping existing schema(s) is no longer possible. When you come that far upgrade scripts are the way to go. 

&lt;BR&gt;


> Baseline can still be kept in sync with changes from upgrade scripts as they represent a different perspective of the database schema. Baselines also provide alternative paths of deploying database schema in a new environment. For example to get to data model version 57 you can install starter baseline of version 1 and apply 56 upgrade scripts to get to datamodel version 57. Or you can install baseline of version 50 and apply missing 7 upgrade scripts to get to the same version 57. It is up to you if you want to keep the baseline scripts up to date and how frequent should they be updated.

6. Deploy datamodel baseline (change directory to examples\day1)
```
C:\sqlmake\examples\day1>sqlmake -install userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo\datamodel
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day1\sqlmake.config
Action: Installs schema based on install scripts

Creating sqlmake registry table ...
Searching for sql scripts ...
Loading sql commands from scripts ...
Executing SQL commands in predefined order...
===========  TABLE ======================================
create table countries...

Version 1 of datamodel installed
Installation successful
Elapsed time 0,70859 sec
```

**NOTE**:

&lt;BR&gt;


Start sqlmake from database project root directory where SQLMake.config is kept. Config files contains various details about project environment and other settings.

&lt;BR&gt;



Sqlmake by default searches for SQLMake.config in following order:
  1. config parameter passed on command line
  1. srcscriptsdir directory passed on command line
  1. current working directory
  1. if no config file is found switch to default values

### PART II: Datamodel upgrade ###
Next day your lead developer informs you of new changes in database schema. You update the local copy of repository with changes from Subversion and upgrade your database schema.

**Upgrade schema to a new version**

7. Run upgrade (change directory to examples\day2)
```
C:\sqlmake\examples\day2>sqlmake -upgrade
 userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day2\sqlmake.config
Action: Upgrades schema based on upgrade scripts

Upgrading data model ...

Execute script .\smdemo\upgrade\2_create_table_towns.sql
  create table towns...
  create primary key towns_pk on towns...
  create foreign key towns_countries_fk on towns...
  create index towns on towns_countries_fk_i...
Setting datamodel version to 2...
Installation of upgrade script 2_create_table_towns.sql successful

All upgrade scripts applied
Elapsed time 0,5080645 sec
```

**Detecting an error during upgrade to a new version**

On third day you have to upgrade your database schema again. This time the upgrade fails due to an error. Example shows how sqlmake handles errors during upgrade process.

> Following example shows what to do when an upgrade script fails in target environment. In real life scenarios upgrade scripts do not fail due to syntax errors as shown. Syntax errors are caught inside development cycle. But there are other possible reasons for errors while executing upgrade scripts for example lack of space in tablespace, constraint violation due to unexpected data and so on.

8. Run upgrade that fails with an error (change directory to examples\day3)
```
C:\sqlmake\examples\day3>sqlmake -upgrade userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day3\sqlmake.config
Action: Upgrades schema based on upgrade scripts

Upgrading data model ...

Execute script .\smdemo\upgrade\3_trigger_an_error.sql
  create seuence town_seq...
Error when executing SQL command
ORA-00901: invalid CREATE command
CREATE SEUENCE TOWN_SEQ

Setting datamodel version to 3...
Errors found during upgrade to version 3
Upgrade failed
Elapsed time 0,449057 sec
```

**Resolve an error during upgrade and clear sqlmake error status**

9. Check schema version and status

```
C:\sqlmake\examples\day3>sqlmake -status userid=smdemo/smdemo@localhost:1521/orcl
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day3\sqlmake.config
Action: Prints target schema status

Current data model version   : 3
Errors during last deployment: 1
Invalid object(s) found      : 0
Schema deployment is invalid
Elapsed time 0,3430436 sec
```

10. List errors

```
C:\sqlmake\examples\day3>sqlmake -list_errors userid=smdemo/smdemo@localhost:1521/orcl
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day3\sqlmake.config
Action: Lists all errors from last install


PROMPT   create seuence town_seq...
REM Sequence id: 1
REM .\smdemo\upgrade\3_trigger_an_error.sql, Line no: 1
REM ORA-00901: invalid CREATE command
CREATE SEUENCE TOWN_SEQ
/
Elapsed time 0,3025384 sec
```

11. Open sql\*plus and manually fix the problem

```
C:\sqlmake\examples\day3>sqlplus smdemo/smdemo

SQL*Plus: Release 11.1.0.6.0 - Production on Sre Sep 16 21:58:24 2009

Copyright (c) 1982, 2007, Oracle.  All rights reserved.


Connected to:
Oracle Database 11g Enterprise Edition Release 11.1.0.6.0 - Production
With the OLAP option

SQL> create sequence town_seq;

Sequence created.

SQL> exit
Disconnected from Oracle Database 11g Enterprise Edition Release 11.1.0.6.0 - Production
With the OLAP option
```

12. Clear errors from sqlmake metadata repository

```
C:\sqlmake\examples\day3>sqlmake -clear_errors userid=smdemo/smdemo@localhost:1521/orcl
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day3\sqlmake.config
Action: Clear all errors from last install

1 error(s) cleared
Elapsed time 0,2445311 sec
```

13. Check schema version and status

```
C:\sqlmake\examples\day3>sqlmake -status userid=smdemo/smdemo@localhost:1521/orcl
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day3\sqlmake.config
Action: Prints target schema status

Current data model version   : 3
Errors during last deployment: 0
Errors cleared after install : 1
Invalid object(s) found      : 0
Installation successful
Elapsed time 0,2240284 sec
```

**Create a new upgrade script**

Finally it is time to do something on your own. Add a new column long name to the table COUNTRIES using upgrade script and deploy it using the upgrade process.

14. Open notepad.exe (in real life you would be using your data modeling tool to create delta script)

_Be careful that notepad will not append default suffix .txt to the filename!_

15. Copy/paste following text

```
ALTER TABLE COUNTRIES
 ADD (LNAME VARCHAR2(50) NULL)
/
```

16. Save in playground as C:\sqlmake\examples\playground\smdemo\upgrade\4`_add_column_countries.sql`

**Deploy a new upgrade script**

Before committing a new upgrade script to subversion test it locally to check for any syntax errors. Most often a separate database environment is created for testing SQL script syntax where changes made by failed upgrade script can be easily reverted.

17. Run sqlmake upgrade (change directory to examples\playground)

```
C:\sqlmake\examples\playground>sqlmake -upgrade userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\playground\sqlmake.config
Action: Upgrades schema based on upgrade scripts

Upgrading data model ...

Execute script .\smdemo\upgrade\4_add_column_countries.sql
  alter table countries...
Setting datamodel version to 4...
Installation of upgrade script 4_add_column_countries.sql successful

All upgrade scripts applied
Elapsed time 0,6675847 sec
```

### PART III: Working with PL/SQL code ###

There is no need for upgrade scripts when dealing with PL/SQL code. As with any other programming language versioning comes pretty straightforward, save your stored procedures inside files (SQL scripts) and version the files. Only big difference is compile & deploy step which is done by running CREATE OR REPLACE statement against target database schema. If you are not happy with the latest revision you can always take one revision before and run it against target schema.

Deploying PL/SQL code from SQL scripts to database schema does require some kind of deployment system. It can be one large script containing all PL/SQL code or install.sql script calling other .pks, .pkb, .trg, .fnc and .prc scripts. The more PL/SQL code you have the more complex it is to maintain deployment scripts. With sqlmake forget about the maintenance overhead. Sqlmake does all the necessary PL/SQL deployment activities just point it to the directory with PL/SQL scripts and target database schema. Sqlmake does it in the following steps:
  * search through all PL/SQL code in scripts directory and build an internal list of PL/SQL objects
  * connect to target database schema and build an internal list of all PL/SQL objects
After comparing list built from the SQL scripts and list built from the database schema sqlmake does the following:
  * drop all extra PL/SQL objects from target schema
  * create new PL/SQL objects in target schema
  * recreate modified PL/SQL objects in target schema
PL/SQL objects that did not change (are equal in source SQL scripts and target database schema) are skipped therefor shortening deployment time.


&lt;BR&gt;


At the end of sqlmake run database schema is equal to PL/SQL content found in source SQL scripts.

**Deploy PL/SQL code**

On third day lead engineer informs you of a new stored procedure he committed to svn. You update your local copy of svn repository and deploy the procedure to your local schema.

18. Run PL/SQL code synchronization (change directory to examples\day3)
```
C:\sqlmake\examples\day3>sqlmake /db -plsql userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo\plsql
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day3\sqlmake.config
Action: Sync PL/SQL differences to database

Loading plsql command list
  Processing scripts in directory . ...
  Processing database catalog ...

PL/SQL differences
  Object only on filesystem
    Creating FUNCTION GET_COUNTRY_NAME
  Object only in DB
    None
  Different objects
    None
  Equal objects count:
    0 object(s)
Elapsed time 0,8341059 sec
```

**Add a new PL/SQL package**

You are given a task to create an API for a new stored procedure returning long name of the country.

19. Open notepad.exe (in real life you would be using your PL/SQL development tool)

20. Copy/paste following text
```
create or replace function get_country_lname(i_id in varchar2) return varchar2
is
begin
  return 'TEST LONG NAME';
end;
/
```

21. Save in playground as C:\sqlmake\examples\playground\smdemo\plsql\get\_country\_lname.fnc

**Deploy a new PL/SQL package**

Sqlmake automatically finds new PL/SQL function and creates it in target database schema. Notice that previously created PL/SQL function GET\_COUNTRY\_NAME did not get recompiled and is counted in category "Equal objects count".

22. Run sqlmake plsql synchronization
```
C:\sqlmake\examples\playground>sqlmake /db -plsql userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo\plsql
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\playground\sqlmake.config
Action: Sync PL/SQL differences to database

Loading plsql command list

  Processing scripts in directory . ...
  Processing database catalog ...

PL/SQL differences
  Object only on filesystem
    Creating FUNCTION GET_COUNTRY_LNAME
  Object only in DB
    None
  Different objects
    None
  Equal objects count:
    1 object(s)
Elapsed time 0,9216171 sec
```

**Modify an existing PL/SQL package**

Now it is time to complet the new stored procedure with some meaningful code.

23. Open get\_country\_lname.fnc from playground in notepad.exe (in real life you would be using your PL/SQL development tool)

24. Modify pl/sql function to return values from database
```
create or replace function get_country_lname(i_id in varchar2) return varchar2
is
  ret_val COUNTRIES.LNAME%TYPE;
begin
  select lname
  into ret_val
  from countries
  where a3 = i_id;

  return ret_Val;
end;
/
```

25. Save modified function to file

**Deploy modified PL/SQL package**

26. Run sqlmake plsql synchronization (playground)
```
C:\sqlmake\examples\playground>sqlmake /db -plsql userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo\plsql
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\playground\sqlmake.config
Action: Sync PL/SQL differences to database

Loading plsql command list
  Processing scripts in directory . ...
  Processing database catalog ...

PL/SQL differences
  Object only on filesystem
    None
  Object only in DB
    None
  Different objects
    Modified FUNCTION GET_COUNTRY_LNAME
  Equal objects count:
    1 object(s)
Elapsed time 0,6310802 sec
```

Sqlmake automatically picks up modified PL/SQL function GET\_COUNTRY\_LNAME and recompiles it in target schema.

## PART IV: Grants ##

Similar to PL/SQL objects sqlmake synchronizes grants found in SQL scripts with actual grants given from target database schema. Sqlmake searches for grants through all available SQL scripts in srcsriptsdir, so you can place your grants in any SQL script you want.

&lt;BR&gt;


I prefer to keep them in a single place like grants.sql or grants\_to\_schemaXY.sql.

Examples are all based on smdemo\grants.sql SQL script.

26. Reconciling grants to other schemas

Grants.sql contains:
```
grant select on countries to smdemo_app;
```

Reconcile grants with database:
```
C:\sqlmake\examples\day1>sqlmake /db -grants userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\sqlmake\examples\day1\sqlmake.config
Processing scripts ... .\smdemo
Processing database catalog ...
Reconciling object grants
SQL> grant SELECT on COUNTRIES to SMDEMO_APP
Elapsed time 0,6035767 sec
```


27. Adding new grants

Just add new grant to grants.sql
```
grant select on countries to smdemo_app;
grant select on towns to smdemo_app;
```

and reconcile grants between scripts and the database:
```
C:\sqlmake\examples\day2>sqlmake /db -grants userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\work\sqlmake\examples\day2\sqlmake.config
Processing scripts ... .\smdemo
Processing database catalog ...
Reconciling object grants
SQL> grant SELECT on TOWNS to SMDEMO_APP
Elapsed time 0,496063 sec
```

28. Revoking grants

Because sqlmake synchronizes grants from scripts to database, revoking grants is done by removing it from grants.sql:
```
REM grant select on countries to smdemo_app;
grant select on towns to smdemo_app;
```
and then reconciling:
```
C:\sqlmake\examples\day3>sqlmake /db -grants userid=smdemo/smdemo@localhost:1521/orcl srcscriptsdir=.\smdemo
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\work\sqlmake\examples\day3\sqlmake.config
Processing scripts ... .\smdemo
Processing database catalog ...
Reconciling object grants
SQL> revoke SELECT on COUNTRIES from SMDEMO_APP
Elapsed time 0,4045514 sec
```

## PART V: Synonyms ##

As with PL/SQL object and privileges sqlmake can also synchronize private synonyms found in SQL scripts with actual database schema synonyms. It searches for synonyms through all available SQL scripts in srcsriptsdir, so you can place your synonyms in any SQL script you want.

&lt;BR&gt;


I prefer to keep them in a single place like synonyms.sql.

Examples are all based on smdemo\_app\synonyms.sql SQL script.

29. Reconciling synonyms

Synonyms.sql contains:
```
create synonym COUNTRIES for smdemo.COUNTRIES;
```

Synonym synchronization with database schema is done using following sqlmake action:

```
C:\sqlmake\examples\day1>sqlmake /db -synonyms userid=smdemo_app/smdemo_app@localhost:1521/orcl srcscriptsdir=.\smdemo_app
SQLMake, Copyright Mitja Golouh 2008-2012, mitja.golouh@gmail.com
Revision 10
Loading settings from config in current folder C:\work\sqlmake\examples\day1\sqlmake.config
Loading synonyms command list
Processing scripts ... .\smdemo_app
Searching for sql scripts
Processing database catalog ...
Sorting command list
Reconciling synonyms
Reconciling synonyms:
SQL> create or replace synonym COUNTRIES for SMDEMO.COUNTRIES
Elapsed time 0,4740602 sec
```

Adding and removing synonyms is done through editing of synonyms.sql and re-running sqlmake synonyms sychronization.
## What next? ##

Sqlmake actions are intentionally kept very atomically. Use NANT to combine them in powerful deployment combination suited to your environment.
# Sqlmake.config #

Config file is an XML based .NET config file. Application settings are in appSettings section.

## Sample config file ##
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <system.diagnostics>
    <trace autoflush="false" indentsize="4">
      <listeners>
        <add name="myListener" type="System.Diagnostics.TextWriterTraceListener" initializeData="Sqlmake.log" />
        <remove name="Default" />
      </listeners>
    </trace>
  </system.diagnostics>
  <appSettings>
       <add key="SQLFileList" value="*.con, *.tab, *.ind, *.pks, *.pkb, *.plb, *.spc, *.bdy, *.trg, *.prc, *.fnc, *.tps, *.tpb, *.typ, *.syn, *.vw, *.sql"/>
       <add key="PLSQLFileList" value="*.pks, *.pkb, *.plb, *.spc, *.bdy, *.trg, *.prc, *.fnc, *.tps, *.tpb"/>
       <add key="IgnoreDirList" value="^[.]svn$,^setup$,^build$,^dba$,^upgrade$,^test_data$,^data$"/>
       <add key="IgnoreFileList" value="^*.config$,(?i)^ut_.*"/>
       <add key="RecurseFlag" value="true"/>


       <add key="InstallOrder" value="SYNONYM, TABLE, VIEW, COMMENT, SEQUENCE, SINGLE TABLE INSERT, PRIMARY KEY, UNIQUE KEY, UNIQUE INDEX, INDEX, FOREIGN KEY, CHECK, FUNCTION, PROCEDURE, PACKAGE, TYPE, PACKAGE BODY, TYPE BODY, TRIGGER, GRANT"/>

       <add key="DatamodelVersionLocation" value="file"/>
       <add key="DatamodelVersionFilename" value="version.sql"/>
       <add key="DatamodelVersionSearchPattern" value="define datamodel_version = ([0-9,\.]*)"/>
       <add key="DatamodelVersionIdDefinition" value="$1"/>
	   	   
       <add key="upgradeFolderName" value="upgrade"/>
	   
       <add key="Encoding" value="windows-1250"/>	   
       <add key="DebugFlag" value="false"/>

       <add key="SQLPlusVariableSubstitutionFlag" value="false"/>       

   </appSettings>
</configuration>
```

Following settings are supported:
  * File & directory filtering commands
    * [SQLFileList](SqlmakeConfig#SQLFileList.md)
    * [PLSQLFileList](SqlmakeConfig#PLSQLFileList.md)
    * [IgnoreDirList](SqlmakeConfig#IgnoreDirList.md)
    * [IgnoreFileList](SqlmakeConfig#IgnoreFileList.md)
    * [RecurseFlag](SqlmakeConfig#RecurseFlag.md)
  * DB Install options
    * [InstallOrder](SqlmakeConfig#InstallOrder.md)
    * [DatamodelVersionLocation](SqlmakeConfig#DatamodelVersionLocation.md)
    * [DatamodelVersionFilename](SqlmakeConfig#DatamodelVersionFilename.md)
    * [DatamodelVersionSearchPattern](SqlmakeConfig#DatamodelVersionSearchPattern.md)
    * [DatamodelVersionIdDefinition](SqlmakeConfig#DatamodelVersionIdDefinition.md)
  * DB Upgrade options
    * [upgradeFolderName](SqlmakeConfig#upgradeFolderName.md)
  * SQL\*plus compatibility settings
    * [SQLPlusVariableSubstitutionFlag](SqlmakeConfig#SQLPlusVariableSubstitutionFlag.md)
  * Others
    * [Encoding](SqlmakeConfig#Encoding.md)
    * [DebugFlag](SqlmakeConfig#DebugFlag.md)


## SQLFileList ##

Comma separated list of inclusive file filters used with install action.

Example:

&lt;BR&gt;


`*.con, *.tab, *.ind, *.pks, *.pkb, *.plb, *.spc, *.bdy, *.trg, *.prc, *.fnc, *.tps, *.tpb, *.typ, *.syn, *.vw, *.sql`

## PLSQLFileList ##

Comma separated list of inclusive file filters used with plsql action.

Example:

&lt;BR&gt;


`*.pks, *.pkb, *.plb, *.spc, *.bdy, *.trg, *.prc, *.fnc, *.tps, *.tpb`

## IgnoreDirList ##

Comma separated list of exclusive directory filters used within all actions dealing with sql scripts.

Example for ignoring:
  * .svn directories - `^[.]svn$`,
  * sql scripts in dba directory - `^dba$`,
  * upgrade directory that is referenced directly within upgrade action - `^upgrade$`,
  * directory with sql scripts for creating test data - `^test_data$` and
  * directory with scripts for population tables with regular data (eg. code lists) - `^data$`

## IgnoreFileList ##

Comma separated list of exclusive file filters used within all actions dealing with sql scripts.

Example on excluding:
  * config files and - `^*.config$`
  * unit test scripts - `(?i)^ut_.*`

## RecurseFlag ##

Recurse into subdirectories when searching for scripts. Possible values:
  * true
  * false

## InstallOrder ##

When installing database schema from scratch Sqlmake uses this setting to determine the correct order for running SQL commands. This setting reflects dependencies between database objects, eg. you can not create index before table.

Example:

&lt;BR&gt;


`SYNONYM, TABLE, VIEW, COMMENT, SEQUENCE, SINGLE TABLE INSERT, PRIMARY KEY, UNIQUE KEY, UNIQUE INDEX, INDEX, FOREIGN KEY, CHECK, FUNCTION, PROCEDURE, PACKAGE, TYPE, PACKAGE BODY, TYPE BODY, TRIGGER, GRANT`

## Retrieving datamodel version ##

When installing database schema from scratch Sqlmake has to retrieve version of the schema baseline from SQL scripts. Schema baseline version can  be stored as a text in a file or as a name of directory containing baseline SQL scripts.
### DatamodelVersionLocation ###
Where is schema baseline version stored:
  * file (as a text in a file)
  * directory (as a name of source scripts directory)

### DatamodelVersionFilename ###
Name of a file eg. version.sql with schema baseline version. If DatamodelVersionLocation=directory, this setting is ignored.

### DatamodelVersionSearchPattern ###
Regular expression used to extract version information eg. `define datamodel_version = ([0-9,\.]*)`

### DatamodelVersionIdDefinition ###
Format string used to format version information extracted with DatamodelVersionSearchPattern.

Most simple example is no formating: `$1`

## Upgrade directory name ##

When using upgrade action this is the directory where sqlmake looks for upgrade scripts ($srcscriptsdir\$upgradeFolderName)

## Encoding ##

Character set of SQL scripts. Encoding can be set as in http://msdn.microsoft.com/en-us/library/system.text.encoding.aspx

If not set, default language for non-Unicode program is used (Control Panel\Regional and Language Options\Advanced).

## DebugFlag ##

Reserved

## SQLPlusVariableSubstitutionFlag ##

Reserved


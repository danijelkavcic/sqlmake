Sqlmake is an Oracle database schema deployment tool. It is used for
  * Installing data model baseline from sql scripts
  * Upgrading existing schema with data model with delta scripts
  * Deploying stored procedures, views, grants to other users, private synonyms

Development of this tool was motivated with day to day issues around deployment of schema changes in development environments. With sqlmake it is very easy to keep local schema up to date with new changes from other developers. Database development must be based on sql scripts preferably stored in a version control like SVN:
  * Data model changes are organized with delta scripts; sqlmake takes care of which delta scripts are already installed, which have to be installed, logs errors during install and so on
  * Stored procedure changes are detected by comparing schema and sql scripts; if any differences are found they are automatically deployed to database schema

One of the goals set when developing sqlmake was to make minimal impact to an existing database development process:
  * Sql scripts require no additional choreography to be supported by sqlmake. Plain old SQL\*Plus syntax is used.
  * How SQL commands are placed inside scripts is completely up to user. All SQL commands can be in one big file for example, every command can be in its own file and so on. This way all existing data model and code generator output can be used with no changes.
  * There is no custom XML or any other configuration needed when adding new content to schema
  * PL/SQL development requires no additional work except for saving stored procedures as files.

To start with sqlmake look at GettingStarted and examples included in [Download](http://code.google.com/p/sqlmake/downloads/list) section.


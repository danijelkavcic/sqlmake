# Using sqlmake with Nant #

USING NANT TARGETS

## Creating a new schema from a scratch ##
  1. Create a local copy of  database folder from SVN (svn scheckout)
  1. Edit local.settings (username, password and database connect string). Do not commit local.setting to svn.
  1. Run `nant db.install`

```
<!-- Oracle deployment environment -->
<!-- Oracle database using easy connect syntx "server_name:port/service_name" -->
<property name="db.user" value = "smdemo"/>
<property name="db.password" value = "smdemo"/>	
<property name="db.datasource" value = "localhost/orcl"/>
```

Alternative is to create separate settings for a different environment:
  1. Create qa.settings (username, password and database connect string) pointing to QA environment
  1. Run `nant db.install -D:db.env=qa`

## Upgrading an existing schema ##
  1. Update local copy of  database folder from SVN with svn update
  1. Run `nant db.upgrade`

## Building from scratch to test for syntax errors in scripts ##
  1. Update local copy of  database folder from SVN with svn update
  1. Run `nant db.build`

**Db.build target is for development environment only as it drops all objects in build schema!** Build executes under separate build user defined in settings file.

```
<!-- Oracle build environment -->
<property name="db.build.user" value = "smdemo_build"/>
<property name="db.build.password" value = "smdemo_build"/>
<property name="db.build.datasource" value = "localhost/orcl"/>	
```
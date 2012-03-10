UNDEFINE user_pwd
DEFINE LINE2 = "=============================================================================="
DEFINE user_name = "SMDEMO"

PROMPT &LINE2
PROMPT Creating user &&user_name
PROMPT &LINE2

PROMPT If user already exists than this command fails
PROMPT All other commands should be without errors
create user &&user_name identified by &user_pwd;

PROMPT Setting default and temporary tablespace
alter user &&user_name default tablespace users temporary tablespace temp;

PROMPT Setting tablespace quotas
alter user &&user_name quota unlimited on users;

PROMPT Granting system privileeges
grant create session to &&user_name;
grant create synonym to &&user_name;
grant create view to &&user_name;
grant create table to &&user_name;
grant create sequence to &&user_name;
grant create database link to &&user_name;
grant create procedure to &&user_name;
grant create type to &&user_name;
grant create trigger to &&user_name;
grant debug connect session to &&user_name;
grant alter session to &&user_name;

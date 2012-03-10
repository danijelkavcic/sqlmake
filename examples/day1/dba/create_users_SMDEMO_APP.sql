UNDEFINE user_pwd
DEFINE LINE2 = "=============================================================================="
DEFINE user_name = "SMDEMO_APP"

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

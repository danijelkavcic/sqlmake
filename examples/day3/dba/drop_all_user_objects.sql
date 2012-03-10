WHENEVER SQLERROR EXIT FAILURE

declare
  cursor c_obj
  is
    SELECT object_type
    ,      object_name
    FROM   user_objects
    where  
            object_type not in ( 'PACKAGE BODY','TYPE BODY', 'INDEX', 'TRIGGER', 'UNDEFINED', 'LOB', 'TABLE PARTITION')
    and    ( object_type <> 'TABLE' or object_name not like 'BIN%')  -- do not attempt to drop already droped objects
    ;
  l_execute varchar2(2000);
begin
  for r_obj in c_obj loop
     l_execute:= 'drop '||r_obj.object_type||' '||r_obj.object_name;
     if r_obj.object_type = 'TABLE' then
       l_execute:= l_execute || ' CASCADE CONSTRAINTS PURGE';
     elsif r_obj.object_type = 'TYPE' then
       l_execute:= l_execute || ' FORCE';
     end if;
     EXECUTE IMMEDIATE l_execute;
  end loop;
end;
/


purge recyclebin;

exit

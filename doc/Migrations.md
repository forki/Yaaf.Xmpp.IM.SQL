# Yaaf.Xmpp.IM.SQL Database Migrations

All commands need to be done in the paket-manager-console!
Make sure to select the right project as your default project in the console AND
set the same project as your startup project in the solution!

## MSSQL

For MSSQL the process is quite simple:

```bash
# Because we use paket instead of nuget (use the currently used version here, from paket.lock)
Install-Package EntityFramework -Version 6.0.2

Update-Database
# (or if needed) to switch the database to a concrete state.
Update-Database –TargetMigration: state

Add-Migration 0_0_1

# Edit the new migration if needed.

# Test your changes
Update-Database
```

## MySQL

For MySQL the process is more complicated as the MySQL provider as a lot of bugs you can run into...

### Getting the initial (most likely not working SQL script)

```bash
# Because we use paket instead of nuget (use the currently used version here, from paket.lock)
Install-Package EntityFramework -Version 6.0.2

Update-Database
Add-Migration 0_0_1
# Edit your migration

Update-Database -Script
```

Now Visual Studio should greet you with a SQL script

You should look into the

```bash
Update-Database
```

If this works you are done, otherwise follow the next sections

### Fixing the script

First you need to replace all existing instances of `information_schema.columns` with `information_schema_columns` and add

```sql
USE xmpp_develop;
CREATE TEMPORARY TABLE information_schema_columns select * from information_schema.columns where table_schema = 'xmpp_develop';
```

on the top and
 
```sql
DROP TEMPORARY TABLE information_schema_columns;
```

on the bottom.
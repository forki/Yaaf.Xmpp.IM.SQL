# Yaaf.Xmpp.IM.SQL Database Migrations

All commands need to be done in the paket-manager-console!
Make sure to select the right project as your default project in the console AND
set the same project as your startup project in the solution!

```bash
Update-Database
# (or if needed) to switch the database to a concrete state.
#Update-Database –TargetMigration: state

Add-Migration 0_0_1

# Edit the new migration if needed.

# Get the script
Update-Database -Script

# Test your changes
Update-Database
```

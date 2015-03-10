# Yaaf.Xmpp.IM.SQL Database Migrations

All commands need to be done in the paket-manager-console!
Make sure to select the right project as your default project in the console AND
set the same project as your startup project in the solution!

```bash
# Because we use paket instead of nuget (use the currently used version here, from paket.lock)

Install-Package EntityFramework -Version 6.0.2

Update-Database
Add-Migration 0_0_1

# to switch the database to a concrete state.
Update-Database –TargetMigration: AddBlogUrl

# ...
# To be continued, once we actually need a migration

```
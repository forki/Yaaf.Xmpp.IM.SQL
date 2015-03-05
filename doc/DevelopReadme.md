# Yaaf.Xmpp.IM.SQL implementation documentation 

## Building

This project uses Yaaf.AdvancedBuilding, see https://matthid.github.io/Yaaf.AdvancedBuilding/DevelopReadme.html for details.

To run all unit tests you need to install a MySQL server instance and create the nunit user:

```
CREATE USER 'nunit'@'localhost' IDENTIFIED BY 'jkYjgeriE8EIEIPrJNb8';
GRANT ALL PRIVILEGES ON rosterstore_nunit.* To 'nunit'@'localhost' IDENTIFIED BY 'jkYjgeriE8EIEIPrJNb8';
```

## General overview:

This is a SQL (MSSQL, MySQL) backend for Yaaf.Xmpp.IM.

### Issues / Features / TODOs

New features are accepted via github pull requests (so just fork away right now!):  https://github.com/matthid/Yaaf.Xmpp.IM.SQL

Issues and TODOs are tracked on github, see: https://github.com/matthid/Yaaf.Xmpp.IM.SQL/issues

Discussions/Forums are on IRC. 

### Versioning: 

http://semver.org/

### High level documentation ordered by project.

- `Yaaf.Xmpp.IM.SQL`: SQL backend for Yaaf.Xmpp.IM .

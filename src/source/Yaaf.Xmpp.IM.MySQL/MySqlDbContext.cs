// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Entity;
using System.Data.Entity;
using Yaaf.Database;
using Yaaf.Database.MySQL;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;

namespace Yaaf.Xmpp.IM.Sql.MySql {

	[DbConfigurationType (typeof (MySqlEFConfiguration))]
	public class MySqlRosterStoreDbContext : AbstractRosterStoreDbContext {
		public MySqlRosterStoreDbContext (string nameOrConnection, bool doInit = false)
            : base(nameOrConnection)
        {
            if (doInit)
            {
                DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
                System.Data.Entity.Database.SetInitializer<MySqlRosterStoreDbContext>(null);
                this.Upgrade();
            }
		}

        public override string FixScript(string s)
        {
            return DatabaseUpgrade.FixScript_MySQL(s);
        }

        public override DbMigrator GetMigrator()
        {
            DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
            var config = new Yaaf.Xmpp.IM.Sql.MySql.Migrations.Configuration();
            config.TargetDatabase =
                new DbConnectionInfo(this.Database.Connection.ConnectionString, "MySql.Data.MySqlClient");
            return new DbMigrator(config);
        }
	}
}

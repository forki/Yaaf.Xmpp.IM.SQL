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

namespace Yaaf.Xmpp.IM.Sql.MySql {

	[DbConfigurationType (typeof (MySqlEFConfiguration))]
	public class MySqlRosterStoreDbContext : AbstractRosterStoreDbContext {
		protected override void Init ()
		{
            DbConfiguration.SetConfiguration (new MySqlEFConfiguration ());
            System.Data.Entity.Database.SetInitializer<MySqlRosterStoreDbContext> (
                       new MigrateDatabaseToLatestVersion<
                           MySqlRosterStoreDbContext, 
                           Yaaf.Xmpp.IM.Sql.MySql.Migrations.MySQLConfiguration<MySqlRosterStoreDbContext>>());
		}

		public MySqlRosterStoreDbContext (string nameOrConnection)
            : base(nameOrConnection)
		{
		}

		//public MySqlRosterStoreDbContext (string nameOrConnection)
		//	: base (nameOrConnection)
		//{
		//}
	}
}

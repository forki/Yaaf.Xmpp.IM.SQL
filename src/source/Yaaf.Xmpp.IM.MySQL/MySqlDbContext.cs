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
using Yaaf.Database.MySQL;

namespace Yaaf.Xmpp.IM.Sql.MySql {

	[DbConfigurationType (typeof (MySqlEFConfiguration))]
	public class MySqlRosterStoreDbContext : AbstractRosterStoreDbContext {
		public override void Init ()
		{
            DbConfiguration.SetConfiguration (new MySqlEFConfiguration ());
            System.Data.Entity.Database.SetInitializer<MySqlRosterStoreDbContext> (
                       new MigrateDatabaseToLatestVersion<
                           MySqlRosterStoreDbContext, 
                           MySQLConfiguration<MySqlRosterStoreDbContext>>());
		}

		public MySqlRosterStoreDbContext (string nameOrConnection, bool doInit = false)
            : base(nameOrConnection, false)
        {
            if (doInit)
            {
                this.DoInit();
            }
		}

		//public MySqlRosterStoreDbContext (string nameOrConnection)
		//	: base (nameOrConnection)
		//{
		//}
	}
}

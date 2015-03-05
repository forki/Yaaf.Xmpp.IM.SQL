// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using global::MySql.Data.Entity;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Yaaf.Xmpp.IM.Sql.MySql.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<Yaaf.Xmpp.IM.Sql.MySql.MySqlRosterStoreDbContext>
    {
        public Configuration()
        {
            CodeGenerator = new MySqlMigrationCodeGenerator();
			SetSqlGenerator ("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator ());
			AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Yaaf.Xmpp.IM.Sql.MySql.MySqlRosterStoreDbContext context)
        {
            //  This method will be called after migrating to the latest version.

        }
    }
}

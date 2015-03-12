// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using global::MySql.Data.Entity;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.History;
using System.Linq;
using Yaaf.Database.MySQL;

namespace Yaaf.Xmpp.IM.Sql.MySql.Migrations
{
    internal sealed class Configuration : MySQLConfiguration<MySqlRosterStoreDbContext>
    {

    }
    
    public class MigrationsContextFactory : IDbContextFactory<MySqlRosterStoreDbContext>
    {
        public MySqlRosterStoreDbContext Create()
        {
            return new MySqlRosterStoreDbContext("RosterStore_MySQL", false);
        }
    }
}

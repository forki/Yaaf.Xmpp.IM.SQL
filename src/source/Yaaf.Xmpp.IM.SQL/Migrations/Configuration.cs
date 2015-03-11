namespace Yaaf.Xmpp.IM.Sql.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Yaaf.Database;

    internal sealed class Configuration : MSSQLConfiguration<MSSQLRosterStoreDbContext>
    {

    }
    public class MigrationsContextFactory : IDbContextFactory<MSSQLRosterStoreDbContext>
    {
        public MSSQLRosterStoreDbContext Create()
        {
            return new MSSQLRosterStoreDbContext("DefaultConnection");
        }
    }

}

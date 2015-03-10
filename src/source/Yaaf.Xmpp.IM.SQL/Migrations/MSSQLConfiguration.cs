namespace Yaaf.Xmpp.IM.Sql.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;


    public sealed class MSSQLConfiguration<TContext> : DbMigrationsConfiguration<TContext> where TContext : DbContext
    {
        public MSSQLConfiguration()
		{
			AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TContext context)
        {
            //  This method will be called after migrating to the latest version.
        }
    }

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

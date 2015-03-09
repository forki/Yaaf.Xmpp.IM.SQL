namespace Yaaf.Xmpp.IM.Sql.Migrations
{
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;


    internal sealed class Configuration : DbMigrationsConfiguration<MSSQLRosterStoreDbContext>
    {
        public Configuration()
		{
			AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MSSQLRosterStoreDbContext context)
        {
            //  This method will be called after migrating to the latest version.
        }
    }
}

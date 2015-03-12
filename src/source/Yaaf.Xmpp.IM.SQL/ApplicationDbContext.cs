// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaaf.Database;
using Yaaf.Xmpp.IM.Sql.Model;

namespace Yaaf.Xmpp.IM.Sql {
	public abstract class AbstractRosterStoreDbContext : AbstractApplicationIdentityDbContext<ApplicationUser>
	{

		public AbstractRosterStoreDbContext (string nameOrConnection)
			: base (nameOrConnection)
		{
		}
		
		/// <summary>
		/// The below Method is used to define the Maping
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating (DbModelBuilder modelBuilder)
		{
			base.OnModelCreating (modelBuilder);

			modelBuilder.Entity<DbRosterItemGroup> ()
				.HasRequired (s => s.RosterItem)
				.WithMany (r => r.Groups)
				.HasForeignKey (s => new { s.ApplicationUserId, s.RosterItemId })
				//.HasKey(s => s.ApplicationUserId)
				.WillCascadeOnDelete (false);

		}

		public DbSet<DbRosterChange> RosterChanges { get; set; }
		public DbSet<DbRosterGroup> RosterGroups { get; set; }
		public DbSet<DbRosterItem> RosterItems { get; set; }
		public DbSet<DbRosterItemGroup> RosterItemGroups { get; set; }
		public DbSet<DbSubscriptionRequest> SubscriptionRequests { get; set; }
    }


	[DbConfigurationType (typeof (EmptyConfiguration))]
	public class MSSQLRosterStoreDbContext : AbstractRosterStoreDbContext
	{		
        public MSSQLRosterStoreDbContext(string nameOrConnection, bool doInit = true)
			: base (nameOrConnection)
		{
            if (doInit)
            {
                DbConfiguration.SetConfiguration(new EmptyConfiguration());
                System.Data.Entity.Database.SetInitializer<MSSQLRosterStoreDbContext>(null);
                this.Upgrade();
            }
		}

        public override DbMigrator GetMigrator()
        {
            DbConfiguration.SetConfiguration(new EmptyConfiguration());
            var config = new Yaaf.Xmpp.IM.Sql.Migrations.Configuration();
            config.TargetDatabase =
                new DbConnectionInfo(this.Database.Connection.ConnectionString, "System.Data.SqlClient");
            return new DbMigrator(config);
        }
	}
}

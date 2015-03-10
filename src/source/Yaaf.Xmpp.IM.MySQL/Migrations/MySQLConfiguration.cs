// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using global::MySql.Data.Entity;
using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.History;
using System.Linq;

namespace Yaaf.Xmpp.IM.Sql.MySql.Migrations
{

    public class MySqlHistoryContext : HistoryContext
    {
        public MySqlHistoryContext(DbConnection connection, string defaultSchema)
            : base(connection, defaultSchema)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<HistoryRow>().Property(h => h.MigrationId).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<HistoryRow>().Property(h => h.ContextKey).HasMaxLength(200).IsRequired();
        }
    } 

    public sealed class MySQLConfiguration<TContext> : DbMigrationsConfiguration<TContext> where TContext : DbContext
    {
        public MySQLConfiguration()
        {
            CodeGenerator = new MySqlMigrationCodeGenerator();
			SetSqlGenerator ("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator ());
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
			AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TContext context)
        {
            //  This method will be called after migrating to the latest version.
            
        }
    }
}

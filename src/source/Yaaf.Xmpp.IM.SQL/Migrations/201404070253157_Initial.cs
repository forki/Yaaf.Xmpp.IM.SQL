namespace Yaaf.Xmpp.IM.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RC",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        Version = c.Int(nullable: false),
                        Jid = c.String(),
                        DbChangeType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.Version })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        RosterVersion = c.Int(),
                        OldestRosterVersion = c.Int(),
                        DbCurrentPresenceData = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RI",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        Jid = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Approved = c.Boolean(nullable: false),
                        DbAskType = c.Int(nullable: false),
                        DbSubscriptionType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.Jid })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.RIG",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        RosterItemId = c.String(nullable: false, maxLength: 128),
                        RosterGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.RosterItemId, t.RosterGroupId })
                .ForeignKey("dbo.RG", t => t.RosterGroupId, cascadeDelete: true)
                .ForeignKey("dbo.RI", t => new { t.ApplicationUserId, t.RosterItemId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.RosterGroupId)
                .Index(t => new { t.ApplicationUserId, t.RosterItemId })
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.RG",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SR",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        FromJid = c.String(nullable: false, maxLength: 128),
                        Content = c.String(),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.FromJid })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SR", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RIG", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RC", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.RIG", new[] { "ApplicationUserId", "RosterItemId" }, "dbo.RI");
            DropForeignKey("dbo.RIG", "RosterGroupId", "dbo.RG");
            DropForeignKey("dbo.RI", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.SR", new[] { "ApplicationUserId" });
            DropIndex("dbo.RIG", new[] { "ApplicationUserId" });
            DropIndex("dbo.RC", new[] { "ApplicationUserId" });
            DropIndex("dbo.RIG", new[] { "ApplicationUserId", "RosterItemId" });
            DropIndex("dbo.RIG", new[] { "RosterGroupId" });
            DropIndex("dbo.RI", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropTable("dbo.SR");
            DropTable("dbo.RG");
            DropTable("dbo.RIG");
            DropTable("dbo.RI");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.RC");
            DropTable("dbo.AspNetRoles");
        }
    }
}

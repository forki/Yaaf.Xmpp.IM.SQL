namespace Yaaf.Xmpp.IM.Sql.MySql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        Name = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "RC",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        Version = c.Int(nullable: false),
                        Jid = c.String(unicode: false),
                        DbChangeType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.Version });
            
            CreateTable(
                "AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        UserName = c.String(unicode: false),
                        PasswordHash = c.String(unicode: false),
                        SecurityStamp = c.String(unicode: false),
                        RosterVersion = c.Int(),
                        OldestRosterVersion = c.Int(),
                        DbCurrentPresenceData = c.String(unicode: false),
                        Discriminator = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(unicode: false),
                        ClaimValue = c.String(unicode: false),
                        User_Id = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        LoginProvider = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        ProviderKey = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey });
            
            CreateTable(
                "AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        RoleId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId });
            
            CreateTable(
                "RI",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        Jid = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        Name = c.String(unicode: false),
                        Approved = c.Boolean(nullable: false),
                        DbAskType = c.Int(nullable: false),
                        DbSubscriptionType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.Jid });
            
            CreateTable(
                "RIG",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        RosterItemId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        RosterGroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.RosterItemId, t.RosterGroupId });
            
            CreateTable(
                "RG",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "SR",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        FromJid = c.String(nullable: false, maxLength: 128, unicode: false, storeType: "nvarchar"),
                        Content = c.String(unicode: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.FromJid });
            
        }
        
        public override void Down()
        {
            DropForeignKey("SR", "FK_dbo.SR_dbo.AspNetUsers_ApplicationUserId");
            DropForeignKey("RIG", "FK_dbo.RIG_dbo.AspNetUsers_ApplicationUserId");
            DropForeignKey("RC", "FK_dbo.RC_dbo.AspNetUsers_ApplicationUserId");
            DropForeignKey("RIG", "FK_dbo.RIG_dbo.RI_ApplicationUserId_RosterItemId");
            DropForeignKey("RIG", "FK_dbo.RIG_dbo.RG_RosterGroupId");
            DropForeignKey("RI", "FK_dbo.RI_dbo.AspNetUsers_ApplicationUserId");
            DropForeignKey("AspNetUserClaims", "FK_dbo.AspNetUserClaims_dbo.AspNetUsers_User_Id");
            DropForeignKey("AspNetUserRoles", "FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId");
            DropForeignKey("AspNetUserRoles", "FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId");
            DropForeignKey("AspNetUserLogins", "FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId");
            DropIndex("SR", new[] { "ApplicationUserId" });
            DropIndex("RIG", new[] { "ApplicationUserId" });
            DropIndex("RC", new[] { "ApplicationUserId" });
            DropIndex("RIG", new[] { "ApplicationUserId", "RosterItemId" });
            DropIndex("RIG", new[] { "RosterGroupId" });
            DropIndex("RI", new[] { "ApplicationUserId" });
            DropIndex("AspNetUserClaims", new[] { "User_Id" });
            DropIndex("AspNetUserRoles", new[] { "UserId" });
            DropIndex("AspNetUserRoles", new[] { "RoleId" });
            DropIndex("AspNetUserLogins", new[] { "UserId" });
            DropTable("SR");
            DropTable("RG");
            DropTable("RIG");
            DropTable("RI");
            DropTable("AspNetUserRoles");
            DropTable("AspNetUserLogins");
            DropTable("AspNetUserClaims");
            DropTable("AspNetUsers");
            DropTable("RC");
            DropTable("AspNetRoles");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewClasses2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lineups",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                        SubId = c.Guid(identity: true),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PlayerStats",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Substitutions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Summaries",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Summaries");
            DropTable("dbo.Substitutions");
            DropTable("dbo.PlayerStats");
            DropTable("dbo.Players");
            DropTable("dbo.Lineups");
        }
    }
}

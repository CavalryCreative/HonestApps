namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewClasses : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stats",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MatchId = c.Guid(nullable: false),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Matches", t => t.MatchId, cascadeDelete: true)
                .Index(t => t.MatchId);
            
            CreateTable(
                "dbo.Rivals",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TeamId = c.Guid(nullable: false),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rivals", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.Stats", "MatchId", "dbo.Matches");
            DropIndex("dbo.Rivals", new[] { "TeamId" });
            DropIndex("dbo.Stats", new[] { "MatchId" });
            DropTable("dbo.Rivals");
            DropTable("dbo.Stats");
        }
    }
}

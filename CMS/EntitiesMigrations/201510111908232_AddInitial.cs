namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
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
                "dbo.Matches",
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
                "dbo.Teams",
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
                "dbo.TeamMatches",
                c => new
                    {
                        MatchId = c.Guid(nullable: false),
                        TeamId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.MatchId, t.TeamId })
                .ForeignKey("dbo.Teams", t => t.MatchId, cascadeDelete: true)
                .ForeignKey("dbo.Matches", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.MatchId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamMatches", "TeamId", "dbo.Matches");
            DropForeignKey("dbo.TeamMatches", "MatchId", "dbo.Teams");
            DropForeignKey("dbo.Events", "MatchId", "dbo.Matches");
            DropIndex("dbo.TeamMatches", new[] { "TeamId" });
            DropIndex("dbo.TeamMatches", new[] { "MatchId" });
            DropIndex("dbo.Events", new[] { "MatchId" });
            DropTable("dbo.TeamMatches");
            DropTable("dbo.Teams");
            DropTable("dbo.Matches");
            DropTable("dbo.Events");
        }
    }
}

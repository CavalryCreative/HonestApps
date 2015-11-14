namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGoalCardetc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Goals",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        SummaryId = c.Guid(nullable: false),
                        IsHomeTeam = c.Boolean(nullable: false),
                        PlayerName = c.String(),
                        APIPlayerId = c.Int(nullable: false),
                        Minute = c.Byte(nullable: false),
                        OwnGoal = c.Boolean(nullable: false),
                        Penalty = c.Boolean(nullable: false),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Summaries", t => t.SummaryId, cascadeDelete: true)
                .Index(t => t.SummaryId);
            
            CreateTable(
                "dbo.RedCards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SummaryId = c.Guid(nullable: false),
                        IsHomeTeam = c.Boolean(nullable: false),
                        PlayerName = c.String(),
                        APIPlayerId = c.Int(nullable: false),
                        Minute = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Summaries", t => t.SummaryId, cascadeDelete: true)
                .Index(t => t.SummaryId);
            
            CreateTable(
                "dbo.YellowCards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SummaryId = c.Guid(nullable: false),
                        IsHomeTeam = c.Boolean(nullable: false),
                        PlayerName = c.String(),
                        APIPlayerId = c.Int(nullable: false),
                        Minute = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Summaries", t => t.SummaryId, cascadeDelete: true)
                .Index(t => t.SummaryId);
            
            AddColumn("dbo.Matches", "Stadium", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Matches", "Attendance", c => c.String(maxLength: 10, unicode: false));
            AddColumn("dbo.Matches", "Time", c => c.String(maxLength: 20, unicode: false));
            AddColumn("dbo.Matches", "Referee", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Summaries", "HomeTeam", c => c.Guid(nullable: false));
            AddColumn("dbo.Summaries", "AwayTeam", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.YellowCards", "SummaryId", "dbo.Summaries");
            DropForeignKey("dbo.RedCards", "SummaryId", "dbo.Summaries");
            DropForeignKey("dbo.Goals", "SummaryId", "dbo.Summaries");
            DropIndex("dbo.YellowCards", new[] { "SummaryId" });
            DropIndex("dbo.RedCards", new[] { "SummaryId" });
            DropIndex("dbo.Goals", new[] { "SummaryId" });
            DropColumn("dbo.Summaries", "AwayTeam");
            DropColumn("dbo.Summaries", "HomeTeam");
            DropColumn("dbo.Matches", "Referee");
            DropColumn("dbo.Matches", "Time");
            DropColumn("dbo.Matches", "Attendance");
            DropColumn("dbo.Matches", "Stadium");
            DropTable("dbo.YellowCards");
            DropTable("dbo.RedCards");
            DropTable("dbo.Goals");
        }
    }
}

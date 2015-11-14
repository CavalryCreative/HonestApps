namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGoalCard2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RedCards", "SummaryId", "dbo.Summaries");
            DropForeignKey("dbo.YellowCards", "SummaryId", "dbo.Summaries");
            DropIndex("dbo.RedCards", new[] { "SummaryId" });
            DropIndex("dbo.YellowCards", new[] { "SummaryId" });
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SummaryId = c.Guid(nullable: false),
                        IsHomeTeam = c.Boolean(nullable: false),
                        PlayerName = c.String(maxLength: 250, unicode: false),
                        APIPlayerId = c.Int(nullable: false),
                        Minute = c.Byte(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Summary_Id = c.Guid(),
                        Summary_Id1 = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Summaries", t => t.Summary_Id)
                .ForeignKey("dbo.Summaries", t => t.Summary_Id1)
                .Index(t => t.Summary_Id)
                .Index(t => t.Summary_Id1);
            
            AlterColumn("dbo.Goals", "PlayerName", c => c.String(maxLength: 250, unicode: false));
            DropTable("dbo.RedCards");
            DropTable("dbo.YellowCards");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Cards", "Summary_Id1", "dbo.Summaries");
            DropForeignKey("dbo.Cards", "Summary_Id", "dbo.Summaries");
            DropIndex("dbo.Cards", new[] { "Summary_Id1" });
            DropIndex("dbo.Cards", new[] { "Summary_Id" });
            AlterColumn("dbo.Goals", "PlayerName", c => c.String());
            DropTable("dbo.Cards");
            CreateIndex("dbo.YellowCards", "SummaryId");
            CreateIndex("dbo.RedCards", "SummaryId");
            AddForeignKey("dbo.YellowCards", "SummaryId", "dbo.Summaries", "Id", cascadeDelete: true);
            AddForeignKey("dbo.RedCards", "SummaryId", "dbo.Summaries", "Id", cascadeDelete: true);
        }
    }
}

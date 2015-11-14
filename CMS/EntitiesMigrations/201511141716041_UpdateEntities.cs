namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerTeams",
                c => new
                    {
                        Player_Id = c.Guid(nullable: false),
                        Team_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Player_Id, t.Team_Id })
                .ForeignKey("dbo.Players", t => t.Player_Id, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team_Id, cascadeDelete: true)
                .Index(t => t.Player_Id)
                .Index(t => t.Team_Id);
            
            AddColumn("dbo.Lineups", "Match_Id", c => c.Guid());
            AddColumn("dbo.Lineups", "Team_Id", c => c.Guid());
            AddColumn("dbo.PlayerStats", "Player_Id", c => c.Guid());
            AddColumn("dbo.Substitutions", "MatchId", c => c.Guid(nullable: false));
            AddColumn("dbo.Summaries", "MatchId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Lineups", "Match_Id");
            CreateIndex("dbo.Substitutions", "MatchId");
            CreateIndex("dbo.Summaries", "MatchId");
            CreateIndex("dbo.Lineups", "Team_Id");
            CreateIndex("dbo.PlayerStats", "Player_Id");
            AddForeignKey("dbo.Lineups", "Match_Id", "dbo.Matches", "Id");
            AddForeignKey("dbo.Substitutions", "MatchId", "dbo.Matches", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Summaries", "MatchId", "dbo.Matches", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Lineups", "Team_Id", "dbo.Teams", "Id");
            AddForeignKey("dbo.PlayerStats", "Player_Id", "dbo.Players", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerTeams", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.PlayerTeams", "Player_Id", "dbo.Players");
            DropForeignKey("dbo.PlayerStats", "Player_Id", "dbo.Players");
            DropForeignKey("dbo.Lineups", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Summaries", "MatchId", "dbo.Matches");
            DropForeignKey("dbo.Substitutions", "MatchId", "dbo.Matches");
            DropForeignKey("dbo.Lineups", "Match_Id", "dbo.Matches");
            DropIndex("dbo.PlayerTeams", new[] { "Team_Id" });
            DropIndex("dbo.PlayerTeams", new[] { "Player_Id" });
            DropIndex("dbo.PlayerStats", new[] { "Player_Id" });
            DropIndex("dbo.Lineups", new[] { "Team_Id" });
            DropIndex("dbo.Summaries", new[] { "MatchId" });
            DropIndex("dbo.Substitutions", new[] { "MatchId" });
            DropIndex("dbo.Lineups", new[] { "Match_Id" });
            DropColumn("dbo.Summaries", "MatchId");
            DropColumn("dbo.Substitutions", "MatchId");
            DropColumn("dbo.PlayerStats", "Player_Id");
            DropColumn("dbo.Lineups", "Team_Id");
            DropColumn("dbo.Lineups", "Match_Id");
            DropTable("dbo.PlayerTeams");
        }
    }
}

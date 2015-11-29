namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch3 : DbMigration
    {
        public override void Up()
        {           
            DropForeignKey("dbo.TeamMatches", "MatchId", "dbo.Teams");
            DropForeignKey("dbo.TeamMatches", "TeamId", "dbo.Matches");
            DropIndex("dbo.TeamMatches", new[] { "MatchId" });
            DropIndex("dbo.TeamMatches", new[] { "TeamId" });
            DropTable(name: "dbo.TeamMatches");
            AddColumn("dbo.Matches", "HomeTeamAPIId", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "AwayTeamAPIId", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "Team_Id", c => c.Guid());
            CreateIndex("dbo.Matches", "Team_Id");
            AddForeignKey("dbo.Matches", "Team_Id", "dbo.Teams", "Id");
            DropColumn("dbo.Matches", "HomeTeamAPI");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "HomeTeamAPI", c => c.Int(nullable: false));
            DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
            DropIndex("dbo.Matches", new[] { "Team_Id" });
            DropColumn("dbo.Matches", "Team_Id");
            DropColumn("dbo.Matches", "AwayTeamAPIId");
            DropColumn("dbo.Matches", "HomeTeamAPIId");
            CreateIndex("dbo.TeamMatches", "TeamId");
            CreateIndex("dbo.TeamMatches", "MatchId");
            AddForeignKey("dbo.TeamMatches", "TeamId", "dbo.Matches", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TeamMatches", "MatchId", "dbo.Teams", "Id", cascadeDelete: true);
            RenameTable(name: "dbo.Matches", newName: "TeamMatches");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlayers : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PlayerTeams", newName: "TeamPlayers");
            AddColumn("dbo.Stats", "HomeTeamTotalShots", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamOnGoalShots", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamFouls", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamCorners", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamOffsides", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamPossessionTime", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamYellowCards", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamRedCards", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "HomeTeamSaves", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamTotalShots", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamOnGoalShots", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamFouls", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamCorners", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamOffsides", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamPossessionTime", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamYellowCards", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamRedCards", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamSaves", c => c.Byte(nullable: false));
            AddColumn("dbo.Players", "SquadNumber", c => c.Byte(nullable: false));
            AddColumn("dbo.Players", "Name", c => c.String(maxLength: 250, unicode: false));
            AddColumn("dbo.Players", "Position", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Players", "APIPlayerId", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "Eleven_Id", c => c.Guid());
            AddColumn("dbo.Players", "Eleven_Id1", c => c.Guid());
            AddColumn("dbo.Players", "Sub_Id", c => c.Guid());
            AddColumn("dbo.Players", "Sub_Id1", c => c.Guid());
            CreateIndex("dbo.Players", "Eleven_Id");
            CreateIndex("dbo.Players", "Eleven_Id1");
            CreateIndex("dbo.Players", "Sub_Id");
            CreateIndex("dbo.Players", "Sub_Id1");
            AddForeignKey("dbo.Players", "Eleven_Id", "dbo.Lineups", "Id");
            AddForeignKey("dbo.Players", "Eleven_Id1", "dbo.Lineups", "Id");
            AddForeignKey("dbo.Players", "Sub_Id", "dbo.Lineups", "Id");
            AddForeignKey("dbo.Players", "Sub_Id1", "dbo.Lineups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "Sub_Id1", "dbo.Lineups");
            DropForeignKey("dbo.Players", "Sub_Id", "dbo.Lineups");
            DropForeignKey("dbo.Players", "Eleven_Id1", "dbo.Lineups");
            DropForeignKey("dbo.Players", "Eleven_Id", "dbo.Lineups");
            DropIndex("dbo.Players", new[] { "Sub_Id1" });
            DropIndex("dbo.Players", new[] { "Sub_Id" });
            DropIndex("dbo.Players", new[] { "Eleven_Id1" });
            DropIndex("dbo.Players", new[] { "Eleven_Id" });
            DropColumn("dbo.Players", "Sub_Id1");
            DropColumn("dbo.Players", "Sub_Id");
            DropColumn("dbo.Players", "Eleven_Id1");
            DropColumn("dbo.Players", "Eleven_Id");
            DropColumn("dbo.Players", "APIPlayerId");
            DropColumn("dbo.Players", "Position");
            DropColumn("dbo.Players", "Name");
            DropColumn("dbo.Players", "SquadNumber");
            DropColumn("dbo.Stats", "AwayTeamSaves");
            DropColumn("dbo.Stats", "AwayTeamRedCards");
            DropColumn("dbo.Stats", "AwayTeamYellowCards");
            DropColumn("dbo.Stats", "AwayTeamPossessionTime");
            DropColumn("dbo.Stats", "AwayTeamOffsides");
            DropColumn("dbo.Stats", "AwayTeamCorners");
            DropColumn("dbo.Stats", "AwayTeamFouls");
            DropColumn("dbo.Stats", "AwayTeamOnGoalShots");
            DropColumn("dbo.Stats", "AwayTeamTotalShots");
            DropColumn("dbo.Stats", "HomeTeamSaves");
            DropColumn("dbo.Stats", "HomeTeamRedCards");
            DropColumn("dbo.Stats", "HomeTeamYellowCards");
            DropColumn("dbo.Stats", "HomeTeamPossessionTime");
            DropColumn("dbo.Stats", "HomeTeamOffsides");
            DropColumn("dbo.Stats", "HomeTeamCorners");
            DropColumn("dbo.Stats", "HomeTeamFouls");
            DropColumn("dbo.Stats", "HomeTeamOnGoalShots");
            DropColumn("dbo.Stats", "HomeTeamTotalShots");
            RenameTable(name: "dbo.TeamPlayers", newName: "PlayerTeams");
        }
    }
}

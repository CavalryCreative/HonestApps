namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateVarious : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PlayerStats", "Player_Id", "dbo.Players");
            DropIndex("dbo.PlayerStats", new[] { "Player_Id" });
            RenameColumn(table: "dbo.PlayerStats", name: "Player_Id", newName: "PlayerId");
            AddColumn("dbo.Events", "Important", c => c.Boolean(nullable: false));
            AddColumn("dbo.Events", "Goal", c => c.Boolean(nullable: false));
            AddColumn("dbo.Events", "Minute", c => c.Byte(nullable: false));
            AddColumn("dbo.Events", "Comment", c => c.String(maxLength: 1000, unicode: false));
            AddColumn("dbo.Events", "APIId", c => c.Int(nullable: false));
            AddColumn("dbo.PlayerStats", "MatchId", c => c.Guid(nullable: false));
            AddColumn("dbo.PlayerStats", "PositionX", c => c.Byte(nullable: false));
            AddColumn("dbo.PlayerStats", "PositionY", c => c.Byte(nullable: false));
            AddColumn("dbo.PlayerStats", "TotalShots", c => c.Byte());
            AddColumn("dbo.PlayerStats", "ShotsOnGoal", c => c.Byte());
            AddColumn("dbo.PlayerStats", "Goals", c => c.Byte());
            AddColumn("dbo.PlayerStats", "Assists", c => c.Byte());
            AddColumn("dbo.PlayerStats", "Offsides", c => c.Byte());
            AddColumn("dbo.PlayerStats", "FoulsDrawn", c => c.Byte());
            AddColumn("dbo.PlayerStats", "FoulsCommitted", c => c.Byte());
            AddColumn("dbo.PlayerStats", "Saves", c => c.Byte());
            AddColumn("dbo.PlayerStats", "YellowCards", c => c.Byte());
            AddColumn("dbo.PlayerStats", "RedCards", c => c.Byte());
            AddColumn("dbo.PlayerStats", "PenaltiesScored", c => c.Byte());
            AddColumn("dbo.PlayerStats", "PenaltiesMissed", c => c.Byte());
            AddColumn("dbo.PlayerStats", "APIId", c => c.Int(nullable: false));
            AddColumn("dbo.Substitutions", "PlayerOff", c => c.String(maxLength: 250, unicode: false));
            AddColumn("dbo.Substitutions", "PlayerOffId", c => c.Guid(nullable: false));
            AddColumn("dbo.Substitutions", "APIPlayerOffId", c => c.Int(nullable: false));
            AddColumn("dbo.Substitutions", "PlayerOn", c => c.String(maxLength: 250, unicode: false));
            AddColumn("dbo.Substitutions", "PlayerOnId", c => c.Guid(nullable: false));
            AddColumn("dbo.Substitutions", "APIPlayerOnId", c => c.Int(nullable: false));
            AddColumn("dbo.Substitutions", "Minute", c => c.Byte(nullable: false));
            AddColumn("dbo.Substitutions", "IsHomeTeam", c => c.Boolean(nullable: false));
            AlterColumn("dbo.PlayerStats", "PlayerId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamTotalShots", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamOnGoalShots", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamFouls", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamCorners", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamOffsides", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamPossessionTime", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamYellowCards", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamRedCards", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamSaves", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamTotalShots", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamOnGoalShots", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamFouls", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamCorners", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamOffsides", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamPossessionTime", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamYellowCards", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamRedCards", c => c.Byte());
            AlterColumn("dbo.Stats", "AwayTeamSaves", c => c.Byte());
            CreateIndex("dbo.PlayerStats", "MatchId");
            CreateIndex("dbo.PlayerStats", "PlayerId");
            AddForeignKey("dbo.PlayerStats", "MatchId", "dbo.Matches", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PlayerStats", "PlayerId", "dbo.Players", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerStats", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.PlayerStats", "MatchId", "dbo.Matches");
            DropIndex("dbo.PlayerStats", new[] { "PlayerId" });
            DropIndex("dbo.PlayerStats", new[] { "MatchId" });
            AlterColumn("dbo.Stats", "AwayTeamSaves", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamRedCards", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamYellowCards", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamPossessionTime", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamOffsides", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamCorners", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamFouls", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamOnGoalShots", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "AwayTeamTotalShots", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamSaves", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamRedCards", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamYellowCards", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamPossessionTime", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamOffsides", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamCorners", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamFouls", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamOnGoalShots", c => c.Byte(nullable: false));
            AlterColumn("dbo.Stats", "HomeTeamTotalShots", c => c.Byte(nullable: false));
            AlterColumn("dbo.PlayerStats", "PlayerId", c => c.Guid());
            DropColumn("dbo.Substitutions", "IsHomeTeam");
            DropColumn("dbo.Substitutions", "Minute");
            DropColumn("dbo.Substitutions", "APIPlayerOnId");
            DropColumn("dbo.Substitutions", "PlayerOnId");
            DropColumn("dbo.Substitutions", "PlayerOn");
            DropColumn("dbo.Substitutions", "APIPlayerOffId");
            DropColumn("dbo.Substitutions", "PlayerOffId");
            DropColumn("dbo.Substitutions", "PlayerOff");
            DropColumn("dbo.PlayerStats", "APIId");
            DropColumn("dbo.PlayerStats", "PenaltiesMissed");
            DropColumn("dbo.PlayerStats", "PenaltiesScored");
            DropColumn("dbo.PlayerStats", "RedCards");
            DropColumn("dbo.PlayerStats", "YellowCards");
            DropColumn("dbo.PlayerStats", "Saves");
            DropColumn("dbo.PlayerStats", "FoulsCommitted");
            DropColumn("dbo.PlayerStats", "FoulsDrawn");
            DropColumn("dbo.PlayerStats", "Offsides");
            DropColumn("dbo.PlayerStats", "Assists");
            DropColumn("dbo.PlayerStats", "Goals");
            DropColumn("dbo.PlayerStats", "ShotsOnGoal");
            DropColumn("dbo.PlayerStats", "TotalShots");
            DropColumn("dbo.PlayerStats", "PositionY");
            DropColumn("dbo.PlayerStats", "PositionX");
            DropColumn("dbo.PlayerStats", "MatchId");
            DropColumn("dbo.Events", "APIId");
            DropColumn("dbo.Events", "Comment");
            DropColumn("dbo.Events", "Minute");
            DropColumn("dbo.Events", "Goal");
            DropColumn("dbo.Events", "Important");
            RenameColumn(table: "dbo.PlayerStats", name: "PlayerId", newName: "Player_Id");
            CreateIndex("dbo.PlayerStats", "Player_Id");
            AddForeignKey("dbo.PlayerStats", "Player_Id", "dbo.Players", "Id");
        }
    }
}

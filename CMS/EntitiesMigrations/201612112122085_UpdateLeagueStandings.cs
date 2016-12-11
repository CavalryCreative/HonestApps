namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLeagueStandings : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LeagueStandings", "GamesPlayed", c => c.Short(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GamesWon", c => c.Short(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GamesDrawn", c => c.Short(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GamesLost", c => c.Short(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GoalsScored", c => c.Short(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GoalsConceded", c => c.Short(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GoalDifference", c => c.Short(nullable: false));
            AlterColumn("dbo.LeagueStandings", "Points", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LeagueStandings", "Points", c => c.Byte(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GoalDifference", c => c.Byte(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GoalsConceded", c => c.Byte(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GoalsScored", c => c.Byte(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GamesLost", c => c.Byte(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GamesDrawn", c => c.Byte(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GamesWon", c => c.Byte(nullable: false));
            AlterColumn("dbo.LeagueStandings", "GamesPlayed", c => c.Byte(nullable: false));
        }
    }
}

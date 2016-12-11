namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLeagueStandings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LeagueStandings",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        APIId = c.Int(nullable: false),
                        Name = c.String(maxLength: 500, unicode: false),
                        Position = c.Byte(nullable: false),
                        GamesPlayed = c.Byte(nullable: false),
                        GamesWon = c.Byte(nullable: false),
                        GamesDrawn = c.Byte(nullable: false),
                        GamesLost = c.Byte(nullable: false),
                        GoalsScored = c.Byte(nullable: false),
                        GoalsConceded = c.Byte(nullable: false),
                        GoalDifference = c.Byte(nullable: false),
                        Points = c.Byte(nullable: false),
                        Description = c.String(maxLength: 500, unicode: false),
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
            DropTable("dbo.LeagueStandings");
        }
    }
}

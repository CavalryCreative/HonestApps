namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateFeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MatchesTodays",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        APIId = c.Int(nullable: false),
                        KickOffTime = c.String(maxLength: 10, unicode: false),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Events", "Score", c => c.String());
            AddColumn("dbo.Events", "HomeTeamMatchRating", c => c.Byte(nullable: false));
            AddColumn("dbo.Events", "AwayTeamMatchRating", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "AwayTeamMatchRating");
            DropColumn("dbo.Events", "HomeTeamMatchRating");
            DropColumn("dbo.Events", "Score");
            DropTable("dbo.MatchesTodays");
        }
    }
}

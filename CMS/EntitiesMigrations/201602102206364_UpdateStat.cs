namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stats", "HomeTeamRating", c => c.Byte(nullable: false));
            AddColumn("dbo.Stats", "AwayTeamRating", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stats", "AwayTeamRating");
            DropColumn("dbo.Stats", "HomeTeamRating");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlayerStats : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "PlayerRating", c => c.Byte(nullable: false));
            AddColumn("dbo.PlayerStats", "Rating", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlayerStats", "Rating");
            DropColumn("dbo.Comments", "PlayerRating");
        }
    }
}

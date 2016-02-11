namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStat1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stats", "MatchRating", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stats", "MatchRating");
        }
    }
}

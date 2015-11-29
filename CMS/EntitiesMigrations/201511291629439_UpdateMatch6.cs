namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "IsToday", c => c.Boolean(nullable: false));
            AddColumn("dbo.Matches", "IsLive", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "IsLive");
            DropColumn("dbo.Matches", "IsToday");
        }
    }
}

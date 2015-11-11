namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSub : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lineups", "IsSub", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lineups", "IsSub");
        }
    }
}

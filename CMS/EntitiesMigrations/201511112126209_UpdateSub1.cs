namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSub1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Lineups", "IsSub");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lineups", "IsSub", c => c.Boolean());
        }
    }
}

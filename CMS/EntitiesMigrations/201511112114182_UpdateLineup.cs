namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLineup : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Lineups", "Id", c => c.Guid(nullable: false));
            DropColumn("dbo.Lineups", "SubId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lineups", "SubId", c => c.Guid(identity: true));
            AlterColumn("dbo.Lineups", "Id", c => c.Guid(nullable: false, identity: true));
        }
    }
}

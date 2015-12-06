namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUpdateHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UpdateHistories", "MatchStats", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UpdateHistories", "MatchStats");
        }
    }
}

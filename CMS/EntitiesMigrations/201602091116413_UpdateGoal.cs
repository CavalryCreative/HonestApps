namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateGoal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Goals", "APIId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Goals", "APIId");
        }
    }
}

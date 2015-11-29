namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "APIId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "APIId");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "APIId", c => c.Guid(nullable: false));
            AddColumn("dbo.Teams", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "Name");
            DropColumn("dbo.Teams", "APIId");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam2 : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.Teams", "APIId", c => c.Int(nullable: false));
            DropColumn("dbo.Teams", "APIId");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Teams", "APIId", c => c.Guid(nullable: false));
        }
    }
}

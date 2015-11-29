namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam3 : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.Teams", "APIId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "APIId", c => c.Int(nullable: false));
        }
    }
}

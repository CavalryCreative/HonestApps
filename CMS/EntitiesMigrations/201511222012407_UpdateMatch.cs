namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "APIId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "APIId");
        }
    }
}

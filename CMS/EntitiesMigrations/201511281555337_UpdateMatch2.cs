namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "HomeTeamAPI", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "HomeTeamAPI");
        }
    }
}

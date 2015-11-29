namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "HomeTeamId", c => c.Guid(nullable: false));
            AddColumn("dbo.Matches", "AwayTeamId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "AwayTeamId");
            DropColumn("dbo.Matches", "HomeTeamId");
        }
    }
}

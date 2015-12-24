namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateJim1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "HomeTeamId", c => c.Guid(nullable: false));
            AddColumn("dbo.Matches", "AwayTeamId", c => c.Guid(nullable: false));
            AlterColumn("dbo.Teams", "Name", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("dbo.Teams", "Stadium", c => c.String(maxLength: 500, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Teams", "Stadium", c => c.String());
            AlterColumn("dbo.Teams", "Name", c => c.String());
            DropColumn("dbo.Matches", "AwayTeamId");
            DropColumn("dbo.Matches", "HomeTeamId");
        }
    }
}

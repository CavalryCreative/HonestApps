namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stats", "HomeTeamPossessionTime", c => c.String());
            AlterColumn("dbo.Stats", "AwayTeamPossessionTime", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stats", "AwayTeamPossessionTime", c => c.Byte());
            AlterColumn("dbo.Stats", "HomeTeamPossessionTime", c => c.Byte());
        }
    }
}

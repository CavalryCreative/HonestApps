namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "Stadium", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "Stadium");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch22 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "EndDate");
        }
    }
}

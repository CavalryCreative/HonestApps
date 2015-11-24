namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "Date");
        }
    }
}

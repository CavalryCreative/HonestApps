namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Matches", "Team_Id");
        }
        
        public override void Down()
        {
        }
    }
}

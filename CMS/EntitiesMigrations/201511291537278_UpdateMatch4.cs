namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
        }
        
        public override void Down()
        {
        }
    }
}

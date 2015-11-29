namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
            //DropIndex("dbo.Matches", new[] { "Team_Id" });
            //DropColumn("dbo.Matches", "Team_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "Team_Id", c => c.Guid());
            CreateIndex("dbo.Matches", "Team_Id");
            AddForeignKey("dbo.Matches", "Team_Id", "dbo.Teams", "Id");
        }
    }
}

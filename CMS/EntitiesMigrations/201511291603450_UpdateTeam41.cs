namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam41 : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
            //DropIndex("dbo.Matches", new[] { "Team_Id" });
            AddColumn("dbo.Matches", "HomeTeamId", c => c.Guid());
            AddColumn("dbo.Matches", "AwayTeamId", c => c.Guid());
            CreateIndex("dbo.Matches", "HomeTeamId");
            CreateIndex("dbo.Matches", "AwayTeamId");
            AddForeignKey("dbo.Matches", "HomeTeamId", "dbo.Teams", "Id");
            AddForeignKey("dbo.Matches", "AwayTeamId", "dbo.Teams", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Matches", "Team_Id1", "dbo.Teams");
            DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
            DropIndex("dbo.Matches", new[] { "Team_Id1" });
            DropIndex("dbo.Matches", new[] { "Team_Id" });
            DropColumn("dbo.Matches", "Team_Id1");
            CreateIndex("dbo.Matches", "Team_Id");
            AddForeignKey("dbo.Matches", "Team_Id", "dbo.Teams", "Id");
        }
    }
}

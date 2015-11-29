namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "AwayTeam_Id", "dbo.Teams");
            DropForeignKey("dbo.Matches", "HomeTeam_Id", "dbo.Teams");
            DropIndex("dbo.Matches", new[] { "AwayTeam_Id" });
            DropIndex("dbo.Matches", new[] { "HomeTeam_Id" });
            AddColumn("dbo.Matches", "HomeTeamAPIId", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "AwayTeamAPIId", c => c.Int(nullable: false));
            DropColumn("dbo.Matches", "AwayTeam_Id");
            DropColumn("dbo.Matches", "HomeTeam_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "HomeTeam_Id", c => c.Guid());
            AddColumn("dbo.Matches", "AwayTeam_Id", c => c.Guid());
            DropColumn("dbo.Matches", "AwayTeamAPIId");
            DropColumn("dbo.Matches", "HomeTeamAPIId");
            CreateIndex("dbo.Matches", "HomeTeam_Id");
            CreateIndex("dbo.Matches", "AwayTeam_Id");
            AddForeignKey("dbo.Matches", "HomeTeam_Id", "dbo.Teams", "Id");
            AddForeignKey("dbo.Matches", "AwayTeam_Id", "dbo.Teams", "Id");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch31 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Matches", "Team_Id", "dbo.Teams");
            //FK_dbo.Matches_dbo.Teams_Team_Id

            //AddColumn("dbo.Matches", "AwayTeam_Id", c => c.Guid());
            //AddColumn("dbo.Matches", "HomeTeam_Id", c => c.Guid());
            //CreateIndex("dbo.Matches", "AwayTeam_Id");
            //CreateIndex("dbo.Matches", "HomeTeam_Id");
            //AddForeignKey("dbo.Matches", "AwayTeam_Id", "dbo.Teams", "Id");
            //AddForeignKey("dbo.Matches", "HomeTeam_Id", "dbo.Teams", "Id");
            //DropColumn("dbo.Matches", "HomeTeamId");
            //DropColumn("dbo.Matches", "AwayTeamId");
            //DropColumn("dbo.Matches", "HomeTeamAPIId");
            //DropColumn("dbo.Matches", "AwayTeamAPIId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Matches", "AwayTeamAPIId", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "HomeTeamAPIId", c => c.Int(nullable: false));
            AddColumn("dbo.Matches", "AwayTeamId", c => c.Guid(nullable: false));
            AddColumn("dbo.Matches", "HomeTeamId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Matches", "HomeTeam_Id", "dbo.Teams");
            DropForeignKey("dbo.Matches", "AwayTeam_Id", "dbo.Teams");
            DropIndex("dbo.Matches", new[] { "HomeTeam_Id" });
            DropIndex("dbo.Matches", new[] { "AwayTeam_Id" });
            DropColumn("dbo.Matches", "HomeTeam_Id");
            DropColumn("dbo.Matches", "AwayTeam_Id");
        }
    }
}

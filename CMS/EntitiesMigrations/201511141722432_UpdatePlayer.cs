namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlayer : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PlayerTeams", newName: "TeamPlayers");
            AddColumn("dbo.Players", "Lineup_Id", c => c.Guid());
            CreateIndex("dbo.Players", "Lineup_Id");
            AddForeignKey("dbo.Players", "Lineup_Id", "dbo.Lineups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "Lineup_Id", "dbo.Lineups");
            DropIndex("dbo.Players", new[] { "Lineup_Id" });
            DropColumn("dbo.Players", "Lineup_Id");
            RenameTable(name: "dbo.TeamPlayers", newName: "PlayerTeams");
        }
    }
}

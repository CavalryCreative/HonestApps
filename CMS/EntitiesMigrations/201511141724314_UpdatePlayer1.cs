namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePlayer1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TeamPlayers", newName: "PlayerTeams");
            DropForeignKey("dbo.Players", "Lineup_Id", "dbo.Lineups");
            DropIndex("dbo.Players", new[] { "Lineup_Id" });
            DropColumn("dbo.Players", "Lineup_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "Lineup_Id", c => c.Guid());
            CreateIndex("dbo.Players", "Lineup_Id");
            AddForeignKey("dbo.Players", "Lineup_Id", "dbo.Lineups", "Id");
            RenameTable(name: "dbo.PlayerTeams", newName: "TeamPlayers");
        }
    }
}

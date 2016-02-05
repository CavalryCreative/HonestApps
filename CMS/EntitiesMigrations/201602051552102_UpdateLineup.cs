namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLineup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lineups", "Player_Id", "dbo.Players");
            DropIndex("dbo.Lineups", new[] { "Player_Id" });
            AddColumn("dbo.Lineups", "PlayerId", c => c.Guid(nullable: false));
            DropColumn("dbo.Lineups", "Player_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lineups", "Player_Id", c => c.Guid());
            DropColumn("dbo.Lineups", "PlayerId");
            CreateIndex("dbo.Lineups", "Player_Id");
            AddForeignKey("dbo.Lineups", "Player_Id", "dbo.Players", "Id");
        }
    }
}

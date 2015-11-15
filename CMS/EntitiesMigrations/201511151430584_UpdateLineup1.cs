namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLineup1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Lineups", "Match_Id", "dbo.Matches");
            DropIndex("dbo.Lineups", new[] { "Match_Id" });
            RenameColumn(table: "dbo.Lineups", name: "Match_Id", newName: "MatchId");
            AlterColumn("dbo.Lineups", "MatchId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Lineups", "MatchId");
            AddForeignKey("dbo.Lineups", "MatchId", "dbo.Matches", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lineups", "MatchId", "dbo.Matches");
            DropIndex("dbo.Lineups", new[] { "MatchId" });
            AlterColumn("dbo.Lineups", "MatchId", c => c.Guid());
            RenameColumn(table: "dbo.Lineups", name: "MatchId", newName: "Match_Id");
            CreateIndex("dbo.Lineups", "Match_Id");
            AddForeignKey("dbo.Lineups", "Match_Id", "dbo.Matches", "Id");
        }
    }
}

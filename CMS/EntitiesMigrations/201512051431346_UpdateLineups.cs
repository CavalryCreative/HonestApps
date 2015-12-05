namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateLineups : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Players", "Eleven_Id", "dbo.Lineups");
            DropForeignKey("dbo.Players", "Eleven_Id1", "dbo.Lineups");
            DropForeignKey("dbo.Players", "Sub_Id", "dbo.Lineups");
            DropForeignKey("dbo.Players", "Sub_Id1", "dbo.Lineups");
            DropForeignKey("dbo.Lineups", "MatchId", "dbo.Matches");
            DropIndex("dbo.Players", new[] { "Eleven_Id" });
            DropIndex("dbo.Players", new[] { "Eleven_Id1" });
            DropIndex("dbo.Players", new[] { "Sub_Id" });
            DropIndex("dbo.Players", new[] { "Sub_Id1" });
            DropIndex("dbo.Lineups", new[] { "MatchId" });
            RenameColumn(table: "dbo.Lineups", name: "MatchId", newName: "Match_Id");
            AddColumn("dbo.Lineups", "MatchAPIId", c => c.Int(nullable: false));
            AddColumn("dbo.Lineups", "IsHomePlayer", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lineups", "IsSub", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lineups", "Player_Id", c => c.Guid());
            AlterColumn("dbo.Lineups", "Id", c => c.Guid(nullable: false, identity: true));
            AlterColumn("dbo.Lineups", "Match_Id", c => c.Guid());
            CreateIndex("dbo.Lineups", "Player_Id");
            CreateIndex("dbo.Lineups", "Match_Id");
            AddForeignKey("dbo.Lineups", "Player_Id", "dbo.Players", "Id");
            AddForeignKey("dbo.Lineups", "Match_Id", "dbo.Matches", "Id");
            DropColumn("dbo.Lineups", "Discriminator");
            DropColumn("dbo.Players", "Eleven_Id");
            DropColumn("dbo.Players", "Eleven_Id1");
            DropColumn("dbo.Players", "Sub_Id");
            DropColumn("dbo.Players", "Sub_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "Sub_Id1", c => c.Guid());
            AddColumn("dbo.Players", "Sub_Id", c => c.Guid());
            AddColumn("dbo.Players", "Eleven_Id1", c => c.Guid());
            AddColumn("dbo.Players", "Eleven_Id", c => c.Guid());
            AddColumn("dbo.Lineups", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.Lineups", "Match_Id", "dbo.Matches");
            DropForeignKey("dbo.Lineups", "Player_Id", "dbo.Players");
            DropIndex("dbo.Lineups", new[] { "Match_Id" });
            DropIndex("dbo.Lineups", new[] { "Player_Id" });
            AlterColumn("dbo.Lineups", "Match_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Lineups", "Id", c => c.Guid(nullable: false));
            DropColumn("dbo.Lineups", "Player_Id");
            DropColumn("dbo.Lineups", "IsSub");
            DropColumn("dbo.Lineups", "IsHomePlayer");
            DropColumn("dbo.Lineups", "MatchAPIId");
            RenameColumn(table: "dbo.Lineups", name: "Match_Id", newName: "MatchId");
            CreateIndex("dbo.Lineups", "MatchId");
            CreateIndex("dbo.Players", "Sub_Id1");
            CreateIndex("dbo.Players", "Sub_Id");
            CreateIndex("dbo.Players", "Eleven_Id1");
            CreateIndex("dbo.Players", "Eleven_Id");
            AddForeignKey("dbo.Lineups", "MatchId", "dbo.Matches", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Players", "Sub_Id1", "dbo.Lineups", "Id");
            AddForeignKey("dbo.Players", "Sub_Id", "dbo.Lineups", "Id");
            AddForeignKey("dbo.Players", "Eleven_Id1", "dbo.Lineups", "Id");
            AddForeignKey("dbo.Players", "Eleven_Id", "dbo.Lineups", "Id");
        }
    }
}

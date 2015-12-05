namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateHistory2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UpdateHistories",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MatchAPIId = c.Int(nullable: false),
                        MatchDetails = c.Boolean(nullable: false),
                        Lineups = c.Boolean(nullable: false),
                        CreatedByUserId = c.Guid(),
                        DateAdded = c.DateTime(),
                        Active = c.Boolean(),
                        Deleted = c.Boolean(),
                        UpdatedByUserId = c.Guid(),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UpdateHistories");
        }
    }
}

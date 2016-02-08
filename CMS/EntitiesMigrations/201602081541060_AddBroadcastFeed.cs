namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBroadcastFeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BroadcastFeeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(maxLength: 500, unicode: false),
                        IPAddress = c.String(maxLength: 100, unicode: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BroadcastFeeds");
        }
    }
}

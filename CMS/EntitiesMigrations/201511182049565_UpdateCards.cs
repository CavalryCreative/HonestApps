namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "CreatedByUserId", c => c.Guid());
            AddColumn("dbo.Cards", "DateAdded", c => c.DateTime());
            AddColumn("dbo.Cards", "Active", c => c.Boolean());
            AddColumn("dbo.Cards", "Deleted", c => c.Boolean());
            AddColumn("dbo.Cards", "UpdatedByUserId", c => c.Guid());
            AddColumn("dbo.Cards", "DateUpdated", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "DateUpdated");
            DropColumn("dbo.Cards", "UpdatedByUserId");
            DropColumn("dbo.Cards", "Deleted");
            DropColumn("dbo.Cards", "Active");
            DropColumn("dbo.Cards", "DateAdded");
            DropColumn("dbo.Cards", "CreatedByUserId");
        }
    }
}

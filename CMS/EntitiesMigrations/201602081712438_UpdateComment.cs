namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateComment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "TeamComment", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "PlayerComment", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "PlayerComment");
            DropColumn("dbo.Comments", "TeamComment");
        }
    }
}

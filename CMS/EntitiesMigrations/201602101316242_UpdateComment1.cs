namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateComment1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "CommentType", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "EventType", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "IsUsersTeam", c => c.Boolean(nullable: false));
            DropColumn("dbo.Comments", "TeamComment");
            DropColumn("dbo.Comments", "PlayerComment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "PlayerComment", c => c.Boolean(nullable: false));
            AddColumn("dbo.Comments", "TeamComment", c => c.Boolean(nullable: false));
            DropColumn("dbo.Comments", "IsUsersTeam");
            DropColumn("dbo.Comments", "EventType");
            DropColumn("dbo.Comments", "CommentType");
        }
    }
}

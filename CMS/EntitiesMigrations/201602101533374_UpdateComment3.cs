namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateComment3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Comments", "CommentType", c => c.Byte(nullable: false));
            AlterColumn("dbo.Comments", "EventType", c => c.Byte(nullable: false));
            AlterColumn("dbo.Comments", "Perspective", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Comments", "Perspective", c => c.Int(nullable: false));
            AlterColumn("dbo.Comments", "EventType", c => c.Int(nullable: false));
            AlterColumn("dbo.Comments", "CommentType", c => c.Int(nullable: false));
        }
    }
}

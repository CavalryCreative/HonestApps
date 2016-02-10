namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateCommentAddPosition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Position", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Position");
        }
    }
}

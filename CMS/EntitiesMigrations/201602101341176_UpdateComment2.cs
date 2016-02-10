namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateComment2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Perspective", c => c.Int(nullable: false));
            DropColumn("dbo.Comments", "IsUsersTeam");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "IsUsersTeam", c => c.Boolean(nullable: false));
            DropColumn("dbo.Comments", "Perspective");
        }
    }
}

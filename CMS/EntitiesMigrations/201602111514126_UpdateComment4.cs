namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateComment4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "TeamRating", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "TeamRating");
        }
    }
}

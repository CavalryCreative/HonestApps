namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "PrimaryColour", c => c.String(maxLength: 50, unicode: false));
            AddColumn("dbo.Teams", "SecondaryColour", c => c.String(maxLength: 50, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Teams", "SecondaryColour");
            DropColumn("dbo.Teams", "PrimaryColour");
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LineupAddPosition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lineups", "Position", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lineups", "Position");
        }
    }
}

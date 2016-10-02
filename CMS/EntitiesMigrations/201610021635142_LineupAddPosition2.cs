namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LineupAddPosition2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Lineups", "Position", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Lineups", "Position", c => c.String());
        }
    }
}

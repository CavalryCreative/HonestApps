namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEvent : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Events", "Score", c => c.String(maxLength: 250, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Events", "Score", c => c.String());
        }
    }
}

namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Matches", "HalfTimeScore", c => c.String(maxLength: 10, unicode: false));
            AddColumn("dbo.Matches", "FullTimeScore", c => c.String(maxLength: 10, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Matches", "FullTimeScore");
            DropColumn("dbo.Matches", "HalfTimeScore");
        }
    }
}

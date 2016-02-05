namespace CMS.EntitiesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteException : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteExceptions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HResult = c.String(maxLength: 50, unicode: false),
                        InnerException = c.String(maxLength: 500, unicode: false),
                        Message = c.String(maxLength: 500, unicode: false),
                        Source = c.String(maxLength: 250, unicode: false),
                        StackTrace = c.String(unicode: false),
                        TargetSite = c.String(maxLength: 250, unicode: false),
                        DateAdded = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SiteExceptions");
        }
    }
}

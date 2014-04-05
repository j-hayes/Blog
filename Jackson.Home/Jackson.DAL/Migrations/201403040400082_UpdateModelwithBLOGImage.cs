namespace Jackson.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModelwithBLOGImage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostImages", "Image", c => c.Binary());
            DropColumn("dbo.PostImages", "ImageLink");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostImages", "ImageLink", c => c.String());
            DropColumn("dbo.PostImages", "Image");
        }
    }
}

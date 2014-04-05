namespace Jackson.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class postwithimages2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PostImages", "PostId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostImages", "PostId", c => c.Int(nullable: false));
        }
    }
}

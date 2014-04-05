namespace Jackson.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Post = c.String(),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostId = c.Int(nullable: false),
                        ImageLink = c.String(),
                        BlogPost_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BlogPosts", t => t.BlogPost_Id)
                .Index(t => t.BlogPost_Id);
            
            CreateTable(
                "dbo.PostTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Tag = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PostHasTag",
                c => new
                    {
                        BlogPostId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BlogPostId, t.TagId })
                .ForeignKey("dbo.BlogPosts", t => t.BlogPostId, cascadeDelete: true)
                .ForeignKey("dbo.PostTags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.BlogPostId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostHasTag", "TagId", "dbo.PostTags");
            DropForeignKey("dbo.PostHasTag", "BlogPostId", "dbo.BlogPosts");
            DropForeignKey("dbo.PostImages", "BlogPost_Id", "dbo.BlogPosts");
            DropIndex("dbo.PostHasTag", new[] { "TagId" });
            DropIndex("dbo.PostHasTag", new[] { "BlogPostId" });
            DropIndex("dbo.PostImages", new[] { "BlogPost_Id" });
            DropTable("dbo.PostHasTag");
            DropTable("dbo.PostTags");
            DropTable("dbo.PostImages");
            DropTable("dbo.BlogPosts");
        }
    }
}

using System.Data.Entity;
namespace Jackson.DAL
{
   
    public class SiteDataContext : DbContext

    {
        
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<PostTag> Tags { get; set; }
        public DbSet<PostImage> Images { get; set; }


        public SiteDataContext() 
        { 
            this.Configuration.LazyLoadingEnabled = true; 
        } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>().HasMany(b => b.Images).WithMany(t => t.PostsWithImage).Map(
                m =>
                {
                    m.MapLeftKey("BlogPostId");
                    m.MapRightKey("ImageId");
                    m.ToTable("PostHasImage");
                });

            modelBuilder.Entity<BlogPost>().
                HasMany(b => b.Tags).
                WithMany(t => t.PostsWithTag).
                Map(
                    m =>
                    {
                        m.MapLeftKey("BlogPostId");
                        m.MapRightKey("TagId");
                        m.ToTable("PostHasTag");

                    });

            

        }
    }
}

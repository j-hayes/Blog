using System.Data.Entity;


namespace Jackson.DAL
{
   
    public class SiteDataContext : DbContext

    {
        public SiteDataContext()
           // : base("name=LocalDbConnectionString")
    {
    }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<PostTag> Tags { get; set; }
        public DbSet<PostImage> Images { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>().
              HasMany(b =>b.Tags).
              WithMany(t =>t.PostsWithTag).
              Map(
               m =>
               {
                   m.MapLeftKey("BlogPostId");
                   m.MapRightKey("TagId");
                   m.ToTable("PostHasTag");
               });
            modelBuilder.Entity<BlogPost>()
                .HasMany(b => b.Images)
                .WithOptional(t => t.BlogPost)
                .WillCascadeOnDelete(false);

        }
    }
}

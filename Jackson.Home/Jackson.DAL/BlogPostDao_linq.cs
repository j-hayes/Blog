using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDatabase;
using NDatabase.Api;
using Sqo;

namespace Jackson.DAL
{
    public class BlogPostDao_linq : IBlogPostDao
    {
        private SiteDataContext _db;

        public BlogPostDao_linq()
        {
            
            _db = new SiteDataContext();
            
        }

        public List<BlogPost> Get()
        {
            return _db.BlogPosts.ToList();
        }

        public BlogPost Get(int id)
        {
            return _db.BlogPosts.First(x => x.Id == id);
            
        }

        public List<BlogPost> GetByDate(DateTime dateTime)
        {
            return _db.BlogPosts.Where(x => x.DateTime == dateTime.Date).ToList();
        }

        public BlogPost GetMostRecent()
        {
            return _db.BlogPosts.OrderByDescending(x => x.DateTime).First();
        }

        public BlogPost Create(BlogPost blogPost)
        {
            _db.BlogPosts.Add(blogPost);
            _db.SaveChanges();
            return blogPost;
        }

        public BlogPost Update(BlogPost blogPost)
        {
            var post =_db.BlogPosts.First(x => x.Id == blogPost.Id);
           
            post.DateTime = blogPost.DateTime;
            post.Images = blogPost.Images;
            post.Post = blogPost.Post;
            post.Tags = blogPost.Tags;
            post.Title = blogPost.Title;
            
            _db.SaveChanges();



            return blogPost;
        }

        public void Delete(BlogPost blogPost)
        {
            _db.BlogPosts.Remove(blogPost);
            _db.SaveChanges();
        }

        public List<PostTag> GetAllTags()
        {
           return _db.Tags.ToList();
        }

        public void DeleteImage(int id)
        {
            _db.Images.Remove(new PostImage()
            {
                Id = id
            });
            
            _db.SaveChanges();
        }
    }
}

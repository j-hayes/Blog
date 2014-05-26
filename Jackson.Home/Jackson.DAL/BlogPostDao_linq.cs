using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.Remoting.Contexts;

namespace Jackson.DAL
{
    public class BlogPostDao_linq : IBlogPostDao
    {
        private SiteDataContext _db;

        public BlogPostDao_linq()
        {
            _db = new SiteDataContext();
        }

        public List<BlogPost> GetAllPosts()
        {
            var bp = _db.BlogPosts.ToList();
            return _db.BlogPosts.ToList();
        }

        public BlogPost Get(int id)
        {

            return _db.BlogPosts.Include(x => x.Images).First(x => x.Id == id && x.IsJornal == false);

        }

        public List<BlogPost> GetByDate(DateTime dateTime)
        {
            return _db.BlogPosts.Where(x => x.DateTime == dateTime.Date && x.IsJornal == false).ToList();
        }

        public BlogPost GetMostRecent()
        {
            return _db.BlogPosts.OrderByDescending(x => x.DateTime).FirstOrDefault(x => x.IsJornal == false);
        }

        public BlogPost Create(BlogPost blogPost)
        {
            blogPost = _db.BlogPosts.Add(blogPost);

            var tags = new List<PostTag>();
            if (blogPost.Tags != null)
            {
                tags.AddRange(blogPost.Tags.Select(tag => (GetTagById(tag.Id))));
            }
            blogPost.Tags = tags;

            _db.SaveChanges();

            return Update(blogPost);
        }

        public BlogPost Update(BlogPost blogPost)
        {
            var post = _db.BlogPosts.First(x => x.Id == blogPost.Id);

            post.DateTime = blogPost.DateTime;
            post.Title = blogPost.Title;
            post.Post = blogPost.Post;


            List<PostTag> tags = new List<PostTag>();
            foreach (var tag in blogPost.Tags)
            {
                if (!post.Tags.Contains(tag))
                {
                    post.Tags.Add(GetTagById(tag.Id));
                }
            }


            _db.SaveChanges();



            return blogPost;
        }

        private PostTag GetTagById(int id)
        {
            return _db.Tags.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(BlogPost blogPost)
        {
            _db.BlogPosts.Remove(blogPost);
            _db.SaveChanges();
        }

        public List<PostTag> GetAllTags()
        {
            var list = _db.Tags.Where(x => x.Tag != null).ToList();

            return list;
        }

        public void DeleteImage(int id)
        {
            _db.Images.Remove(new PostImage()
            {
                Id = id
            });

            _db.SaveChanges();
        }

        public List<PostImage> GetImagesForDate(DateTime date)

        {
            return _db.Images.Where(x => DbFunctions.TruncateTime(x.DateTime) == date.Date).ToList();
        }

        public byte[] GetDefaultImage()
        {
            var firstOrDefault = _db.Images.FirstOrDefault();
            if (firstOrDefault != null) return firstOrDefault.Image;

            else
            {
                throw new Exception("There are no images in the database");
            }
        }

        public PostImage AddImage(PostImage image)
        {
            image = _db.Images.Add(image);
            _db.SaveChanges();
            return image;
        }

        public PostImage GetImage(int id)
        {
            var image = _db.Images.FirstOrDefault(x => x.Id == id);
            return image;
        }

        public List<PostImage> GetImagesForMonth(DateTime date)
        {
            return _db.Images.Where(x => x.DateTime.Month == date.Month).ToList();
        }

        public BlogPost GetNextPublicPost(int id)
        {

            var oldPost = _db.BlogPosts.First(x => x.Id == id);
            var newPosts =
                _db.BlogPosts.Where(x => x.DateTime > oldPost.DateTime && x.IsJornal == false)
                    .OrderBy(x => x.DateTime);

            if (newPosts.Any())
            {
                return newPosts.First();

            }
            else
            {
                throw new DaoException("There is no Next Post");
            }
        }

        public BlogPost GetPreviousPublicPost(int id)
        {

            var oldPost = _db.BlogPosts.First(x => x.Id == id);
            var newPosts =
                _db.BlogPosts.Where(x => x.DateTime < oldPost.DateTime && x.IsJornal == false)
                    .OrderByDescending(x => x.DateTime);

            if (newPosts.Any())
            {
                var newPost = newPosts.First();
                return newPost;
            }
            else
            {
                throw new DaoException("No Previous Post Exists");
            }

        }

    }
}
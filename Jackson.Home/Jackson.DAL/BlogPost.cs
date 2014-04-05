using System;
using System.Collections.Generic;
using NDatabase;
using NDatabase.Api;
 using NDatabase.Oid;

namespace Jackson.DAL
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Post { get; set; }
        public virtual List<PostImage> Images { get; set; }
        public List<PostTag> Tags { get; set; }
        public DateTime DateTime { get; set; }



        public BlogPost()
        {
            Tags = new List<PostTag>();
            Images = new List<PostImage>();//this may mess stuff up in loading with the dbcontext
        }
    }
}

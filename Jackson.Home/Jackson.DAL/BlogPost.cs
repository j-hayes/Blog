using System;
using System.Collections.Generic;

namespace Jackson.DAL
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Post { get; set; }
        public bool IsJornal { get; set; }
        public virtual ICollection<PostTag> Tags { get; set; }
        public virtual ICollection<PostImage> Images { get; set; }
        public DateTime DateTime { get; set; }



        public BlogPost()
        {
          
        }
    }
}

using System.Collections.Generic;

namespace Jackson.DAL
{
    public class PostTag
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public List<BlogPost> PostsWithTag { get; set; } 
     
    }
}
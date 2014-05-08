using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Jackson.DAL
{
    public class PostTag
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public virtual ICollection<BlogPost> PostsWithTag { get; set; }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
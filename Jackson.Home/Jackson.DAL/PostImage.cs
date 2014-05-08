using System;
using System.Collections.Generic;
using System.IO;


namespace Jackson.DAL
{
    public class PostImage
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public byte[] Image { get; set; }
        public string Caption { get; set; }
        public virtual ICollection<BlogPost> PostsWithImage { get; set; }


        public PostImage()
        {

        }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
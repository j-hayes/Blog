﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Jackson.DAL
{
    public interface IBlogPostDao
    {
        /*
         Gets all Posts
         */
        List<BlogPost> Get();
        BlogPost Get(int id);
        /*
         Get all posts for a Date
         */
        List<BlogPost> GetByDate(DateTime dateTime);
        BlogPost GetMostRecent();


        BlogPost Create(BlogPost blogPost);

        BlogPost Update(BlogPost blogPost);
        void Delete(BlogPost blogPost);


        List<PostTag> GetAllTags();
        void DeleteImage(int id);
    }
}

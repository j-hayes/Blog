using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Jackson.DAL;

namespace Jackson.Home.Helpers
{
    public static class PostImageBuilder
    {
        public static PostImage CreateFromPostedFile(HttpPostedFileBase newImage)
        {
            var binaryReader = new BinaryReader(newImage.InputStream);
            var fileData = binaryReader.ReadBytes(newImage.ContentLength);


            PostImage image = new PostImage { Image = fileData, DateTime = DateTime.Now };

            return image;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Jackson.DAL;
using Jackson.Home.Models;
using BlogPost = Jackson.DAL.BlogPost;

namespace Jackson.Home.Controllers
{
    public class WaiGuoAtHomeController : Controller
    {
        private IBlogPostDao _blogPostDao;

        public WaiGuoAtHomeController()
        {
            _blogPostDao = new BlogPostDao_linq();
        }

        //
        // GET: /WaiGuoAtHome/

        public ActionResult Admin()
        {

            _blogPostDao = new BlogPostDao_linq();

            List<BlogPost> blogPosts = _blogPostDao.Get().ToList();


            return View(blogPosts);
        }

        public ActionResult Post(int id)
        {
            return View(_blogPostDao.Get(id));
        }


        public ActionResult Create()
        {


            EditPostViewModel viewModel = new EditPostViewModel
            {
                PostTags = _blogPostDao.GetAllTags(),
                Post = new BlogPost {DateTime = DateTime.Now}
            };

            viewModel.TagList = new List<SelectListItem>();

            foreach (var postTag in viewModel.PostTags)
            {
                viewModel.TagList.Add(new SelectListItem()
                {
                    Value = postTag.Id.ToString(),
                    Text = postTag.Tag
                });
            }

            //viewModel.SelectedListTags = new List<SelectListItem>();


            return View(viewModel);
        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EditPostViewModel viewModel, FormCollection collection, HttpPostedFileBase NewImage)
        {
            string s = collection["PostTags"];

            string[] tagStrings = s.Split(',');

            viewModel.Post.Tags = new List<PostTag>();
            foreach (string tagString in tagStrings)
            {
                viewModel.Post.Tags.Add(new PostTag()
                {
                    Id = Int32.Parse(tagString)
                });

            }

            var binaryReader = new BinaryReader(NewImage.InputStream);

            var fileData = binaryReader.ReadBytes(NewImage.ContentLength);


            PostImage image = new PostImage {Image = fileData};
            viewModel.Post.Images.Add(image);
            _blogPostDao.Create(viewModel.Post);
            return RedirectToAction("Admin");

        }

        public ActionResult Edit(int id)
        {
            return View(_blogPostDao.Get(id));
        }

        [HttpPost]
        public ActionResult Edit(BlogPost blogPost, FormCollection collection, HttpPostedFileBase NewImage)
        {

            //   blogPost.Images.Add(NewImage);
            _blogPostDao.Update(blogPost);



            return RedirectToAction("Admin");

        }


        public ActionResult Delete(int id)
        {
            _blogPostDao.Delete(_blogPostDao.Get(id));
            return RedirectToAction("Admin");
        }

      

        public ActionResult DeleteImage(int id, int returnEditId)
        {
            _blogPostDao.DeleteImage(id);
            return View("ImagesForPost", _blogPostDao.Get(returnEditId).Images);
        }
    }

}



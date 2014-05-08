using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Jackson.DAL;
using Jackson.Home.Helpers;
using Jackson.Home.Models;
using Microsoft.Ajax.Utilities;
using BlogPost = Jackson.DAL.BlogPost;

namespace Jackson.Home.Controllers
{
    public class WaiGuoAtHomeController : Controller
    {
        private IBlogPostDao _blogPostDao;

        public WaiGuoAtHomeController()
        {
            _blogPostDao = new BlogPostDao_linq();//
        }

        //
        // GET: /WaiGuoAtHome/

        public ActionResult Admin()
        {

            _blogPostDao = new BlogPostDao_linq();

            List<BlogPost> blogPosts = _blogPostDao.GetAllPosts().ToList();


            return View(blogPosts);
        }

        public ActionResult Post(int id)
        {
            return View(_blogPostDao.Get(id));
        }


        public ActionResult Create()
        {
            var viewModel = GetNewEditPostViewModel();


            //viewModel.SelectedListTags = new List<SelectListItem>();
            return View(viewModel);
        }

        


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(EditPostViewModel viewModel, FormCollection collection, HttpPostedFileBase NewImage)
        {
            if (viewModel.Post.Id != 0)
            {
                return Edit(viewModel, collection, NewImage);
            }
            else
            {

                string s = collection["SelectedTagIds"];
            if (!string.IsNullOrEmpty(s))
            {
                string[] tagStrings = s.Split(',');

                viewModel.Post.Tags = new List<PostTag>();
                foreach (string tagString in tagStrings)
                {
                    viewModel.Post.Tags.Add(new PostTag()
                    {
                        Id = Int32.Parse(tagString)
                    });

                }
            }
            if (NewImage != null)
            {
                viewModel.Post.Images = new List<PostImage>();
                var image = PostImageBuilder.CreateFromPostedFile(NewImage);
                image.Caption = "";
                viewModel.Post.Images.Add(image);
                
            }

           

            _blogPostDao.Create(viewModel.Post);
            return RedirectToAction("Admin");
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(EditPostViewModel viewModel, FormCollection collection, HttpPostedFileBase NewImage)
        {
            string s = collection["SelectedTagIds"];
            if (!string.IsNullOrEmpty(s))
            {
                string[] tagStrings = s.Split(',');
            
            viewModel.Post.Tags = new List<PostTag>();
            foreach (string tagString in tagStrings)
            {
                viewModel.Post.Tags.Add(new PostTag()
                {
                    Id = Int32.Parse(tagString)
                });

            }
            }
            if (NewImage != null) 
            { 
                UploadPicture(NewImage);  
            }

            //todo: add an imamge to the DB without going through a post
            
            _blogPostDao.Update(viewModel.Post);
            return RedirectToAction("Admin");

        }

       

        public ActionResult Edit(int id)
        {

             EditPostViewModel viewModel = new EditPostViewModel
            {
                Post = _blogPostDao.Get(id),
                EligiableTagList = new List<SelectListItem>()
             
            };

          
            viewModel.SelectedTagIds = viewModel.Post.Tags.Where(x => x.Id > 0).Select(z => z.Id).ToArray();
        
            
            foreach (var postTag in _blogPostDao.GetAllTags())
            {
                viewModel.EligiableTagList.Add(new SelectListItem()
                {
                    Value = postTag.Id.ToString(),
                    Text = postTag.Tag, 
                   
                     });
            }
     

     

            return View(viewModel);
        }
   

        public ActionResult Delete(int id)
        {
            _blogPostDao.Delete(_blogPostDao.Get(id));
            return RedirectToAction("Admin");
        }

        public ActionResult AddPhoto()
        {
            var addPhotoViewModel = new AddPhotoViewModel();
            return View(addPhotoViewModel);
        }
        
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPhoto(AddPhotoViewModel viewModel, HttpPostedFileBase NewImage)

        {
            var image = UploadPicture(NewImage);

            viewModel.Image = image;
            viewModel.Message = "Image Below successfully uploaded";
            return View(viewModel);
        }


        private EditPostViewModel GetNewEditPostViewModel()
        {
            EditPostViewModel viewModel = EditViewModelFactory.GetNewEditPostViewModel();

            return viewModel;
        }

        //probably not necessary any more
        public ActionResult DeleteImage(int id, int returnEditId)
        {
            throw new NotImplementedException();
            
            
        }


        private PostImage UploadPicture(HttpPostedFileBase NewImage)
        {
            var binaryReader = new BinaryReader(NewImage.InputStream);
            var fileData = binaryReader.ReadBytes(NewImage.ContentLength);


            PostImage image = new PostImage { Image = fileData, DateTime = DateTime.Now };

            return _blogPostDao.AddImage(image);
        }
    }

}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using ImageResizer;
using ImageResizer.Caching;
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
            _blogPostDao = new BlogPostDao_linq(); //
        }

        public ActionResult Index()
        {
            var viewmodel = _blogPostDao.GetMostRecent();
            ViewBag.Title = "WaiGuoAtHome";
            return View("Post", viewmodel);
        }

     

        [HttpGet]
        public ActionResult Post(int id)
        {
            var viewModel = _blogPostDao.Get(id);
            viewModel = GetAndResizeImages(viewModel);

            return View(viewModel);
        }
    

        public ActionResult Previous(FormCollection collection)
        {
            int postId = Int32.Parse(collection["postId"]);
       
            try
            {
                
                return Redirect("~/WaiGuoAtHome/Post/" + _blogPostDao.GetPreviousPublicPost(postId).Id);
            }
            catch (DaoException e)
            {
                return Redirect("~/WaiGuoAtHome/Post/" + postId+"?isEariest=true");
            }

            
        }

        public ActionResult Next(FormCollection collection)
        {
            int postId = Int32.Parse(collection["postId"]);

            try
            {
             int id =   _blogPostDao.GetNextPublicPost(postId).Id;
                return Redirect("~/WaiGuoAtHome/Post/" + id);
            }
            catch (DaoException e)
            {
                return Redirect("~/WaiGuoAtHome/Post/" + postId + "?isNewest=true");
            }
        }

        private BlogPost GetAndResizeImages(BlogPost viewmodel)
        {
            if (viewmodel.Images.Count < 1)
            {
                viewmodel.Images = _blogPostDao.GetImagesForDate(viewmodel.DateTime);
            }
            foreach (var postImage in viewmodel.Images)
            {
                var outStream = ReizeImage(postImage);
                
            }
            return viewmodel;
        }

        private static MemoryStream ReizeImage(PostImage postImage)
        {
            return ResizeImage(postImage, 1000, 1000);
        }



        private static MemoryStream ResizeImage(PostImage postImage, int height , int width)
        {
            MemoryStream outStream;
            using (outStream = new MemoryStream())
            {
                using (var inStream = new MemoryStream(postImage.Image))
                {
                    var settings = new ResizeSettings("maxwidth="+width+"&maxheight="+height);
                    ImageResizer.ImageBuilder.Current.Build(inStream, outStream, settings);
                    postImage.Image = outStream.ToArray();
                    //   return new FileContentResult(outBytes, "image/jpeg");
                }
            }
            return outStream;
        }

        public ActionResult Image(int id)
        {
            PostImage viewModel = _blogPostDao.GetImage(id);
            return View(viewModel);
        }

        public ActionResult Images(int? id, int? month, int? day) // id== year in this case weird overloading routes thing
        {
          
           ImagesViewModel viewModel = GetImageForDate(id, month, day);
          
            foreach (PostImage postImage in viewModel.Images)
            {
                postImage.Image = ResizeImage(postImage, 500, 250).GetBuffer();
            }
            return View(viewModel);
        }


        public ActionResult NextMonthsImages(DateTime date)
        {
            date = date.AddMonths(1);
            return Redirect("~/WaiGuoAtHome/Images/" + date.Year +"/"+date.Month);
        }
        public ActionResult PreviousMonthsImages(DateTime date)
        {
            date = date.AddMonths(-1);
            return Redirect("~/WaiGuoAtHome/Images/" + date.Year + "/" + date.Month);
        }
        
        public ActionResult Admin()
        {

            _blogPostDao = new BlogPostDao_linq();

            List<BlogPost> blogPosts = _blogPostDao.GetAllPosts().ToList();


            return View(blogPosts);
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

            if (viewModel.Post.IsJornal) {
                viewModel.Post.Id = int.Parse(DateTime.Now.ToString("ddmmyyy"));
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
                UploadPicture(NewImage, viewModel.ImageCaption);  
            }

            //todo: add an imamge to the DB without going through a post
            
            _blogPostDao.Update(viewModel.Post);
            return RedirectToAction("Admin");

        }

        public ActionResult Edit(int id)
        {

             EditPostViewModel viewModel = new EditPostViewModel
            {
                Post = _blogPostDao.GetAny(id),
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
            _blogPostDao.Delete(_blogPostDao.GetAny(id));
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
            var image = UploadPicture(NewImage, viewModel.Caption);

        //    viewModel.Image = image; when the internet is good uncomment
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
            _blogPostDao.DeleteImage(id);

            return RedirectToAction("ManageImages");
        }

        public ActionResult ManageImages(int? id, int? month, int? day)
        {//todo: build ProxyImage class
            ImagesViewModel viewModel = GetImageForDate(id, month, day);
       
            
            return View( viewModel);
        }

        private PostImage UploadPicture(HttpPostedFileBase NewImage, string caption)
        {
            var binaryReader = new BinaryReader(NewImage.InputStream);
            var fileData = binaryReader.ReadBytes(NewImage.ContentLength);

            PostImage image = new PostImage { Image = fileData, DateTime = DateTime.Now, Caption = caption};
            return _blogPostDao.AddImage(image);
        }

        public ActionResult EditImage(int id)
        {
            throw new NotImplementedException();
        }

        private ImagesViewModel GetImageForDate(int? year, int? month, int? day)
        {
            var viewModel = new ImagesViewModel();
            if (!year.HasValue)//default
            {
                year = DateTime.Now.Year;
                month = DateTime.Now.Month;
                viewModel.Images = _blogPostDao.GetImagesForMonth(new DateTime(year.Value, month.Value, 1));
                viewModel.DateForImages = DateTime.Now;
                viewModel.DisplayDateString = string.Format("{0}/{1}", year.Value, month.Value);
            }
            else
            {
                if (!month.HasValue)//year/
                {
                    month = 1;
                    viewModel.DateForImages = new DateTime(year.Value, month.Value, 1);
                    viewModel.Images = _blogPostDao.GetImagesForMonth(viewModel.DateForImages);
                    viewModel.DisplayDateString = string.Format("{0}/{1}", year.Value, month.Value);
                }

                else if (day.HasValue)//year/month/day
                {
                    viewModel.DateForImages = new DateTime(year.Value, month.Value, day.Value);
                    viewModel.Images = _blogPostDao.GetImagesForMonth(viewModel.DateForImages);
                    viewModel.DisplayDateString = string.Format("{0}/{1}/{2}", year.Value, month.Value, day.Value);
                }
                else // year/month
                {
                    viewModel.DateForImages = new DateTime(year.Value, month.Value, 1);
                    viewModel.Images = _blogPostDao.GetImagesForMonth(viewModel.DateForImages);
                    viewModel.DisplayDateString = string.Format("{0}/{1}", year.Value, month.Value);
                }
            }
            return viewModel;
        }
    }
}



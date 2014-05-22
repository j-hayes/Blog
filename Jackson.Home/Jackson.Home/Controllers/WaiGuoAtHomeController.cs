using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ImageResizer;
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

        public ActionResult Index()
        {
            var viewmodel = _blogPostDao.GetMostRecent();
            viewmodel = GetAndResizeImages(viewmodel);

            return View(viewmodel);
        }


        

        [HttpGet]
        public ActionResult Post(int? id, int? direction)
        {
            try
            {
                if (id != null)
                {
                    var viewModel = _blogPostDao.Get(id.Value);
                    if (viewModel.IsJornal) //block showing journal entires and skipping to next post
                    {
                        var nextId = id + 1*(direction ?? 1);
                        return Post(nextId,direction??1);
                    }
                    viewModel = GetAndResizeImages(viewModel);

                    return View(viewModel);
                }
                else
                {
                  return RedirectToAction("Index");
                }
            }
            catch (System.InvalidOperationException e)
            {
                return View("Oops", new ErrorViewModel()
                {
                    Message = String.Format("No Post With {0} Id Found", id.Value)
                
                });
            }

            
        }
        [HttpPost]
        public ActionResult Post(FormCollection collection)
        {
            int postId = Int32.Parse(collection["postId"]);
            int direction = Int32.Parse(collection["direction"]);



            return Post(postId, direction);
        }

        private BlogPost GetAndResizeImages(BlogPost viewmodel)
        {
            if (viewmodel.Images.Count < 1)
            {
                viewmodel.Images = _blogPostDao.GetImagesForDate(viewmodel.DateTime);
            }
            foreach (var postImage in viewmodel.Images)
            {
                using (var outStream = new MemoryStream())
                {
                    using (var inStream = new MemoryStream(postImage.Image))
                    {
                        var settings = new ResizeSettings("maxwidth=1000&maxheight=1000");
                        ImageResizer.ImageBuilder.Current.Build(inStream, outStream, settings);
                        postImage.Image = outStream.ToArray();
                        //   return new FileContentResult(outBytes, "image/jpeg");
                    }
                }
                
            }
            return viewmodel;
        }

        public ActionResult Image(int id)
        {
            PostImage viewModel = _blogPostDao.GetImage(id);
            return View(viewModel);
        }

        //
        // GET: /WaiGuoAtHome/

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
            var image = UploadPicture(NewImage, viewModel.Caption);

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
            _blogPostDao.DeleteImage(id);

            return RedirectToAction("ManageImages");
        }

        public ActionResult ManageImages()
        {//todo: build ProxyImage class
           return View( _blogPostDao.GetImagesForMonth(DateTime.Now.Month));
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
    }

}



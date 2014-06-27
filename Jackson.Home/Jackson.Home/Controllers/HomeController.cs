using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Jackson.DAL;
using Jackson.Home.Helpers;
using Jackson.Home.Models;
using NDatabase;


namespace Jackson.Home.Controllers
{
    public class HomeController : Controller
    {
        private IBlogPostDao _blogPostDao;

        public HomeController()
        {
            _blogPostDao = new BlogPostDao_linq();
        }

        public ActionResult Index()
        {
            var mostRecentBlogPost = _blogPostDao.GetMostRecent();
            
            var imageForPost = _blogPostDao.GetImagesForPost(mostRecentBlogPost);

            var firstOrDefault = imageForPost.FirstOrDefault();
            if (firstOrDefault != null)
            {
                var model = new HomePageViewModel
                {
                    MostRecentBlogPost = mostRecentBlogPost,
                    ImageForPost = firstOrDefault.Image //too simple should eventually be a specific image
                };


                return View(model);
            }

            else
            {
                var model = new HomePageViewModel
                {
                    MostRecentBlogPost = mostRecentBlogPost,
                    ImageForPost = _blogPostDao.GetDefaultImage()
                    
                };
                return View(model);
            }

         
        }

        public ActionResult Daily()
        {


            var model = new DailyViewModel()
            {
                DailyReadingsHtml = DailyReadingsScraper.GetReadings(),
                EditPostViewModel = EditViewModelFactory.GetNewEditPostViewModel(),
                //todo:getflashcards

            };






            return View(model);
        }
    }
}

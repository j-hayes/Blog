using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Jackson.DAL;
using Jackson.Home.Models;
using NDatabase;
using Sqo;

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
           
            
            var model = new HomePageViewModel { MostRecentBlogPost = _blogPostDao.GetMostRecent() };
            return View(model);
          
          //  return View(model);
        }
    }
}

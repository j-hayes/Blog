using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Jackson.Home
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
              name: "Date",
              url: "{controller}/{action}/{id}/{month}/{day}", //wierd overloading thing so I have to have id mean year here
              defaults: new { controller = "WaiGuoAtHome", action = "Images", year = DateTime.Now.Year.ToString("yyyy"),  
                  month = UrlParameter.Optional, day = UrlParameter.Optional }
          );
        }
    }
}

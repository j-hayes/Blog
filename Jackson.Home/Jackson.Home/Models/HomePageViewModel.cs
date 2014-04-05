using Jackson.DAL;

namespace Jackson.Home.Models

{
    public class HomePageViewModel
    {
        public BlogPost MostRecentBlogPost { get; set; }

        public HomePageViewModel()
        {
        }
    }
}
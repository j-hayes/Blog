using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jackson.DAL;

namespace Jackson.Home.Models
{
    public class EditPostViewModel
    {
        public BlogPost Post { get; set; }
        public List<PostTag> SelectedTags { get; set; } 
        public List<SelectListItem> EligiableTagList { get; set; }
        public List<PostImage> Images { get; set; }
        public int[] SelectedTagIds { get; set; }
    }
    
}
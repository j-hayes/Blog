using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jackson.DAL;
using Jackson.Home.Models;

namespace Jackson.Home.Helpers
{
    public static class EditViewModelFactory
    {
        public static EditPostViewModel GetNewEditPostViewModel()
        {
            IBlogPostDao _blogPostDao = new BlogPostDao_linq();
            var availibleTags = _blogPostDao.GetAllTags();

            EditPostViewModel viewModel = new EditPostViewModel
            {
                Post = new BlogPost { DateTime = DateTime.Now },
                EligiableTagList = new List<SelectListItem>(),
                SelectedTags = new List<PostTag>(),
                Images = new List<PostImage>()
            };


            foreach (var postTag in availibleTags)
            {
                viewModel.EligiableTagList.Add(new SelectListItem()
                {
                    Value = postTag.Id.ToString(),
                    Text = postTag.Tag
                });
            }
            return viewModel;
        }
    }
}
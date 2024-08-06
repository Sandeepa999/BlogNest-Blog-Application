using BlogNest.Web.Models.Domain;
using BlogNest.Web.Models.ViewModels;
using BlogNest.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogNest.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository tagRepository;
        private readonly IBlogPostRepository blogPostRepository;

        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this.tagRepository = tagRepository;
            this.blogPostRepository = blogPostRepository;
        }

        [HttpGet]
       public async Task<IActionResult> Add()
        {
            //get tags from repository
            var tags = await tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
        {
            //Map view model to domain model
            var blogPost = new BlogPost
            {
                Heading= addBlogPostRequest.Heading,
                PageTitle= addBlogPostRequest.PageTitle,
                Content=addBlogPostRequest.Content,
                ShortDescription=addBlogPostRequest.ShortDescription,
                FeaturedImageUrl=addBlogPostRequest.FeaturedImageUrl,
                UrlHandle=addBlogPostRequest.UrlHandle,
                PublishedDate=addBlogPostRequest.PublishedDate,
                Author=addBlogPostRequest.Author,
                Visible=addBlogPostRequest.Visible,
            };

            //Map Tag from selected tags
            var selectedTags = new List<Tag>();
            foreach(var selectedTagId in addBlogPostRequest.SelectedTags)
            {
                var selectedTagIdGuid = Guid.Parse(selectedTagId);
                var existingTag=await tagRepository.GetAsync(selectedTagIdGuid);

                if(existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
            }

            //Mapping tags back to domain model
            blogPost.Tags = selectedTags;

            await blogPostRepository.AddAsync(blogPost);
            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            //call the repositoty
            var blogPosts = await blogPostRepository.GetAllAsync();
            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //Retrive the result from the repository
            var blogPost = await blogPostRepository.GetAync(id);
            var tagsDomainModel = await tagRepository.GetAllAsync();

            if (blogPost != null)
            {
                //map the domain model into the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    Author = blogPost.Author,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
                };
                return View(model);
            }
            //pass data to the view
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            //map view model back to domain model
            var blogPostDomainModel = new BlogPost
            {
                Id=editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                Author = editBlogPostRequest.Author,
                PageTitle=editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                FeaturedImageUrl=editBlogPostRequest.FeaturedImageUrl,
                UrlHandle=editBlogPostRequest.UrlHandle,
                ShortDescription=editBlogPostRequest.ShortDescription,
                PublishedDate=editBlogPostRequest.PublishedDate,
                Visible = editBlogPostRequest.Visible,
            };

            //map tags into domain model
            var selectedTags = new List<Tag>();
            foreach(var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if(Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await tagRepository.GetAsync(tag);

                    if(foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            //submit information to repository to update
            var updatedBlog = await blogPostRepository.UpdateAsync(blogPostDomainModel);

            if(updatedBlog != null)
            {
                return RedirectToAction("List");
            }
            //reditect to GET
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            //Talk to repository to delete this blog post and tags
            var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if(deletedBlogPost != null)
            {
                return RedirectToAction("List");
            }
            //display response

            return RedirectToAction("Edit", new {id=editBlogPostRequest.Id});
        }

    }
}

using BlogNest.Web.Data;
using BlogNest.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogNest.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogNestDbContext blogNestDbContext;

        public BlogPostRepository(BlogNestDbContext blogNestDbContext)
        {
            this.blogNestDbContext = blogNestDbContext;
        }
        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await blogNestDbContext.AddAsync(blogPost);
            await blogNestDbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlog = await blogNestDbContext.BlogPosts.FindAsync(id);
            if(existingBlog != null)
            {
                blogNestDbContext.BlogPosts.Remove(existingBlog);
                await blogNestDbContext.SaveChangesAsync();
                return existingBlog;
            }
            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await blogNestDbContext.BlogPosts.Include(x => x.Tags).ToListAsync();
        }

        public async Task<BlogPost?> GetAync(Guid id)
        {
            return await blogNestDbContext.BlogPosts.Include(x=>x.Tags).FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await blogNestDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
           var existingBlog = await blogNestDbContext.BlogPosts.Include(x=>x.Tags).FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlog != null)
            {
                existingBlog.Id=blogPost.Id;
                existingBlog.Heading=blogPost.Heading;
                existingBlog.Author=blogPost.Author;
                existingBlog.PageTitle=blogPost.PageTitle;
                existingBlog.Content=blogPost.Content;
                existingBlog.ShortDescription=blogPost.ShortDescription;
                existingBlog.FeaturedImageUrl=blogPost.FeaturedImageUrl;
                existingBlog.UrlHandle=blogPost.UrlHandle;
                existingBlog.Visible=blogPost.Visible;
                existingBlog.PublishedDate=blogPost.PublishedDate;
                existingBlog.Tags=blogPost.Tags;

                await blogNestDbContext.SaveChangesAsync(); 
                return existingBlog;
            }
            return (null);
        }
    }
}

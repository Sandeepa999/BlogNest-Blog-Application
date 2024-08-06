using BlogNest.Web.Data;
using BlogNest.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogNest.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BlogNestDbContext blogNestDbContext;

        public TagRepository(BlogNestDbContext blogNestDbContext)
        {
            this.blogNestDbContext = blogNestDbContext;
        }
        public async Task<Tag> AddAsync(Tag tag)
        {
            await blogNestDbContext.Tags.AddAsync(tag);
            await blogNestDbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var existingTag = await blogNestDbContext.Tags.FindAsync(id);

            if(existingTag != null)
            {
                blogNestDbContext.Tags.Remove(existingTag);
                await blogNestDbContext.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
             return await blogNestDbContext.Tags.ToListAsync();
        }

        public Task<Tag?> GetAsync(Guid id)
        {
           return blogNestDbContext.Tags.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
           var existingTag =  await blogNestDbContext.Tags.FindAsync(tag.Id);
           if (existingTag != null)
            {
                existingTag.Name = tag.Name;
                existingTag.DisplayName = tag.DisplayName;

                await blogNestDbContext.SaveChangesAsync();
                return existingTag;
            }
            return null;
        }
    }
}

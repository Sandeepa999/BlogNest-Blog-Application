using BlogNest.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogNest.Web.Data
{
    public class BlogNestDbContext : DbContext
    {
        public BlogNestDbContext(DbContextOptions<BlogNestDbContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        
    }
}

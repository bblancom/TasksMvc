using Microsoft.EntityFrameworkCore;
using TasksMvc.Entities;

namespace TasksMvc
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Entities.Task> Tasks { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<AttachedFile> AttachedFiles { get; set; }
    }
}

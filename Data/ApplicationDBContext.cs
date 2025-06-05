
using Microsoft.EntityFrameworkCore;
using Prospecteurs44Back.Model;

namespace Prospecteurs44Back.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<AlerteSOS> AlerteSOS { get; set; } = default!;

        public DbSet<Topic> Topics { get; set; }
        public DbSet<TopicMessages> TopicMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Topic>()
            .HasMany(t => t.Messages)
            .WithOne(m => m.Topic)
            .HasForeignKey(m => m.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
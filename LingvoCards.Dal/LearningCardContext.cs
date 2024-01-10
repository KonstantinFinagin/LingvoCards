using LingvoCards.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal
{
    public class LearningCardContext : DbContext
    {
        public LearningCardContext()
        {
        }

        public LearningCardContext(DbContextOptions<LearningCardContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<Tag> Tags { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>()
                .HasMany(c => c.Tags)
                .WithMany(t => t.Cards);

            // Additional model configuration goes here
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite();
        }
    }
}

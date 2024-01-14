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
            // migration is performed in the AppData\Local\Packages\com.companyname.lingvocards.app_... folder
            Database.Migrate();
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
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Filename=lingvocards.db");
            }
        }
    }
}

using LingvoCards.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal.Repositories;

public class CardRepository : BaseRepository<Card>
{
    public CardRepository(LearningCardContext context) : base(context)
    {
    }

    public new async Task<IEnumerable<Card>> GetAllAsync()
    {
        return await DbSet
            .AsNoTracking()
            .Include(t => t.Tags)
            .ToListAsync();
    }

    public Card? GetCard(Guid id)
    {
        return DbSet.Include(c => c.Tags).FirstOrDefault(c => c.Id == id);
    }

    public async Task<List<Card>> GetByTermOrDescriptionAsync(string searchTerm)
    {
        return await DbSet
            .AsNoTracking()
            .Where(c => c.Term.Contains(searchTerm) || c.Description.Contains(searchTerm))
            .Include(t => t.Tags)
            .ToListAsync();
    }

    public async Task<List<Card>> GetFilteredAsync(Tag? selectedTag, ELevel selectedLevel, DateTime? dateFrom, DateTime? dateTo, int maxCardsInExercise)
    {
        // Load the cards first (without tag filter)
        var cards = DbSet
            .AsNoTracking()
            .Where(c => c.CreatedOn >= (dateFrom ?? DateTime.Parse("1990-01-01")))
            .Where(c => c.CreatedOn <= (dateTo ?? DateTime.Parse("2100-01-01")))
            .Where(c => c.Level == selectedLevel);

        // Apply tag filter in-memory if necessary
        if (selectedTag != null)
        {
            cards = cards.Where(c => c.Tags.Any(t => t.Id == selectedTag.Id));
        }

        return await cards.ToListAsync();
    }

    public async Task<List<Card>> GetDefaultFilteredAsync(int maxCardsInExercise)
    {
        // Load the cards first (without tag filter)
        return await DbSet.AsNoTracking()
            .OrderBy(c => (double) c.SuccessCount / (c.FailureCount+1))
            .Take(maxCardsInExercise)
            .ToListAsync();
    }
}
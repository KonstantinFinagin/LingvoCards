using LingvoCards.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal.Repositories;

public class CardRepository : BaseRepository<Card>
{
    public CardRepository(LearningCardContext context) : base(context)
    {
    }

    public new IEnumerable<Card> GetAll()
    {
        return DbSet
            .AsNoTracking()
            .Include(t => t.Tags)
            .ToList();
    }

    public Card? GetCard(Guid id)
    {
        return DbSet.Include(c => c.Tags).FirstOrDefault(c => c.Id == id);
    }

    public List<Card> GetByTermOrDescription(string searchTerm)
    {
        return DbSet
            .AsNoTracking()
            .Where(c => c.Term.Contains(searchTerm) || c.Description.Contains(searchTerm))
            .Include(t => t.Tags)
            .ToList();
    }

    public List<Card> GetFiltered(Tag? selectedTag, ELevel selectedLevel, DateTime? dateFrom, DateTime? dateTo, int maxCardsInExercise)
    {
        // Load the cards first (without tag filter)
        var cards = DbSet.AsNoTracking()
            .Where(c => c.CreatedOn >= (dateFrom ?? DateTime.Parse("1990-01-01")))
            .Where(c => c.CreatedOn <= (dateTo ?? DateTime.Parse("2100-01-01")))
            .Where(c => c.Level == selectedLevel)
            .OrderBy(c => (double) c.SuccessCount / c.FailureCount)
            .Take(maxCardsInExercise);

        // Apply tag filter in-memory if necessary
        if (selectedTag != null)
        {
            cards = cards.Where(c => c.Tags != null && c.Tags.Any(t => t.Id == selectedTag.Id));
        }

        // TODO error when a tag is selected - fix

        return cards.ToList();
    }

    public List<Card> GetDefaultFiltered(int maxCardsInExercise)
    {
        // Load the cards first (without tag filter)
        return DbSet.AsNoTracking()
            .OrderBy(c => (double) c.SuccessCount / c.FailureCount)
            .Take(maxCardsInExercise)
            .ToList();
    }
}
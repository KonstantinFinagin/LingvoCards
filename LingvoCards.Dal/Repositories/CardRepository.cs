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

    public List<Card> GetFiltered(Tag selectedTag, ELevel selectedLevel, DateTime? dateFrom, DateTime? dateTo, int i)
    {
        return DbSet.AsNoTracking().ToList();
    }
}
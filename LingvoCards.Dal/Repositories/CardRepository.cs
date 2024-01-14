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
            .Include(t => t.Tags)
            .ToList();
    }

    public List<Card> GetByTermOrDescription(string searchTerm)
    {
        return DbSet
            .Where(c => c.Term.Contains(searchTerm) || c.Description.Contains(searchTerm))
            .Include(t => t.Tags)
            .ToList();
    }
}
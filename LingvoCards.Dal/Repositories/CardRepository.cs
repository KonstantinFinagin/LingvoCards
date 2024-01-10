using LingvoCards.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal.Repositories;

public class CardRepository : BaseRepository<Card>
{
    public CardRepository(LearningCardContext context) : base(context)
    {
    }

    public List<Card> GetByTermOrDescription(string searchTerm)
    {
        return DbSet
            .AsNoTracking()
            .Where(c => c.Term.Contains(searchTerm) || c.Description.Contains(searchTerm))
            .ToList();
    }
}
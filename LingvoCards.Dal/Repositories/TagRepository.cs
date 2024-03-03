using LingvoCards.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal.Repositories;

public class TagRepository : BaseRepository<Tag>
{
    public TagRepository(LearningCardContext context) : base(context)
    { }

    public new async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await DbSet
            .ToListAsync();
    }

    public async Task<ICollection<Tag>> GetDefaultAsync()
    {
        return await DbSet.Where(t => t.IsDefault).ToListAsync();
    }
}
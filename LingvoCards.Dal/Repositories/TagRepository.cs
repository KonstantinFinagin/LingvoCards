using LingvoCards.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal.Repositories;

public class TagRepository : BaseRepository<Tag>
{
    public TagRepository(LearningCardContext context) : base(context)
    { }

    public new IEnumerable<Tag> GetAll()
    {
        return DbSet
            .ToList();
    }
}
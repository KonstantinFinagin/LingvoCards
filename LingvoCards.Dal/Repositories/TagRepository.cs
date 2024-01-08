using LingvoCards.Domain.Model;

namespace LingvoCards.Dal.Repositories;

public class TagRepository : BaseRepository<Tag>
{
    public TagRepository(LearningCardContext context) : base(context)
    {
    }
}
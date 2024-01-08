namespace LingvoCards.Domain.Model;

/// <summary>
///     Tag for grouping and searching cards
/// </summary>
public class Tag
{
    public Guid Id { get; set; }

    public string TagText { get; set; }

    public string Description { get; set; }

    public IEnumerable<Card> Cards { get; set; }
}
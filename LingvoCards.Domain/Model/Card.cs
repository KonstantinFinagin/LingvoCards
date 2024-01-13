using System.ComponentModel.DataAnnotations.Schema;

namespace LingvoCards.Domain.Model
{
    /// <summary>
    ///     Language learning card, created by a user, with a word and a term to repeat
    /// </summary>
    public class Card
    {
        public Guid Id { get; set; }
    
        public DateTimeOffset CreatedOn { get; set; }

        public string Term { get; set; }

        public string Description { get; set; }

        public IEnumerable<Tag> Tags { get; set; }

        // statistics
        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }
    }
}

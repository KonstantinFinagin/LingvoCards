﻿using System.ComponentModel.DataAnnotations.Schema;

namespace LingvoCards.Domain.Model
{
    public enum ELevel
    {
        Bronze = 0,
        Silver = 1,
        Gold = 2,
        Diamond = 3
    }

    /// <summary>
    ///     Language learning card, created by a user, with a word and a term to repeat
    /// </summary>
    public class Card
    {
        public Guid Id { get; set; }
    
        public DateTime CreatedOn { get; set; }

        public string Term { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Tag>? Tags { get; set; }

        // statistics
        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }

        [NotMapped] 
        public string SuccessRateText => $"{SuccessCount}/{FailureCount}";

        public ELevel Level { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        [NotMapped]
        public string FormattedTags
        {
            get
            {
                if (Tags != null && Tags.Any())
                {
                    return string.Join(", ", Tags.OrderBy(t => t.Text).Select(t => t.Text));
                }
                return string.Empty;
            }
        }

    }
}

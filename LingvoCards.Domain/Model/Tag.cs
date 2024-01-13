﻿using System.ComponentModel.DataAnnotations.Schema;

namespace LingvoCards.Domain.Model;

/// <summary>
///     Tag for grouping and searching cards
/// </summary>
public class Tag
{
    public Guid Id { get; set; }

    public string Text { get; set; }

    public bool IsDefault { get; set; }

    public IEnumerable<Card> Cards { get; set; }

    [NotMapped]
    public bool IsSelected { get; set; }
}
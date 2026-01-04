using System;
using System.Collections.Generic;

namespace WebPortal.Domain.Entities;

public sealed class Link
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? Tags { get; set; }
    public int Priority { get; set; } = 0;
    public string RolePrefix { get; set; } = default!;

    public bool IsActive { get; set; } = true;

    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Category? Category { get; set; }
    public List<FavoriteLink> FavoriteLinks { get; set; } = new();
    public List<LinkSequence> LinkSequences { get; set; } = new();
}

using System;
using System.Collections.Generic;
using WebPortal.Domain.Common;
using WebPortal.Domain.Enums;

namespace WebPortal.Domain.Entities;

public sealed class User : IAuditable
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? FullName { get; set; }
    public string? Email { get; set; }

    public ViewMode ViewMode { get; set; } = ViewMode.Grid;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<FavoriteLink> FavoriteLinks { get; set; } = new();
    public List<LinkSequence> LinkSequences { get; set; } = new();
    public List<CategorySequence> CategorySequences { get; set; } = new();
}

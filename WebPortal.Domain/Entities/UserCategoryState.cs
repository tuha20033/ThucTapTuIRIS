using System;

namespace WebPortal.Domain.Entities;

public sealed class UserCategoryState
{
    public string UserId { get; set; } = default!;
    public Guid CategoryId { get; set; }

    public bool IsCollapsed { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

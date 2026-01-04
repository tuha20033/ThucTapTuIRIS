using System;

namespace WebPortal.Domain.Entities;

public sealed class FavoriteLink
{
    public string UserId { get; set; } = default!;
    public Guid LinkId { get; set; }

    public DateTime PinnedAt { get; set; }

    // Navigation
    public User? User { get; set; }
    public Link? Link { get; set; }
}

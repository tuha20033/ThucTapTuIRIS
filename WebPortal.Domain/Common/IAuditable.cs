using System;

namespace WebPortal.Domain.Common;

/// <summary>
/// Marker for entities with CreatedAt/UpdatedAt timestamps.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

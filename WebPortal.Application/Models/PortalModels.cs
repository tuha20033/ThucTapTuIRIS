using System;
using System.Collections.Generic;
using WebPortal.Domain.Enums;

namespace WebPortal.Application.Models;

public sealed record PortalModel
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
    public required ViewMode ViewMode { get; init; }

    /// <summary>Pinned links shown at the top of the portal.</summary>
    public required List<PortalLinkModel> PinnedLinks { get; init; }

    public required List<PortalCategoryModel> Categories { get; init; }

    public required DateTime GeneratedAtUtc { get; init; }
}

public sealed record PortalCategoryModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }

    public required int DefaultPriority { get; init; }

    public required bool IsCollapsed { get; init; } // UI-only state (not persisted in DB)

    public required List<PortalLinkModel> Links { get; init; }
}

public sealed record PortalLinkModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public string? Tags { get; init; }

    public required string RolePrefix { get; init; }

    public required Guid CategoryId { get; init; }

    public required bool IsPinned { get; init; }

    /// <summary>Domain/host extracted from Url for hover preview.</summary>
    public required string Domain { get; init; }
}

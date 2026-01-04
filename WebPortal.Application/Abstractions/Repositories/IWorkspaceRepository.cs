using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebPortal.Domain.Entities;

namespace WebPortal.Application.Abstractions.Repositories;

public interface IWorkspaceRepository
{
    Task<IReadOnlyList<FavoriteLink>> GetFavoritesAsync(string userId, CancellationToken ct = default);
    Task<bool> IsFavoriteAsync(string userId, Guid linkId, CancellationToken ct = default);
    Task PinAsync(string userId, Guid linkId, CancellationToken ct = default);
    Task UnpinAsync(string userId, Guid linkId, CancellationToken ct = default);

    Task<IReadOnlyList<LinkSequence>> GetLinkSequencesAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<CategorySequence>> GetCategorySequencesAsync(string userId, CancellationToken ct = default);

    /// <summary>
    /// Per-user collapse/expand state for categories.
    /// </summary>
    Task<IReadOnlyList<UserCategoryState>> GetCategoryStatesAsync(string userId, CancellationToken ct = default);

    /// <summary>
    /// Persists the category collapse state for the user.
    /// </summary>
    Task SetCategoryCollapsedAsync(string userId, Guid categoryId, bool isCollapsed, CancellationToken ct = default);

    /// <summary>
    /// Replaces the user's order for the provided link ids. Existing sequences for these links are updated/created.
    /// Links not included are left unchanged (they will fall back to default Priority on display).
    /// </summary>
    Task SetLinkOrderAsync(string userId, IReadOnlyList<Guid> orderedLinkIds, CancellationToken ct = default);

    /// <summary>
    /// Replaces the user's order for the provided category ids.
    /// Categories not included are left unchanged (they will fall back to default Priority on display).
    /// </summary>
    Task SetCategoryOrderAsync(string userId, IReadOnlyList<Guid> orderedCategoryIds, CancellationToken ct = default);
}

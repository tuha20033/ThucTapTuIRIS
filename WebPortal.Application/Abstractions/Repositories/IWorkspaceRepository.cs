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

    
  
    Task<IReadOnlyList<UserCategoryState>> GetCategoryStatesAsync(string userId, CancellationToken ct = default);

   
    Task SetCategoryCollapsedAsync(string userId, Guid categoryId, bool isCollapsed, CancellationToken ct = default);

  
    Task SetLinkOrderAsync(string userId, IReadOnlyList<Guid> orderedLinkIds, CancellationToken ct = default);

   
    Task SetCategoryOrderAsync(string userId, IReadOnlyList<Guid> orderedCategoryIds, CancellationToken ct = default);
}

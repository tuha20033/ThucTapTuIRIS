using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebPortal.Domain.Entities;

namespace WebPortal.Application.Abstractions.Repositories;

public interface ILinkRepository
{
    Task<IReadOnlyList<Link>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Link>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Link>> GetActiveByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    Task<Link?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Link> CreateAsync(Link link, CancellationToken ct = default);
    Task UpdateAsync(Link link, CancellationToken ct = default);

    /// <summary>Soft delete via IsActive=false.</summary>
    Task SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    /// <summary>Hard delete.</summary>
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

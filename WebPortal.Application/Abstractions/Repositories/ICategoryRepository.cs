using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebPortal.Domain.Entities;

namespace WebPortal.Application.Abstractions.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Category> CreateAsync(Category category, CancellationToken ct = default);
    Task UpdateAsync(Category category, CancellationToken ct = default);


    Task SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

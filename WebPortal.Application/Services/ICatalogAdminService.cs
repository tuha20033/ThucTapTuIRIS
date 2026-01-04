using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebPortal.Application.Models;
using WebPortal.Domain.Entities;

namespace WebPortal.Application.Services;

public interface ICatalogAdminService
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Link>> GetLinksAsync(CancellationToken ct = default);

    Task<Category> UpsertCategoryAsync(CategoryUpsertModel model, CancellationToken ct = default);
    Task<Link> UpsertLinkAsync(LinkUpsertModel model, CancellationToken ct = default);

    Task SetCategoryActiveAsync(Guid categoryId, bool isActive, CancellationToken ct = default);
    Task SetLinkActiveAsync(Guid linkId, bool isActive, CancellationToken ct = default);


    Task DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default);

    Task DeleteLinkAsync(Guid linkId, CancellationToken ct = default);
}

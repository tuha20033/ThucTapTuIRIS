using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebPortal.Application.Abstractions.Caching;
using WebPortal.Application.Abstractions.Repositories;
using WebPortal.Application.Abstractions.Security;
using WebPortal.Application.Common;
using WebPortal.Application.Models;
using WebPortal.Domain.Entities;
using WebPortal.Domain.Guards;

namespace WebPortal.Application.Services;

public sealed class CatalogAdminService : ICatalogAdminService
{
    private readonly IUserContext _user;
    private readonly ICategoryRepository _categories;
    private readonly ILinkRepository _links;
    private readonly ICacheService _cache;

    public CatalogAdminService(IUserContext user, ICategoryRepository categories, ILinkRepository links, ICacheService cache)
    {
        _user = user;
        _categories = categories;
        _links = links;
        _cache = cache;
    }

    public Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);
        return _categories.GetAllAsync(ct);
    }

    public Task<IReadOnlyList<Link>> GetLinksAsync(CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);
        return _links.GetAllAsync(ct);
    }

    public async Task<Category> UpsertCategoryAsync(CategoryUpsertModel model, CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);

        if (string.IsNullOrWhiteSpace(model.Name))
            throw new ArgumentException("Category name is required.");

        if (model.Id is null || model.Id == Guid.Empty)
        {
            var created = await _categories.CreateAsync(new Category
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                Priority = model.Priority,
                IsActive = model.IsActive
            }, ct);

            await InvalidateCatalogCacheAsync(ct);
            return created;
        }
        else
        {
            var existing = await _categories.GetByIdAsync(model.Id.Value, ct)
                           ?? throw new InvalidOperationException("Category not found.");

            existing.Name = model.Name.Trim();
            existing.Description = model.Description?.Trim();
            existing.Priority = model.Priority;
            existing.IsActive = model.IsActive;

            await _categories.UpdateAsync(existing, ct);
            await InvalidateCatalogCacheAsync(ct);
            return existing;
        }
    }

    public async Task<Link> UpsertLinkAsync(LinkUpsertModel model, CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);

        if (string.IsNullOrWhiteSpace(model.Name))
            throw new ArgumentException("Link name is required.");
        if (string.IsNullOrWhiteSpace(model.Url))
            throw new ArgumentException("Url is required.");
        if (string.IsNullOrWhiteSpace(model.RolePrefix))
            throw new ArgumentException("RolePrefix is required.");

        var url = model.Url.Trim();
        if (UrlSecurityGuard.IsBlocked(url))
            throw new ArgumentException("URL is blocked by security policy (javascript:, data:).");

        if (model.Id is null || model.Id == Guid.Empty)
        {
            var created = await _links.CreateAsync(new Link
            {
                Name = model.Name.Trim(),
                Url = url,
                Icon = model.Icon?.Trim(),
                Color = model.Color?.Trim(),
                Tags = model.Tags?.Trim(),
                Priority = model.Priority,
                RolePrefix = model.RolePrefix.Trim(),
                IsActive = model.IsActive,
                CategoryId = model.CategoryId
            }, ct);

            await InvalidateCatalogCacheAsync(ct);
            return created;
        }
        else
        {
            var existing = await _links.GetByIdAsync(model.Id.Value, ct)
                           ?? throw new InvalidOperationException("Link not found.");

            existing.Name = model.Name.Trim();
            existing.Url = url;
            existing.Icon = model.Icon?.Trim();
            existing.Color = model.Color?.Trim();
            existing.Tags = model.Tags?.Trim();
            existing.Priority = model.Priority;
            existing.RolePrefix = model.RolePrefix.Trim();
            existing.IsActive = model.IsActive;
            existing.CategoryId = model.CategoryId;

            await _links.UpdateAsync(existing, ct);
            await InvalidateCatalogCacheAsync(ct);
            return existing;
        }
    }

    public async Task SetCategoryActiveAsync(Guid categoryId, bool isActive, CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);

        await _categories.SetActiveAsync(categoryId, isActive, ct);
        await InvalidateCatalogCacheAsync(ct);
    }

    public async Task SetLinkActiveAsync(Guid linkId, bool isActive, CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);

        await _links.SetActiveAsync(linkId, isActive, ct);
        await InvalidateCatalogCacheAsync(ct);
    }

    public async Task DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);
        await _categories.DeleteAsync(categoryId, ct);
        await InvalidateCatalogCacheAsync(ct);
    }

    public async Task DeleteLinkAsync(Guid linkId, CancellationToken ct = default)
    {
        AdminGuard.EnsureAdmin(_user);
        await _links.DeleteAsync(linkId, ct);
        await InvalidateCatalogCacheAsync(ct);
    }

    private async Task InvalidateCatalogCacheAsync(CancellationToken ct)
    {
        await _cache.RemoveAsync(CacheKeys.ActiveCategories, ct);
        await _cache.RemoveAsync(CacheKeys.ActiveLinks, ct);
    }
}

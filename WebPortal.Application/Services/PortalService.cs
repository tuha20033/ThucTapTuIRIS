using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebPortal.Application.Abstractions.Caching;
using WebPortal.Application.Abstractions.Repositories;
using WebPortal.Application.Abstractions.Security;
using WebPortal.Application.Common;
using WebPortal.Application.Models;
using WebPortal.Domain.Entities;
using WebPortal.Domain.Enums;
using WebPortal.Domain.Guards;

namespace WebPortal.Application.Services;

public sealed class PortalService : IPortalService
{
    private static readonly TimeSpan CatalogTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan WorkspaceTtl = TimeSpan.FromMinutes(5);

    private readonly IUserContext _user;
    private readonly IUserRepository _users;
    private readonly ICategoryRepository _categories;
    private readonly ILinkRepository _links;
    private readonly IWorkspaceRepository _workspace;
    private readonly ICacheService _cache;

    public PortalService(
        IUserContext user,
        IUserRepository users,
        ICategoryRepository categories,
        ILinkRepository links,
        IWorkspaceRepository workspace,
        ICacheService cache)
    {
        _user = user;
        _users = users;
        _categories = categories;
        _links = links;
        _workspace = workspace;
        _cache = cache;
    }

    public async Task<PortalModel> GetPortalAsync(string? searchText, CancellationToken ct = default)
    {
        var ensuredUser = await _users.EnsureUserAsync(
            id: _user.UserId,
            userName: string.IsNullOrWhiteSpace(_user.UserName) ? _user.UserId : _user.UserName!,
            fullName: null,
            email: null,
            ct: ct);

        var categories = await _cache.GetOrCreateAsync(
            CacheKeys.ActiveCategories,
            CatalogTtl,
            async token => (await _categories.GetActiveAsync(token)).ToList(),
            ct);

        var links = await _cache.GetOrCreateAsync(
            CacheKeys.ActiveLinks,
            CatalogTtl,
            async token => (await _links.GetActiveAsync(token)).ToList(),
            ct);

        var ws = await _cache.GetOrCreateAsync(
            CacheKeys.Workspace(_user.UserId),
            WorkspaceTtl,
            async token => await LoadWorkspaceSnapshotAsync(_user.UserId, token),
            ct);

        var allowedLinks = links
            .Where(l => l.IsActive)
            .Where(l => !UrlSecurityGuard.IsBlocked(l.Url))
            .Where(l => RolePrefixGuard.IsAllowedByPrefix(_user.Roles, l.RolePrefix))
            .ToList();

        var q = (searchText ?? string.Empty).Trim();
        bool hasQuery = q.Length > 0;
        string qLower = q.ToLowerInvariant();

        bool Match(Link l, Category c)
        {
            if (!hasQuery) return true;
            if (l.Name.ToLowerInvariant().Contains(qLower)) return true;
            if (!string.IsNullOrWhiteSpace(l.Tags) && l.Tags!.ToLowerInvariant().Contains(qLower)) return true;
            if (c.Name.ToLowerInvariant().Contains(qLower)) return true;
            return false;
        }

      
        var categoryOrder = ws.CategoryOrderIndex;
        var orderedCategories = categories
            .Where(c => c.IsActive)
            .OrderBy(c => categoryOrder.TryGetValue(c.Id, out _) ? 0 : 1)
            .ThenBy(c => categoryOrder.TryGetValue(c.Id, out var idx) ? idx : c.Priority)
            .ThenBy(c => c.Name)
            .ToList();

        var favoriteSet = ws.FavoriteLinkIds;

        var pinnedLinks = allowedLinks
            .Where(l => favoriteSet.Contains(l.Id))
            .OrderBy(l => ws.LinkOrderIndex.TryGetValue(l.Id, out _) ? 0 : 1)
            .ThenBy(l => ws.LinkOrderIndex.TryGetValue(l.Id, out var idx) ? idx : l.Priority)
            .ThenBy(l => l.Name)
            .Select(l => ToPortalLink(l, isPinned: true))
            .ToList();

        if (hasQuery)
            pinnedLinks = pinnedLinks.Where(pl => pl.Name.ToLowerInvariant().Contains(qLower)
                                              || (!string.IsNullOrWhiteSpace(pl.Tags) && pl.Tags!.ToLowerInvariant().Contains(qLower))
                                              || (!string.IsNullOrWhiteSpace(pl.Domain) && pl.Domain.ToLowerInvariant().Contains(qLower)))
                                     .ToList();

        var linkOrder = ws.LinkOrderIndex;

        var categoryModels = new List<PortalCategoryModel>();

        foreach (var c in orderedCategories)
        {
            var catLinks = allowedLinks
                .Where(l => l.CategoryId == c.Id)
                .Where(l => Match(l, c))
                .OrderBy(l => linkOrder.TryGetValue(l.Id, out _) ? 0 : 1)
                .ThenBy(l => linkOrder.TryGetValue(l.Id, out var idx) ? idx : l.Priority)
                .ThenBy(l => l.Name)
                .Select(l => ToPortalLink(l, isPinned: favoriteSet.Contains(l.Id)))
                .ToList();

            if (hasQuery && catLinks.Count == 0)
                continue;

            categoryModels.Add(new PortalCategoryModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                DefaultPriority = c.Priority,
                IsCollapsed = ws.CategoryCollapsed.TryGetValue(c.Id, out var collapsed) && collapsed,
                Links = catLinks
            });
        }

        return new PortalModel
        {
            UserId = ensuredUser.Id,
            UserName = ensuredUser.UserName,
            ViewMode = ensuredUser.ViewMode,
            PinnedLinks = pinnedLinks,
            Categories = categoryModels,
            GeneratedAtUtc = DateTime.UtcNow
        };
    }

    public async Task PinAsync(Guid linkId, CancellationToken ct = default)
    {
        var link = await _links.GetByIdAsync(linkId, ct);
        if (link is null) throw new InvalidOperationException("Link not found.");
        if (!link.IsActive) throw new InvalidOperationException("Link is inactive.");
        if (UrlSecurityGuard.IsBlocked(link.Url)) throw new InvalidOperationException("URL is blocked by security policy.");

        await _workspace.PinAsync(_user.UserId, linkId, ct);
        await _cache.RemoveAsync(CacheKeys.Workspace(_user.UserId), ct);
    }

    public async Task UnpinAsync(Guid linkId, CancellationToken ct = default)
    {
        await _workspace.UnpinAsync(_user.UserId, linkId, ct);
        await _cache.RemoveAsync(CacheKeys.Workspace(_user.UserId), ct);
    }

    public async Task SetViewModeAsync(ViewMode mode, CancellationToken ct = default)
    {
        await _users.SetViewModeAsync(_user.UserId, mode, ct);
        await _cache.RemoveAsync(CacheKeys.Workspace(_user.UserId), ct);
    }

    public async Task SetLinkOrderAsync(IReadOnlyList<Guid> orderedLinkIds, CancellationToken ct = default)
    {
        await _workspace.SetLinkOrderAsync(_user.UserId, orderedLinkIds, ct);
        await _cache.RemoveAsync(CacheKeys.Workspace(_user.UserId), ct);
    }

    public async Task SetCategoryOrderAsync(IReadOnlyList<Guid> orderedCategoryIds, CancellationToken ct = default)
    {
        await _workspace.SetCategoryOrderAsync(_user.UserId, orderedCategoryIds, ct);
        await _cache.RemoveAsync(CacheKeys.Workspace(_user.UserId), ct);
    }

    public async Task SetCategoryCollapsedAsync(Guid categoryId, bool isCollapsed, CancellationToken ct = default)
    {
        await _workspace.SetCategoryCollapsedAsync(_user.UserId, categoryId, isCollapsed, ct);
        await _cache.RemoveAsync(CacheKeys.Workspace(_user.UserId), ct);
    }

    private static PortalLinkModel ToPortalLink(Link l, bool isPinned)
        => new()
        {
            Id = l.Id,
            Name = l.Name,
            Url = l.Url,
            Icon = l.Icon,
            Color = l.Color,
            Tags = l.Tags,
            RolePrefix = l.RolePrefix,
            CategoryId = l.CategoryId,
            IsPinned = isPinned,
            Domain = UrlSecurityGuard.TryGetDomain(l.Url)
        };

    private async Task<WorkspaceSnapshot> LoadWorkspaceSnapshotAsync(string userId, CancellationToken ct)
    {
        var favorites = await _workspace.GetFavoritesAsync(userId, ct);
        var linkSeq = await _workspace.GetLinkSequencesAsync(userId, ct);
        var catSeq = await _workspace.GetCategorySequencesAsync(userId, ct);

        var categoryCollapsed = catSeq.ToDictionary(x => x.CategoryId, x => x.IsCollapsed);

        return new WorkspaceSnapshot(
            FavoriteLinkIds: favorites.Select(f => f.LinkId).ToHashSet(),
            LinkOrderIndex: linkSeq.ToDictionary(x => x.LinkId, x => x.OrderIndex),
            CategoryOrderIndex: catSeq.ToDictionary(x => x.CategoryId, x => x.OrderIndex),
            CategoryCollapsed: categoryCollapsed
                );
    }

    private sealed record WorkspaceSnapshot(
        HashSet<Guid> FavoriteLinkIds,
        Dictionary<Guid, int> LinkOrderIndex,
        Dictionary<Guid, int> CategoryOrderIndex,
        Dictionary<Guid, bool> CategoryCollapsed
    );
}

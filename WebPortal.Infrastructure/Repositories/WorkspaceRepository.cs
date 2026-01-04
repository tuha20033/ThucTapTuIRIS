using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebPortal.Application.Abstractions.Repositories;
using WebPortal.Domain.Entities;
using WebPortal.Infrastructure.Persistence;

namespace WebPortal.Infrastructure.Repositories;

public sealed class WorkspaceRepository : IWorkspaceRepository
{
    private readonly WebPortalDbContext _db;

    public WorkspaceRepository(WebPortalDbContext db) => _db = db;

    public async Task<IReadOnlyList<FavoriteLink>> GetFavoritesAsync(string userId, CancellationToken ct = default)
    {
        return await _db.FavoriteLinks
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.PinnedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> IsFavoriteAsync(string userId, Guid linkId, CancellationToken ct = default)
    {
        return await _db.FavoriteLinks.AnyAsync(x => x.UserId == userId && x.LinkId == linkId, ct);
    }

    public async Task PinAsync(string userId, Guid linkId, CancellationToken ct = default)
    {
        var exists = await _db.FavoriteLinks.AnyAsync(x => x.UserId == userId && x.LinkId == linkId, ct);
        if (exists) return;

        _db.FavoriteLinks.Add(new FavoriteLink
        {
            UserId = userId,
            LinkId = linkId,
            PinnedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
    }

    public async Task UnpinAsync(string userId, Guid linkId, CancellationToken ct = default)
    {
        var existing = await _db.FavoriteLinks.FirstOrDefaultAsync(x => x.UserId == userId && x.LinkId == linkId, ct);
        if (existing is null) return;

        _db.FavoriteLinks.Remove(existing);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<LinkSequence>> GetLinkSequencesAsync(string userId, CancellationToken ct = default)
    {
        return await _db.LinkSequence
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CategorySequence>> GetCategorySequencesAsync(string userId, CancellationToken ct = default)
    {
        return await _db.CategorySequence
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserCategoryState>> GetCategoryStatesAsync(string userId, CancellationToken ct = default)
    {
        return await _db.UserCategoryStates
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task SetCategoryCollapsedAsync(string userId, Guid categoryId, bool isCollapsed, CancellationToken ct = default)
    {
        var existing = await _db.UserCategoryStates
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CategoryId == categoryId, ct);

        if (existing is null)
        {
            _db.UserCategoryStates.Add(new UserCategoryState
            {
                UserId = userId,
                CategoryId = categoryId,
                IsCollapsed = isCollapsed,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.IsCollapsed = isCollapsed;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task SetLinkOrderAsync(string userId, IReadOnlyList<Guid> orderedLinkIds, CancellationToken ct = default)
    {
        if (orderedLinkIds.Count == 0) return;

        // Update/Insert the provided order set.
        var existing = await _db.LinkSequence
            .Where(x => x.UserId == userId && orderedLinkIds.Contains(x.LinkId))
            .ToListAsync(ct);

        var existingMap = existing.ToDictionary(x => x.LinkId, x => x);

        for (int i = 0; i < orderedLinkIds.Count; i++)
        {
            var linkId = orderedLinkIds[i];

            if (existingMap.TryGetValue(linkId, out var row))
            {
                row.OrderIndex = i;
            }
            else
            {
                _db.LinkSequence.Add(new LinkSequence
                {
                    UserId = userId,
                    LinkId = linkId,
                    OrderIndex = i
                });
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task SetCategoryOrderAsync(string userId, IReadOnlyList<Guid> orderedCategoryIds, CancellationToken ct = default)
    {
        if (orderedCategoryIds.Count == 0) return;

        var existing = await _db.CategorySequence
            .Where(x => x.UserId == userId && orderedCategoryIds.Contains(x.CategoryId))
            .ToListAsync(ct);

        var existingMap = existing.ToDictionary(x => x.CategoryId, x => x);

        for (int i = 0; i < orderedCategoryIds.Count; i++)
        {
            var categoryId = orderedCategoryIds[i];

            if (existingMap.TryGetValue(categoryId, out var row))
            {
                row.OrderIndex = i;
            }
            else
            {
                _db.CategorySequence.Add(new CategorySequence
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    OrderIndex = i
                });
            }
        }

        await _db.SaveChangesAsync(ct);
    }
}

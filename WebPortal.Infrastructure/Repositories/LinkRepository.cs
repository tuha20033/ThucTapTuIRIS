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

public sealed class LinkRepository : ILinkRepository
{
    private readonly WebPortalDbContext _db;

    public LinkRepository(WebPortalDbContext db) => _db = db;

    public async Task<IReadOnlyList<Link>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _db.Links
            .AsNoTracking()
            .Where(l => l.IsActive)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Link>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Links
            .AsNoTracking()
            .OrderByDescending(l => l.IsActive)
            .ThenBy(l => l.CategoryId)
            .ThenBy(l => l.Priority)
            .ThenBy(l => l.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Link>> GetActiveByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _db.Links
            .AsNoTracking()
            .Where(l => l.IsActive && l.CategoryId == categoryId)
            .ToListAsync(ct);
    }

    public Task<Link?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Links.FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<Link> CreateAsync(Link link, CancellationToken ct = default)
    {
        _db.Links.Add(link);
        await _db.SaveChangesAsync(ct);
        return link;
    }

    public async Task UpdateAsync(Link link, CancellationToken ct = default)
    {
        _db.Links.Update(link);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var existing = await _db.Links.FirstOrDefaultAsync(l => l.Id == id, ct)
                       ?? throw new InvalidOperationException("Link not found.");

        existing.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Links.FirstOrDefaultAsync(l => l.Id == id, ct)
                       ?? throw new InvalidOperationException("Link not found.");

        _db.Links.Remove(existing);
        await _db.SaveChangesAsync(ct);
    }
}

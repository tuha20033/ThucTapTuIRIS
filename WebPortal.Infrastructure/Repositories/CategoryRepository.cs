using Microsoft.EntityFrameworkCore;
using WebPortal.Application.Abstractions.Repositories;
using WebPortal.Domain.Entities;
using WebPortal.Infrastructure.Persistence;

namespace WebPortal.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly WebPortalDbContext _db;

    public CategoryRepository(WebPortalDbContext db) => _db = db;

    public async Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken ct = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Priority)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .OrderByDescending(c => c.IsActive)
            .ThenBy(c => c.Priority)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Category> CreateAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SetActiveAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        var existing = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct)
                       ?? throw new InvalidOperationException("Category not found.");

        existing.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct)
                       ?? throw new InvalidOperationException("Category not found.");

        // Prevent FK violation by checking links first (no cascade from Category -> Links).
        var hasLinks = await _db.Links.AnyAsync(l => l.CategoryId == id, ct);
        if (hasLinks)
            throw new InvalidOperationException("Cannot delete category because it still has links. Remove/move links or use Deactivate.");

        _db.Categories.Remove(existing);
        await _db.SaveChangesAsync(ct);
    }
}

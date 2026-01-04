using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebPortal.Application.Abstractions.Repositories;
using WebPortal.Domain.Entities;
using WebPortal.Domain.Enums;
using WebPortal.Infrastructure.Persistence;

namespace WebPortal.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly WebPortalDbContext _db;

    public UserRepository(WebPortalDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
        => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User> EnsureUserAsync(string id, string userName, string? fullName, string? email, CancellationToken ct = default)
    {
        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (existing is not null)
        {
          
            if (!string.Equals(existing.UserName, userName, StringComparison.Ordinal))
                existing.UserName = userName;

            if (!string.IsNullOrWhiteSpace(fullName))
                existing.FullName = fullName;

            if (!string.IsNullOrWhiteSpace(email))
                existing.Email = email;

            existing.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return existing;
        }

        var created = new User
        {
            Id = id,
            UserName = userName,
            FullName = fullName,
            Email = email,
            ViewMode = ViewMode.Grid,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(created);
        await _db.SaveChangesAsync(ct);
        return created;
    }

    public async Task SetViewModeAsync(string userId, ViewMode mode, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        user.ViewMode = mode;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }
}

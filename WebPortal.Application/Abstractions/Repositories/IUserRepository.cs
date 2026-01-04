using System.Threading;
using System.Threading.Tasks;
using WebPortal.Domain.Entities;
using WebPortal.Domain.Enums;

namespace WebPortal.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);

    Task<User> EnsureUserAsync(string id, string userName, string? fullName, string? email, CancellationToken ct = default);

    Task SetViewModeAsync(string userId, ViewMode mode, CancellationToken ct = default);
}

using System;
using System.Linq;
using WebPortal.Application.Abstractions.Security;

namespace WebPortal.Application.Common;

public static class AdminGuard
{
   
    public const string AdminRolePrefix = "PORTAL_ADMIN";

    public static void EnsureAdmin(IUserContext user)
    {
        if (user is null) throw new ArgumentNullException(nameof(user));

        var roles = user.Roles ?? Array.Empty<string>();
        var ok = roles.Any(r => !string.IsNullOrWhiteSpace(r) && r.StartsWith(AdminRolePrefix, StringComparison.OrdinalIgnoreCase));
        if (!ok)
            throw new UnauthorizedAccessException($"Cần có Admin '{AdminRolePrefix}').");
    }
}

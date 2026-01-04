using System;
using System.Collections.Generic;

namespace WebPortal.Domain.Guards;

public static class RolePrefixGuard
{
    public static bool IsAllowedByPrefix(IEnumerable<string> userRoles, string rolePrefix)
    {
        if (string.IsNullOrWhiteSpace(rolePrefix)) return false;

        foreach (var r in userRoles)
        {
            if (r is null) continue;

            if (r.StartsWith(rolePrefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}

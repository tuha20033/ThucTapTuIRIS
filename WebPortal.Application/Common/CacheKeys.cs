using System;
using System.Security.Cryptography;
using System.Text;

namespace WebPortal.Application.Common;

public static class CacheKeys
{
    public static string ActiveCategories => "catalog:categories:active";
    public static string ActiveLinks => "catalog:links:active";

    public static string Workspace(string userId) => $"workspace:{userId}";

    public static string Portal(string userId, string rolesHash, string? searchText)
    {
        var searchPart = string.IsNullOrWhiteSpace(searchText) ? "" : $":q={Normalize(searchText)}";
        return $"portal:{userId}:{rolesHash}{searchPart}";
    }

    public static string HashRoles(System.Collections.Generic.IReadOnlyCollection<string> roles)
    {
        var joined = string.Join('|', roles);
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(joined));
        return Convert.ToHexString(bytes)[..12].ToLowerInvariant();
    }

    private static string Normalize(string s)
        => s.Trim().ToLowerInvariant().Replace(' ', '_');
}

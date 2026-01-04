using System;

namespace WebPortal.Domain.Guards;


public static class UrlSecurityGuard
{
    private static readonly string[] BlockedSchemes =
    [
        "javascript:",
        "data:"
    ];

    public static bool IsBlocked(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;

        var u = url.TrimStart();

        foreach (var scheme in BlockedSchemes)
        {
            if (u.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
    public static string TryGetDomain(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;

        if (Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri))
            return uri.Host ?? string.Empty;

        return string.Empty;
    }
}

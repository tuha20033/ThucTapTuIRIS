using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using MudBlazor;

namespace WebPortal.Web.UI;

public static class IconHelper
{
    private static readonly ConcurrentDictionary<string, string?> Cache = new(StringComparer.OrdinalIgnoreCase);

    public static bool IsImageUrl(string? icon)
    {
        if (string.IsNullOrWhiteSpace(icon)) return false;
        var s = icon.Trim();
        if (s.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || s.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return true;

        return s.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            || s.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || s.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || s.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
            || s.EndsWith(".webp", StringComparison.OrdinalIgnoreCase);
    }

    public static string? ResolveMudIcon(string? iconKey)
    {
        if (string.IsNullOrWhiteSpace(iconKey)) return null;
        var key = iconKey.Trim();
        return Cache.GetOrAdd(key, ResolveInternal);
    }

    private static string? ResolveInternal(string key)
    {
       
        if (IsImageUrl(key)) return null;

       
        if (key.StartsWith("Icons.Material.", StringComparison.OrdinalIgnoreCase))
        {
           
            var parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 4)
            {
                var style = parts[2];
                var name = string.Join('.', parts.Skip(3));

                var t = style switch
                {
                    "Filled" => typeof(Icons.Material.Filled),
                    "Outlined" => typeof(Icons.Material.Outlined),
                    "Round" => typeof(Icons.Material.Rounded),
                    "Sharp" => typeof(Icons.Material.Sharp),
                    "TwoTone" => typeof(Icons.Material.TwoTone),
                    _ => null
                };

                if (t is not null)
                {
                    const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;

                 
                    var field = t.GetField(name, flags);
                    if (field?.GetValue(null) is string fs && !string.IsNullOrWhiteSpace(fs))
                        return fs;

                    var prop = t.GetProperty(name, flags);
                    if (prop?.GetValue(null) is string ps && !string.IsNullOrWhiteSpace(ps))
                        return ps;
                }
            }
        }
        return key;
    }
}

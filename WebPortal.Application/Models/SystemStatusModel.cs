using System;

namespace WebPortal.Application.Models;

public sealed record SystemStatusModel(
    DateTime UtcNow,
    TimeSpan Uptime,
    string Version,
    bool DatabaseOk,
    bool RedisOk,
    int Users,
    int Categories,
    int Links,
    int FavoriteLinks
);

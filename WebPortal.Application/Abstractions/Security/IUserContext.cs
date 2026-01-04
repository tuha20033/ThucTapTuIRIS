using System.Collections.Generic;

namespace WebPortal.Application.Abstractions.Security;

/// <summary>
/// Provides the current user identity and roles from the host (Web/Grpc).
/// In production this should read from authentication/claims, not from client input.
/// </summary>
public interface IUserContext
{
    string UserId { get; }
    string? UserName { get; }
    IReadOnlyCollection<string> Roles { get; }
}

using System.Security.Claims;
using WebPortal.Application.Abstractions.Security;

namespace WebPortal.Grpc.Security;
public sealed class GrpcUserContext : IUserContext
{
    private readonly IHttpContextAccessor _http;

    public GrpcUserContext(IHttpContextAccessor http) => _http = http;

    public string UserId
    {
        get
        {
            var ctx = _http.HttpContext;
            var fromClaim = ctx?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? ctx?.User?.FindFirstValue("sub");
            if (!string.IsNullOrWhiteSpace(fromClaim)) return fromClaim;

            var fromHeader = ctx?.Request.Headers["x-user-id"].ToString();
            if (!string.IsNullOrWhiteSpace(fromHeader)) return fromHeader;

            return "dev-user";
        }
    }

    public string? UserName
    {
        get
        {
            var ctx = _http.HttpContext;
            var fromClaim = ctx?.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(fromClaim)) return fromClaim;

            var fromHeader = ctx?.Request.Headers["x-user-name"].ToString();
            if (!string.IsNullOrWhiteSpace(fromHeader)) return fromHeader;

            return UserId;
        }
    }

    public IReadOnlyCollection<string> Roles
    {
        get
        {
            var ctx = _http.HttpContext;
            if (ctx?.User?.Identity?.IsAuthenticated == true)
            {
                var roles = ctx.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
                    .Select(c => c.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .ToList();

                if (roles.Count > 0) return roles;
            }

            var header = ctx?.Request.Headers["x-roles"].ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(header)) return Array.Empty<string>();

            var parts = header.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts;
        }
    }
}

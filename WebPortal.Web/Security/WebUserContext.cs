using System.Security.Claims;
using WebPortal.Application.Abstractions.Security;

namespace WebPortal.Web.Security;

/// <summary>
/// Blazor Server implementation of IUserContext.
/// Production: configure authentication so roles come from ClaimsPrincipal.
/// Development: set DevUser in appsettings.json.
/// </summary>
public sealed class WebUserContext : IUserContext
{
    private readonly IHttpContextAccessor _http;
    private readonly IConfiguration _config;

    public WebUserContext(IHttpContextAccessor http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public string UserId
    {
        get
        {
            var ctx = _http.HttpContext;
            var fromClaim = ctx?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? ctx?.User?.FindFirstValue("sub");

            if (!string.IsNullOrWhiteSpace(fromClaim)) return fromClaim;

            return _config["DevUser:UserId"] ?? "dev-user";
        }
    }

    public string? UserName
    {
        get
        {
            var ctx = _http.HttpContext;
            var fromClaim = ctx?.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(fromClaim)) return fromClaim;

            return _config["DevUser:UserName"] ?? UserId;
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

            // Dev roles
            var rolesCsv = _config["DevUser:Roles"] ?? "";
            if (string.IsNullOrWhiteSpace(rolesCsv)) return Array.Empty<string>();

            return rolesCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }
}

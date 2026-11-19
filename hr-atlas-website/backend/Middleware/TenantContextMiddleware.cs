using System.Security.Claims;
using HrAtlas.SaaS.Services;

namespace HrAtlas.SaaS.Middleware;

public class TenantContextMiddleware
{
    private readonly RequestDelegate _next;

    public TenantContextMiddleware(RequestDelegate _next)
    {
        this._next = _next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirst("TenantId")?.Value;
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;
            var orgSlugClaim = context.User.FindFirst("OrgSlug")?.Value;

            if (!string.IsNullOrEmpty(tenantIdClaim) && !string.IsNullOrEmpty(userIdClaim))
            {
                tenantContext.SetContext(
                    tenantIdClaim,
                    userIdClaim,
                    emailClaim ?? string.Empty,
                    roleClaim ?? string.Empty,
                    orgSlugClaim ?? string.Empty
                );
            }
        }

        await _next(context);
    }
}


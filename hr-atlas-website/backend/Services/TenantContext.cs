namespace HrAtlas.SaaS.Services;

public interface ITenantContext
{
    string? TenantId { get; }
    string? UserId { get; }
    string? UserEmail { get; }
    string? Role { get; }
    string? OrgSlug { get; }
    bool IsAuthenticated { get; }

    void SetContext(string tenantId, string userId, string userEmail, string role, string orgSlug);
}

public class TenantContext : ITenantContext
{
    public string? TenantId { get; private set; }
    public string? UserId { get; private set; }
    public string? UserEmail { get; private set; }
    public string? Role { get; private set; }
    public string? OrgSlug { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(TenantId) && !string.IsNullOrEmpty(UserId);

    public void SetContext(string tenantId, string userId, string userEmail, string role, string orgSlug)
    {
        TenantId = tenantId;
        UserId = userId;
        UserEmail = userEmail;
        Role = role;
        OrgSlug = orgSlug;
    }
}


using HrAtlas.SaaS.Data;
using HrAtlas.SaaS.DTOs;
using HrAtlas.SaaS.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HrAtlas.SaaS.Services;

public interface ITenantService
{
    Task<ApiResponse<TenantRegistrationResponseDto>> RegisterTenantAsync(TenantRegistrationRequestDto request);
    Task<ApiResponse<OrganisationUsageResponseDto>> GetOrganisationUsageAsync(string tenantId);
    Task<ApiResponse<bool>> CheckSlugAvailabilityAsync(string slug);
}

public class TenantService : ITenantService
{
    private readonly MongoDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IOtpService _otpService;
    private readonly SaasSettings _saasSettings;

    public TenantService(
        MongoDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOtpService otpService,
        IOptions<SaasSettings> saasSettings)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _otpService = otpService;
        _saasSettings = saasSettings.Value;
    }

    public async Task<ApiResponse<bool>> CheckSlugAvailabilityAsync(string slug)
    {
        var cleanSlug = slug.Trim().ToLowerInvariant();
        var existing = await _context.Tenants.Find(t => t.Slug == cleanSlug).FirstOrDefaultAsync();
        return ApiResponse<bool>.Ok(existing == null, existing == null ? "Workspace domain is available." : "Workspace domain is already taken.");
    }

    public async Task<ApiResponse<TenantRegistrationResponseDto>> RegisterTenantAsync(TenantRegistrationRequestDto request)
    {
        var normalizedEmail = request.AdminEmail.Trim().ToLowerInvariant();
        var cleanSlug = request.Slug.Trim().ToLowerInvariant();

        // 1. Validate Email OTP
        var isOtpValid = await _otpService.VerifyOtpAsync(normalizedEmail, request.OtpCode, "TenantRegistration");
        if (!isOtpValid)
        {
            return ApiResponse<TenantRegistrationResponseDto>.Fail("Invalid or expired verification code. Please request a new code to complete registration.");
        }

        // 2. Validate uniqueness of slug
        var existingSlug = await _context.Tenants.Find(t => t.Slug == cleanSlug).FirstOrDefaultAsync();
        if (existingSlug != null)
        {
            return ApiResponse<TenantRegistrationResponseDto>.Fail($"The workspace domain '{cleanSlug}.hratlas.com' is already registered. Please choose a different subdomain.");
        }

        // 3. Validate uniqueness of user email
        var existingUser = await _context.Users.Find(u => u.Email == normalizedEmail).FirstOrDefaultAsync();
        if (existingUser != null)
        {
            return ApiResponse<TenantRegistrationResponseDto>.Fail("An account with this work email already exists. Please sign in or use a different email.");
        }

        // 4. Determine plan parameters
        var planId = string.IsNullOrWhiteSpace(request.PlanId) ? "growth" : request.PlanId.ToLowerInvariant();
        int maxSeats = planId switch
        {
            "starter" => 15,
            "enterprise" => 1000,
            _ => 50 // growth
        };

        var planName = planId switch
        {
            "starter" => "Starter Plan",
            "enterprise" => "Enterprise Plan",
            _ => "Growth Plan"
        };

        decimal pricePerMonth = planId switch
        {
            "starter" => 1499,
            "enterprise" => 9999,
            _ => 3999
        };

        // 5. Generate secure unique TenantId
        var tenantId = $"org_{Guid.NewGuid():N}"[..12];

        var trialEnd = DateTime.UtcNow.AddDays(_saasSettings.TrialDays);

        var tenant = new Tenant
        {
            TenantId = tenantId,
            CompanyName = request.CompanyName.Trim(),
            Slug = cleanSlug,
            Industry = request.Industry,
            CompanySize = request.CompanySize,
            Country = request.Country,
            Currency = request.Currency,
            Timezone = request.Timezone,
            Status = "Active",
            Subscription = new TenantSubscription
            {
                PlanId = planId,
                PlanName = planName,
                Status = "Trial",
                BillingCycle = request.BillingCycle,
                TrialEndsAt = trialEnd,
                CurrentPeriodStart = DateTime.UtcNow,
                CurrentPeriodEnd = trialEnd,
                MaxSeats = maxSeats,
                CurrentSeats = 1,
                PricePerMonth = pricePerMonth
            },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Tenants.InsertOneAsync(tenant);

        // 6. Create Primary Admin Account
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var adminUser = new User
        {
            TenantId = tenantId,
            Email = normalizedEmail,
            PasswordHash = passwordHash,
            FullName = request.AdminFullName.Trim(),
            PhoneNumber = request.AdminPhoneNumber.Trim(),
            Role = "TenantAdmin",
            JobTitle = string.IsNullOrWhiteSpace(request.JobTitle) ? "Primary Administrator" : request.JobTitle.Trim(),
            IsEmailVerified = true,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        await _context.Users.InsertOneAsync(adminUser);

        // 7. Generate JWT Session Token
        var token = _tokenService.GenerateJwtToken(adminUser, tenant);

        var workspaceUrl = $"{_saasSettings.DashboardBaseUrl}/workspace/{tenant.Slug}";

        var responseData = new TenantRegistrationResponseDto
        {
            TenantId = tenant.TenantId,
            CompanyName = tenant.CompanyName,
            Slug = tenant.Slug,
            AdminEmail = adminUser.Email,
            WorkspaceUrl = workspaceUrl,
            Token = token,
            Subscription = new TenantSubscriptionDto
            {
                PlanId = tenant.Subscription.PlanId,
                PlanName = tenant.Subscription.PlanName,
                Status = tenant.Subscription.Status,
                MaxSeats = tenant.Subscription.MaxSeats,
                CurrentSeats = tenant.Subscription.CurrentSeats,
                TrialEndsAt = tenant.Subscription.TrialEndsAt,
                CurrentPeriodEnd = tenant.Subscription.CurrentPeriodEnd
            }
        };

        return ApiResponse<TenantRegistrationResponseDto>.Ok(responseData, "Organisation successfully registered and 14-day full access trial activated.");
    }

    public async Task<ApiResponse<OrganisationUsageResponseDto>> GetOrganisationUsageAsync(string tenantId)
    {
        var tenant = await _context.Tenants.Find(t => t.TenantId == tenantId).FirstOrDefaultAsync();
        if (tenant == null)
        {
            return ApiResponse<OrganisationUsageResponseDto>.Fail("Organisation not found.");
        }

        var userCount = (int)await _context.Users.CountDocumentsAsync(u => u.TenantId == tenantId && u.Status == "Active");
        if (userCount == 0) userCount = 1;

        var isTrialActive = tenant.Subscription.Status == "Trial" && tenant.Subscription.TrialEndsAt > DateTime.UtcNow;
        var trialDaysRemaining = isTrialActive
            ? Math.Max(0, (int)Math.Ceiling((tenant.Subscription.TrialEndsAt - DateTime.UtcNow).TotalDays))
            : 0;

        var seatsRemaining = Math.Max(0, tenant.Subscription.MaxSeats - userCount);
        var utilization = Math.Round((double)userCount / tenant.Subscription.MaxSeats * 100, 1);

        var activeModules = new List<string>
        {
            "Employee Lifecycle Management",
            "Attendance & Auto Clock-Out",
            "Leave & Comp-Off Management",
            "Corporate Broadcasts",
            "Real-Time Notifications"
        };

        if (tenant.Subscription.PlanId is "growth" or "enterprise")
        {
            activeModules.AddRange(new[]
            {
                "Timesheets & Project Tracking",
                "Salary & Payroll Engine",
                "Automated PDF Payslips",
                "Inventory & Asset Tracking",
                "Automated Employee Onboarding Workflows"
            });
        }

        var response = new OrganisationUsageResponseDto
        {
            TenantId = tenant.TenantId,
            CompanyName = tenant.CompanyName,
            PlanId = tenant.Subscription.PlanId,
            PlanName = tenant.Subscription.PlanName,
            Status = tenant.Subscription.Status,
            CurrentSeats = userCount,
            MaxSeats = tenant.Subscription.MaxSeats,
            SeatsRemaining = seatsRemaining,
            SeatUtilizationPercent = utilization,
            IsTrialActive = isTrialActive,
            TrialDaysRemaining = trialDaysRemaining,
            SubscriptionPeriodEnd = tenant.Subscription.CurrentPeriodEnd,
            ActiveModules = activeModules
        };

        return ApiResponse<OrganisationUsageResponseDto>.Ok(response);
    }
}


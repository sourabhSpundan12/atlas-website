using System.Security.Cryptography;
using System.Text;
using HrAtlas.SaaS.Data;
using HrAtlas.SaaS.DTOs;
using HrAtlas.SaaS.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HrAtlas.SaaS.Services;

public interface ISubscriptionService
{
    Task<ApiResponse<List<SubscriptionPlanDto>>> GetPlansAsync();
    Task<ApiResponse<CheckoutSessionResponseDto>> CreateCheckoutSessionAsync(string tenantId, CreateCheckoutRequestDto request);
    Task<ApiResponse<bool>> ProcessWebhookAsync(WebhookPayloadDto payload);
}

public class SubscriptionService : ISubscriptionService
{
    private readonly MongoDbContext _context;
    private readonly SaasSettings _saasSettings;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        MongoDbContext context,
        IOptions<SaasSettings> saasSettings,
        ILogger<SubscriptionService> logger)
    {
        _context = context;
        _saasSettings = saasSettings.Value;
        _logger = logger;
    }

    public Task<ApiResponse<List<SubscriptionPlanDto>>> GetPlansAsync()
    {
        var plans = new List<SubscriptionPlanDto>
        {
            new()
            {
                Id = "starter",
                Name = "Starter",
                Tagline = "Ideal for agile startups and emerging teams scaling up their HR operations.",
                MonthlyPriceINR = 1499,
                AnnualPriceINR = 1199,
                MonthlyPriceUSD = 29,
                AnnualPriceUSD = 22,
                MaxSeats = 15,
                IsPopular = false,
                Features = new List<string>
                {
                    "Up to 15 Active Employees",
                    "Employee Lifecycle & Digital Profiles",
                    "Attendance Tracking & Auto Clock-Out",
                    "Leave & Comp-Off Management",
                    "Company Broadcasts & Announcements",
                    "Real-Time Web Notifications",
                    "Standard Email Support (24-48h)"
                },
                HighlightBadges = new List<string> { "Self-Serve", "Quick Setup" }
            },
            new()
            {
                Id = "growth",
                Name = "Growth",
                Tagline = "Complete workforce automation for scaling companies with advanced payroll & tracking.",
                MonthlyPriceINR = 3999,
                AnnualPriceINR = 3199,
                MonthlyPriceUSD = 79,
                AnnualPriceUSD = 62,
                MaxSeats = 50,
                IsPopular = true,
                Features = new List<string>
                {
                    "Up to 50 Active Employees",
                    "Everything in Starter, plus:",
                    "Salary Structuring & Payroll Engine",
                    "One-Click Automated PDF Payslips",
                    "Timesheets & Project Billable Tracking",
                    "Inventory & Hardware Asset Management",
                    "Automated Employee Onboarding Workflows",
                    "Priority Chat & Email Support"
                },
                HighlightBadges = new List<string> { "Most Popular", "Best Value" }
            },
            new()
            {
                Id = "enterprise",
                Name = "Enterprise",
                Tagline = "Tailored governance, dedicated infrastructure, and unlimited scale for enterprises.",
                MonthlyPriceINR = 9999,
                AnnualPriceINR = 7999,
                MonthlyPriceUSD = 199,
                AnnualPriceUSD = 159,
                MaxSeats = 1000,
                IsPopular = false,
                Features = new List<string>
                {
                    "Custom & Unlimited Employee Seats",
                    "Everything in Growth, plus:",
                    "Multi-Branch & Custom Policy Engines",
                    "Dedicated Account Manager & 99.9% SLA",
                    "Custom Role-Based Access Control (RBAC)",
                    "Audit Logs & Compliance Reporting",
                    "REST API & Custom Webhook Integrations",
                    "Assisted Data Migration & Training"
                },
                HighlightBadges = new List<string> { "Unlimited Scale", "Custom SLA" }
            }
        };

        return Task.FromResult(ApiResponse<List<SubscriptionPlanDto>>.Ok(plans));
    }

    public async Task<ApiResponse<CheckoutSessionResponseDto>> CreateCheckoutSessionAsync(string tenantId, CreateCheckoutRequestDto request)
    {
        var tenant = await _context.Tenants.Find(t => t.TenantId == tenantId).FirstOrDefaultAsync();
        if (tenant == null)
        {
            return ApiResponse<CheckoutSessionResponseDto>.Fail("Organisation not found.");
        }

        var plansResponse = await GetPlansAsync();
        var selectedPlan = plansResponse.Data?.FirstOrDefault(p => p.Id.Equals(request.PlanId, StringComparison.OrdinalIgnoreCase));
        if (selectedPlan == null)
        {
            return ApiResponse<CheckoutSessionResponseDto>.Fail("Invalid subscription plan requested.");
        }

        bool isAnnual = request.BillingCycle.Equals("Annual", StringComparison.OrdinalIgnoreCase);
        decimal amount = isAnnual ? selectedPlan.AnnualPriceINR * 12 : selectedPlan.MonthlyPriceINR;

        var sessionId = $"cs_{Guid.NewGuid():N}";
        var checkoutUrl = $"https://checkout.hratlas.com/pay/{sessionId}?tenant={tenant.Slug}&plan={selectedPlan.Id}&cycle={request.BillingCycle}";

        var response = new CheckoutSessionResponseDto
        {
            SessionId = sessionId,
            CheckoutUrl = checkoutUrl,
            PlanId = selectedPlan.Id,
            Amount = amount,
            Currency = "INR"
        };

        return ApiResponse<CheckoutSessionResponseDto>.Ok(response, "Checkout session successfully created.");
    }

    public async Task<ApiResponse<bool>> ProcessWebhookAsync(WebhookPayloadDto payload)
    {
        // 1. Verify HMAC-SHA256 Signature
        if (!VerifyWebhookSignature(payload))
        {
            _logger.LogWarning("Webhook signature verification failed for TenantId: {TenantId}", payload.TenantId);
            return ApiResponse<bool>.Fail("Invalid webhook cryptographic signature.");
        }

        var tenant = await _context.Tenants.Find(t => t.TenantId == payload.TenantId).FirstOrDefaultAsync();
        if (tenant == null)
        {
            return ApiResponse<bool>.Fail("Tenant not found.");
        }

        switch (payload.EventType.ToLowerInvariant())
        {
            case "payment.succeeded":
            case "subscription.activated":
                int seats = payload.PlanId.ToLowerInvariant() switch
                {
                    "starter" => 15,
                    "enterprise" => 1000,
                    _ => 50
                };
                string planName = payload.PlanId.ToLowerInvariant() switch
                {
                    "starter" => "Starter Plan",
                    "enterprise" => "Enterprise Plan",
                    _ => "Growth Plan"
                };

                var now = DateTime.UtcNow;
                var periodEnd = payload.BillingCycle.Equals("Annual", StringComparison.OrdinalIgnoreCase)
                    ? now.AddYears(1)
                    : now.AddMonths(1);

                var updateSub = Builders<Tenant>.Update
                    .Set(t => t.Subscription.Status, "Active")
                    .Set(t => t.Subscription.PlanId, payload.PlanId.ToLowerInvariant())
                    .Set(t => t.Subscription.PlanName, planName)
                    .Set(t => t.Subscription.MaxSeats, seats)
                    .Set(t => t.Subscription.CurrentPeriodStart, now)
                    .Set(t => t.Subscription.CurrentPeriodEnd, periodEnd)
                    .Set(t => t.Subscription.BillingCycle, payload.BillingCycle)
                    .Set(t => t.UpdatedAt, now);

                await _context.Tenants.UpdateOneAsync(t => t.TenantId == payload.TenantId, updateSub);
                _logger.LogInformation("Subscription successfully activated for Tenant [{TenantId}] on plan [{PlanId}]", payload.TenantId, payload.PlanId);
                break;

            case "subscription.cancelled":
                var cancelUpdate = Builders<Tenant>.Update
                    .Set(t => t.Subscription.Status, "Cancelled")
                    .Set(t => t.UpdatedAt, DateTime.UtcNow);
                await _context.Tenants.UpdateOneAsync(t => t.TenantId == payload.TenantId, cancelUpdate);
                _logger.LogInformation("Subscription cancelled for Tenant [{TenantId}]", payload.TenantId);
                break;

            default:
                _logger.LogInformation("Unhandled webhook event type [{EventType}] for Tenant [{TenantId}]", payload.EventType, payload.TenantId);
                break;
        }

        return ApiResponse<bool>.Ok(true, "Webhook processed successfully.");
    }

    private bool VerifyWebhookSignature(WebhookPayloadDto payload)
    {
        if (string.IsNullOrEmpty(payload.Signature) || string.IsNullOrEmpty(_saasSettings.WebhookSecret))
            return false;

        var payloadData = $"{payload.EventType}|{payload.TenantId}|{payload.PlanId}|{payload.Timestamp}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_saasSettings.WebhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadData));
        var computedSignature = Convert.ToHexString(hash).ToLowerInvariant();

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(payload.Signature.ToLowerInvariant())
        );
    }
}


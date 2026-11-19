using System.ComponentModel.DataAnnotations;

namespace HrAtlas.SaaS.DTOs;

public class TenantRegistrationRequestDto
{
    // Company Profile
    [Required, StringLength(100, MinimumLength = 2)]
    public string CompanyName { get; set; } = string.Empty;

    [Required, RegularExpression("^[a-z0-9-]+$", ErrorMessage = "Workspace slug must be lowercase alphanumeric with hyphens.")]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string Industry { get; set; } = string.Empty;

    [Required]
    public string CompanySize { get; set; } = string.Empty;

    public string Country { get; set; } = "India";
    public string Currency { get; set; } = "INR";
    public string Timezone { get; set; } = "Asia/Kolkata";

    // Primary Admin
    [Required, StringLength(100, MinimumLength = 2)]
    public string AdminFullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string AdminEmail { get; set; } = string.Empty;

    [Required, Phone]
    public string AdminPhoneNumber { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string JobTitle { get; set; } = "Primary Administrator";

    // Verification & Subscription
    [Required, StringLength(6, MinimumLength = 6)]
    public string OtpCode { get; set; } = string.Empty;

    public string PlanId { get; set; } = "growth"; // starter, growth, enterprise
    public string BillingCycle { get; set; } = "Monthly"; // Monthly, Annual
}

public class TenantRegistrationResponseDto
{
    public string TenantId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string WorkspaceUrl { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public TenantSubscriptionDto Subscription { get; set; } = new();
}

public class TenantSubscriptionDto
{
    public string PlanId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int MaxSeats { get; set; }
    public int CurrentSeats { get; set; }
    public DateTime TrialEndsAt { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
}

public class ContactInquiryRequestDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string CompanySize { get; set; } = string.Empty;

    public List<string> ModulesOfInterest { get; set; } = new();

    public string Message { get; set; } = string.Empty;

    public DateTime? PreferredDate { get; set; }

    public string Type { get; set; } = "ContactEnquiry"; // DemoRequest, ContactEnquiry
}

public class SubscriptionPlanDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public decimal MonthlyPriceINR { get; set; }
    public decimal AnnualPriceINR { get; set; }
    public decimal MonthlyPriceUSD { get; set; }
    public decimal AnnualPriceUSD { get; set; }
    public int MaxSeats { get; set; }
    public bool IsPopular { get; set; }
    public List<string> Features { get; set; } = new();
    public List<string> HighlightBadges { get; set; } = new();
}

public class CreateCheckoutRequestDto
{
    [Required]
    public string PlanId { get; set; } = "growth";

    public string BillingCycle { get; set; } = "Monthly"; // Monthly, Annual

    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}

public class CheckoutSessionResponseDto
{
    public string SessionId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
}

public class WebhookPayloadDto
{
    public string EventType { get; set; } = string.Empty; // payment.succeeded, subscription.renewed, subscription.cancelled
    public string TenantId { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public string BillingCycle { get; set; } = "Monthly";
    public string Signature { get; set; } = string.Empty;
    public long Timestamp { get; set; }
}

public class OrganisationUsageResponseDto
{
    public string TenantId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int CurrentSeats { get; set; }
    public int MaxSeats { get; set; }
    public int SeatsRemaining { get; set; }
    public double SeatUtilizationPercent { get; set; }
    public bool IsTrialActive { get; set; }
    public int TrialDaysRemaining { get; set; }
    public DateTime SubscriptionPeriodEnd { get; set; }
    public List<string> ActiveModules { get; set; } = new();
}


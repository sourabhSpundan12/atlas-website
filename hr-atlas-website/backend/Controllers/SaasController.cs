using HrAtlas.SaaS.DTOs;
using HrAtlas.SaaS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrAtlas.SaaS.Controllers;

[ApiController]
[Route("api/saas")]
public class SaasController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IInquiryService _inquiryService;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<SaasController> _logger;

    public SaasController(
        ITenantService tenantService,
        ISubscriptionService subscriptionService,
        IInquiryService inquiryService,
        ITenantContext tenantContext,
        ILogger<SaasController> logger)
    {
        _tenantService = tenantService;
        _subscriptionService = subscriptionService;
        _inquiryService = inquiryService;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new SaaS organisation, provisions workspace, creates primary admin, and activates 14-day trial.
    /// </summary>
    [HttpPost("register-tenant")]
    [ProducesResponseType(typeof(ApiResponse<TenantRegistrationResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TenantRegistrationResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterTenant([FromBody] TenantRegistrationRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<TenantRegistrationResponseDto>.Fail("Validation failed. Please verify form values."));

        var result = await _tenantService.RegisterTenantAsync(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Checks real-time availability of a company workspace subdomain.
    /// </summary>
    [HttpGet("check-slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckSlug(string slug)
    {
        var result = await _tenantService.CheckSlugAvailabilityAsync(slug);
        return Ok(result);
    }

    /// <summary>
    /// Captures product demo bookings and contact inquiries.
    /// </summary>
    [HttpPost("contact-inquiry")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ContactInquiry([FromBody] ContactInquiryRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<bool>.Fail("Validation failed. Please verify enquiry details."));

        var result = await _inquiryService.SubmitInquiryAsync(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all public SaaS subscription plans and feature comparisons.
    /// </summary>
    [HttpGet("subscription/plans")]
    [ProducesResponseType(typeof(ApiResponse<List<SubscriptionPlanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans()
    {
        var result = await _subscriptionService.GetPlansAsync();
        return Ok(result);
    }

    /// <summary>
    /// Creates a secure payment checkout session for upgrading or renewing a subscription.
    /// Tenant identity is extracted securely from the validated JWT authentication context.
    /// </summary>
    [Authorize]
    [HttpPost("subscription/create-checkout")]
    [ProducesResponseType(typeof(ApiResponse<CheckoutSessionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CheckoutSessionResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateCheckout([FromBody] CreateCheckoutRequestDto request)
    {
        if (string.IsNullOrEmpty(_tenantContext.TenantId))
            return Unauthorized(ApiResponse<CheckoutSessionResponseDto>.Fail("Unauthorized tenant context."));

        var result = await _subscriptionService.CreateCheckoutSessionAsync(_tenantContext.TenantId, request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Secure payment gateway webhook endpoint with HMAC-SHA256 signature verification.
    /// </summary>
    [HttpPost("subscription/webhook")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubscriptionWebhook([FromBody] WebhookPayloadDto payload)
    {
        var result = await _subscriptionService.ProcessWebhookAsync(payload);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves live seat utilization, license limits, and active modules for the authenticated organisation.
    /// Tenant identity is resolved solely from cryptographically validated token context.
    /// </summary>
    [Authorize]
    [HttpGet("organisation/usage")]
    [ProducesResponseType(typeof(ApiResponse<OrganisationUsageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OrganisationUsageResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrganisationUsage()
    {
        if (string.IsNullOrEmpty(_tenantContext.TenantId))
            return Unauthorized(ApiResponse<OrganisationUsageResponseDto>.Fail("Valid tenant credentials required."));

        var result = await _tenantService.GetOrganisationUsageAsync(_tenantContext.TenantId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}


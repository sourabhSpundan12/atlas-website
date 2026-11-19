using HrAtlas.SaaS.DTOs;
using HrAtlas.SaaS.Services;
using Microsoft.AspNetCore.Mvc;

namespace HrAtlas.SaaS.Controllers;

[ApiController]
[Route("api")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IOtpService _otpService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IOtpService otpService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _otpService = otpService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates an organisation user and issues a tenant-scoped JWT token.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<LoginResponseDto>.Fail("Invalid credentials payload."));

        var result = await _authService.LoginAsync(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Dispatches a cryptographic 6-digit OTP to the specified work email address.
    /// </summary>
    [HttpPost("user/sendOtp")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid email address."));

        var code = await _otpService.GenerateAndSendOtpAsync(request.Email, request.Purpose);
        
        // Return confirmation; in non-production, return development preview code if desired
        return Ok(ApiResponse<string>.Ok(
            code,
            $"A 6-digit verification code has been dispatched to {request.Email}. Valid for 10 minutes."
        ));
    }

    /// <summary>
    /// Verifies the 6-digit OTP code against the registered hash and expiration.
    /// </summary>
    [HttpPost("user/verifyOtp")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<bool>.Fail("Invalid OTP payload."));

        var isValid = await _otpService.VerifyOtpAsync(request.Email, request.Otp, request.Purpose);
        if (!isValid)
            return BadRequest(ApiResponse<bool>.Fail("Invalid or expired verification code."));

        return Ok(ApiResponse<bool>.Ok(true, "Email verification successful."));
    }

    /// <summary>
    /// Resets an account password after verified OTP confirmation.
    /// </summary>
    [HttpPut("user/forgotPassword")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<bool>.Fail("Invalid password reset payload."));

        var result = await _authService.ForgotPasswordAsync(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}


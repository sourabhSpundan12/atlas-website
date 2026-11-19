using HrAtlas.SaaS.Data;
using HrAtlas.SaaS.DTOs;
using HrAtlas.SaaS.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HrAtlas.SaaS.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequestDto request);
}

public class AuthService : IAuthService
{
    private readonly MongoDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IOtpService _otpService;
    private readonly SaasSettings _saasSettings;

    public AuthService(
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

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .Find(u => u.Email == normalizedEmail && u.Status == "Active")
            .FirstOrDefaultAsync();

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ApiResponse<LoginResponseDto>.Fail("Invalid work email or password. Please verify your credentials.");
        }

        var tenant = await _context.Tenants
            .Find(t => t.TenantId == user.TenantId)
            .FirstOrDefaultAsync();

        if (tenant == null || tenant.Status != "Active")
        {
            return ApiResponse<LoginResponseDto>.Fail("Your organisation account is currently inactive or suspended. Please contact your administrator.");
        }

        // Update last login timestamp
        var updateLastLogin = Builders<User>.Update.Set(u => u.LastLoginAt, DateTime.UtcNow);
        await _context.Users.UpdateOneAsync(u => u.Id == user.Id, updateLastLogin);

        var token = _tokenService.GenerateJwtToken(user, tenant);

        var responseData = new LoginResponseDto
        {
            Token = token,
            ExpiresIn = 86400,
            User = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                JobTitle = user.JobTitle
            },
            Tenant = new TenantProfileDto
            {
                TenantId = tenant.TenantId,
                CompanyName = tenant.CompanyName,
                Slug = tenant.Slug,
                PlanId = tenant.Subscription.PlanId,
                SubscriptionStatus = tenant.Subscription.Status,
                WorkspaceUrl = $"{_saasSettings.DashboardBaseUrl}/workspace/{tenant.Slug}"
            }
        };

        return ApiResponse<LoginResponseDto>.Ok(responseData, "Login successful. Welcome back to HR Atlas.");
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var isValidOtp = await _otpService.VerifyOtpAsync(normalizedEmail, request.Otp, "PasswordReset");
        if (!isValidOtp)
        {
            return ApiResponse<bool>.Fail("Invalid or expired verification code. Please request a new code.");
        }

        var user = await _context.Users
            .Find(u => u.Email == normalizedEmail)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return ApiResponse<bool>.Fail("No registered account found with the specified work email.");
        }

        var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        var update = Builders<User>.Update.Set(u => u.PasswordHash, newPasswordHash);
        await _context.Users.UpdateOneAsync(u => u.Id == user.Id, update);

        return ApiResponse<bool>.Ok(true, "Password has been successfully updated. You may now sign in.");
    }
}


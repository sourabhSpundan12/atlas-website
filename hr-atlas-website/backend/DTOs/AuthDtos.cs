using System.ComponentModel.DataAnnotations;

namespace HrAtlas.SaaS.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? new List<string> { message } };
}

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserProfileDto User { get; set; } = new();
    public TenantProfileDto Tenant { get; set; } = new();
}

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
}

public class TenantProfileDto
{
    public string TenantId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public string SubscriptionStatus { get; set; } = string.Empty;
    public string WorkspaceUrl { get; set; } = string.Empty;
}

public class SendOtpRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Purpose { get; set; } = "TenantRegistration"; // TenantRegistration, PasswordReset
}

public class VerifyOtpRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(6, MinimumLength = 6)]
    public string Otp { get; set; } = string.Empty;

    public string Purpose { get; set; } = "TenantRegistration";
}

public class ForgotPasswordRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(6, MinimumLength = 6)]
    public string Otp { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}


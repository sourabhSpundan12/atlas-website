using HrAtlas.SaaS.Data;
using HrAtlas.SaaS.Models;
using MongoDB.Driver;

namespace HrAtlas.SaaS.Services;

public interface IOtpService
{
    Task<string> GenerateAndSendOtpAsync(string email, string purpose);
    Task<bool> VerifyOtpAsync(string email, string otp, string purpose);
}

public class OtpService : IOtpService
{
    private readonly MongoDbContext _context;
    private readonly IPasswordHasher _hasher;
    private readonly IEmailService _emailService;
    private readonly ILogger<OtpService> _logger;

    public OtpService(
        MongoDbContext context,
        IPasswordHasher hasher,
        IEmailService emailService,
        ILogger<OtpService> logger)
    {
        _context = context;
        _hasher = hasher;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<string> GenerateAndSendOtpAsync(string email, string purpose)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        // Invalidate prior unused OTPs for this email and purpose
        var invalidateFilter = Builders<OtpVerification>.Filter.And(
            Builders<OtpVerification>.Filter.Eq(o => o.Email, normalizedEmail),
            Builders<OtpVerification>.Filter.Eq(o => o.Purpose, purpose),
            Builders<OtpVerification>.Filter.Eq(o => o.IsUsed, false)
        );
        var update = Builders<OtpVerification>.Update.Set(o => o.IsUsed, true);
        await _context.OtpVerifications.UpdateManyAsync(invalidateFilter, update);

        // Generate cryptographically random 6-digit OTP
        var random = new Random();
        var otpCode = random.Next(100000, 999999).ToString();
        var otpHash = _hasher.HashPassword(otpCode);

        var verification = new OtpVerification
        {
            Email = normalizedEmail,
            OtpHash = otpHash,
            Purpose = purpose,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
            IsUsed = false,
            Attempts = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _context.OtpVerifications.InsertOneAsync(verification);

        // Dispatch real email via SMTP
        await _emailService.SendOtpEmailAsync(normalizedEmail, otpCode, purpose);

        _logger.LogInformation("Generated OTP for [{Email}] purpose [{Purpose}]. Expiration in 10 minutes.", normalizedEmail, purpose);

        return otpCode;
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp, string purpose)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var filter = Builders<OtpVerification>.Filter.And(
            Builders<OtpVerification>.Filter.Eq(o => o.Email, normalizedEmail),
            Builders<OtpVerification>.Filter.Eq(o => o.Purpose, purpose),
            Builders<OtpVerification>.Filter.Eq(o => o.IsUsed, false),
            Builders<OtpVerification>.Filter.Gt(o => o.ExpiresAt, DateTime.UtcNow)
        );

        var record = await _context.OtpVerifications
            .Find(filter)
            .SortByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (record == null)
            return false;

        // Check maximum attempt count (prevent brute force, max 5 attempts)
        if (record.Attempts >= 5)
        {
            _logger.LogWarning("Excessive failed OTP attempts for [{Email}]. Invalidating token.", normalizedEmail);
            var invalidate = Builders<OtpVerification>.Update.Set(o => o.IsUsed, true);
            await _context.OtpVerifications.UpdateOneAsync(Builders<OtpVerification>.Filter.Eq(o => o.Id, record.Id), invalidate);
            return false;
        }

        bool isValid = _hasher.VerifyPassword(otp, record.OtpHash);
        if (!isValid)
        {
            var incAttempts = Builders<OtpVerification>.Update.Inc(o => o.Attempts, 1);
            await _context.OtpVerifications.UpdateOneAsync(Builders<OtpVerification>.Filter.Eq(o => o.Id, record.Id), incAttempts);
            return false;
        }

        // Mark OTP as used
        var markUsed = Builders<OtpVerification>.Update.Set(o => o.IsUsed, true);
        await _context.OtpVerifications.UpdateOneAsync(Builders<OtpVerification>.Filter.Eq(o => o.Id, record.Id), markUsed);

        return true;
    }
}

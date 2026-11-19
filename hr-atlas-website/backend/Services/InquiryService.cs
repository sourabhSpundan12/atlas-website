using HrAtlas.SaaS.Data;
using HrAtlas.SaaS.DTOs;
using HrAtlas.SaaS.Models;

namespace HrAtlas.SaaS.Services;

public interface IInquiryService
{
    Task<ApiResponse<bool>> SubmitInquiryAsync(ContactInquiryRequestDto request);
}

public class InquiryService : IInquiryService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<InquiryService> _logger;

    public InquiryService(MongoDbContext context, ILogger<InquiryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> SubmitInquiryAsync(ContactInquiryRequestDto request)
    {
        var inquiry = new ContactInquiry
        {
            Type = request.Type,
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber.Trim(),
            CompanyName = request.CompanyName.Trim(),
            CompanySize = request.CompanySize,
            ModulesOfInterest = request.ModulesOfInterest ?? new List<string>(),
            Message = request.Message?.Trim() ?? string.Empty,
            PreferredDate = request.PreferredDate,
            Status = "New",
            CreatedAt = DateTime.UtcNow
        };

        await _context.ContactInquiries.InsertOneAsync(inquiry);

        _logger.LogInformation("New {Type} received from {Email} ({Company}). Modules: {Modules}",
            inquiry.Type, inquiry.Email, inquiry.CompanyName, string.Join(", ", inquiry.ModulesOfInterest));

        string userMessage = inquiry.Type == "DemoRequest"
            ? "Your tailored demo request has been received! Our product specialist will confirm your session shortly."
            : "Thank you for contacting HR Atlas. Our enterprise team will respond within 2-4 business hours.";

        return ApiResponse<bool>.Ok(true, userMessage);
    }
}


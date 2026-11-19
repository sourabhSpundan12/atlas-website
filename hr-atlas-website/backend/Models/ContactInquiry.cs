using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HrAtlas.SaaS.Models;

public class ContactInquiry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("type")]
    public string Type { get; set; } = "DemoRequest"; // DemoRequest, ContactEnquiry

    [BsonElement("fullName")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BsonElement("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [BsonElement("companySize")]
    public string CompanySize { get; set; } = string.Empty;

    [BsonElement("modulesOfInterest")]
    public List<string> ModulesOfInterest { get; set; } = new();

    [BsonElement("message")]
    public string Message { get; set; } = string.Empty;

    [BsonElement("preferredDate")]
    public DateTime? PreferredDate { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "New"; // New, Contacted, Qualified, Closed

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


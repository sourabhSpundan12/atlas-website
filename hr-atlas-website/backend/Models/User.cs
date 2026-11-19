using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HrAtlas.SaaS.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("tenantId")]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("fullName")]
    public string FullName { get; set; } = string.Empty;

    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [BsonElement("role")]
    public string Role { get; set; } = "TenantAdmin"; // TenantAdmin, HRManager, Employee

    [BsonElement("jobTitle")]
    public string JobTitle { get; set; } = "Founder / Admin";

    [BsonElement("isEmailVerified")]
    public bool IsEmailVerified { get; set; } = false;

    [BsonElement("status")]
    public string Status { get; set; } = "Active";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("lastLoginAt")]
    public DateTime? LastLoginAt { get; set; }
}


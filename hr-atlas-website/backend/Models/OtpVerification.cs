using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HrAtlas.SaaS.Models;

public class OtpVerification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("otpHash")]
    public string OtpHash { get; set; } = string.Empty;

    [BsonElement("purpose")]
    public string Purpose { get; set; } = "TenantRegistration"; // TenantRegistration, PasswordReset

    [BsonElement("expiresAt")]
    public DateTime ExpiresAt { get; set; }

    [BsonElement("isUsed")]
    public bool IsUsed { get; set; } = false;

    [BsonElement("attempts")]
    public int Attempts { get; set; } = 0;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


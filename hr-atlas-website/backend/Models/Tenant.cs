using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HrAtlas.SaaS.Models;

public class Tenant
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("tenantId")]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("companyName")]
    public string CompanyName { get; set; } = string.Empty;

    [BsonElement("slug")]
    public string Slug { get; set; } = string.Empty;

    [BsonElement("industry")]
    public string Industry { get; set; } = string.Empty;

    [BsonElement("companySize")]
    public string CompanySize { get; set; } = string.Empty;

    [BsonElement("country")]
    public string Country { get; set; } = "India";

    [BsonElement("currency")]
    public string Currency { get; set; } = "INR";

    [BsonElement("timezone")]
    public string Timezone { get; set; } = "Asia/Kolkata";

    [BsonElement("status")]
    public string Status { get; set; } = "Active"; // Active, Suspended, Inactive

    [BsonElement("subscription")]
    public TenantSubscription Subscription { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class TenantSubscription
{
    [BsonElement("planId")]
    public string PlanId { get; set; } = "growth"; // starter, growth, enterprise

    [BsonElement("planName")]
    public string PlanName { get; set; } = "Growth Plan";

    [BsonElement("status")]
    public string Status { get; set; } = "Trial"; // Trial, Active, PastDue, Cancelled

    [BsonElement("billingCycle")]
    public string BillingCycle { get; set; } = "Monthly"; // Monthly, Annual

    [BsonElement("trialEndsAt")]
    public DateTime TrialEndsAt { get; set; }

    [BsonElement("currentPeriodStart")]
    public DateTime CurrentPeriodStart { get; set; } = DateTime.UtcNow;

    [BsonElement("currentPeriodEnd")]
    public DateTime CurrentPeriodEnd { get; set; }

    [BsonElement("maxSeats")]
    public int MaxSeats { get; set; } = 50;

    [BsonElement("currentSeats")]
    public int CurrentSeats { get; set; } = 1;

    [BsonElement("pricePerMonth")]
    public decimal PricePerMonth { get; set; } = 3999;
}


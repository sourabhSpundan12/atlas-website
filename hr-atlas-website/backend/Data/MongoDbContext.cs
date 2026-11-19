using HrAtlas.SaaS.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HrAtlas.SaaS.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<DatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
        InitializeIndexes();
    }

    public IMongoCollection<Tenant> Tenants => _database.GetCollection<Tenant>("Tenants");
    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    public IMongoCollection<OtpVerification> OtpVerifications => _database.GetCollection<OtpVerification>("OtpVerifications");
    public IMongoCollection<ContactInquiry> ContactInquiries => _database.GetCollection<ContactInquiry>("ContactInquiries");

    private void InitializeIndexes()
    {
        try
        {
            // Unique TenantId and Slug for Tenants
            var tenantIndexKeys = Builders<Tenant>.IndexKeys.Ascending(t => t.TenantId);
            var tenantIndexOptions = new CreateIndexOptions { Unique = true };
            Tenants.Indexes.CreateOne(new CreateIndexModel<Tenant>(tenantIndexKeys, tenantIndexOptions));

            var slugIndexKeys = Builders<Tenant>.IndexKeys.Ascending(t => t.Slug);
            Tenants.Indexes.CreateOne(new CreateIndexModel<Tenant>(slugIndexKeys, tenantIndexOptions));

            // Compound index for Users: TenantId + Email
            var userIndexKeys = Builders<User>.IndexKeys.Ascending(u => u.TenantId).Ascending(u => u.Email);
            Users.Indexes.CreateOne(new CreateIndexModel<User>(userIndexKeys, tenantIndexOptions));

            // TTL index for OTP expiration
            var otpTtlKeys = Builders<OtpVerification>.IndexKeys.Ascending(o => o.ExpiresAt);
            var otpTtlOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
            OtpVerifications.Indexes.CreateOne(new CreateIndexModel<OtpVerification>(otpTtlKeys, otpTtlOptions));
        }
        catch
        {
            // Indexes already exist or server starting up
        }
    }
}


namespace HrAtlas.SaaS.Data;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "HrAtlas_SaaS_Db";
}

public class JwtSettings
{
    public string Secret { get; set; } = "HR_ATLAS_SUPER_SECRET_KEY_FOR_JWT_ENCRYPTION_AND_SIGNING_2026_PRODUCTION_GRADE!";
    public string Issuer { get; set; } = "HrAtlas.SaaS";
    public string Audience { get; set; } = "HrAtlas.Clients";
    public int ExpirationHours { get; set; } = 24;
}

public class SaasSettings
{
    public int TrialDays { get; set; } = 14;
    public string DefaultCurrency { get; set; } = "INR";
    public string WebhookSecret { get; set; } = "whsec_hr_atlas_prod_signature_key_2026";
    public string DashboardBaseUrl { get; set; } = "https://app.hratlas.com";
}

public class SmtpSettings
{
    public string Host { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string SenderEmail { get; set; } = "";
    public string SenderName { get; set; } = "HR Atlas";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}

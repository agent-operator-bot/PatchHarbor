using PatchHarbor.Web;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<ReportStore>();
builder.Services.AddSingleton<CommunitySettingsStore>();
builder.Services.AddSingleton<AccessPolicy>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("public-submit", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();
app.UseRateLimiter();
app.MapGet("/", () => Results.Content("PatchHarbor is running. Use /api/reports to submit or triage vulnerability disclosures.", "text/plain"));
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "patchharbor" }));

app.Run();

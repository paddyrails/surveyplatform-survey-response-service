using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using SurveyPlatform.SurveyResponseService.Api.Middleware;
using SurveyPlatform.SurveyResponseService.Application;
using SurveyPlatform.SurveyResponseService.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(new CompactJsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var identityProvider = builder.Configuration["IdentityProvider:Provider"] ?? "Keycloak";

if (identityProvider == "Keycloak")
{
    var authority = builder.Configuration["IdentityProvider:Keycloak:Authority"];
    var audience = builder.Configuration["IdentityProvider:Keycloak:Audience"] ?? "account";
    var requireHttps = builder.Configuration.GetValue<bool>("IdentityProvider:Keycloak:RequireHttpsMetadata", false);
    
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = authority;
            options.RequireHttpsMetadata = requireHttps;
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = true,
                ValidAudiences = new[] { audience, "account", "surveyplatform-api", "surveyplatform-customer-portal" },
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
            
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                    if (claimsIdentity != null)
                    {
                        var realmAccessClaim = context.Principal?.FindFirst("realm_access");
                        if (realmAccessClaim != null)
                        {
                            try
                            {
                                var realmAccess = System.Text.Json.JsonDocument.Parse(realmAccessClaim.Value);
                                if (realmAccess.RootElement.TryGetProperty("roles", out var roles))
                                {
                                    foreach (var role in roles.EnumerateArray())
                                    {
                                        claimsIdentity.AddClaim(new System.Security.Claims.Claim(
                                            System.Security.Claims.ClaimTypes.Role, 
                                            role.GetString() ?? ""));
                                    }
                                }
                            }
                            catch { }
                        }
                        
                        var resourceAccessClaim = context.Principal?.FindFirst("resource_access");
                        if (resourceAccessClaim != null)
                        {
                            try
                            {
                                var resourceAccess = System.Text.Json.JsonDocument.Parse(resourceAccessClaim.Value);
                                foreach (var client in resourceAccess.RootElement.EnumerateObject())
                                {
                                    if (client.Value.TryGetProperty("roles", out var roles))
                                    {
                                        foreach (var role in roles.EnumerateArray())
                                        {
                                            claimsIdentity.AddClaim(new System.Security.Claims.Claim(
                                                System.Security.Claims.ClaimTypes.Role, 
                                                role.GetString() ?? ""));
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Log.Warning("Authentication failed: {Error}", context.Exception.Message);
                    return Task.CompletedTask;
                }
            };
        });
}
else
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"{builder.Configuration["IdentityProvider:AzureAd:Instance"]}{builder.Configuration["IdentityProvider:AzureAd:TenantId"]}";
            options.Audience = builder.Configuration["IdentityProvider:AzureAd:Audience"];
        });
}

builder.Services.AddAuthorization();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddApiVersioning(o => {
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
}).AddApiExplorer(o => { o.GroupNameFormat = "'v'VVV"; o.SubstituteApiVersionInUrl = true; });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => {
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Survey Response Service", Version = "v1" });
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header, 
        Type = SecuritySchemeType.ApiKey, 
        Scheme = "Bearer"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "postgresql");

builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment()) 
{ 
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/health/live", () => Results.Ok(new { status = "Live" }));
app.MapGet("/health/ready", () => Results.Ok(new { status = "Ready" }));

Log.Information("Starting Survey Response Service");
app.Run();

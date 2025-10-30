using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SkillBridge.Application.Interfaces;
using SkillBridge.Application.Services;
using SkillBridge.Core.Security;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Email;
using SkillBridge.Infrastructure.Security;
using SkillBridge.Notifications.Hubs;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SkillBridge API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddDbContext<SkillBridgeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=skillbridge.db"));

builder.Services.AddOptions<PasswordHashOptions>()
    .Bind(builder.Configuration.GetSection("PasswordHash"))
    .Validate(o => o.Iterations > 0 && o.SaltSize >= 16 && !string.IsNullOrWhiteSpace(o.Pepper),
              "Invalid password hash options");

builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations()
    .Validate(o => !string.IsNullOrWhiteSpace(o.Key) && o.Key.Length >= 32,
              "Jwt:Key must be at least 32 characters.");

builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<PasswordCredentialService>();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddSignalR();

var jwtOpt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new()
        {
            ValidIssuer = jwtOpt.Issuer,
            ValidAudience = jwtOpt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(jwtOpt.ClockSkewMinutes),
            NameClaimType = JwtRegisteredClaimNames.UniqueName,
            RoleClaimType = ClaimTypes.Role
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hub/notifications");

app.Run();

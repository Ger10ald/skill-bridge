using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SkillBridge.Application.Interfaces;
using SkillBridge.Data;
using SkillBridge.Infrastructure.Email;
using SkillBridge.Infrastructure.Security;
using SkillBridge.Notifications.Hubs;
using SkillBridge.Core.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SkillBridge API",
        Version = "v1",
        Description = "API for connection users to share and request skills",
    });
});

// Wire up DbContext
builder.Services.AddDbContext<SkillBridgeDbContext>(options =>
    options.UseSqlite("Data Source=skillbridge.db"));

builder.Services.AddOptions<PasswordHashOptions>()
    .Bind(builder.Configuration.GetSection("PasswordHash"))
    .Validate(o => o.Iterations > 0 && o.SaltSize >= 16 && !string.IsNullOrWhiteSpace(o.Pepper),
              "Invalid password hash options");

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddSignalR();


var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hub/notifications");

app.Run();

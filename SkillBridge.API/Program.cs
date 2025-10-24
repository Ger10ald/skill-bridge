using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Data;
using SkillBridge.Application.Interfaces;
using SkillBridge.Infrastructure.Email;
using SkillBridge.Notifications.Hubs;

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

builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddSignalR();


var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillBridge API v1");
//        options.RoutePrefix = string.Empty;
//    });
//}

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

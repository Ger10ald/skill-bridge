using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SkillBridge.Data
{
    public sealed class SkillBridgeDbContextFactory : IDesignTimeDbContextFactory<SkillBridgeDbContext>
    {
        public SkillBridgeDbContext CreateDbContext(string[] args)
        {
        
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "SkillBridge.API"));
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

         
            var conn = config.GetConnectionString("Default")
                       ?? "Data Source=skillbridge-dev.db";   

            var options = new DbContextOptionsBuilder<SkillBridgeDbContext>()
                .UseSqlite(conn)                         
                .Options;

            return new SkillBridgeDbContext(options);
        }
    }
}

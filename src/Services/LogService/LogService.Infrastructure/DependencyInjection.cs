using LogService.Application.Abstractions;
using LogService.Application.Logs;
using LogService.Infrastructure.Data;
using LogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<LogDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("LogDb"));
        });

        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<ILogUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<LogDbContext>());
        services.AddScoped<ILogApplicationService, LogApplicationService>();

        return services;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SurveyPlatform.SurveyResponseService.Application.Interfaces;
using SurveyPlatform.SurveyResponseService.Domain.Interfaces;
using SurveyPlatform.SurveyResponseService.Infrastructure.Messaging;
using SurveyPlatform.SurveyResponseService.Infrastructure.Persistence;
using SurveyPlatform.SurveyResponseService.Infrastructure.Repositories;
using SurveyPlatform.SurveyResponseService.Infrastructure.Services;

namespace SurveyPlatform.SurveyResponseService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ResponseDbContext>(o => 
            o.UseNpgsql(config.GetConnectionString("DefaultConnection"), 
                b => {
                    b.MigrationsAssembly(typeof(ResponseDbContext).Assembly.FullName);
                    b.MigrationsHistoryTable("__EFMigrationsHistory", "survey_response");
                }));

        services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ResponseDbContext>());

        services.TryAddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        var messagingProvider = config["Messaging:Provider"] ?? "RabbitMQ";
        if (messagingProvider == "RabbitMQ")
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

        var redis = config.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redis))
            services.AddStackExchangeRedisCache(o => { o.Configuration = redis; o.InstanceName = "SurveyResponse_"; });
        else
            services.AddDistributedMemoryCache();

        return services;
    }
}

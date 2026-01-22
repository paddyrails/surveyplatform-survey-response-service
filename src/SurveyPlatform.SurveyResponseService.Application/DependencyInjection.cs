using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SurveyPlatform.SurveyResponseService.Application.Behaviors;
using SurveyPlatform.SurveyResponseService.Application.Mappings;
using System.Reflection;

namespace SurveyPlatform.SurveyResponseService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddAutoMapper(typeof(ResponseMappingProfile).Assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        return services;
    }
}

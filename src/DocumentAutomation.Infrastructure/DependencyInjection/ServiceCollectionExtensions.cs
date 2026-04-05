using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Services;
using DocumentAutomation.Infrastructure.Configuration;
using DocumentAutomation.Infrastructure.Demo;
using DocumentAutomation.Infrastructure.Persistence;
using DocumentAutomation.Infrastructure.Runtime;
using DocumentAutomation.Infrastructure.Storage;
using DocumentAutomation.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentAutomation.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentAutomationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = DocumentAutomationConnectionResolver.Resolve(configuration);
        var storage = DocumentAutomationConnectionResolver.ResolveStorage(configuration);

        services.AddSingleton<IAppRuntimeContext>(new AppRuntimeContext(
            !connection.CanConnect,
            connection.Description,
            storage.TemplateRootPath,
            storage.OutputRootPath));
        services.AddSingleton<ITemplateStorage>(_ => new FileSystemTemplateStorage(storage.TemplateRootPath));
        services.AddSingleton<IOutputStorage>(_ => new FileSystemOutputStorage(storage.OutputRootPath));
        services.AddSingleton<IAuthorizationService, ApplicationAuthorizationService>();
        services.AddTransient<IDocumentPreparationService, DocumentPreparationService>();

        // The app is intentionally dual-mode.
        // If PostgreSQL is reachable we use real persistence services.
        // If not, we still keep the product explorable through seeded demo services.
        if (connection.CanConnect && !string.IsNullOrWhiteSpace(connection.ConnectionString))
        {
            services.AddTransient(_ => new DocumentAutomationDbContext(connection.ConnectionString!));
            services.AddTransient<ICurrentUserContext, DbCurrentUserContext>();
            services.AddTransient<ISecurityAdministrationService, DbSecurityAdministrationService>();
            services.AddTransient<IProjectDataService, DbProjectDataService>();
            services.AddTransient<ITemplateCatalogService, DbTemplateCatalogService>();
        }
        else
        {
            services.AddSingleton<DemoDataStore>();
            services.AddSingleton<ICurrentUserContext, DemoCurrentUserContext>();
            services.AddSingleton<ISecurityAdministrationService, DemoSecurityAdministrationService>();
            services.AddSingleton<IProjectDataService, DemoProjectDataService>();
            services.AddSingleton<ITemplateCatalogService, DemoTemplateCatalogService>();
        }

        return services;
    }
}

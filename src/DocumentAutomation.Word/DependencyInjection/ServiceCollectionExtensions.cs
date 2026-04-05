using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Word.OpenXml;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentAutomation.Word.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentAutomationWordServices(this IServiceCollection services)
    {
        services.AddSingleton<OpenXmlTemplateScanner>();
        services.AddSingleton<ITemplateScanner>(sp => sp.GetRequiredService<OpenXmlTemplateScanner>());
        services.AddSingleton<ITemplateFieldExtractor>(sp => sp.GetRequiredService<OpenXmlTemplateScanner>());
        services.AddSingleton<IDocumentGenerator, OpenXmlDocumentGenerator>();
        return services;
    }
}

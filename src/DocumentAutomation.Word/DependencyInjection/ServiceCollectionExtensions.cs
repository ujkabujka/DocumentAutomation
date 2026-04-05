using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Word.Abstractions;
using DocumentAutomation.Word.OpenXml;
using DocumentAutomation.Word.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentAutomation.Word.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentAutomationWordServices(this IServiceCollection services)
    {
        services.AddSingleton<PlaceholderParser>();
        services.AddSingleton<TemplateFieldDefinitionFactory>();
        services.AddSingleton<TextTemplateScanner>();
        services.AddSingleton<ITemplateTextReader, OpenXmlTemplateTextReader>();
        services.AddSingleton<OpenXmlTemplateScanner>();
        services.AddSingleton<ITemplateScanner>(sp => sp.GetRequiredService<OpenXmlTemplateScanner>());
        services.AddSingleton<ITemplateFieldExtractor>(sp => sp.GetRequiredService<OpenXmlTemplateScanner>());
        services.AddSingleton<IDocumentGenerator, OpenXmlDocumentGenerator>();
        return services;
    }
}

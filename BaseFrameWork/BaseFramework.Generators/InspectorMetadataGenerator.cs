using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BaseFramework.Generators;

[Generator]
public sealed class InspectorMetadataGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var types = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0,
                transform: static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node) as INamedTypeSymbol)
            .Where(static symbol => symbol is not null)
            .Select(static (symbol, _) => symbol!);

        context.RegisterSourceOutput(types, static (spc, typeSymbol) =>
        {
            var hasAttr = typeSymbol.GetAttributes().Any(a => a.AttributeClass?.Name == "GenerateInspectorMetadataAttribute");
            if (!hasAttr)
            {
                return;
            }

            var ns = typeSymbol.ContainingNamespace.IsGlobalNamespace ? "" : $"namespace {typeSymbol.ContainingNamespace.ToDisplayString()};";
            var fullName = typeSymbol.ToDisplayString();
            var source = new StringBuilder();
            source.AppendLine("using System.Collections.Generic;");
            source.AppendLine("using BaseFramework.Core.Generated;");
            source.AppendLine("using BaseFramework.Core.Metadata;");
            if (!string.IsNullOrWhiteSpace(ns)) source.AppendLine(ns);
            source.AppendLine($"internal static class {typeSymbol.Name}GeneratedMetadataRegistration");
            source.AppendLine("{");
            source.AppendLine("    [System.Runtime.CompilerServices.ModuleInitializer]");
            source.AppendLine("    internal static void Register()");
            source.AppendLine("    {");
            source.AppendLine($"        GeneratedMetadataRegistry.Register(typeof({fullName}), static () => new InspectableTypeMetadata(typeof({fullName}), new List<InspectableMemberMetadata>()));");
            source.AppendLine("    }");
            source.AppendLine("}");

            spc.AddSource($"{typeSymbol.Name}.InspectorMetadata.g.cs", source.ToString());
        });
    }
}

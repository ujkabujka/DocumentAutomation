using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Domain.Templates;
using System.Windows;
using System.Windows.Controls;

namespace DocumentAutomation.App.Views;

public partial class DesignerPage : UserControl
{
    private readonly ITemplateCatalogService _templateCatalogService;
    private readonly ITemplateStorage _templateStorage;
    private readonly ITemplateScanner _templateScanner;
    private IReadOnlyList<TemplateDefinition> _templates = Array.Empty<TemplateDefinition>();

    public DesignerPage(
        ITemplateCatalogService templateCatalogService,
        ITemplateStorage templateStorage,
        ITemplateScanner templateScanner)
    {
        _templateCatalogService = templateCatalogService;
        _templateStorage = templateStorage;
        _templateScanner = templateScanner;
        InitializeComponent();
        Loaded += HandleLoadedAsync;
    }

    private async void HandleLoadedAsync(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _templates = await _templateCatalogService.GetTemplatesAsync();
        TemplateList.ItemsSource = _templates;
        if (_templates.Count > 0 && TemplateList.SelectedItem is null)
        {
            TemplateList.SelectedIndex = 0;
        }

        RefreshSelectedTemplate();
    }

    private void OnTemplateSelectionChanged(object sender, SelectionChangedEventArgs e)
        => RefreshSelectedTemplate();

    private async void OnRescanClicked(object sender, RoutedEventArgs e)
    {
        if (TemplateList.SelectedItem is not TemplateDefinition template)
        {
            DesignerStatusText.Text = "Select a template before rescanning.";
            return;
        }

        var scan = await _templateScanner.ScanAsync(_templateStorage.ResolveTemplatePath(template.RelativePath));
        template.Fields = scan.Fields.ToList();
        await _templateCatalogService.SaveTemplateAsync(template);
        // The designer page is the easiest visible explanation of the template-reader workflow.
        // We surface both the discovered field list and any warnings so the scanner stays teachable.
        DesignerStatusText.Text = scan.Warnings.Count == 0
            ? $"Template '{template.Name}' scanned successfully. Discovered {scan.Fields.Count} placeholder fields."
            : $"Template '{template.Name}' scanned with warnings:{Environment.NewLine}{string.Join(Environment.NewLine, scan.Warnings)}";

        await RefreshAsync();
    }

    private void RefreshSelectedTemplate()
    {
        if (TemplateList.SelectedItem is not TemplateDefinition template)
        {
            TemplateSummaryText.Text = "Select a template to inspect its field model.";
            FieldGrid.ItemsSource = null;
            return;
        }

        var orderedFields = template.Fields.OrderBy(field => field.Order).ToList();
        TemplateSummaryText.Text = $"{template.Name} | {template.DocumentType} | {template.RelativePath}{Environment.NewLine}This template currently asks for {orderedFields.Count} fields.";
        FieldGrid.ItemsSource = orderedFields.Select(CreateFieldRow).ToList();
    }

    private static TemplateFieldRow CreateFieldRow(TemplateFieldDefinition field)
        => new(
            field.FieldKey,
            field.DisplayName,
            field.FieldType.ToString(),
            field.IsRequired,
            DescribeSuggestedSource(field),
            field.DatabaseKey ?? string.Empty,
            field.Section ?? string.Empty,
            field.Category ?? string.Empty);

    private static string DescribeSuggestedSource(TemplateFieldDefinition field)
    {
        if (field.SourcePriority.Length > 0)
        {
            return string.Join(" -> ", field.SourcePriority);
        }

        if (!string.IsNullOrWhiteSpace(field.DatabaseKey))
        {
            return "Database";
        }

        return field.FieldType is TemplateFieldType.Image or TemplateFieldType.Table or TemplateFieldType.File
            ? "User"
            : "User or Database";
    }

    private sealed record TemplateFieldRow(
        string FieldKey,
        string DisplayName,
        string FieldType,
        bool IsRequired,
        string SuggestedSource,
        string DatabaseKey,
        string Section,
        string Category);
}

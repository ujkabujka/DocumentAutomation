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
        DesignerStatusText.Text = scan.Warnings.Count == 0
            ? $"Template '{template.Name}' scanned successfully."
            : string.Join(Environment.NewLine, scan.Warnings);

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

        TemplateSummaryText.Text = $"{template.Name} | {template.DocumentType} | {template.RelativePath}";
        FieldGrid.ItemsSource = template.Fields.OrderBy(field => field.Order).ToList();
    }
}

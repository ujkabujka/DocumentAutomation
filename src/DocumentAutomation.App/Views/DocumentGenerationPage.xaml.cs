using BaseFramework.Core.Access;
using BaseFramework.Core.Services;
using BaseFramework.Wpf.Controls;
using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Documents;
using DocumentAutomation.Domain.Templates;
using System.Windows;
using System.Windows.Controls;

namespace DocumentAutomation.App.Views;

public partial class DocumentGenerationPage : UserControl
{
    private readonly IProjectDataService _projectDataService;
    private readonly ITemplateCatalogService _templateCatalogService;
    private readonly IDocumentPreparationService _documentPreparationService;
    private readonly IDocumentGenerator _documentGenerator;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IObjectMetadataProvider _metadataProvider = new ReflectionObjectMetadataProvider();
    private DocumentPreparationResult? _currentPreparation;

    public DocumentGenerationPage(
        IProjectDataService projectDataService,
        ITemplateCatalogService templateCatalogService,
        IDocumentPreparationService documentPreparationService,
        IDocumentGenerator documentGenerator,
        ICurrentUserContext currentUserContext)
    {
        _projectDataService = projectDataService;
        _templateCatalogService = templateCatalogService;
        _documentPreparationService = documentPreparationService;
        _documentGenerator = documentGenerator;
        _currentUserContext = currentUserContext;
        InitializeComponent();
        Loaded += HandleLoadedAsync;
    }

    private async void HandleLoadedAsync(object sender, RoutedEventArgs e)
    {
        await LoadSelectionsAsync();
    }

    private async Task LoadSelectionsAsync()
    {
        ProjectSelector.ItemsSource = await _projectDataService.GetProjectsAsync();
        TemplateSelector.ItemsSource = await _templateCatalogService.GetTemplatesAsync();

        if (ProjectSelector.Items.Count > 0 && ProjectSelector.SelectedItem is null)
        {
            ProjectSelector.SelectedIndex = 0;
        }

        if (TemplateSelector.Items.Count > 0 && TemplateSelector.SelectedItem is null)
        {
            TemplateSelector.SelectedIndex = 0;
        }
    }

    private async void OnPrepareClicked(object sender, RoutedEventArgs e)
    {
        if (ProjectSelector.SelectedItem is not ProjectRecord project || TemplateSelector.SelectedItem is not TemplateDefinition template)
        {
            GenerationStatusText.Text = "Select both a project and a template before preparing the document.";
            return;
        }

        _currentPreparation = await _documentPreparationService.PrepareAsync(project.Id, template.Id);
        var accessContext = _currentPreparation.CurrentUser.ToAccessContext();
        FieldInspector.Bind(
            _currentPreparation.Form,
            _metadataProvider,
            new DefaultMemberAccessEvaluator(),
            accessContext,
            new DefaultInspectorEditorRegistry());
        PreparedFieldGrid.ItemsSource = _currentPreparation.InitialFieldValues;
        GenerationStatusText.Text = "Template prepared. Review autofilled values and complete any missing fields in the inspector.";
    }

    private async void OnSaveFieldsClicked(object sender, RoutedEventArgs e)
    {
        if (_currentPreparation is null)
        {
            GenerationStatusText.Text = "Prepare a document before saving field values.";
            return;
        }

        var values = _currentPreparation.Form.SnapshotValues().Where(field => field.CanPersistBack).ToList();
        await _projectDataService.SaveFieldValuesAsync(_currentPreparation.Project, values);
        GenerationStatusText.Text = "Persistable field values were saved back to the project data store.";
    }

    private async void OnGenerateClicked(object sender, RoutedEventArgs e)
    {
        if (_currentPreparation is null)
        {
            GenerationStatusText.Text = "Prepare a document before generating output.";
            return;
        }

        var request = await _documentPreparationService.CreateGenerationRequestAsync(_currentPreparation);
        var result = await _documentGenerator.GenerateAsync(request);
        PreparedFieldGrid.ItemsSource = result.FinalFieldValues;
        var warnings = result.Warnings.Count == 0 ? string.Empty : $"{Environment.NewLine}{string.Join(Environment.NewLine, result.Warnings)}";
        GenerationStatusText.Text = $"Document generated at: {result.OutputPath}{warnings}";
    }
}

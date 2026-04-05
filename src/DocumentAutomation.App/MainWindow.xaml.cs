using BaseFramework.Wpf.Controls.Navigation;
using DocumentAutomation.Application.Contracts;
using DocumentAutomation.Application.Models;
using DocumentAutomation.Domain.Security;
using DocumentAutomation.Word.OpenXml;
using System.Windows;

namespace DocumentAutomation.App;

public partial class MainWindow : Window
{
    private readonly IAppRuntimeContext _runtimeContext;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IAuthorizationService _authorizationService;
    private readonly ITemplateStorage _templateStorage;
    private readonly Views.AdminPage _adminPage;
    private readonly Views.DesignerPage _designerPage;
    private readonly Views.DocumentGenerationPage _documentGenerationPage;
    private CurrentUserSession? _currentUser;

    public MainWindow(
        IAppRuntimeContext runtimeContext,
        ICurrentUserContext currentUserContext,
        IAuthorizationService authorizationService,
        ITemplateStorage templateStorage,
        Views.AdminPage adminPage,
        Views.DesignerPage designerPage,
        Views.DocumentGenerationPage documentGenerationPage)
    {
        _runtimeContext = runtimeContext;
        _currentUserContext = currentUserContext;
        _authorizationService = authorizationService;
        _templateStorage = templateStorage;
        _adminPage = adminPage;
        _designerPage = designerPage;
        _documentGenerationPage = documentGenerationPage;

        InitializeComponent();
        Loaded += HandleLoadedAsync;
    }

    private async void HandleLoadedAsync(object sender, RoutedEventArgs e)
    {
        Loaded -= HandleLoadedAsync;

        await _templateStorage.EnsureSeedTemplatesAsync();
        await DemoTemplateDocumentWriter.EnsureSeedTemplateAsync(_templateStorage.ResolveTemplatePath("system-acceptance-report.docx"));

        _currentUser = await _currentUserContext.GetCurrentUserAsync();
        CurrentUserText.Text = $"{_currentUser.DisplayName} ({_currentUser.UserName})";
        CurrentRoleText.Text = $"Roles: {string.Join(", ", _currentUser.Roles)}";
        ConnectionStatusText.Text = _runtimeContext.ConnectionDescription ?? "Runtime status unavailable.";

        if (_runtimeContext.IsDemoMode)
        {
            DemoModeBanner.Visibility = Visibility.Visible;
            DemoModeText.Text = "Running in demo mode because PostgreSQL is not available. The document workflow stays explorable with seeded in-memory data.";
        }

        RegisterPages(_currentUser);
    }

    private void RegisterPages(CurrentUserSession currentUser)
    {
        if (_authorizationService.HasPermission(currentUser, PermissionNames.UsersView) ||
            _authorizationService.HasPermission(currentUser, PermissionNames.UsersManage) ||
            _authorizationService.HasPermission(currentUser, PermissionNames.RolesManage))
        {
            NavigationHost.AddPage(new NavigationPage("Admin", _adminPage, "A"));
        }

        if (_authorizationService.HasPermission(currentUser, PermissionNames.TemplatesView) ||
            _authorizationService.HasPermission(currentUser, PermissionNames.TemplatesManage))
        {
            NavigationHost.AddPage(new NavigationPage("Designer", _designerPage, "D"));
        }

        if (_authorizationService.HasPermission(currentUser, PermissionNames.DocumentsGenerate))
        {
            NavigationHost.AddPage(new NavigationPage("Generate", _documentGenerationPage, "G"));
        }
    }
}

using System.Windows.Media;
using BaseFramework.Core.Services;
using BaseFramework.Wpf.Controls.Navigation;
using BaseFramework.WpfHost.Models;
using BaseFramework.WpfHost.Views;

namespace BaseFramework.WpfHost;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var provider = new ReflectionObjectMetadataProvider();

        var basics = new InspectorExamplePage();
        basics.Initialize(
            "1-5. Basic To Computed Examples",
            "Start here for scalar members, dropdown/value sources, multiline text, a note editor, a computed read-only field, and command invocation.",
            new BasicDocumentExampleModel(),
            provider);

        var nested = new InspectorExamplePage();
        nested.Initialize(
            "6-8. Nested Objects And Collections",
            "This page demonstrates rejection-driven conditional visibility, nested inspectable objects, collections, and collection-manipulating commands.",
            new NestedWorkflowExampleModel(),
            provider);

        var roles = new RoleAwarePage();
        roles.Initialize(provider);

        var runtimeTemplate = new InspectorExamplePage();
        runtimeTemplate.Initialize(
            "11. Runtime Template Field Model",
            "This form is built entirely from runtime metadata instead of reflected CLR properties. It shows how a scanned template can still flow through the same inspector and editor pipeline.",
            new TemplateDrivenExampleForm(),
            provider,
            BaseFramework.Core.Access.InspectableAccessContext.Create(
                "engineer",
                ["SystemEngineer"],
                ["documents.generate"]));

        var dashboard = new DashboardPage();
        dashboard.Initialize(new Test_Class_3(), provider);

        NavigationHost.AddPage(new NavigationPage("Basics", basics, "01"));
        NavigationHost.AddPage(new NavigationPage("Nested", nested, "02"));
        NavigationHost.AddPage(new NavigationPage("Roles", roles, "03"));
        NavigationHost.AddPage(new NavigationPage("Template", runtimeTemplate, "04"));
        NavigationHost.AddPage(new NavigationPage("Schedule", dashboard, "05"));
    }

    private static UIElement BuildPlaceholderPage(string message)
        => new Border
        {
            Padding = new Thickness(32),
            Background = Brushes.Transparent,
            Child = new TextBlock
            {
                Text = message,
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                Opacity = 0.6,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
}

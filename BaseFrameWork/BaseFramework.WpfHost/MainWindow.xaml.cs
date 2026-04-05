using System.Windows.Media;
using BaseFramework.Core.Services;
using BaseFramework.WpfHost.Controls.Navigation;
using BaseFramework.WpfHost.Models;
using BaseFramework.WpfHost.Views;

namespace BaseFramework.WpfHost;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var model = new Test_Class_3();
        var provider = new ReflectionObjectMetadataProvider();

        var dashboard = new DashboardPage();
        dashboard.Initialize(model, provider);

        NavigationHost.AddPage(new NavigationPage("Dashboard", dashboard));
        NavigationHost.AddPage(new NavigationPage("Reports", BuildPlaceholderPage("Reports coming soon")));
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

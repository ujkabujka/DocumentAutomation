using DocumentAutomation.Infrastructure.DependencyInjection;
using DocumentAutomation.Word.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace DocumentAutomation.App;

public partial class App : System.Windows.Application
{
    private IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Generic Host keeps startup readable: configuration, DI, and lifetime all live in one familiar place.
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.SetBasePath(AppContext.BaseDirectory);
                configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false);
                configuration.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddDocumentAutomationInfrastructure(context.Configuration);
                services.AddDocumentAutomationWordServices();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<Views.AdminPage>();
                services.AddSingleton<Views.DesignerPage>();
                services.AddSingleton<Views.DocumentGenerationPage>();
            })
            .Build();

        await _host.StartAsync();

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}

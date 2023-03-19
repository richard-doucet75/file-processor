using System.Windows;
using FileProcessor.Gateways;
using FileProcessor.Shared.Gateways;
using FileProcessor.UseCase;
using FileProcessorWpf.ViewModel;
using FileProcessorWpf.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FileProcessorWpf;
public partial class App
{
    private readonly ServiceProvider _serviceProvider;
    public App()
    {
        var services = new ServiceCollection();
        services.AddTransient<ProcessorViewModel>();
        services.AddTransient<IFileDataGateway, FileDataGateway>();
        services.AddSingleton<MainView>(s => new MainView()
        {
            DataContext = s.GetRequiredService<ProcessorViewModel>()
        });

        services.AddTransient<GetOperationDefinition>();
        services.AddTransient<ProcessDataSet>();

        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        var mainView = _serviceProvider.GetRequiredService<MainView>();
        mainView.Show();
    }
    
}

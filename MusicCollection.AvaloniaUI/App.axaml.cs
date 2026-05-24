using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using MusicCollection.Application;
using MusicCollection.AvaloniaUI.ViewModels;
using MusicCollection.AvaloniaUI.Views;
using MusicCollection.Infrastructure.Persistence;

namespace MusicCollection.AvaloniaUI;

public partial class App : Avalonia.Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        const string connectionString =
            @"Server=(localdb)\mssqllocaldb;Database=MusicCDDB;Trusted_Connection=True;";

        // Подключаем слои данных, логики и окна
        services.AddPersistenceServices(connectionString);
        services.AddApplicationServices();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<MainWindow>();

        services.AddTransient<AddAlbumWindowViewModel>();
        services.AddTransient<AddAlbumWindow>();

        ServiceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}

using Data.Services;
using Logic.Services;
using Microsoft.Extensions.DependencyInjection;
using Shared.Entities;
using Shared.Interfaces;
using Shared.Providers;
using System.Windows;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        public App()
        {
            ServiceCollection services = new();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IDocumentDataBaseAPI,LiteDBDAL>();
            services.AddSingleton<ResourceProvider<Ingrediant>>();
            services.AddSingleton<ResourceProvider<Beverage>>();
            services.AddSingleton<IngrediantService>();
            services.AddSingleton<BeverageService>();
            services.AddSingleton<MainWindowVM>();
            services.AddSingleton<MainWindow>();
        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}

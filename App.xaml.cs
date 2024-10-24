using Neshangar.Windows;
using System.Windows;
using Neshangar.Core.Data;
using Neshangar.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.Win32;

namespace Neshangar
{

    public partial class App : System.Windows.Application
    {
        private NotifySystem _notifySystem;
        private FloatingWidget floatingWidget;
        private Client _client;
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureService(services);
            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);

            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            ApplyTheme();

            InitialClient();

            floatingWidget = ServiceProvider.GetRequiredService<FloatingWidget>();
            floatingWidget.Show();

            _notifySystem = ServiceProvider.GetRequiredService<NotifySystem>();
           
        }

        private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General)
            {
                ApplyTheme();
            }
        }
        private void ApplyTheme()
        {
            bool isDarkMode = IsDarkModeEnabled();
            ResourceDictionary themeDict = new ResourceDictionary();
            if (isDarkMode)
            {
                themeDict.Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative);
            }
            else
            {
                themeDict.Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative);
            }

            // Clear the current theme and apply the new one
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(themeDict);
        }
        private bool IsDarkModeEnabled()
        {
            const string registryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string registryValue = "AppsUseLightTheme";

            object value = Registry.GetValue(registryKey, registryValue, null);
            if (value != null && value is int intValue)
            {
                return intValue == 0;
            }

            return false;
        }

        private async void InitialClient()
        {

            //_client = ServiceProvider.GetRequiredService<Client>();
            //try
            //{
            //    await _client.SetStatusViaTimer(StatusEnum.Online ,null);
            //}
            //catch (Exception ex)
            //{

            //    Trace.WriteLine($"Error initializing SignalR client: {ex.Message}");
            //}

            
        }

        private void ConfigureService(ServiceCollection services)
        {
            services.AddSingleton<FloatingWidget>();
            services.AddSingleton<UsersList>();
            services.AddSingleton<Settings>();
            services.AddSingleton<ChangeStatus>();
            services.AddSingleton<NotifySystem>();

            services.AddSingleton<DataFileContext>(provider =>
            {
                return new DataFileContext("Neshangar.json");
            });

            services.AddSingleton<Client>(provider =>
            {
                return new Client($"https://localhost:44384/indicator", ServiceProvider.GetRequiredService<DataFileContext>());
            });
            
        }

        public void ToggleFloatingWidget()
        {

            if (floatingWidget.IsVisible)
            {
                floatingWidget.Hide();
                _notifySystem.toggleFloatingWidget.Text = "Show Widget";

            }
            else
            {
                floatingWidget.Show();
                _notifySystem.toggleFloatingWidget.Text = "Hide Widget";

            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _client?.CloseConnection();

            base.OnExit(e);
        }

    }

}

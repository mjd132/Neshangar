using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Neshangar.Core.Data;
using Neshangar.Windows;
using Application = System.Windows.Application;

namespace Neshangar
{
    public partial class App : Application
    {
        private NotifySystem _notifySystem;
        private FloatingWidget _floatingWidget;
        private Client _client;
        private Connect _connect;
        public static IServiceProvider ServiceProvider { get; private set; }
        public IConfigurationRoot Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureService(services);
            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);

            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
            ApplyTheme();

            InitialClient();

            _floatingWidget = ServiceProvider.GetRequiredService<FloatingWidget>();
            _connect = ServiceProvider.GetRequiredService<Connect>();
            _connect.Show();
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
            const string registryKey =
                @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string registryValue = "AppsUseLightTheme";

            object value = Registry.GetValue(registryKey, registryValue, null);
            if (value != null && value is int intValue)
            {
                return intValue == 0;
            }

            return false;
        }

        private void InitialClient()
        {
            _client = ServiceProvider.GetRequiredService<Client>();
            _client.UserLoggedIn += OnLoggedIn;
        }

        private void OnLoggedIn()
        {
            Current.Dispatcher.Invoke(() =>
            {
                if (_connect.IsVisible)
                {
                    _connect.Hide();
                }

                _floatingWidget.Show();
                _notifySystem = ServiceProvider.GetRequiredService<NotifySystem>();
            });
        }

        private void ConfigureService(ServiceCollection services)
        {
            services.AddSingleton<FloatingWidget>();
            services.AddSingleton<UsersList>();
            services.AddSingleton<Settings>();
            services.AddSingleton<Connect>();
            services.AddSingleton<ChangeStatus>();
            services.AddSingleton<NotifySystem>();

            services.AddSingleton<DataFileContext>(provider => new DataFileContext());
            services.AddSingleton<Client>(provider =>
                new Client(ServiceProvider.GetRequiredService<DataFileContext>()));
        }

        public void ToggleFloatingWidget()
        {
            if (_floatingWidget.IsVisible)
            {
                _floatingWidget.Hide();
                _notifySystem.toggleFloatingWidget.Text = "Show Widget";
            }
            else
            {
                _floatingWidget.Show();
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
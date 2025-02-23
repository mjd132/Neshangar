using System.Diagnostics;
using System.Windows;
using Neshangar.Core.Data;
using Neshangar.Core.Entities;

namespace Neshangar.Windows;

public partial class Connect : Window
{
    private readonly Client _client;
    private readonly DataFileContext _context;
    

    public Connect(Client client, DataFileContext context)
    {
        _client = client;
        _context = context;

        _client.ConnectionClosed += OnConnectionClosed;
        _client.ConnectionReconnected += OnReconnected;
        
        InitializeComponent();

        SubmitButton.Click += async (_, _) => await SubmitButton_Click();
        _ = InitializeFormAsync();
    }

    private void OnReconnected()
    {
        Dispatcher.Invoke(() =>
        {
            SubmitButton.Content = "Connected";
        });
    }

    private void OnConnectionClosed()
    {
        Dispatcher.Invoke(() =>
        {
            SubmitButton.IsEnabled = false;
            SubmitButton.Content = "Reconnecting...";
        });
    }

    private async Task InitializeFormAsync()
    {
        try
        {
            Setting setting = _context.Setting;

            if (!string.IsNullOrWhiteSpace(setting.ServerUrl) && 
                !string.IsNullOrWhiteSpace(setting.UserToken))
            {
                ServerUrlTextBox.Text = setting.ServerUrl;
                NameTextBox.Text = setting.UserName;

                SubmitButton.IsEnabled = false;
                SubmitButton.Content = "Connecting...";

                await _client.ConnectAsync(setting.ServerUrl).ConfigureAwait(false);
                await _client.LoginAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                SubmitButton.IsEnabled = true;
                SubmitButton.Content = "Retry";
            });

            Trace.WriteLine($"Error during initialization: {ex.Message}");
        }
    }

    private async Task SubmitButton_Click()
    {
        string serverUrl = ServerUrlTextBox.Text;
        string name = NameTextBox.Text;

        Setting setting = new Setting();
        setting.ServerUrl = serverUrl;
        setting.UserName = name;

        _context.Setting = setting;

        SubmitButton.IsEnabled = false;
        SubmitButton.Content = "Connecting...";

        await _client.ConnectAsync(serverUrl);
        await _client.RegisterAsync(name);
    }
}
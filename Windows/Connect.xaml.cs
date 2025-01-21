using System.Net;
using Neshangar.Core.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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
        InitializeComponent();

        InitialForm();
    }

    private void InitialForm()
    {
        Setting setting = _context.Setting;
        if (!String.IsNullOrWhiteSpace(setting.ServerUrl) && !String.IsNullOrWhiteSpace(setting.UserToken))
        {
            ServerUrlTextBox.Text = setting.ServerUrl;
            NameTextBox.Text = setting.UserName;

            SubmitButton.IsEnabled = false;
            SubmitButton.Content = "Connecting...";

            _client.Connect(setting.ServerUrl);
            _client.Login();
        }
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        string serverUrl = ServerUrlTextBox.Text;
        string name = NameTextBox.Text;
        
        Setting setting = new Setting();
        setting.ServerUrl = serverUrl;
        setting.UserName = name;
        
        _context.Setting = setting;
        
        SubmitButton.IsEnabled = false;
        SubmitButton.Content = "Connecting...";

        _client.Connect(serverUrl);
        _client.Register(name);
    }
    
    
}
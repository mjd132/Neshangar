using System.ComponentModel;
using System.Windows;
using Neshangar.Core.Data;
using RadioButton = System.Windows.Controls.RadioButton;

namespace Neshangar.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private readonly Client _client;

        private Array lastSetting;

        private string name;
        public Settings(Client client)
        {
            _client = client;
            InitializeComponent();
            AdjustComponent();

        }
        
        private void AdjustComponent()
        {
            IsVisibleChanged += (_,_) => InitialSettingData() ;
            ApplyButton.Click += ApplyButton_Click;
            OkButton.Click += OkButton_Click;
            CancelButton.Click += (_,_) => Close();
        }
        private void InitialSettingData()
        {
            if (!IsVisible)
                return;
            if (_client.user != null)
            {
                NameTextBox.Text = _client.user.Name;
                name = _client.user.Name;
            }

            if (StartupService.IsInStartup())
            {
                StartOnStartupCheckBox.IsChecked = true;
            }
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyButton_Click(sender, e);
            Close();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client.user?.Name != NameTextBox.Text)
            {
                await _client.ChangeName(NameTextBox.Text);
            }

            if (StartOnStartupCheckBox.IsChecked == true)
            {
                StartupService.AddToStartup();
            }
            else
            {
                StartupService.RemoveFromStartup();
            }
        }
        
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var selectedRadio = sender as RadioButton;

            //theme = (System.Windows.Controls.RadioButton)sender;

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

}

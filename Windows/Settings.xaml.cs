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
            IsVisibleChanged += (object sender, DependencyPropertyChangedEventArgs e) => initialSettingData() ;
            ApplyButton.Click += ApplyButton_Click;
            OkButton.Click += OkButton_Click;
            CancelButton.Click += (object sender, RoutedEventArgs e) => Close();
        }
        private void initialSettingData()
        {
            if (!IsVisible)
                return;
            if (_client.user != null)
            {
                NameTextBox.Text = _client.user.Name;
                name = _client.user.Name;
            }
        }
        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client.user?.Name != NameTextBox.Text)
            {
                await _client.ChangeName(NameTextBox.Text);
            }
            Close();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client.user?.Name != NameTextBox.Text)
            {
                await _client.ChangeName(NameTextBox.Text);
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

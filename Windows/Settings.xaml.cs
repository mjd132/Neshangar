using Neshangar.Core.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        }
        private void initialSettingData()
        {
            if (!IsVisible)
                return;
            if (_client.user != null)
            {
                NameTextBox.Text = _client.user.name;
                name = _client.user.name;
            }
        }
        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client.user?.name != NameTextBox.Text)
            {
                await _client.ChangeName(NameTextBox.Text);
            }
            Close();
        }

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client.user?.name != NameTextBox.Text)
            {
                await _client.ChangeName(NameTextBox.Text);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var selectedRadio = sender as System.Windows.Controls.RadioButton;

            //theme = (System.Windows.Controls.RadioButton)sender;

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }

}

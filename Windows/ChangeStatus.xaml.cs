using Neshangar.Core.Data;
using Neshangar.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Interaction logic for ChangeStatus.xaml
    /// </summary>
    public partial class ChangeStatus : Window
    {
        private readonly Client _client;

        public ChangeStatus(Client client)
        {
            _client = client;
            InitializeComponent();
            AdjustComponent();
        }

        private void AdjustComponent()
        {
            var statusList = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>().ToList();
            StatusSelect.ItemsSource = statusList;
            StatusSelect.SelectedIndex = (int)_client.user?.status;
            BorderInput.Background = _client.user?.status != StatusEnum.Busy ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Transparent);
            TimeSpanInput.Focusable= _client.user?.status != StatusEnum.Busy ? false :true;
            StatusSelect.SelectionChanged += StatusComboBox_SelectionChanged;

            TimeSpanInput.Text = _client.user?.expireInterval?.TotalMinutes.ToString();
            TimeSpanInput.PreviewTextInput += TextBox_OnPreviewTextInput;
            ChangeButton.Click += ChangeStatusClickEvent;
        }

        private async void ChangeStatusClickEvent(object sender, RoutedEventArgs e)
        {
            var user = _client.user;
            var status = (StatusEnum)StatusSelect.SelectedItem;
            var expiresInterval = int.TryParse(TimeSpanInput.Text, out int minutes) ? TimeSpan.FromMinutes(minutes) : TimeSpan.FromMinutes(10);

            if (user != null)
            {
                if (user.status != status)
                {
                    await _client.SetStatusViaTimer(status, expiresInterval);
                    Close();
                }
            }
        }
        private void TextBox_OnPreviewTextInput(object? sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            var fullText = textBox?.Text.Insert(textBox.SelectionStart, e.Text);

            // If parsing is successful, set Handled to false
            e.Handled = !int.TryParse(fullText,
                                         NumberStyles.Integer,
                                         CultureInfo.InvariantCulture,
                                         out int val);
        }
        private TimeSpan? ShowError()
        {
            return new TimeSpan();
        }

        private void StatusComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (StatusSelect.SelectedItem != null)
            {
                var selectedItem = (StatusEnum)StatusSelect.SelectedItem;
                if (selectedItem != StatusEnum.Busy)
                {
                    TimeSpanInput.Focusable = false;
                    BorderInput.Background = new SolidColorBrush(Colors.LightGray);
                }
                else
                {
                    BorderInput.Background = new SolidColorBrush(Colors.Transparent);
                    TimeSpanInput.Focusable = true;
                }
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}

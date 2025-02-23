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

            this.Activated += OnWindowActivated;
        }

        private void OnWindowActivated(object? sender, EventArgs e)
        {
            AdjustComponent();
        }

        private void AdjustComponent()
        {
            var statusList = Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>().ToList();
            StatusSelect.ItemsSource = statusList;
            StatusSelect.SelectedIndex = (int)_client.User?.Status;
            BorderInput.Background = _client.User?.Status != StatusEnum.Busy
                ? new SolidColorBrush(Colors.LightGray)
                : new SolidColorBrush(Colors.Transparent);
            TimeSpanInput.Focusable = _client.User?.Status != StatusEnum.Busy ? false : true;
            StatusSelect.SelectionChanged += StatusComboBox_SelectionChanged;

            TimeSpanInput.Text = _client.User?.ExpireInterval?.TotalMinutes.ToString();
            TimeSpanInput.PreviewTextInput += TextBox_OnPreviewTextInput;

            ChangeButton.Click += ChangeStatusClickEvent;
            CancelButton.Click += (object sender, RoutedEventArgs e) => Close();
        }

        private void ChangeStatusClickEvent(object sender, RoutedEventArgs e)
        {
            var user = _client.User;
            var status = (StatusEnum)StatusSelect.SelectedItem;
            TimeSpan? expiresInterval = null;

            if (user != null)
            {
                if (user.Status != status)
                {
                    
                    if (status == StatusEnum.Busy)
                    {
                        if (int.TryParse(TimeSpanInput.Text, out int minutes) &&
                            minutes is >= 5 and <= 120)
                            expiresInterval = TimeSpan.FromMinutes(minutes);
                        else
                            return;
                    }
                    
                    try
                    {
                        _client.SetStatusViaTimer(status, expiresInterval);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                    finally
                    {
                        Close();
                    }
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
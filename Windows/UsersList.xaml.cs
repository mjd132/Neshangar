using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Neshangar.Core.Data;
using Neshangar.Core.Entities;

namespace Neshangar.Windows
{
    /// <summary>
    /// Interaction logic for UsersList.xaml
    /// </summary>
    public partial class UsersList : Window
    {
        private DispatcherTimer _countdonwTimer;
        private ObservableCollection<User> _users;
        private readonly Client _client;

        public UsersList(Client client)
        {
            _client = client;

            InitializeComponent();
            AdjustWindow();

            _users = new ObservableCollection<User>();
            UsersListBox.ItemsSource = _users;
            _client.UserListReceived += OnUserListReceived;

            if (_client.userList != null)
            {
                OnUserListReceived(_client.userList);
                _client.userList = null;
            }

            _countdonwTimer = new DispatcherTimer();
            _countdonwTimer.Interval = TimeSpan.FromSeconds(1);
            _countdonwTimer.Tick += CountdownTimer_Tick;
            _countdonwTimer.Start();
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            foreach (var user in _users)
            {
                if (user.ExpiredAt != null)
                {
                    var remainingTime = user.ExpiredAt - DateTime.Now;
                    if (remainingTime < TimeSpan.Zero)
                    {
                        continue;
                    }

                    user.RemainingTime = remainingTime;
                    user.RemainingTimeString = user.RemainingTime?.ToString(@"hh\:mm\:ss");
                }
            }

            UsersListBox.Items.Refresh();
        }

        public void UpdateUserList(List<User> users)
        {
            _users.Clear();
            foreach (var user in users)
            {
                _users.Add(user);
            }
        }

        private void AdjustWindow()
        {
            Width = SystemParameters.PrimaryScreenWidth * 0.25;
            Height = Screen.PrimaryScreen.WorkingArea.Bottom;

            Left = Screen.PrimaryScreen.WorkingArea.Right - Width;
            Top = 0;

            UsersListBox.FontSize = 16;
        }

        private void OnUserListReceived(List<User> users)
        {
            try
            {
                Dispatcher.InvokeAsync(() => UpdateUserList(users));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        // Minimize window event handler
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Close window event handler
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
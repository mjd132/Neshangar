
using Neshangar.Core.Data;
using Neshangar.Core.Entities;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Neshangar.Windows
{
    /// <summary>
    /// Interaction logic for Widget.xaml
    /// </summary>
    public partial class FloatingWidget : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOSIZE = 0x0001;
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_SHOWWINDOW = 0x0040;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_NOSENDCHANGING = 0x0400;

        private Client _client;
        private UsersList _usersList;
        private readonly ChangeStatus _changeStatus;
        private DispatcherTimer _countdonwTimer;
        public FloatingWidget(Client client, UsersList usersList, ChangeStatus changeStatus)
        {
            _client = client;
            _usersList = usersList;
            _changeStatus = changeStatus;
            _client.UserChanged += OnUserChangedReceived;
           
            InitializeComponent();
            SetFloatingWidgetOnTop();
            this.Deactivated += (object? sender, EventArgs e) => SetFloatingWidgetOnTop();
            this.Loaded += FloatingWidget_Loaded;
            this.Loaded += AdjustWindowSize;
            this.MouseRightButtonDown += OnMouseRightButtonDown;
            //this.LocationChanged += FloatingWidget_LocationChanged;
            AdjustComponent();

            _countdonwTimer = new DispatcherTimer();
            _countdonwTimer.Interval = TimeSpan.FromMinutes(1);
            _countdonwTimer.Tick += (object? sender,EventArgs e)=> UpdateFloatingWidget();

        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_changeStatus.IsVisible)
            {
                _changeStatus.Focus();
            }
            else
            {
                _changeStatus.Show();
            }
        }

        private void UpdateFloatingWidget()
        {

            if (_client.user?.expiredAt != null)
            {
                var remainingTime = _client.user?.expiredAt - DateTime.Now + TimeSpan.FromMinutes(1);
                var remainingTimeString = "~"+remainingTime?.ToString(@"hh\:mm");
                FloatingWidgetText.Text = remainingTimeString;
            }
        }
        private void OnUserChangedReceived(User user)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                switch (user.status)
                {
                    case StatusEnum.Online:
                        FloatingWidgetBorder.Background = new SolidColorBrush(Colors.Green);
                        FloatingWidgetText.Text = StatusEnum.Online.ToString();
                        break;
                    case StatusEnum.Busy:
                        FloatingWidgetBorder.Background = new SolidColorBrush(Colors.Red);
                        UpdateFloatingWidget();
                        _countdonwTimer.Start();
                        break;
                    case StatusEnum.Idle:
                        FloatingWidgetBorder.Background = new SolidColorBrush(Colors.DarkOrange);
                        FloatingWidgetText.Text = StatusEnum.Idle.ToString();
                        break;
                    case StatusEnum.AFK:
                        FloatingWidgetBorder.Background = new SolidColorBrush(Colors.DarkKhaki);
                        FloatingWidgetText.Text = StatusEnum.AFK.ToString();
                        break;
                    case StatusEnum.Offline:
                        FloatingWidgetBorder.Background = new SolidColorBrush(Colors.DarkGray);
                        FloatingWidgetText.Text = StatusEnum.Offline.ToString();
                        break;
                }
            });

        }
        private void SetFloatingWidgetOnTop()
        {
            var handle = new WindowInteropHelper(this).Handle;
            SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        private void AdjustComponent()
        {
            var screenWidth = SystemParameters.PrimaryScreenHeight * 0.01;
            FloatingWidgetText.FontSize = screenWidth;
            FloatingWidgetBorder.CornerRadius = new CornerRadius(screenWidth / 2);

            FloatingWidgetText.Visibility = Visibility.Hidden;
            
            if (_client.user != null)
            {
                OnUserChangedReceived(_client.user);
            }
        }

        private void AdjustWindowSize(object sender, RoutedEventArgs e)
        {
            double screenHeight = SystemParameters.PrimaryScreenHeight * 0.025;

            var workingArea = Screen.PrimaryScreen.WorkingArea;

            // Set the position of the floating widget
            this.Left = workingArea.Right - this.Width;
            this.Top = workingArea.Bottom - this.Height;

            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            //this.WindowState = WindowState.Maximized;

        }
        private async void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FloatingWidgetText.FontSize += 5;
            FloatingWidgetBorder.Width += 20;
            FloatingWidgetBorder.Height += 20;
            //this.Width += 24;
            FloatingWidgetText.Visibility = Visibility.Visible;
            FloatingWidgetCloseButton.Visibility = Visibility.Visible;

        }
        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FloatingWidgetText.FontSize -= 5;
            FloatingWidgetBorder.Width -= 20;
            FloatingWidgetBorder.Height -= 20;
            //this.Width -= 24;
            FloatingWidgetText.Visibility = Visibility.Hidden;
            FloatingWidgetCloseButton.Visibility = Visibility.Hidden;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var app = System.Windows.Application.Current as App;

            app.ToggleFloatingWidget();

        }

        // Override the Closing event to prevent the window from fully closing
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }

        private void Window_MouseDouble(object sender, MouseButtonEventArgs e)
        {
            if (_usersList.IsVisible)
            {
                _usersList.Focus();
            }
            else
            {
                _usersList.Show();
            }
        }

        private void Window_Deactivated(object? sender, EventArgs e)
        {
            // Ensure the window stays topmost
            SetWindowPos(new WindowInteropHelper(this).Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            Topmost = true; // Redundant, but to enforce topmost behavior
            Activate(); // Reactivate the window
        }


        private void FloatingWidget_LocationChanged(object? sender, EventArgs e)
        {
            var windowHandle = new WindowInteropHelper(this).Handle;

            var currentScreen = Screen.FromHandle(windowHandle);

            var workingArea = currentScreen.WorkingArea;

            if (this.Left < workingArea.Left) this.Left = workingArea.Left;
            if (this.Top < workingArea.Top) this.Top = workingArea.Top;
            if (this.Left + this.Width > workingArea.Right) this.Left = workingArea.Right - this.Width;
            if (this.Top + this.Height > workingArea.Bottom) this.Top = workingArea.Bottom - this.Height;
        }
        private void FloatingWidget_Loaded(object sender, RoutedEventArgs e)
        {
            SetFloatingWidgetOnTop();
            //var timer = new System.Windows.Threading.DispatcherTimer
            //{
            //    Interval = TimeSpan.FromMilliseconds(1000) // Every second
            //};
            //timer.Tick += (s, args) =>
            //{
            //    SetFloatingWidgetOnTop(); // Ensure topmost state every tick
            //};
            //timer.Start();
        }
    }
}

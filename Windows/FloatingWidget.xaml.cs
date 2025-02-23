using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Neshangar.Core.Data;
using Neshangar.Core.Entities;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Neshangar.Windows;

public partial class FloatingWidget
{
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    private static readonly IntPtr HwndTopmost = new IntPtr(-1);
    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoMove = 0x0002;
    private const uint SwpShowWindow = 0x0040;

    private readonly Client _client;
    private readonly UsersList _usersList;
    private readonly ChangeStatus _changeStatus;
    private readonly DispatcherTimer _countdownTimer;

    public FloatingWidget(Client client, UsersList usersList, ChangeStatus changeStatus)
    {
        _client = client;
        _usersList = usersList;
        _changeStatus = changeStatus;
        _client.UserChanged += OnUserChangedReceived;

        InitializeComponent();
        SetFloatingWidgetOnTop();
        Deactivated += (_, _) => SetFloatingWidgetOnTop();
        Loaded += FloatingWidget_Loaded;
        Loaded += AdjustWindowSize;
        MouseRightButtonDown += OnMouseRightButtonDown;

        AdjustComponent();

        _countdownTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _countdownTimer.Tick += (_, _) => UpdateFloatingWidget();
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
        if (_client.User is { ExpiredAt: not null, Status: StatusEnum.Busy })
        {
            if (!_countdownTimer.IsEnabled)
            {
                _countdownTimer.Start();
            }

            var remainingTime = _client.User?.ExpiredAt - DateTime.Now + TimeSpan.FromMinutes(1);
            var remainingTimeString = "~" + remainingTime?.ToString(@"hh\:mm");
            FloatingWidgetText.Text = remainingTimeString;
        }
        else
        {
            if (_countdownTimer.IsEnabled)
            {
                _countdownTimer.Stop();
            }

            FloatingWidgetText.Text = _client.User?.Status.ToString();
        }
    }

    private void OnUserChangedReceived(User user)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            switch (user.Status)
            {
                case StatusEnum.Online:
                    FloatingWidgetBorder.Background = new SolidColorBrush(Colors.Green);
                    FloatingWidgetText.Text = StatusEnum.Online.ToString();
                    break;
                case StatusEnum.Busy:
                    FloatingWidgetBorder.Background = new SolidColorBrush(Colors.Red);
                    UpdateFloatingWidget();
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
        SetWindowPos(handle, HwndTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpShowWindow);
    }

    private void AdjustComponent()
    {
        var screenWidth = SystemParameters.PrimaryScreenHeight * 0.01;
        FloatingWidgetText.FontSize = screenWidth;
        FloatingWidgetBorder.CornerRadius = new CornerRadius(screenWidth / 2);

        FloatingWidgetText.Visibility = Visibility.Hidden;

        if (_client.User != null)
        {
            OnUserChangedReceived(_client.User);
        }
        else
        {
            FloatingWidgetText.Text = "Connecting";
        }
    }

    private void AdjustWindowSize(object sender, RoutedEventArgs e)
    {
        if (Screen.PrimaryScreen != null)
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;

            // Set the position of the floating widget
            Left = workingArea.Right - Width;
            Top = workingArea.Bottom - Height;
        }

        ResizeMode = ResizeMode.NoResize;
        WindowStyle = WindowStyle.None;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void Window_MouseEnter(object sender, MouseEventArgs e)
    {
        FloatingWidgetText.FontSize += 5;
        FloatingWidgetBorder.Width += 20;
        FloatingWidgetBorder.Height += 20;
        FloatingWidgetText.Visibility = Visibility.Visible;
        FloatingWidgetCloseButton.Visibility = Visibility.Visible;
    }

    private void Window_MouseLeave(object sender, MouseEventArgs e)
    {
        FloatingWidgetText.FontSize -= 5;
        FloatingWidgetBorder.Width -= 20;
        FloatingWidgetBorder.Height -= 20;
        FloatingWidgetText.Visibility = Visibility.Hidden;
        FloatingWidgetCloseButton.Visibility = Visibility.Hidden;
    }

    private void Close(object sender, RoutedEventArgs e)
    {
        var app = Application.Current as App;

        app?.ToggleFloatingWidget();
    }

    // Override the Closing event to prevent the window from fully closing
    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;

        Hide();
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

    private void FloatingWidget_Loaded(object sender, RoutedEventArgs e)
    {
        SetFloatingWidgetOnTop();
    }
}
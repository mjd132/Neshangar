using System.Runtime.InteropServices;
using System.Windows.Threading;
using Neshangar.Core.Data;
using Neshangar.Core.Entities;

namespace Neshangar;

public class UserTracker
{
    private readonly Client _client;

    private DispatcherTimer _activityTimer;
    private bool _isAfk = false;

    public UserTracker(Client client)
    {
        _client = client;
        _activityTimer = new DispatcherTimer();
        _activityTimer.Interval = TimeSpan.FromSeconds(5);
        _activityTimer.Tick += CheckInactivity;
        _activityTimer.Start();
    }

    private void CheckInactivity(object? sender, EventArgs e)
    {
        int idleTime = GetIdleTime();

        if (!_isAfk && idleTime >= 10 * 60 * 1000)
        {
            _isAfk = true;
            _client.SetStatusViaTimerAsync(StatusEnum.AFK, null).ConfigureAwait(false);
        }
        else if (_isAfk && idleTime < 10 * 60 * 1000)
        {
            _isAfk = false;
            _client.SetStatusViaTimerAsync(StatusEnum.Online, null).ConfigureAwait(false);
        }
    }

    private int GetIdleTime()
    {
        LastInputInfo lii = new LastInputInfo
        {
            cbSize = (uint)Marshal.SizeOf(typeof(LastInputInfo))
        };

        if (GetLastInputInfo(ref lii))
        {
            return Environment.TickCount - (int)lii.dwTime;
        }

        return 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LastInputInfo
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LastInputInfo plii);
}
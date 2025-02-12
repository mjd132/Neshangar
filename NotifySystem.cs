using System.IO;
using Neshangar.Core.Data;
using Neshangar.Core.Entities;
using Neshangar.Windows;
using Application = System.Windows.Application;

namespace Neshangar
{
    public class NotifySystem
    {
        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenuStrip;
        public ToolStripMenuItem? toggleFloatingWidget;
        private FloatingWidget _floatingWidget;
        private UsersList _usersList;
        private readonly Settings _settings;
        private readonly ChangeStatus _changeStatus;
        private readonly Client _client;

        public NotifySystem(FloatingWidget floatingWidget, UsersList usersList, Settings settings,
            ChangeStatus changeStatus, Client client)
        {
            _floatingWidget = floatingWidget;
            _usersList = usersList;
            _settings = settings;
            _changeStatus = changeStatus;
            _client = client;
            InitNotifyIcon();
            InitContextMenu();
        }

        private void InitContextMenu()
        {
            _contextMenuStrip = new ContextMenuStrip();

            // Show / Hide Widget
            toggleFloatingWidget = new ToolStripMenuItem("Hide Widget");
            toggleFloatingWidget.Click += ToggleFloatingWidget;
            _contextMenuStrip.Items.Add(toggleFloatingWidget);

            // Divider
            _contextMenuStrip.Items.Add(new ToolStripSeparator());

            //Set to Busy for 15 minutes
            var setBusy = new ToolStripMenuItem("Busy for 15 minutes");
            setBusy.Click += async (object? sender, EventArgs e) =>
            {
                _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(15));
            };
            _contextMenuStrip.Items.Add(setBusy);

            //Set to Busy for 30 minutes
            setBusy = new ToolStripMenuItem("Busy for 30 minutes");
            setBusy.Click += async (object? sender, EventArgs e) =>
            {
                _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(30));
            };
            _contextMenuStrip.Items.Add(setBusy);

            //Set to Busy for 45 minutes
            setBusy = new ToolStripMenuItem("Busy for 40 minutes");
            setBusy.Click += async (object? sender, EventArgs e) =>
            {
                _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(45));
            };
            _contextMenuStrip.Items.Add(setBusy);

            //Set to Busy for 60 minutes
            setBusy = new ToolStripMenuItem("Busy for 60 minutes");
            setBusy.Click += async (object? sender, EventArgs e) =>
            {
                _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(60));
            };
            _contextMenuStrip.Items.Add(setBusy);

            // Divider
            _contextMenuStrip.Items.Add(new ToolStripSeparator());

            //Set to online
            var setOnline = new ToolStripMenuItem("Online");
            setOnline.Click += async (object? sender, EventArgs e) =>
            {
                _client.SetStatusViaTimer(StatusEnum.Online, null);
            };
            _contextMenuStrip.Items.Add(setOnline);

            //Set to idle
            var setIdle = new ToolStripMenuItem("Idle");
            setIdle.Click += async (object? sender, EventArgs e) =>
            {
                _client.SetStatusViaTimer(StatusEnum.Idle, null);
            };
            _contextMenuStrip.Items.Add(setIdle);

            //Set to afk
            var setAfk = new ToolStripMenuItem("AFK");
            setAfk.Click += async (object? sender, EventArgs e) => { _client.SetStatusViaTimer(StatusEnum.AFK, null); };
            _contextMenuStrip.Items.Add(setAfk);

            // Divider
            _contextMenuStrip.Items.Add(new ToolStripSeparator());

            //Users List
            var usersListItem = new ToolStripMenuItem("Users List");
            usersListItem.Click += UsersListItem_Click;
            _contextMenuStrip.Items.Add(usersListItem);

            //Change Status Window
            var changeStatusItem = new ToolStripMenuItem("Change Status");
            changeStatusItem.Click += ChangeStatusItem_Click;
            _contextMenuStrip.Items.Add(changeStatusItem);

            //Settings
            var settingsItem = new ToolStripMenuItem("Settings");
            settingsItem.Click += SettingsItem_Click;
            _contextMenuStrip.Items.Add(settingsItem);

            // Divider
            _contextMenuStrip.Items.Add(new ToolStripSeparator());

            // Exit
            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;
            _contextMenuStrip.Items.Add(exitMenuItem);


            _notifyIcon.ContextMenuStrip = _contextMenuStrip;
        }

        public void Hide()
        {
            if (_notifyIcon != null) _notifyIcon.Visible = false;
            _contextMenuStrip?.Hide();
        }

        public void Show()
        {
            if (_notifyIcon != null) _notifyIcon.Visible = true;
        }

        private void ChangeStatusItem_Click(object? sender, EventArgs e)
        {
            if (_changeStatus.IsVisible)
            {
                _changeStatus.Hide();
            }
            else
            {
                _changeStatus.Show();
            }
        }

        private void ToggleFloatingWidget(object? sender, EventArgs e)
        {
            if (_floatingWidget.IsVisible)
            {
                _floatingWidget.Hide();
                toggleFloatingWidget.Text = "Show Widget";
            }
            else
            {
                _floatingWidget.Show();
                toggleFloatingWidget.Text = "Hide Widget";
            }
        }

        private void UsersListItem_Click(object? sender, EventArgs e)
        {
            if (_usersList.IsVisible)
            {
                _usersList.Hide();
            }
            else
            {
                _usersList.Show();
            }
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            Application.Current.Shutdown();
        }

        private void SettingsItem_Click(object? sender, EventArgs e)
        {
            if (_settings.IsVisible)
            {
                _settings.Hide();
            }
            else
            {
                _settings.Show();
            }
        }

        private void InitNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();
            var iconUri = new Uri("pack://application:,,,/Resources/icon.ico", UriKind.Absolute);
            var iconStream = Application.GetResourceStream(iconUri)?.Stream;
            if (iconStream == null)
            {
                throw new FileNotFoundException("Icon file not found in resources.");
            }
            
            _notifyIcon.Icon = new Icon(iconStream);
            _notifyIcon.Text = "Neshangar";
            _notifyIcon.Click += (_, _) =>
            {
                _floatingWidget.Show();
                _floatingWidget.Activate();
                _floatingWidget.Focus();
            };
            _notifyIcon.DoubleClick += (_, _) =>
            {
                _changeStatus.Show();
                _changeStatus.Activate();
                _changeStatus.Focus();
            };
        }
    }
}
using Neshangar.Core.Data;
using Neshangar.Core.Entities;
using Neshangar.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        public NotifySystem(FloatingWidget floatingWidget, UsersList usersList, Settings settings, ChangeStatus changeStatus, Client client)
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
            setBusy.Click += async (object? sender, EventArgs e) => { await _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(15)); };
            _contextMenuStrip.Items.Add(setBusy);

            //Set to Busy for 30 minutes
            setBusy = new ToolStripMenuItem("Busy for 30 minutes");
            setBusy.Click += async (object? sender, EventArgs e) => { await _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(30)); };
            _contextMenuStrip.Items.Add(setBusy);

            //Set to Busy for 45 minutes
            setBusy = new ToolStripMenuItem("Busy for 40 minutes");
            setBusy.Click += async (object? sender, EventArgs e) => { await _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(45)); };
            _contextMenuStrip.Items.Add(setBusy);

            //Set to Busy for 60 minutes
            setBusy = new ToolStripMenuItem("Busy for 60 minutes");
            setBusy.Click += async (object? sender, EventArgs e) => { await _client.SetStatusViaTimer(StatusEnum.Busy, TimeSpan.FromMinutes(60)); };
            _contextMenuStrip.Items.Add(setBusy);

            // Divider
            _contextMenuStrip.Items.Add(new ToolStripSeparator());

            //Set to online
            var setOnline = new ToolStripMenuItem("Online");
            setOnline.Click += async (object? sender, EventArgs e) => { await _client.SetStatusViaTimer(StatusEnum.Online, null); };
            _contextMenuStrip.Items.Add(setOnline);

            //Set to idle
            var setIdle = new ToolStripMenuItem("Idle");
            setIdle.Click += async (object? sender, EventArgs e) => { await _client.SetStatusViaTimer(StatusEnum.Idle, null); };
            _contextMenuStrip.Items.Add(setIdle);

            //Set to afk
            var setAfk = new ToolStripMenuItem("AFK");
            setAfk.Click += async (object? sender, EventArgs e) => { await _client.SetStatusViaTimer(StatusEnum.AFK, null); };
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
            var app = System.Windows.Application.Current as App;

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
            System.Windows.Application.Current.Shutdown();
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
            _notifyIcon.Icon = new Icon("red-circle.ico");
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Neshangar";
            _notifyIcon.Click += (object? sender, EventArgs e) =>
            {
                _floatingWidget.Show();
                _floatingWidget.Activate();
                _floatingWidget.Focus();
            };
            _notifyIcon.DoubleClick += (object? sender, EventArgs e) =>
            {
                _changeStatus.Show();
                _changeStatus.Activate();
                _changeStatus.Focus();
            };
        }
    }
}

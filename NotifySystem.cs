using Neshangar.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Neshangar
{
    public class NotifySystem
    {
        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenuStrip;
        public void Init()
        {
            InitNotifyIcon();
            InitContextMenu();
        }

        private void InitContextMenu()
        {
            _contextMenuStrip = new ContextMenuStrip();

            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;

            var showWidgetMenuItem = new ToolStripMenuItem("Hide Widget");
            showWidgetMenuItem.Click += ToggleFloatingWidget;

            _contextMenuStrip.Items.AddRange(new ToolStripMenuItem[] { showWidgetMenuItem, exitMenuItem });

            _notifyIcon.ContextMenuStrip = _contextMenuStrip;
        }

        private void ToggleFloatingWidget(object sender, EventArgs e)
        {

            var app = System.Windows.Application.Current as App;
            var menuItem = sender as ToolStripMenuItem;

            if (app.floatingWidget.IsVisible)
            {
                app.floatingWidget.Hide();
                menuItem.Text = "Show Widget";

            }
            else
            {
                app.floatingWidget.Show();
                menuItem.Text = "Hide Widget";

            }

        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            System.Windows.Application.Current.Shutdown();
        }

        private void InitNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new Icon("red-circle.ico");
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "Neshangar";
        }
    }
}

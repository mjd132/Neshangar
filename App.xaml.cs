using Neshangar.Windows;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Neshangar
{

    public partial class App : System.Windows.Application
    {
        private NotifySystem? _notifySystem;
        public FloatingWidget floatingWidget;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Show the floating widget instead of the main window
            floatingWidget = new FloatingWidget();
            floatingWidget.Show();

            _notifySystem = new NotifySystem();
            _notifySystem.Init();
            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

    }

}

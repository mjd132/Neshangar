using System;
using System.Collections.Generic;
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
    /// Interaction logic for Widget.xaml
    /// </summary>
    public partial class FloatingWidget : Window
    {
        public FloatingWidget()
        {
            AdjustWindowSize();
            InitializeComponent();
            AdjustComponent();

        }

        private void AdjustComponent()
        {
            var screenWidth = SystemParameters.PrimaryScreenHeight * 0.01;
            FloatingWidgetText.FontSize = screenWidth;
            FloatingWidgetBorder.CornerRadius = new CornerRadius(screenWidth / 2);

            //FloatingWidgetBorder.Width = screenWidth;
            //FloatingWidgetBorder.Height = screenWidth;

            FloatingWidgetText.Visibility = Visibility.Hidden;
        }

        private void AdjustWindowSize()
        {
            double screenWidth = SystemParameters.PrimaryScreenHeight * 0.025;

            var workingArea = Screen.PrimaryScreen.WorkingArea;

            // Set the position of the floating widget
            this.Left = workingArea.Right - this.ActualWidth;
            this.Top = workingArea.Bottom - this.ActualHeight;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            FloatingWidgetText.Text = "3:00";
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

        private void Close (object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        // Override the Closing event to prevent the window from fully closing
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }
    }
}

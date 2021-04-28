using System;
using System.Windows;

namespace CeVIOAIProxy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        public MainWindow()
        {
            Instance = this;

            this.InitializeComponent();

            if (Config.Instance.IsMinimizeStartup)
            {
                this.ShowInTaskbar = false;
                this.WindowState = WindowState.Minimized;

                this.Loaded += (_, _) =>
                {
                    this.ToHide();
                    this.ShowInTaskbar = true;
                };
            }
            else
            {
                this.Loaded += (_, _) => this.Activate();
            }

            this.StateChanged += this.MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ToHide();
            }
            else
            {
                this.ToShow();
            }
        }

        public void ToShow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.NotifyIcon.Visibility = Visibility.Collapsed;

            this.Activate();
        }

        public void ToHide()
        {
            this.NotifyIcon.Visibility = Visibility.Visible;
            this.Hide();
        }

        public void HideNotifyIcon()
        {
            this.NotifyIcon.Visibility = Visibility.Collapsed;
        }

        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            this.ToShow();
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            this.HideNotifyIcon();
            this.Close();
        }
    }
}

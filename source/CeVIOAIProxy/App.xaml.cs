using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace CeVIOAIProxy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.ShutdownMode = ShutdownMode.OnMainWindowClose;
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            this.Startup += this.App_Startup;
            this.Exit += this.App_Exit;
            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var c = Config.Instance;
            c.SetStartup(c.IsStartupWithWindows);
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            CeVIOAIProxy.MainWindow.Instance?.ToHide();
            Config.Instance.Save();
        }

        private async void App_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            await Task.Run(() =>
            {
                File.WriteAllText(
                    @".\CeVIOAIProxy.error.log",
                    e.Exception.ToString(),
                    new UTF8Encoding(false));
            });

            MessageBox.Show(
                "予期しない例外を検知しました。アプリケーションを終了します。\n\n" +
                e.Exception,
                "Fatal",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            CeVIOAIProxy.MainWindow.Instance?.ToHide();
            Config.Instance.Save();

            this.Shutdown(1);
        }
    }
}

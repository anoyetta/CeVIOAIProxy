using Prism.Mvvm;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CeVIOAIProxy.Servers
{
    public class IpcRemotingServerController : BindableBase, IDisposable
    {
        public static IpcRemotingServerController Current { get; private set; }

        public IpcRemotingServerController()
        {
            Current = this;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(disposing: true);
        }

        ~IpcRemotingServerController() => this.Dispose(disposing: false);

        protected virtual void Dispose(bool disposing) => this.Close();

        private bool isAvailable;

        public bool IsAvailable
        {
            get => this.isAvailable;
            set => this.SetProperty(ref this.isAvailable, value);
        }

        private IpcRemotingServer ipcServer;
        private FNF.Utility.BouyomiChanRemoting ipcRemoteObject;

        public async void Open()
        {
            await Task.Run(() =>
            {
                lock (this)
                {
                    this.ipcServer = new IpcRemotingServer(Config.Instance.IPCChannelName);
                    this.ipcRemoteObject = new FNF.Utility.BouyomiChanRemoting();
                    this.ipcServer.RegisterRemotingObject(this.ipcRemoteObject, "Remoting");

                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        this.IsAvailable = true;
                    });
                }
            });
        }

        public void Close()
        {
            if (this.ipcServer != null)
            {
                lock (this)
                {
                    this.ipcServer.Dispose();
                    this.ipcServer = null;
                    this.ipcRemoteObject = null;
                    this.IsAvailable = false;
                }
            }
        }
    }
}

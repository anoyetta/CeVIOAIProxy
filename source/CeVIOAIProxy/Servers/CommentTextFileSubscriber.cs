using System;
using System.IO;
using System.Text;

namespace CeVIOAIProxy.Servers
{
    internal class CommentTextFileSubscriber : IDisposable
    {
        public static CommentTextFileSubscriber Current { get; private set; }

        public CommentTextFileSubscriber()
        {
            Current = this;
        }

        public void Dispose()
        {
            Current = null;

            GC.SuppressFinalize(this);
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();
        }


        private FileSystemWatcher watcher;

        public void Start()
        {
            if (!Config.Instance.IsEnabledTextPolling)
            {
                return;
            }

            if (string.IsNullOrEmpty(Config.Instance.CommentTextFilePath))
            {
                return;
            }

            lock (this)
            {
                this.watcher = new FileSystemWatcher();
                this.watcher.Path = Path.GetDirectoryName(Config.Instance.CommentTextFilePath);
                this.watcher.Filter = Path.GetFileName(Config.Instance.CommentTextFilePath);
                this.watcher.IncludeSubdirectories = false;
                this.watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
                this.watcher.InternalBufferSize = 64 * 1024;

                this.watcher.Created += this.Watcher_Created;
                this.watcher.Changed += this.Watcher_Changed;

                this.watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (this.watcher != null)
                {
                    this.watcher.EnableRaisingEvents = false;
                    this.watcher.Dispose();
                    this.watcher = null;
                }
            }
        }

        private async void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                if (!Config.Instance.IsEnabledTextPolling)
                {
                    return;
                }

                if (this.watcher == null)
                {
                    return;
                }
            }

            var comment = File.ReadAllText(e.FullPath, new UTF8Encoding(false));
            await CeVIO.SpeakAsync(comment);
        }

        private async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                if (!Config.Instance.IsEnabledTextPolling)
                {
                    return;
                }

                if (this.watcher == null)
                {
                    return;
                }
            }

            var comment = File.ReadAllText(e.FullPath, new UTF8Encoding(false));
            await CeVIO.SpeakAsync(comment);
        }
    }
}

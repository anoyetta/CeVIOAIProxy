using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
                this.watcher.NotifyFilter = NotifyFilters.LastWrite;
                this.watcher.InternalBufferSize = 64 * 1024;

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

        private string lastComment = string.Empty;
        private DateTime lastCommentTimestamp = DateTime.MinValue;

        private async void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var comment = string.Empty;

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

            var interval = 10;
            var timeout = 1000;

            for (int i = 0; i < (timeout / interval); i++)
            {
                try
                {
                    comment = File.ReadAllText(e.FullPath, new UTF8Encoding(false));
                    break;
                }
                catch (IOException)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(interval));
                }
            }

            if (string.IsNullOrEmpty(comment))
            {
                return;
            }

            lock (this)
            {
                var now = DateTime.Now;

                if ((now - this.lastCommentTimestamp) < TimeSpan.FromSeconds(1))
                {
                    if (this.lastComment.Equals(comment))
                    {
                        return;
                    }
                }

                this.lastComment = comment;
                this.lastCommentTimestamp = now;
            }

            await CeVIO.SpeakAsync(comment);
        }
    }
}

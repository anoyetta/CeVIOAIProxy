using Hjson;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace CeVIOAIProxy.Servers
{
    public class BouyomiChanHttpServer : IDisposable
    {
        private HttpListener server;

        public BouyomiChanHttpServer(
            int port)
        {
            this.server = new HttpListener();
            this.server.Prefixes.Add($"http://localhost:{port}/");
            this.server.Start();
            this.server.BeginGetContext(this.GetContextCallback, null);
        }

        ~BouyomiChanHttpServer()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(disposing: true);
        }

        private volatile bool isClosing;

        protected virtual void Dispose(bool disposing)
        {
            if (this.server != null)
            {
                this.isClosing = true;
                this.server.Close();
                this.server = null;
            }
        }

        private void GetContextCallback(
            IAsyncResult result)
        {
            if (this.server == null || this.isClosing)
            {
                return;
            }

            try
            {
                var context = this.server.EndGetContext(result);

                try
                {
                    this.ProcessContext(context);
                }
                finally
                {
                    context.Response.Close();
                }
            }
            finally
            {
                this.server.BeginGetContext(this.GetContextCallback, null);
            }
        }

        private async void ProcessContext(
            HttpListenerContext context)
        {
            var request = context.Request;
            if (request == null)
            {
                return;
            }

            if (!request.Url.LocalPath.ToLower().Contains("/talk"))
            {
                return;
            }

            var query = HttpUtility.ParseQueryString(request.Url.Query, Encoding.UTF8);
            var text = query["text"];
            var callback = query["callback"];

            var response = context.Response;
            var outputEncoding = Encoding.UTF8;

            response.Headers.Add(HttpResponseHeader.CacheControl, "no-cache");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.ContentEncoding = outputEncoding;
            response.ContentType = callback == null ?
                $"application/json;charset={outputEncoding.HeaderName}" :
                $"application/javascript;charset={outputEncoding.HeaderName}";

            var json = new JsonObject();
            json.Add("taskId", this.GetTaskId());

            using (var bw = new BinaryWriter(response.OutputStream))
            {
                var contents = callback == null ?
                    json.ToString() :
                    $"{callback}({json})";

                bw.Write(outputEncoding.GetBytes(contents));
            }

            await CeVIO.SpeakAsync(text);
        }

        private int taskId;

        private int GetTaskId() => Interlocked.Increment(ref this.taskId);
    }
}

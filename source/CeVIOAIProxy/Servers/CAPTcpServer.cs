using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CeVIOAIProxy.Servers
{
    public class CAPTcpServer : IDisposable
    {
        public CAPTcpServer()
        {
        }

        private TcpListener listener;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing) => this.Close();

        ~CAPTcpServer() => this.Dispose(disposing: false);

        public void Open(
            int port)
        {
            this.isClosing = false;

            this.listener = new TcpListener(IPAddress.Any, port);

            /*
            this.listener.Server.SetSocketOption(
                SocketOptionLevel.Socket,
                SocketOptionName.ReuseAddress,
                true);
            */

            this.listener.Start();
            this.listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
        }

        public void Close()
        {
            if (this.listener != null)
            {
                this.isClosing = true;
                this.listener.Server.Close();
                this.listener.Stop();
                this.listener = null;
            }
        }

        private volatile bool isClosing;

        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            if (this.listener == null || this.isClosing)
            {
                return;
            }

            try
            {
                using (var tcpClient = this.listener.EndAcceptTcpClient(ar))
                using (var s = tcpClient.GetStream())
                using (var br = new BinaryReader(s))
                {
                    this.ProcessStream(s, br);
                }
            }
            finally
            {
                this.listener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
            }
        }

        private async void ProcessStream(
            NetworkStream stream,
            BinaryReader br)
        {
            var opcode = br.ReadInt16();

            var speed = default(short);
            var tone = default(short);
            var volume = default(short);
            var type = default(short);
            var encode = default(byte);
            var count = default(int);
            var chars = default(byte[]);
            var text = string.Empty;

            switch (opcode)
            {
                case 0:
                    speed = br.ReadInt16();
                    volume = br.ReadInt16();
                    type = br.ReadInt16();
                    encode = br.ReadByte();
                    count = br.ReadInt32();
                    chars = br.ReadBytes(count);

                    text = encode switch
                    {
                        0 => Encoding.UTF8.GetString(chars),
                        1 => Encoding.Unicode.GetString(chars),
                        2 => Encoding.GetEncoding("Shift_JIS").GetString(chars),
                        _ => Encoding.UTF8.GetString(chars),
                    };

                    await CeVIO.SpeakAsync(text);
                    break;

                case 1:
                    speed = br.ReadInt16();
                    tone = br.ReadInt16();
                    volume = br.ReadInt16();
                    type = br.ReadInt16();
                    encode = br.ReadByte();
                    count = br.ReadInt32();
                    chars = br.ReadBytes(count);

                    text = encode switch
                    {
                        0 => Encoding.UTF8.GetString(chars),
                        1 => Encoding.Unicode.GetString(chars),
                        2 => Encoding.GetEncoding("Shift_JIS").GetString(chars),
                        _ => Encoding.UTF8.GetString(chars),
                    };

                    await CeVIO.SpeakAsync(text);
                    break;

                case 16:
                    // ポーズ
                    // 未実装
                    break;

                case 32:
                    // ポーズ解除
                    // 未実装
                    break;

                case 48:
                    // 次のキューへ
                    // 未実装
                    break;

                case 64:
                    // キューをクリアする
                    // 未実装
                    break;

                case 272:
                    // ポーズ中か返す
                    // 未実装
                    break;

                case 288:
                    // 再生中か返す
                    // 未実装
                    break;

                case 304:
                    // 現在のキュー数を返す
                    // 未実装
                    break;

                case 12078:
                    // コマンドを実行する
                    // 未実装
                    break;
            }
        }
    }
}

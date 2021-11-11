using CeVIO.Talk.RemoteService2;
using CeVIOAIProxy.Servers;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CeVIOAIProxy
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
        }

        public Config Config => Config.Instance;

        public ObservableCollection<string> Casts { get; } = new ObservableCollection<string>();

        public ObservableCollection<CeVIOTalkerComponent> CurrentComponets { get; } = new ObservableCollection<CeVIOTalkerComponent>();

        public async void OnLoaded()
        {
            if (!ServiceControl2.IsHostStarted)
            {
                await Task.Run(() => ServiceControl2.StartHost(false));
            }

            var talker = new Talker2();

            foreach (var cast in Talker2.AvailableCasts)
            {
                this.Casts.Add(cast);

                talker.Cast = cast;
                var components = talker.Components;

                var isFirst = true;
                foreach (var c in components)
                {
                    if (!this.Config.Components.Any(x => x.ID == c.Id))
                    {
                        this.Config.Components.Add(new CeVIOTalkerComponent()
                        {
                            Cast = cast,
                            ID = c.Id,
                            Name = c.Name,
                            Value = (uint)(isFirst ? 100 : 0),
                        });

                        isFirst = false;
                    }
                }
            }

            this.Config.Save();

            this.Config.OnCastChanged += (_, _) => this.SetCurrentComponents();
            this.SetCurrentComponents();
        }

        private void SetCurrentComponents()
        {
            this.CurrentComponets.Clear();

            if (string.IsNullOrEmpty(this.Config.Cast))
            {
                return;
            }

            var nexts = this.Config.Components
                .Where(x => x.Cast == this.Config.Cast);

            foreach (var item in nexts)
            {
                this.CurrentComponets.Add(item);
            }
        }

        private string ipcServerStatus;

        public string IPCServerStatus
        {
            get => this.ipcServerStatus;
            set => this.SetProperty(ref this.ipcServerStatus, value);
        }

        private DelegateCommand testCommand;

        public DelegateCommand TestCommand =>
            this.testCommand ?? (this.testCommand = new DelegateCommand(this.ExecuteTestCommand));

        private async void ExecuteTestCommand()
        {
            await CeVIO.SpeakAsync("CeVIOへの接続は正常です。");

            await Task.Run(() =>
            {
                using (var tcp = new TcpClient("127.0.0.1", this.Config.TcpServerPort))
                using (var ns = tcp.GetStream())
                using (var bw = new BinaryWriter(ns))
                {
                    var sMessage = "TCPからの接続は正常です。";

                    byte bCode = 0;
                    short iVoice = 1;
                    short iVolume = -1;
                    short iSpeed = -1;
                    short iTone = -1;
                    short iCommand = 0x0001;

                    var bMessage = Encoding.UTF8.GetBytes(sMessage);
                    var iLength = bMessage.Length;

                    bw.Write(iCommand); // コマンド（ 0:メッセージ読み上げ）
                    bw.Write(iSpeed);   // 速度    （-1:棒読みちゃん画面上の設定）
                    bw.Write(iTone);    // 音程    （-1:棒読みちゃん画面上の設定）
                    bw.Write(iVolume);  // 音量    （-1:棒読みちゃん画面上の設定）
                    bw.Write(iVoice);   // 声質    （ 0:棒読みちゃん画面上の設定、1:女性1、2:女性2、3:男性1、4:男性2、5:中性、6:ロボット、7:機械1、8:機械2、10001～:SAPI5）
                    bw.Write(bCode);    // 文字列のbyte配列の文字コード(0:UTF-8, 1:Unicode, 2:Shift-JIS)
                    bw.Write(iLength);  // 文字列のbyte配列の長さ
                    bw.Write(bMessage); // 文字列のbyte配列

                    bw.Flush();
                }
            });
        }
    }
}

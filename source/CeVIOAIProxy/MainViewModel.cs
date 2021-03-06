using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CeVIO.Talk.RemoteService2;
using CeVIOAIProxy.Servers;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;

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

            this.Config.OnCommentFileSubscriberChanged += (_, _) =>
            {
                CommentTextFileSubscriber.Current?.Stop();

                if (this.Config.IsEnabledTextPolling)
                {
                    CommentTextFileSubscriber.Current?.Start();
                }
            };

            if (this.Config.IsEnabledTextPolling)
            {
                CommentTextFileSubscriber.Current?.Start();
            }
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

        private DelegateCommand openFileCommand;

        public DelegateCommand OpenFileCommand =>
            this.openFileCommand ?? (this.openFileCommand = new DelegateCommand(this.ExecuteOpenFileCommand));

        private void ExecuteOpenFileCommand()
        {
            var d = new OpenFileDialog()
            {
                FilterIndex = 1,
                Filter = "????????????????????????|*.txt|????????????????????????|*.*",
                RestoreDirectory = true
            };

            if (File.Exists(Config.Instance.CommentTextFilePath))
            {
                d.InitialDirectory = Path.GetDirectoryName(Config.Instance.CommentTextFilePath);
                d.FileName = Path.GetFileName(Config.Instance.CommentTextFilePath);
            }

            var result = d.ShowDialog() ?? false;
            if (result)
            {
                Config.Instance.CommentTextFilePath = d.FileName;
            }
        }

        private DelegateCommand testCommand;

        public DelegateCommand TestCommand =>
            this.testCommand ?? (this.testCommand = new DelegateCommand(this.ExecuteTestCommand));

        private async void ExecuteTestCommand()
        {
            await CeVIO.SpeakAsync("CeVIO??????????????????????????????");

            await Task.Run(() =>
            {
                using (var tcp = new TcpClient("127.0.0.1", this.Config.TcpServerPort))
                using (var ns = tcp.GetStream())
                using (var bw = new BinaryWriter(ns))
                {
                    var sMessage = "TCP?????????????????????????????????";

                    byte bCode = 0;
                    short iVoice = 1;
                    short iVolume = -1;
                    short iSpeed = -1;
                    short iTone = -1;
                    short iCommand = 0x0001;

                    var bMessage = Encoding.UTF8.GetBytes(sMessage);
                    var iLength = bMessage.Length;

                    bw.Write(iCommand); // ??????????????? 0:??????????????????????????????
                    bw.Write(iSpeed);   // ??????    ???-1:???????????????????????????????????????
                    bw.Write(iTone);    // ??????    ???-1:???????????????????????????????????????
                    bw.Write(iVolume);  // ??????    ???-1:???????????????????????????????????????
                    bw.Write(iVoice);   // ??????    ??? 0:???????????????????????????????????????1:??????1???2:??????2???3:??????1???4:??????2???5:?????????6:???????????????7:??????1???8:??????2???10001???:SAPI5???
                    bw.Write(bCode);    // ????????????byte????????????????????????(0:UTF-8, 1:Unicode, 2:Shift-JIS)
                    bw.Write(iLength);  // ????????????byte???????????????
                    bw.Write(bMessage); // ????????????byte??????

                    bw.Flush();
                }
            });
        }
    }
}

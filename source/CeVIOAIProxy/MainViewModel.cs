using System.Collections.ObjectModel;
using System.Speech.Synthesis;
using CeVIO.Talk.RemoteService2;
using Prism.Commands;
using Prism.Mvvm;

namespace CeVIOAIProxy
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            using (var synth = new SpeechSynthesizer())
            {
                foreach (var voice in synth.GetInstalledVoices())
                {
                    this.Voices.Add(voice);
                }
            }
        }

        public Config Config => Config.Instance;

        public ObservableCollection<InstalledVoice> Voices { get; } = new ObservableCollection<InstalledVoice>();

        private DelegateCommand testCommand;

        public DelegateCommand TestCommand =>
            this.testCommand ?? (this.testCommand = new DelegateCommand(this.ExecuteTestCommand));

        private void ExecuteTestCommand()
        {
            if (!ServiceControl2.IsHostStarted)
            {
                ServiceControl2.StartHost(false);
            }

            var talker = new Talker2();

            talker.Cast = "IA";
            var components = talker.Components;
            var stat = talker.Speak("本日は晴天なり。");
            stat.Wait();
        }
    }
}

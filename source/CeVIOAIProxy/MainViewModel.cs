using System.Collections.ObjectModel;
using System.Speech.Synthesis;
using Prism.Mvvm;

namespace CeVIOAIProxy
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            var synth = new SpeechSynthesizer();

            foreach (var v in synth.GetInstalledVoices())
            {
                this.Voices.Add(v.VoiceInfo);
            }
        }

        public Config Config => Config.Instance;

        public ObservableCollection<VoiceInfo> Voices = new ObservableCollection<VoiceInfo>();
    }
}

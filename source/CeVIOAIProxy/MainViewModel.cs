using System.Collections.ObjectModel;
using System.Speech.Synthesis;
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
    }
}

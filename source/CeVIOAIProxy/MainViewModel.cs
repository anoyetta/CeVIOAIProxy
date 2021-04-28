using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CeVIO.Talk.RemoteService2;
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

                foreach (var c in components)
                {
                    if (!this.Config.Components.Any(x => x.ID == c.Id))
                    {
                        this.Config.Components.Add(new CeVIOTalkerComponent()
                        {
                            Cast = cast,
                            ID = c.Id,
                            Name = c.Name,
                            Value = 50,
                        });
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

        private DelegateCommand testCommand;

        public DelegateCommand TestCommand =>
            this.testCommand ?? (this.testCommand = new DelegateCommand(this.ExecuteTestCommand));

        private async void ExecuteTestCommand()
        {
            await CeVIO.SpeakAsync("本日は晴天なり。");
        }
    }
}

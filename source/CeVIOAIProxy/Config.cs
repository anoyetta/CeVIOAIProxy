using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace CeVIOAIProxy
{
    public partial class Config : JsonConfigBase
    {
        #region Lazy Singleton

        private readonly static Lazy<Config> instance = new Lazy<Config>(() => Load<Config>(_fileName, out bool _));

        public static Config Instance => instance.Value;

        public Config()
        {
        }

        #endregion Lazy Singleton

        private const string _fileName = @".\CeVIOAIProxy.config.json";

        public override string FileName => _fileName;

        [JsonIgnore]
        public string AppName
        {
            get
            {
                var att = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                return att.Length == 0 ? string.Empty : ((AssemblyTitleAttribute)att[0]).Title;
            }
        }

        [JsonIgnore]
        public string AppNameWithVersion => $"{this.AppName} - {this.AppVersionString}";

        [JsonIgnore]
        public Version AppVersion => Assembly.GetExecutingAssembly().GetName().Version;

        [JsonIgnore]
        public string AppVersionString => $"v{this.AppVersion}";

        private bool isStartupWithWindows;

        [JsonProperty(PropertyName = "is_startup_with_windows")]
        public bool IsStartupWithWindows
        {
            get => this.isStartupWithWindows;
            set
            {
                if (this.SetProperty(ref this.isStartupWithWindows, value))
                {
                    this.SetStartup(value);
                }
            }
        }

        public async void SetStartup(
            bool isStartup) =>
            await Task.Run(() =>
            {
                using (var regkey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run",
                    true))
                {
                    var att = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    var product = att.Length == 0 ? string.Empty : ((AssemblyProductAttribute)att[0]).Product;

                    if (isStartup)
                    {
                        regkey.SetValue(
                            product,
                            $"\"{Assembly.GetExecutingAssembly().Location}\"");
                    }
                    else
                    {
                        regkey.DeleteValue(
                            product,
                            false);
                    }
                }
            });

        private bool isMinimizeStartup;

        [JsonProperty(PropertyName = "is_minimize_startup")]
        public bool IsMinimizeStartup
        {
            get => this.isMinimizeStartup;
            set => this.SetProperty(ref this.isMinimizeStartup, value);
        }

        private string voiceID;

        [JsonProperty(PropertyName = "voice_id")]
        public string VoiceID
        {
            get => this.voiceID;
            set => this.SetProperty(ref this.voiceID, value);
        }

        private int rate;

        [JsonProperty(PropertyName = "rate")]
        public int Rate
        {
            get => this.rate;
            set => this.SetProperty(ref this.rate, value);
        }

        private string cast;

        [JsonProperty(PropertyName = "cast")]
        public string Cast
        {
            get => this.cast;
            set => this.SetProperty(ref this.cast, value);
        }

        private uint volume;

        [JsonProperty(PropertyName = "volume")]
        public uint Volume
        {
            get => this.volume;
            set => this.SetProperty(ref this.volume, value);
        }

        private uint tone;

        [JsonProperty(PropertyName = "tone")]
        public uint Tone
        {
            get => this.tone;
            set => this.SetProperty(ref this.tone, value);
        }

        private uint alpha;

        [JsonProperty(PropertyName = "alpha")]
        public uint Alpha
        {
            get => this.alpha;
            set => this.SetProperty(ref this.alpha, value);
        }

        private uint toneScale;

        [JsonProperty(PropertyName = "tone_scale")]
        public uint ToneScale
        {
            get => this.toneScale;
            set => this.SetProperty(ref this.toneScale, value);
        }

        private ObservableCollection<CeVIOTalkerComponent> components = new ObservableCollection<CeVIOTalkerComponent>();

        [JsonProperty(PropertyName = "components")]
        public ObservableCollection<CeVIOTalkerComponent> Components
        {
            get => this.components;
            set => this.SetProperty(ref this.components, value);
        }
    }

    public class CeVIOTalkerComponent : BindableBase
    {
        private string cast;

        [JsonProperty(PropertyName = "cast")]
        public string Cast
        {
            get => this.cast;
            set => this.SetProperty(ref this.cast, value);
        }

        private string id;

        [JsonProperty(PropertyName = "id")]
        public string ID
        {
            get => this.id;
            set => this.SetProperty(ref this.id, value);
        }

        private string name;

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        private uint value;

        [JsonProperty(PropertyName = "value")]
        public uint Value
        {
            get => this.value;
            set => this.SetProperty(ref this.value, value);
        }
    }
}

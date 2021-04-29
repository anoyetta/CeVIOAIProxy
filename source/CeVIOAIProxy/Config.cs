using System;
using System.Collections.ObjectModel;
using System.IO;
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

        #endregion Lazy Singleton

        public Config()
        {
            this.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(this.Cast))
                {
                    this.OnCastChanged?.Invoke(this, new EventArgs());
                }
            };
        }

        public static readonly string AppData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "anoyetta",
            "CeVIOAIProxy");

        private static readonly string _fileName = Path.Combine(AppData, @"CeVIOAIProxy.config.json");

        public override string FileName => _fileName;

        public event EventHandler OnCastChanged;

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

        private double x;

        [JsonProperty(PropertyName = "x")]
        public double X
        {
            get => this.x;
            set => this.SetProperty(ref this.x, Math.Round(value));
        }

        private double y;

        [JsonProperty(PropertyName = "y")]
        public double Y
        {
            get => this.y;
            set => this.SetProperty(ref this.y, Math.Round(value));
        }

        private bool isMinimizeStartup;

        [JsonProperty(PropertyName = "is_minimize_startup")]
        public bool IsMinimizeStartup
        {
            get => this.isMinimizeStartup;
            set => this.SetProperty(ref this.isMinimizeStartup, value);
        }

        private int tcpServerPort = TcpServerPortDefaultValue;

        [JsonProperty(PropertyName = "tcp_server_port")]
        public int TcpServerPort
        {
            get => this.tcpServerPort;
            set => this.SetProperty(ref this.tcpServerPort, value);
        }

        private string cast;

        [JsonProperty(PropertyName = "cast")]
        public string Cast
        {
            get => this.cast;
            set => this.SetProperty(ref this.cast, value);
        }

        private uint volume = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "volume")]
        public uint Volume
        {
            get => this.volume;
            set => this.SetProperty(ref this.volume, value);
        }

        private uint speed = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "speed")]
        public uint Speed
        {
            get => this.speed;
            set => this.SetProperty(ref this.speed, value);
        }

        private uint tone = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "tone")]
        public uint Tone
        {
            get => this.tone;
            set => this.SetProperty(ref this.tone, value);
        }

        private uint alpha = CeVIOBasicParameterDefaultValue;

        [JsonProperty(PropertyName = "alpha")]
        public uint Alpha
        {
            get => this.alpha;
            set => this.SetProperty(ref this.alpha, value);
        }

        private uint toneScale = CeVIOBasicParameterDefaultValue;

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

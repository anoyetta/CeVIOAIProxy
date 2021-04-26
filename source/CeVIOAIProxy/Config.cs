using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace CeVIOAIProxy
{
    public partial class Config : JsonConfigBase
    {
        #region Lazy Singleton

        private readonly static Lazy<Config> instance = new Lazy<Config>(Load<Config>(_fileName, out bool _));

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

        private string voice;

        [JsonProperty(PropertyName = "voice")]
        public string Voice
        {
            get => this.voice;
            set => this.SetProperty(ref this.voice, value);
        }
    }
}

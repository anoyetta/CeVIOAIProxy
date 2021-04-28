using System.Linq;
using System.Threading.Tasks;
using CeVIO.Talk.RemoteService2;

namespace CeVIOAIProxy
{
    public static class CeVIO
    {
        private static readonly object Locker = new object();

        public async static Task SpeakAsync(
            string text,
            Config config = null)
            => await Task.Run(() => Speak(text, config));

        public static void Speak(
            string text,
            Config config = null)
        {
            if (config == null)
            {
                config = Config.Instance;
            }

            if (string.IsNullOrEmpty(config.Cast))
            {
                return;
            }

            if (!ServiceControl2.IsHostStarted)
            {
                return;
            }

            lock (Locker)
            {
                var talker = new Talker2();

                talker.Cast = config.Cast;
                talker.Volume = config.Volume;
                talker.Speed = config.Speed;
                talker.Tone = config.Tone;
                talker.Alpha = config.Alpha;
                talker.ToneScale = config.ToneScale;

                foreach (var c in talker.Components)
                {
                    var componentConfig = config.Components
                        .FirstOrDefault(x => x.ID == c.Id);

                    if (componentConfig != null)
                    {
                        c.Value = componentConfig.Value;
                    }
                }

                var stat = talker.Speak(text);
                stat.Wait();
            }
        }
    }
}

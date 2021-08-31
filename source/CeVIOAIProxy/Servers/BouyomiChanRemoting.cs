using System;

namespace CeVIOAIProxy.Servers
{
    public class BouyomiChanRemoting : MarshalByRefObject
    {
        public async void AddTalkTask(string sTalkText)
        {
            await CeVIO.SpeakAsync(sTalkText);
        }

        public async void AddTalkTask(string sTalkText, int iSpeed, int iVolume, int vType)
        {
            await CeVIO.SpeakAsync(sTalkText);
        }

        public async void AddTalkTask(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            await CeVIO.SpeakAsync(sTalkText);
        }

        public int AddTalkTask2(string sTalkText)
        {
            throw null;
        }

        public int AddTalkTask2(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            throw null;
        }

        public void ClearTalkTasks()
        {
        }

        public void SkipTalkTask()
        {
        }

        public int TalkTaskCount { get { throw null; } }
        public int NowTaskId { get { throw null; } }
        public bool NowPlaying { get { throw null; } }
        public bool Pause { get { throw null; } set { } }
    }
}

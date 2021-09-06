using System;

namespace FNF.Utility
{
    /// <summary>
    /// .NET Remotingのためのクラス。（本クラスの内容を変更してしまうと通信できなくなってしまいます）
    /// </summary>
    public class BouyomiChanRemoting : MarshalByRefObject
    {
        public async void AddTalkTask(string sTalkText)
        {
            await CeVIOAIProxy.CeVIO.SpeakAsync(sTalkText);
        }

        public async void AddTalkTask(string sTalkText, int iSpeed, int iVolume, int vType)
        {
            await CeVIOAIProxy.CeVIO.SpeakAsync(sTalkText);
        }

        public async void AddTalkTask(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            await CeVIOAIProxy.CeVIO.SpeakAsync(sTalkText);
        }

        public int AddTalkTask2(string sTalkText)
        {
            CeVIOAIProxy.CeVIO.SpeakAsync(sTalkText).Wait();
            return 0;
        }

        public int AddTalkTask2(string sTalkText, int iSpeed, int iTone, int iVolume, int vType)
        {
            CeVIOAIProxy.CeVIO.SpeakAsync(sTalkText).Wait();
            return 0;
        }

        public void ClearTalkTasks()
        {
        }

        public void SkipTalkTask()
        {
        }

        public int TalkTaskCount => 0;
        public int NowTaskId => 0;
        public bool NowPlaying => false;

        public bool Pause
        {
            get => true;
            set { }
        }
    }
}

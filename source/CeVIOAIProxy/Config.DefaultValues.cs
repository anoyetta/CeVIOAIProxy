using System.Collections.Generic;

namespace CeVIOAIProxy
{
    public partial class Config : JsonConfigBase
    {
        public static readonly uint CeVIOBasicParameterDefaultValue = 50;
        public static readonly int TcpServerPortDefaultValue = 50001;

        public override Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { nameof(IsStartupWithWindows), false },
            { nameof(IsMinimizeStartup), false },
            { nameof(TcpServerPort), TcpServerPortDefaultValue },
            { nameof(IsEnabledIPCServer), false },
            { nameof(IPCChannelName), "BouyomiChan" },
            { nameof(RestApiPortNo), 50080 },

            { nameof(Volume), CeVIOBasicParameterDefaultValue },
            { nameof(Speed), CeVIOBasicParameterDefaultValue },
            { nameof(Tone), CeVIOBasicParameterDefaultValue },
            { nameof(Alpha), CeVIOBasicParameterDefaultValue },
            { nameof(ToneScale), CeVIOBasicParameterDefaultValue },
        };
    }
}

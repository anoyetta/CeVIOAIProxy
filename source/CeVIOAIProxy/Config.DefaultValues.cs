using System.Collections.Generic;

namespace CeVIOAIProxy
{
    public partial class Config : JsonConfigBase
    {
        public static readonly uint CeVIOBasicParameterDefaultValue = 50;

        public override Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { nameof(IsStartupWithWindows), false },
            { nameof(IsMinimizeStartup), false },

            { nameof(Volume), CeVIOBasicParameterDefaultValue },
            { nameof(Speed), CeVIOBasicParameterDefaultValue },
            { nameof(Tone), CeVIOBasicParameterDefaultValue },
            { nameof(Alpha), CeVIOBasicParameterDefaultValue },
            { nameof(ToneScale), CeVIOBasicParameterDefaultValue },
        };
    }
}

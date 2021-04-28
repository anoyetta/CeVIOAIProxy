using System.Collections.Generic;

namespace CeVIOAIProxy
{
    public partial class Config : JsonConfigBase
    {
        public override Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { nameof(IsStartupWithWindows), false },
            { nameof(IsMinimizeStartup), false },

            { nameof(Volume), 50 },
            { nameof(Speed), 50 },
            { nameof(Tone), 50 },
            { nameof(Alpha), 50 },
            { nameof(ToneScale), 50 },
        };
    }
}

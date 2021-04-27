using System.Collections.Generic;

namespace CeVIOAIProxy
{
    public partial class Config : JsonConfigBase
    {
        public override Dictionary<string, object> DefaultValues => new Dictionary<string, object>()
        {
            { nameof(IsStartupWithWindows), false },
            { nameof(IsMinimizeStartup), false },
            { nameof(Rate), 0 },
        };
    }
}

using System.Collections.Generic;

namespace VSCode
{
    public class CompletionOptions
    {
        public bool ResolveProvider { get; set; } = false;
        public IList<string> TriggerCharacters { get; set; } = null;
    }
}
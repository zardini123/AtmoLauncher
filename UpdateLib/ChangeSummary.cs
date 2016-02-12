
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UpdateLib {
    public class ChangeSummary {
        [JsonProperty("new_sizes")]
        public Dictionary<string, int> NewSizes { get; private set; }

        public ChangeSummary(Dictionary<string, int> newSizes) {
            NewSizes = newSizes;
        }
    }
}

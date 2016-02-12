using System;
using Newtonsoft.Json;

namespace UpdateLib {
    public class VersionDelta {
        [JsonProperty("current")]
        public Version Current { get; set; }

        [JsonProperty("target")]
        public Version Target { get; set; }
    }
}

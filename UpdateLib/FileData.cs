using System;
using Newtonsoft.Json;

namespace UpdateLib {
    public class FileData {
        [JsonProperty("remote_path")]
        public string RemotePath { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }
    }
}
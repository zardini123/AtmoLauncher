using Newtonsoft.Json;

namespace Server {
    public class ServerConfiguration {
        [JsonProperty("projects_root")]
        public string ProjectsRoot { get; private set; }
    }
}
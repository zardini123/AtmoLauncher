using System.IO;
using Newtonsoft.Json;

namespace UpdateLib {
    public class ChangeCache {
        [JsonIgnore]
        private string _filename;

        [JsonProperty("Version")]
        public Version Version;

        public void SetVersion(Version newVersion) {
            Version = newVersion;
            File.WriteAllText(_filename, JsonConvert.SerializeObject(this));
        }

        public static ChangeCache FromFile(string filename) {
            var result = File.Exists(filename)
                ? JsonConvert.DeserializeObject<ChangeCache>(File.ReadAllText(filename))
                : new ChangeCache { Version = new Version() };
            result._filename = filename;
            return result;
        }
    }
}
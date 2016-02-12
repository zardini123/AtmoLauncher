using System;
using System.IO;
using Newtonsoft.Json;

namespace UpdateLib {
    public class ChangeCache {
        [JsonIgnore]
        private string _filename;
        public Version Version { get; private set; }

        private ChangeCache() {
            Version = new Version(0, 0);
        }

        public void SetVersion(Version newVersion) {
            File.WriteAllText(_filename, JsonConvert.SerializeObject(this));
        }

        public static ChangeCache FromFile(string filename) {
            var result = File.Exists(filename)
                ? JsonConvert.DeserializeObject<ChangeCache>(File.ReadAllText(filename))
                : new ChangeCache();
            result._filename = filename;
            return result;
        }
    }
}

using System;
using System.IO;
using Newtonsoft.Json;

namespace UpdateLib
{
    public class ChangeCache
    {
        [JsonIgnore]
        private string _filename;
        [JsonIgnore]
        public Version Version { get { return new Version(major, minor, build, 0); } }

        [JsonProperty(PropertyName = "Major")]
        private int major;
        [JsonProperty(PropertyName = "Minor")]
        private int minor;
        [JsonProperty(PropertyName = "Build")]
        private int build;

        private ChangeCache()
        {
            major = 0;
            minor = 0;
            build = 0;
        }

        public void SetVersion(Version newVersion)
        {
            major = newVersion.Major;
            minor = newVersion.Minor;
            build = newVersion.Build;

            File.WriteAllText(_filename, JsonConvert.SerializeObject(this));
        }

        public static ChangeCache FromFile(string filename)
        {
            var result = File.Exists(filename)
                ? JsonConvert.DeserializeObject<ChangeCache>(File.ReadAllText(filename))
                : new ChangeCache();
            result._filename = filename;
            return result;
        }
    }
}
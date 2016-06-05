using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib
{
    public class UpdateProgress
    {
        // For the life of me I can't figure out why it won't work if I just use a version object directly. The comparisons between this and the Launcher's version don't return the right values.
        [Newtonsoft.Json.JsonIgnore]
        public Version TargetVersion { get { return new Version(targetMajor, targetMinor, targetBuild); } }
        public uint targetMajor, targetMinor, targetBuild;
        public List<string> DownloadedFiles;

        public int Downloaded { get { return DownloadedFiles.Count; } }

        public UpdateProgress() {
            DownloadedFiles = new List<string>();
            targetMajor = 0;
            targetMinor = 0;
            targetBuild = 0;
        }

        public void setVersion(Version v) {
            setVersion(v.Major, v.Minor, v.Build);
        }

        public void setVersion(uint major, uint minor, uint build) {
            targetMajor = major;
            targetMinor = minor;
            targetBuild = build;
        }
    }
}

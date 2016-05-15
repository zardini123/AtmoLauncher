using System;
using System.Linq;
using System.Net;
using UpdateLib;

namespace Server.Requests {
    [Request("get_changes", typeof(VersionDelta))]
    class GetChanges : Request {
        public override object Handle(Project project, object body, HttpListenerResponse response) {
            var delta = (VersionDelta) body;
            var versions = project.GetVersions()
                                  .Where(v => v > delta.Current && v <= delta.Target);

            foreach (var f in project.GetLatestFiles(versions)) {
                var urif = new Uri(f.FullName);
                var relst = project.GetRelativeString(new Uri(f.FullName));
            }
            return new ChangeSummary(
                project.GetLatestFiles(versions).ToDictionary(
                    fi => project.GetRelativeString(new Uri(fi.FullName)),
                    fi => (int) fi.Length
                )
            );
        }
    }
}

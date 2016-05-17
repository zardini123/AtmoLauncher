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

            return new ChangeSummary(
                project.GetLatestFiles(versions).ToDictionary(
                    fi => project.GetRelativeString(new Uri(fi.FullName)),
                    fi => (int) fi.Length
                )
            );
        }
    }
}

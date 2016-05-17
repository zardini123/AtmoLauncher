using System;
using System.Linq;
using System.Net;
using Version = UpdateLib.Version;

namespace Server.Requests {
    [Request("get_version", typeof(Version))]
    class GetVersion : Request {
        public override object Handle(Project project, object body, HttpListenerResponse response) {
            return project
                .GetVersions()
                .OrderByDescending(v => v)
                .FirstOrDefault();
        }
    }
}

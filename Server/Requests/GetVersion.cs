using System;
using System.Linq;
using System.Net;

namespace Server.Requests {
    [Request("get_version", typeof(Version))]
    class GetVersion : Request {
        public override object Handle(Project project, object body, HttpListenerResponse response) {
            var version = (Version) body;
            var allVersions = project.GetVersions();
            if (version != null) {
                allVersions = allVersions.Where(v => v > version);
            }
            return allVersions.OrderByDescending(v => v)
                              .FirstOrDefault();
        }
    }
}

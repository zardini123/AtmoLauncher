using System;
using System.IO;
using System.Net;
using UpdateLib;

namespace Server.Requests {
    [Request("get_file", typeof(FileData))]
    class GetFile : Request {
        private const int BufferSize = 1024;
        public override object Handle(Project project, object body, HttpListenerResponse response) {
            var data = (FileData) body;
            var path = Path.Combine(
                project.VersionRoot.AbsoluteUnescaped(),
                data.Version.ToString(),
                data.RemotePath.Replace("..", "")
            );
            response.SendChunked = true;

            Console.WriteLine("File Requested: " + data.Version.ToString() + "/" + data.RemotePath.Replace("..", ""));

            if (File.Exists(path))
            {
                using (var file = File.OpenRead(path))
                using (var reader = new StreamReader(file))
                using (var writer = new StreamWriter(response.OutputStream))
                {
                    var buffer = new char[BufferSize];
                    int length;
                    while ((length = reader.ReadBlock(buffer, 0, BufferSize)) > 0)
                    {
                        writer.Write(buffer, 0, length);
                    }
                }
            }
            else {
                Console.WriteLine("File does not exist. Checking previous versions.");
                bool keepSearching = true;
                Version currentVersion = data.Version;
                System.Collections.Generic.List<Version> versionsToCheck = new System.Collections.Generic.List<Version>();
                while (keepSearching) {
                    if (getPreviousVersion(currentVersion) != new Version(0, 0, 0, 0))
                    {
                        currentVersion = getPreviousVersion(currentVersion);
                        versionsToCheck.Add(currentVersion);
                    }
                    else {
                        keepSearching = false;
                    }
                }

                foreach (Version v in versionsToCheck) {
                    path = Path.Combine(
                        project.VersionRoot.AbsoluteUnescaped(),
                        v.ToString(),
                        data.RemotePath.Replace("..", "")
                    );

                    if (File.Exists(path)) {
                        Console.WriteLine("Sending {0}", path);
                        using (var file = File.OpenRead(path))
                        using (var reader = new StreamReader(file))
                        using (var writer = new StreamWriter(response.OutputStream))
                        {
                            var buffer = new char[BufferSize];
                            int length;
                            while ((length = reader.ReadBlock(buffer, 0, BufferSize)) > 0)
                            {
                                writer.Write(buffer, 0, length);
                            }
                        }

                        break;
                    }
                }
            }

            return null;
        }
        public Version getPreviousVersion(Version currentVersion) {
            int major = currentVersion.Major;
            int minor = currentVersion.Minor;
            int build = currentVersion.Build;

            if (build > 0)
            {
                build -= 1;
            }
            else if (minor > 0)
            {
                minor -= 1;
            }
            else if (major > 1)
            {
                major -= 1;
            }
            else {
                return new Version(0,0,0,0);
            }

            return new Version(major, minor, build, 0);
        }
    }
}

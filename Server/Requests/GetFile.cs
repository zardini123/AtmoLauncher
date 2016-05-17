using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using UpdateLib;

namespace Server.Requests {
    [Request("get_file", typeof(FileData))]
    class GetFile : Request {

        public override object Handle(Project project, object body, HttpListenerResponse response) {
            var data = (FileData) body;
            var remotePath = data.RemotePath.Replace("..", "");
            var path = Path.Combine(
                project.VersionRoot.AbsoluteUnescaped(),
                data.Version.ToString(),
                remotePath
            );

            response.SendChunked = true;
            response.ContentType = MediaTypeNames.Application.Octet;
            response.ContentEncoding = Encoding.ASCII;

            Console.WriteLine("File Requested: " + data.Version + "/" + remotePath);

            if (File.Exists(path)) {
                SendFile(path, response);
            } else {
                Console.WriteLine("File does not exist. Checking previous versions.");
                var versionsToCheck = project.GetVersions()
                    .Where(v => v < data.Version)
                    .OrderBy(v => v)
                    .ToList();

                foreach (var v in versionsToCheck) {
                    path = Path.Combine(
                        project.VersionRoot.AbsoluteUnescaped(),
                        v.ToString(),
                        remotePath
                    );

                    if (!File.Exists(path)) {
                        continue;
                    }
                    Console.WriteLine("Sending {0}", path);
                    SendFile(path, response);
                }
            }

            return null;
        }

        private static void SendFile(string path, HttpListenerResponse target) {
            target.ContentLength64 = new FileInfo(path).Length;

            using (var file = new BufferedStream(File.OpenRead(path)))
            using (var output = target.OutputStream) {
                file.CopyTo(output);
            }
        }
    }
}

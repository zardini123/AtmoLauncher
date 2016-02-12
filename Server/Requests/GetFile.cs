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
            using (var file = File.OpenRead(path))
            using (var reader = new StreamReader(file))
            using (var writer = new StreamWriter(response.OutputStream)) {
                var buffer = new char[BufferSize];
                int length;
                while ((length = reader.ReadBlock(buffer, 0, BufferSize)) > 0) {
                    writer.Write(buffer, 0, length);
                }
            }
            return null;
        }
    }
}

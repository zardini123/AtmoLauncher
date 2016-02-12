using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UpdateLib {
    public class UpdaterClient {
        public delegate void Progress(long downloaded, long fileSize);

        private readonly Uri _endpoint;
        private readonly string _project;

        public UpdaterClient(string remoteEndpoint, string projectName) {
            _endpoint = new Uri(remoteEndpoint);
            _project = projectName;
        }

        public async Task<Version> FindLatestVersion() {
            var response = await Post<Version>("get_version");
            return response.Body;
        }

        public async Task<ChangeSummary> GetChanges(Version current, Version target) {
            var response = await Post<VersionDelta, ChangeSummary>("get_changes", new VersionDelta {
                Current = current,
                Target = target
            });
            return response.Body;
        }

        public async Task Download(string remotePath, string localPath, Version version, Progress onProgress = null) {
            var directory = Path.GetDirectoryName(localPath);
            if (directory != null && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            using (var file = File.OpenWrite(localPath))
            using (var writer = new StreamWriter(file)) {
                var progress = RawPost("get_file", new FileData {
                    RemotePath = remotePath,
                    Version = version
                }, onProgress);
                var response = await progress;
                await writer.WriteAsync(response);
            }
        }

        private async Task<string> RawPost<TRequest>(string requestName, TRequest content, Progress onProgress = null) {
            var message = new UpdaterMessage<TRequest> {
                Request = requestName,
                Project = _project,
                Body = content
            };
            var json = JsonConvert.SerializeObject(message);
            var postContent = new StringContent(json);
            postContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using (var req = new HttpClient()) {
                var res = await req.PostAsync(_endpoint, postContent);
                if (!res.Content.Headers.ContentLength.HasValue) {
                    throw new HttpRequestException("Content-Length is not set!");
                }
                var buffer = new byte[res.Content.Headers.ContentLength.Value];
                var length = 0;

                using (var resStream = await res.Content.ReadAsStreamAsync()) {
                    int bytesRead;
                    var readSize = buffer.Length;
                    while (length < buffer.Length && (bytesRead = await resStream.ReadAsync(buffer, length, readSize)) > 0) {
                        readSize = buffer.Length - length;
                        length += bytesRead;
                        onProgress?.Invoke(length, buffer.Length);
                    }
                }
                return Encoding.UTF8.GetString(buffer, 0, length);
            }
        }

        private async Task<UpdaterMessage<TResponse>> Post<TRequest, TResponse>(string requestName, TRequest content) {
            var post = await RawPost(requestName, content);
            return JsonConvert.DeserializeObject<UpdaterMessage<TResponse>>(post);
        }

        private async Task<UpdaterMessage<TResponse>> Post<TResponse>(string requestName) {
            return await Post<object, TResponse>(requestName, null);
        }
    }
}

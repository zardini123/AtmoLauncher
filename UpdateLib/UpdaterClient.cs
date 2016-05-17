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

        private const int BufferSize = 1024;

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
            localPath = localPath.Replace("%20", " ");
            var directory = Path.GetDirectoryName(localPath);
            if (directory != null && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            using (var file = File.OpenWrite(localPath)) {
                await RawPost("get_file", new FileData {
                    RemotePath = remotePath,
                    Version = version
                }, file, onProgress);
            }
        }

        private async Task RawPost<TRequest>(string requestName, TRequest content, Stream output, Progress onProgress = null) {
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
                
                using (var resStream = await res.Content.ReadAsStreamAsync())
                using (var buffered = new BufferedStream(resStream)) {
                    var buffer = new byte[BufferSize];
                    int length;
                    long bytesRead = 0;
                    while ((length = await buffered.ReadAsync(buffer, 0, BufferSize)) > 0) {
                        bytesRead += length;
                        await output.WriteAsync(buffer, 0, length);
                        onProgress?.Invoke(bytesRead, buffer.Length);
                    }
                }
            }
        }

        private async Task<UpdaterMessage<TResponse>> Post<TRequest, TResponse>(string requestName, TRequest content) {
            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream)) {
                await RawPost(requestName, content, stream);
                stream.Flush();
                stream.Position = 0;
                return JsonConvert.DeserializeObject<UpdaterMessage<TResponse>>(reader.ReadToEnd());
            }
        }

        private async Task<UpdaterMessage<TResponse>> Post<TResponse>(string requestName) {
            return await Post<object, TResponse>(requestName, null);
        }
    }
}

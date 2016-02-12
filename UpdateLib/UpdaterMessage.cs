using Newtonsoft.Json;

namespace UpdateLib {
    public class UpdaterMessage<T> {
        [JsonProperty("request")]
        public string Request { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("body")]
        public T Body { get; set; }

        public UpdaterMessage<TResponse> Respond<TResponse>(TResponse response) {
            return new UpdaterMessage<TResponse> {
                Request = Request,
                Project = Project,
                Body = response
            };
        }
    }
}
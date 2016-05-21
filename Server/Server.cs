using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UpdateLib;

namespace Server {
    class Server {
        private readonly Dictionary<string, Tuple<Type, Request>> _requestHandlers;
        private static ServerConfiguration _config;

        public static ServerConfiguration Configuration {
            get {
                if (_config == null) {
                    _config = JsonConvert.DeserializeObject<ServerConfiguration>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")));
                }
                return _config;
            }
        }

        public Server() {
            _requestHandlers = new Dictionary<string, Tuple<Type, Request>>();
        }

        private void RegisterHandlers() {
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(typeof(Request).IsAssignableFrom)
                .Where(t => Attribute.IsDefined(t, typeof (RequestAttribute)))
                .Select(t => Tuple.Create(t.GetCustomAttribute<RequestAttribute>(), t));
            foreach (var tuple in types) {
                var requestName = tuple.Item1.Request;
                var bodyType = tuple.Item1.BodyType;
                var instance = Activator.CreateInstance(tuple.Item2) as Request;
                if (instance != null) {
                    _requestHandlers[requestName] = Tuple.Create(bodyType, instance);
                }
            }
        }

        private void ServeRequest(HttpListenerContext context) {
            using (var reader = new StreamReader(context.Request.InputStream)) {
                try {
                    var input = reader.ReadToEnd();
                    var request = JsonConvert.DeserializeObject<UpdaterMessage<JObject>>(input);
                    if (request != null) {
                        Tuple<Type, Request> handler;
                        if (_requestHandlers.TryGetValue(request.Request, out handler)) {
                            var result = handler.Item2.Handle(
                                Project.FromName(request.Project),
                                request.Body?.ToObject(handler.Item1),
                                context.Response
                            );
                            context.Response.StatusCode = 200;
                            if (result != null) {
                                var output = JsonConvert.SerializeObject(request.Respond(result));
                                using (var writer = new StreamWriter(context.Response.OutputStream)) {
                                    writer.Write(output);
                                }
                            }
                            context.Response.Close();
                            return;
                        }
                    }
                    context.Response.StatusCode = 500;
                    context.Response.Close(Encoding.UTF8.GetBytes(input), false);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }

        static void Main(string[] args) {
            using (var listener = new HttpListener()) {
                listener.Prefixes.Add("http://localhost:9900/");
                listener.Start();

                var server = new Server();
                server.RegisterHandlers();
                while (true) {
                    var context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(o => server.ServeRequest(context));
                }
                
            }
        }
    }
}

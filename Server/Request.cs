using System.Net;

namespace Server {
    public abstract class Request {
        public abstract object Handle(Project project, object body, HttpListenerResponse response);
    }
}
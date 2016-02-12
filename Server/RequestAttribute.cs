using System;

namespace Server {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    internal class RequestAttribute : Attribute {
        public string Request { get; }
        public Type BodyType { get; }

        public RequestAttribute(string request, Type bodyType) {
            Request = request;
            BodyType = bodyType;
        }
    }
}
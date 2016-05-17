using System;
using Newtonsoft.Json;

namespace UpdateLib {
    public class Version : IComparable, IEquatable<Version> {

        [JsonProperty("Major")]
        public uint Major { get; }
        
        [JsonProperty("Minor")]
        public uint Minor { get; }

        [JsonProperty("Build")]
        public uint Build { get; }

        public Version(uint major = 0, uint minor = 0, uint build = 0) {
            Major = major;
            Minor = minor;
            Build = build;
        }

        public static Version Parse(string input) {
            Version version;
            if (TryParse(input, out version)) {
                return version;
            }
            throw new FormatException();
        }
        public static bool TryParse(string input, out Version result) {
            var parts = input.Split('.');

            uint major, minor, build;
            if (uint.TryParse(parts[0], out major)
                && uint.TryParse(parts[1], out minor)
                && uint.TryParse(parts[2], out build)) {
                
                result = new Version(major, minor, build);
                return true;
            }
            result = new Version();
            return false;
        }

        public override string ToString() => Major + "." + Minor + "." + Build;

        public int CompareTo(object obj) {
            var version = (Version) obj;

            if (this < version)
                return -1;
            else if (this > version)
                return 1;
            return 0;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (int)Major;
                hashCode = (hashCode * 397) ^ (int)Minor;
                hashCode = (hashCode * 397) ^ (int)Build;
                return hashCode;
            }
        }

        public bool Equals(Version other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return Major == other.Major
                && Minor == other.Minor
                && Build == other.Build;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((Version) obj);
        }



        public static bool operator ==(Version v1, Version v2) {
            object o1 = v1, o2 = v2;

            if (o1 == null && o2 == null) return true;
            if (o1 == null) return false;
            if (o2 == null) return false;

            return v1.Major == v2.Major
                && v1.Minor == v2.Minor
                && v1.Build == v2.Build;
        }

        public static bool operator !=(Version v1, Version v2) {
            return !(v1 == v2);
        }

        public static bool operator <(Version v1, Version v2) {
            return v1.Major < v2.Major
                || v1.Minor < v2.Minor
                || v1.Build < v2.Build;
        }
        public static bool operator >(Version v1, Version v2) {
            return v1.Major > v2.Major
                || v1.Minor > v2.Minor
                || v1.Build > v2.Build;
        }

        public static bool operator <=(Version v1, Version v2) => (v1 == v2) || (v1 < v2);
        public static bool operator >=(Version v1, Version v2) => (v1 == v2) || (v1 > v2);
    }
}
 
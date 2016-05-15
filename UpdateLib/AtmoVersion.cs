using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib
{
    public class AtmoVersion : IComparable
    {
        private int _major;
        private int _minor;
        private int _build;

        public AtmoVersion()
            : this (0, 0, 0) {}
        public AtmoVersion(int major)
            : this(major, 0, 0) {}
        public AtmoVersion(int major, int minor)
            : this(major, minor, 0) {}
        public AtmoVersion(int major, int minor, int build) {
            if (major >= 0 && minor >= 0 && build >= 0)
            {
                _major = major;
                _minor = minor;
                _build = build;
            }
            else throw new ArgumentOutOfRangeException();
        }

        public int Major { get { return _major; } }
        public int Minor { get { return _minor; } }
        public int Build { get { return _build; } }

        public static AtmoVersion Parse(string input) {
            string[] parts = input.Split('.');

            int major = -1;
            int minor = -1;
            int build = -1;

            foreach (string part in parts) {
                int res;
                if (Int32.TryParse(part, out res))
                {
                    if (major == -1)
                        major = res;
                    else if (minor == -1)
                        minor = res;
                    else if (build == -1)
                        build = res;
                }
                else throw new ArgumentException();
            }

            if (major < 0)
                major = 0;
            if (minor < 0)
                minor = 0;
            if (build < 0)
                build = 0;

            return new AtmoVersion(major, minor, build);
        }
        public static bool TryParse(string input, out AtmoVersion result) {
            AtmoVersion res = null;
            try { res = Parse(input); }
            catch (Exception ex) { result = null; return false; }

            if (res == null)
            {
                result = null;
                return false;
            }
            else {
                result = res;
                return true;
            }
            
        }

        public override string ToString()
        {
            return _major + "." + _minor + "." + _build;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            AtmoVersion avObj = (AtmoVersion)obj;
            if (this < avObj)
                return -1;
            else if (this > avObj)
                return 1;

            return 0;
        }

        public static bool operator ==(AtmoVersion v1, AtmoVersion v2) {
            object v1obj = v1 as object;
            object v2obj = v2 as object;

            if (v1obj == null && v2obj == null)
                return true;
            if (v1obj == null && v2obj != null)
                return false;
            if (v1obj != null && v2obj == null)
                return false;

            bool majorRes = false;
            bool minorRes = false;
            bool buildRes = false;

            if (v1.Major == v2.Major)
                majorRes = true;

            if (v1.Minor == v2.Minor)
                minorRes = true;

            if (v1.Build == v2.Build)
                buildRes = true;

            if (majorRes == true && minorRes == true && buildRes == true)
                return true;

            return false;
        }
        public static bool operator !=(AtmoVersion v1, AtmoVersion v2) {
            object v1obj = v1 as object;
            object v2obj = v2 as object;

            if (v1obj == null && v2obj == null)
                return false;
            if (v1obj == null && v2obj != null)
                return true;
            if (v1obj != null && v2obj == null)
                return true;

            bool majorRes = false;
            bool minorRes = false;
            bool buildRes = false;

            if (v1.Major == v2.Major)
                majorRes = true;

            if (v1.Minor == v2.Minor)
                minorRes = true;

            if (v1.Build == v2.Build)
                buildRes = true;

            if (majorRes == true && minorRes == true && buildRes == true)
                return false;

            return true;
        }
        public static bool operator <(AtmoVersion v1, AtmoVersion v2) {
            if (v1.Major < v2.Major)
                return true;
            if (v1.Major == v2.Major) {
                if (v1.Minor < v2.Minor)
                    return true;
                if (v1.Minor == v2.Minor && v1.Build < v2.Build)
                    return true;
            }

            return false;
        }
        public static bool operator >(AtmoVersion v1, AtmoVersion v2) {
            if (v1.Major > v2.Major)
                return true;
            if (v1.Major == v2.Major)
            {
                if (v1.Minor > v2.Minor)
                    return true;
                if (v1.Minor == v2.Minor && v1.Build > v2.Build)
                    return true;
            }

            return false;
        }
        public static bool operator <=(AtmoVersion v1, AtmoVersion v2) {
            // EQUAL
            object v1obj = v1 as object;
            object v2obj = v2 as object;

            if (v1obj == null && v2obj == null)
                return true;
            if (v1obj == null && v2obj != null)
                return false;
            if (v1obj != null && v2obj == null)
                return false;

            bool majorRes = false;
            bool minorRes = false;
            bool buildRes = false;

            if (v1.Major == v2.Major)
                majorRes = true;

            if (v1.Minor == v2.Minor)
                minorRes = true;

            if (v1.Build == v2.Build)
                buildRes = true;

            if (majorRes == true && minorRes == true && buildRes == true)
                return true;

            // LESS THAN
            if (v1.Major < v2.Major)
                return true;
            if (v1.Major == v2.Major)
            {
                if (v1.Minor < v2.Minor)
                    return true;
                if (v1.Minor == v2.Minor && v1.Build < v2.Build)
                    return true;
            }

            return false;
        }
        public static bool operator >=(AtmoVersion v1, AtmoVersion v2) {
            // EQUAL
            object v1obj = v1 as object;
            object v2obj = v2 as object;

            if (v1obj == null && v2obj == null)
                return true;
            if (v1obj == null && v2obj != null)
                return false;
            if (v1obj != null && v2obj == null)
                return false;

            bool majorRes = false;
            bool minorRes = false;
            bool buildRes = false;

            if (v1.Major == v2.Major)
                majorRes = true;

            if (v1.Minor == v2.Minor)
                minorRes = true;

            if (v1.Build == v2.Build)
                buildRes = true;

            if (majorRes == true && minorRes == true && buildRes == true)
                return true;

            // GREATER THAN
            if (v1.Major > v2.Major)
                return true;
            if (v1.Major == v2.Major)
            {
                if (v1.Minor > v2.Minor)
                    return true;
                if (v1.Minor == v2.Minor && v1.Build > v2.Build)
                    return true;
            }

            return false;
        }
    }
}
 
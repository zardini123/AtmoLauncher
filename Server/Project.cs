using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Server {
    internal static class UriExtensions {
        public static string AbsoluteUnescaped(this Uri uri) {
            return uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
        }
    }

    public class Project {
        public string Name { get; private set; }

        public Uri VersionRoot => new Uri(Path.GetFullPath(Path.Combine(Server.Configuration.ProjectsRoot, Name) + Path.DirectorySeparatorChar), UriKind.Absolute);

        private Project() {}

        public IEnumerable<Version> GetVersions() {
            Console.WriteLine("GetVersions Requested.");
            return Directory.GetDirectories(VersionRoot.AbsoluteUnescaped())
                .Select(d => {
                    Version v;
                    Version.TryParse(Path.GetFileName(d), out v);
                    return v;
                })
                .Where(v => v != null);
        } 
        public IEnumerable<FileInfo> GetLatestFiles(IEnumerable<Version> versions) {
            Console.WriteLine("GetLatestFiles Requested");
            var changes = versions.OrderByDescending(v => v)
                                  .Select(v => new Uri(VersionRoot, v.ToString()))
                                  .Select(p => Tuple.Create(p, Directory.GetFiles(p.AbsoluteUnescaped(), "*", SearchOption.AllDirectories)
                                                                        .Select(fp => new Uri(fp))))
                                  .Where(f => f.Item2 != null);

            var result = new List<string>();
            foreach (var version in changes) {
                Console.WriteLine("Going through version " + version.Item1);
                foreach (var file in version.Item2) {
                    Console.WriteLine("File " + file.AbsolutePath);
                    if (!result.Any(f => f.EndsWith(GetRelativeString(file)))) {
                        result.Add(file.AbsoluteUnescaped());
                    }
                }
            }

            return result.Select(f => new FileInfo(f));
        }

        public string GetRelativeString(Uri fullPath) {
            return string.Join("/", VersionRoot.MakeRelativeUri(fullPath).ToString().Split('/').Skip(1));
        }

        public static Project FromName(string projectName) {
            var path = Path.Combine(Server.Configuration.ProjectsRoot, projectName, "project.json");
            var project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(path));
            project.Name = projectName;
            return project;
        }
    }
}

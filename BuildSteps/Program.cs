using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

namespace BuildSteps {
    class Program {
        /// <summary>
        /// Compiles Setup.json and Interface.glade into the GZipped format
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            if (args.Length != 2) {
                throw new ArgumentException($"Incorrect number of arguments passed to build tool: {args.Length}");
            }
            MakeLauncherSetup(args[0], args[1]);
        }

        private static void MakeLauncherSetup(string inFolder, string outPath) {
            var setupPath = Path.Combine(inFolder, "Setup.json");
            var interfacePath = Path.Combine(inFolder, "Interface.glade");

            using (var outFile = new GZipStream(File.OpenWrite(outPath), CompressionMode.Compress))
            using (var outWriter = new StreamWriter(outFile)) {
                Console.WriteLine("Created " + outPath);

                using (var setupFile = File.OpenRead(setupPath))
                using (var reader = new StreamReader(setupFile)) {
                    var config = JsonConvert.DeserializeObject(reader.ReadToEnd());
                    outWriter.WriteLine(JsonConvert.SerializeObject(config, new JsonSerializerSettings {
                        Formatting = Formatting.None
                    }));

                    Console.WriteLine("Embedded " + setupPath);
                }

                using (var interfaceFile = File.OpenRead(interfacePath))
                using (var reader = new StreamReader(interfaceFile)) {
                    outWriter.Write(reader.ReadToEnd());
                    Console.WriteLine("Embedded " + interfacePath);
                }
            }
        }
    }
}

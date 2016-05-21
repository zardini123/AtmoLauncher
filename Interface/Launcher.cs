using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ByteSizeLib;
using Gtk;
using UpdateLib;
using Newtonsoft.Json;

namespace Interface {
    internal class Launcher {
        private const string ProgressFormat = "{1} of {2} downloaded";

        private readonly LauncherSetup _setup;
        private readonly Builder _builder;

        public Window Window;
        public ProgressBar ProgressBar;
        public Button PlayButton;

        public Launcher(LauncherSetup setup, Builder builder) {
            _setup = setup;
            _builder = builder;
        }

        public void Initialize() {
            Window = (Window) _builder.GetObject("LauncherWindow");
            Window.Title = _setup.Title;
            Window.Hidden += (sender, eventArgs) => Application.Quit();
            Window.Show();
            ProgressBar = (ProgressBar) _builder.GetObject("ProgressBar");
            PlayButton = (Button) _builder.GetObject("PlayButton");
            PlayButton.Clicked += (sender, args) => {
                Program.StartGame(_setup);
            };

            Task.Run(() => CheckAndUpdate());
        }

        private async void CheckAndUpdate() {
            await UpdateProject(new UpdaterClient(_setup.RemoteEndpoint, _setup.LauncherProject), "");
            await UpdateProject(new UpdaterClient(_setup.RemoteEndpoint, _setup.GameProject), _setup.GameFolder);
        }

        private async Task UpdateProject(UpdaterClient updater, string projectRoot) {
            var targetPath = "";

            if (Program.IsUnix)
                targetPath = Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).Parent.Parent.ToString(), projectRoot);  
            else
                targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), projectRoot);

            PlayButton.Sensitive = false;
            PlayButton.Label = "Updating";
            ProgressBar.Text = "Checking for updates";

            try
            {
                var cache = ChangeCache.FromFile(Path.Combine(targetPath, "version.json"));
                var version = await updater.FindLatestVersion();
                Console.WriteLine("Local version: {0}, Latest version: {1}", cache.Version, version);
                if (cache.Version >= version)
                {
                    Console.WriteLine("No updates available.");
                    Application.Invoke((sender, args) =>
                    {
                        PlayButton.Sensitive = true;
                        PlayButton.Label = "Play";
                        ProgressBar.Text = "No updates available";
                    });
                    return;
                }

                Application.Invoke((sender, args) =>
                {
                    ProgressBar.Text = "Getting version " + version + " from server...";
                    PlayButton.Sensitive = false;
                    PlayButton.Label = "Updating";
                });

                var changes = await updater.GetChanges(cache.Version, version);

                string progressFile = Path.Combine(targetPath, "updateProgress.json");

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                string curProgress = "";

                if (File.Exists(progressFile))
                    curProgress = File.ReadAllText(progressFile);

                if (curProgress == "")
                    curProgress = "{}";

                UpdateProgress progress = JsonConvert.DeserializeObject<UpdateProgress>(curProgress);

                if (progress == null)
                    progress = new UpdateProgress();

                List<KeyValuePair<string, int>> changesLeft = changes.NewSizes.Where(c => !progress.DownloadedFiles.Contains(c.Key)).ToList();

                var totalSize = ByteSize.FromBytes(changesLeft.Sum(kvp => kvp.Value));
                long currentDownloaded = 0;
                foreach (var change in changesLeft)
                {
                    var relativePath = change.Key;
                    long fileSize = 0;
                    await updater.Download(relativePath, Path.Combine(targetPath, relativePath), version, (current, size) =>
                    {
                        fileSize = size;
                        var currentTotalBytes = ByteSize.FromBytes(current + currentDownloaded);
                        Application.Invoke((_, args) =>
                        {
                            UpdateDownloadProgress(relativePath, currentTotalBytes, totalSize);
                        });
                    });
                    currentDownloaded += fileSize;

                    progress.DownloadedFiles.Add(change.Key);
                    File.WriteAllText(progressFile, JsonConvert.SerializeObject(progress));
                }
                cache.SetVersion(version);

                if (File.Exists(progressFile))
                    File.Delete(progressFile);

                Application.Invoke((sender, args) =>
                {
                    PlayButton.Sensitive = true;
                    PlayButton.Label = "Play";
                    ProgressBar.Text = "Finished Updating";
                });
            }
            catch (Exception e)
            {
                if (e is System.Net.WebException || e is System.Net.Http.HttpRequestException || e is System.Net.Sockets.SocketException)
                {
                    Application.Invoke((sender, args) =>
                    {
                        PlayButton.Sensitive = true;
                        PlayButton.Label = "Play";
                        ProgressBar.Text = "Couldn't connect to update server.";
                    });
                }
                else {
                    Application.Invoke((sender, args) => {
                        Console.WriteLine(e);
                        var dialog = new MessageDialog(Window, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                                                        false, e.GetType() + "An error ocurred, please report this at {0}:\n{1}", _setup.SupportSite, e)
                        {
                            Title = "Update error"
                        };
                        dialog.Run();
                        dialog.Destroy();
                    });
                }
            }
        }

        private void UpdateDownloadProgress(string fileName, ByteSize current, ByteSize total) {
            ProgressBar.Fraction = current.Bytes / total.Bytes;
            ProgressBar.Text = string.Format(ProgressFormat, fileName, current, total);
        }
        
    }
}
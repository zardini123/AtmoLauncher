using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ByteSizeLib;
using Gtk;
using UpdateLib;

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
            var targetPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), projectRoot);

            PlayButton.Sensitive = false;
            PlayButton.Label = "Updating";
            ProgressBar.Text = "Checking for updates";

            try
            {
                var cache = ChangeCache.FromFile(Path.Combine(targetPath, "version.json"));
                var version = await updater.FindLatestVersion();
                Console.WriteLine("Local version: v{0}, Latest version: v{1}", cache.Version.ToString(), version.ToString());
                if (cache.Version >= version)
                {
                    Console.WriteLine("No updates available.");
                    Application.Invoke((sender, args) => {
                        PlayButton.Sensitive = true;
                        PlayButton.Label = "Play";
                        ProgressBar.Text = "No updates available";
                    });
                    return;
                }

                Application.Invoke((sender, args) => {
                    ProgressBar.Text = "Getting version " + version + " from server...";
                    PlayButton.Sensitive = false;
                    PlayButton.Label = "Updating";
                });

                var changes = await updater.GetChanges(cache.Version, version);
                var totalSize = ByteSize.FromBytes(changes.NewSizes.Sum(kvp => kvp.Value));
                long currentDownloaded = 0;
                foreach (var change in changes.NewSizes)
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
                }
                cache.SetVersion(version);

                Application.Invoke((sender, args) => {
                    PlayButton.Sensitive = true;
                    PlayButton.Label = "Play";
                    ProgressBar.Text = "Finished Updating";
                });
            }
            catch (Exception e)
            {
                Application.Invoke((sender, args) =>
                {
                    Console.WriteLine(e);
                    var dialog = new MessageDialog(Window, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
                                                    false, "An error ocurred, please report this at {0}:\n{1}", _setup.SupportSite, e)
                    { Title = "Update error" };
                    dialog.Run();
                    dialog.Destroy();
                });
            }
            finally {
                
            }         
        }

        private void UpdateDownloadProgress(string fileName, ByteSize current, ByteSize total) {
            ProgressBar.Fraction = current.Bytes / total.Bytes;
            ProgressBar.Text = string.Format(ProgressFormat, fileName, current, total);
        }
        
    }
}
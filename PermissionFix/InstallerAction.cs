using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace InstallerExtras
{
    [RunInstaller(true)]
    public partial class InstallerAction : System.Configuration.Install.Installer
    {
        public InstallerAction()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            // Fix Permissions
            var installDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Atmosphir");
            DirectoryInfo dInfo = new DirectoryInfo(installDir);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            // Clean Up extra files, while maintaining /game
            var installDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Atmosphir");
            DirectoryInfo di = new DirectoryInfo(installDir);

            foreach (FileInfo file in di.GetFiles()) {
                file.Delete();
            }
        }
    }
}

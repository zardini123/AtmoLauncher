using System;
using System.Diagnostics;
using System.IO;

namespace LaunchFromProgramData
{
    class Program
    {
        /* Why does this exist?
        /  Well, turns out if the launch application is installed into the Program Files directory it doesn't have permissions to change any files in there,
        /  which we obviously need to do. We'll install this application into Program Files and through it, launch the real Launcher application located
        /  inside ProgramData
        */
        static void Main(string[] args)
        {
            string programData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Atmosphir");
            string launchExe = Path.Combine(programData, "Interface.exe");
            if(File.Exists(launchExe))
                Process.Start(launchExe);
            return;
        }
    }
}

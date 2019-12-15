using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Forms;
using System.Linq;
using WixSharp.Bootstrapper;

// DON'T FORGET to update NuGet package "WixSharp".
// NuGet console: Update-Package WixSharp
// NuGet Manager UI: updates tab

namespace LiveClockSetup
{
    internal class Program
    {
        private static void Main()
        {
            var linkName = "Live Clock";
            var exeFileLink = @"%ProgramFiles%\Kevenin\LiveClock\Kevenin.LiveClock.exe";
            var appFiles = System.IO.Directory.GetFiles(@"C:\Users\kblan\Source\Repos\Kevenin\Kevenin.WpfBase\Kevenin.LiveClock\bin\Release\", "*.dll")
                .Select(x => new File(x)).ToList();
            appFiles.Add(new File(@"C:\Users\kblan\Source\Repos\Kevenin\Kevenin.WpfBase\Kevenin.LiveClock\bin\Release\Kevenin.LiveClock.exe"));

            var directory = new Dir(@"%ProgramFiles%\Kevenin\LiveClock");
            var myCompanyDir = directory.Dirs[0];
            var myProductDir = myCompanyDir.Dirs[0];
            myProductDir.Files = appFiles.ToArray();

            var project = new ManagedProject("LiveClock");
            project.Dirs = new[]
            {
                directory,
                new Dir("%Desktop%", new ExeFileShortcut(linkName, exeFileLink, string.Empty)),
                new Dir("%ProgramMenu%", new ExeFileShortcut(linkName, exeFileLink, string.Empty))
            };

            project.GUID = new Guid("1ce10564-8dba-4e63-8bc5-2298d9febac7");
            project.UpgradeCode = new Guid("8567ee00-a72c-41f5-a03a-21355b4d4a7f");
            project.Version = new Version(1, 2, 0, 0);

            //project.ManagedUI = ManagedUI.Empty;    //no standard UI dialogs
            //project.ManagedUI = ManagedUI.Default;  //all standard UI dialogs
            project.MajorUpgrade = new MajorUpgrade
            {
                Schedule = UpgradeSchedule.afterInstallInitialize,
                DowngradeErrorMessage = "A later version of [ProductName] is already installed. Setup will now exit."
            };

            //custom set of standard UI dialogs
            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                            .Add(Dialogs.Licence)
                                            .Add(Dialogs.InstallDir)
                                            .Add(Dialogs.Progress)
                                            .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                           .Add(Dialogs.Features)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.Load += Msi_Load;
            project.BeforeInstall += Msi_BeforeInstall;
            project.AfterInstall += Msi_AfterInstall;

            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }

        private static void Msi_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "AfterExecute");
        }

        private static void Msi_BeforeInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "BeforeInstall");
        }

        private static void Msi_Load(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "Load");
        }
    }
}
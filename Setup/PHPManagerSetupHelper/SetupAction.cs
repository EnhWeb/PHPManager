using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;

namespace Web.Management.PHP.Setup
{

    [RunInstaller(true)]
    public class SetupAction : Installer
    {

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            string assemblyFullName = assemblyName.FullName;
            string clientAssemblyFullName = assemblyFullName.Replace(assemblyName.Name, "Web.Management.PHP");

            InstallUtil.RemoveUIModuleProvider("PHP"); // This is necessary for the upgrade scenario
            InstallUtil.AddUIModuleProvider("PHP", "Web.Management.PHP.PHPProvider, " + clientAssemblyFullName);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);

            InstallUtil.RemoveUIModuleProvider("PHP");
        }
    }
}

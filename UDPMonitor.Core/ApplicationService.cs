using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UDPMonitor.Core
{
    public static class ApplicationService
    {
        public static Version Version
        {
            get => Assembly.GetEntryAssembly().GetName().Version;
        }

        public static string ApplicationPath
        {
            get => $@"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}";
        }

        public static string ConfigFolderPath
        {
            get => $@"{ApplicationPath}\Config";
        }
    }
}
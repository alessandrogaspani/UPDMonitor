using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace UDPMonitor.Core
{
    public static class ApplicationService
    {
        public static Version Version
        {
            get => Assembly.GetEntryAssembly().GetName().Version;
        }

        public static string ApplicationName
        {
            get => Assembly.GetEntryAssembly().GetName().Name;
        }

        public static string Creator
        {
            get => "Alessandro Gaspani";
        }

        public static string ApplicationPath
        {
            get => $@"{Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)}";
        }

        public static string ConfigFolderPath
        {
            get => $@"{ApplicationPath}\Config";
        }

        public static string LogFolderPath
        {
            get => $@"{ApplicationPath}\Logs";
        }


        public static void CloseApplication(int exitCode = 0)
        {
            Application.Current.Shutdown(exitCode);
        }
    }
}
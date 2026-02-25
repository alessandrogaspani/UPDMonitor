using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using UDPMonitor.Core;
using UDPMonitor.ViewModels.Base;

namespace UDPMonitor.ViewModels
{
    public class About_ViewModel : DialogViewModelBase
    {
        private string _softwareName;
        public string SoftwareName { get => _softwareName; set => SetProperty(ref _softwareName, value); }

        private string _version;
        public string Version { get => _version; set => SetProperty(ref _version, value); }

        private string _creator;
        public string Creator { get => _creator; set => SetProperty(ref _creator, value); }

        private string _copyright;
        public string Copyright { get => _copyright; set => SetProperty(ref _copyright, value); }

        private string _os;
        public string OS { get => _os; set => SetProperty(ref _os, value); }

        private string _runtime;
        public string Runtime { get => _runtime; set => SetProperty(ref _runtime, value); }

        private string _architecture;
        public string Architecture { get => _architecture; set => SetProperty(ref _architecture, value); }

        public DelegateCommand CopyDiagnosticsCommand { get; }
        public DelegateCommand OpenLogsFolderCommand { get; }
        public DelegateCommand OpenRepositoryCommand { get; }
        public About_ViewModel()
        {
            CopyDiagnosticsCommand = new DelegateCommand(CopyDiagnostics);
            OpenLogsFolderCommand = new DelegateCommand(OpenLogsFolder);
            OpenRepositoryCommand = new DelegateCommand(OpenRepository);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "About UDP Monitor";

            SoftwareName = parameters.GetValue<string>("ApplicationName");
            Version = parameters.GetValue<string>("Version");
            Creator = parameters.GetValue<string>("Creator");

            // Se non ce l’hai in ApplicationService, mettilo lì, oppure calcolalo qui:
            Copyright =
                $"© {DateTime.Now:yyyy} {Creator}. All rights reserved.";

            OS = RuntimeInformation.OSDescription;
            Architecture = RuntimeInformation.ProcessArchitecture.ToString();

            // Runtime: dipende se sei su .NET Framework o .NET
            Runtime = RuntimeInformation.FrameworkDescription;
        }

        private void CopyDiagnostics()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{SoftwareName} {Version}");
            sb.AppendLine(Copyright);
            sb.AppendLine($"OS: {OS}");
            sb.AppendLine($"Runtime: {Runtime}");
            sb.AppendLine($"Arch: {Architecture}");

            Clipboard.SetText(sb.ToString());
        }

        private void OpenLogsFolder()
        {
            // Se hai un path definito:
            var path = ApplicationService.LogFolderPath;
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        private void OpenRepository()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/alessandrogaspani/UPDMonitor",
                UseShellExecute = true
            });
        }
    }
}
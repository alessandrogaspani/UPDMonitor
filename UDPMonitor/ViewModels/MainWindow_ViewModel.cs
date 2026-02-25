using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using UDPMonitor.Core;
using UDPMonitor.ViewModels.Base;

namespace UDPMonitor.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
        private string _title = "UDPMonitor";
        private readonly IDialogService _dialogService;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand CloseWindowCommand { get; private set; }
        public DelegateCommand MinimizeWindowCommand { get; private set; }
        public DelegateCommand ToggleMaximizeWindowCommand { get; private set; }
        public DelegateCommand OpenAboutDialogCommand { get; private set; }


        public MainWindow_ViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Title = $"{ApplicationService.ApplicationName}";
        }

        protected override void OnInitializeCommands()
        {
            base.OnInitializeCommands();

            CloseWindowCommand = new DelegateCommand(() =>
            {
                var w = Application.Current?.MainWindow;
                if (w != null) w.Close();
                else Environment.Exit(0);
            });

            MinimizeWindowCommand = new DelegateCommand(() =>
            {
                var w = Application.Current?.MainWindow;
                if (w != null) w.WindowState = WindowState.Minimized;
            });

            ToggleMaximizeWindowCommand = new DelegateCommand(() =>
            {
                var w = Application.Current?.MainWindow;
                if (w == null) return;

                w.WindowState = (w.WindowState == WindowState.Maximized)
                    ? WindowState.Normal
                    : WindowState.Maximized;
            });


            OpenAboutDialogCommand = new DelegateCommand(() =>
            {
                var parameters = new DialogParameters
                {
                    { "Version", ApplicationService.Version.ToString() },
                    { "Creator", ApplicationService.Creator },
                    { "ApplicationName", ApplicationService.ApplicationName }
                };

                _dialogService.ShowDialog(App.RegisteredViews.DialogAbout_View, parameters, (DialogResult) => { });
            });
        }

      
    }
}
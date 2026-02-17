using UDPMonitor.ViewModels.Base;

namespace UDPMonitor.ViewModels
{
    public class MainWindow_ViewModel : ViewModelBase
    {
        private string _title = "UDPMonitor";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindow_ViewModel()
        {
        }
    }
}
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using TBMX.Core.Services;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.Business.Models;
using UDPMonitor.ViewModels.Base;
using static UDPMonitor.App;

namespace UDPMonitor.ViewModels
{
    public class ODTMessage : BindableBase
    {
        private string _timeStamp;

        public string TimeStamp
        {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value); }
        }

        private string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }
    }

    public class Inbound_ViewModel : ViewModelBase
    {
        private readonly IInboundService _inboundService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<ODTMessage> _inMessages;

        public ObservableCollection<ODTMessage> InMessages
        {
            get { return _inMessages; }
            set { SetProperty(ref _inMessages, value); }
        }

        private string _port;

        public string Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }

        private bool _isConnected;

        public bool IsConnected
        {
            get { return _isConnected; }
            set { SetProperty(ref _isConnected, value); }
        }

        private ODTMessage _messageSelected;
        public ODTMessage MessageSelected
        {
            get { return _messageSelected; }
            set { SetProperty(ref _messageSelected, value); }
        }

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand<ODTMessage> CopySelectedCommand { get; private set; }
        public DelegateCommand<ODTMessage> ShowDetailCommand { get; private set; }

        public Inbound_ViewModel(IInboundService inboundService, IDialogService dialogService)
        {
            _inboundService = inboundService;
            _dialogService = dialogService;

            InMessages = new ObservableCollection<ODTMessage>();

            Port = "5000";
        }

        protected override void OnInitializeCommands()
        {
            base.OnInitializeCommands();

            ConnectCommand = new DelegateCommand(OnConnect);
            ClearCommand = new DelegateCommand(Clear);
            CopySelectedCommand = new DelegateCommand<ODTMessage>(CopySelected);
            ShowDetailCommand = new DelegateCommand<ODTMessage>(ShowDetail);
        }

        private void ShowDetail(ODTMessage message)
        {
            if (message == null) return;

            var dialogParameters = new DialogParameters
                {
                    { "message", message }
                };

            _dialogService.ShowDialog(RegisteredViews.DialogDetail_View, dialogParameters, null);
        }

        private void CopySelected(ODTMessage message)
        {
            if (message == null) return;

            Clipboard.SetText($"{message.Text}");
        }

        private void Clear()
        {
            InMessages.Clear();
        }

        private void OnConnect()
        {
            if (!_isConnected)
            {
                _inboundService.StartListen(int.Parse(Port));
                IsConnected = true;
            }
            else
            {
                _inboundService.StopListen(Port);
                IsConnected = false;
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            _inboundService.OnInboundMessageReceived += OnMessageArrived;
        }

        private void OnMessageArrived(InMessage message)
        {
            var odtMessage = new ODTMessage
            {
                TimeStamp = message.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                Text = message.Text
            };

            UIThread.Invoke(() =>
            {
                InMessages.Add(odtMessage);
            });
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _inboundService.OnInboundMessageReceived -= OnMessageArrived;
        }
    }
}
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Threading;
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
        private DateTime _timeStamp;

        public DateTime TimeStamp
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

        private TimeSpan _relativeTime;

        public TimeSpan RelativeTime
        {
            get { return _relativeTime; }
            set { SetProperty(ref _relativeTime, value); }
        }
    }

    public class Inbound_ViewModel : ViewModelBase
    {
   
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

        private int _messageCounter;

        public int MessageCounter
        {
            get { return _messageCounter; }
            set { SetProperty(ref _messageCounter, value); }
        }

        private double _messagesPerSecond;

        public double MessagesPerSecond
        {
            get { return _messagesPerSecond; }
            set { SetProperty(ref _messagesPerSecond, value); }
        }

        private DateTime? _lastReceivedAt;

        public DateTime? LastReceivedAt
        {
            get { return _lastReceivedAt; }
            set { SetProperty(ref _lastReceivedAt, value); }
        }

        private TimeSpan _lastDelta;

        public TimeSpan LastDelta
        {
            get { return _lastDelta; }
            set { SetProperty(ref _lastDelta, value); }
        }

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand<ODTMessage> CopySelectedCommand { get; private set; }
        public DelegateCommand<ODTMessage> ShowDetailCommand { get; private set; }

        // interni per calcolo rate
        private long _receivedSinceLastTick;
        private Timer _rateTimer;
        private readonly IInboundService _inboundService;
        private readonly IDialogService _dialogService;

        public Inbound_ViewModel(IInboundService inboundService, IDialogService dialogService)
        {
            _inboundService = inboundService;
            _dialogService = dialogService;

            InMessages = new ObservableCollection<ODTMessage>();

            Port = _inboundService.Port.ToString();

            _rateTimer = new Timer(_ =>
            {
                var c = Interlocked.Exchange(ref _receivedSinceLastTick, 0);

                UIThread.Invoke(() =>
                {
                    MessagesPerSecond = c;
                });

            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
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

        private static void CopySelected(ODTMessage message)
        {
            if (message == null) return;

            Clipboard.SetText($"{message.Text}");
        }

        private void Clear()
        {
            InMessages.Clear();
            MessageCounter = 0;
            MessagesPerSecond = 0;
            LastReceivedAt = null;
            LastDelta = TimeSpan.Zero;
            Interlocked.Exchange(ref _receivedSinceLastTick, 0);
        }

        private void OnConnect()
        {
            if (!_isConnected)
            {
                _inboundService.Port = int.Parse(Port);
                _inboundService.StartListen();
                IsConnected = true;
            }
            else
            {
                _inboundService.StopListen();
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
            DateTime? last = null;

            if (InMessages.Count > 0)
                last = InMessages[^1].TimeStamp;

            var delta = last.HasValue
                ? (message.TimeStamp - last.Value)
                : TimeSpan.Zero;

            var odtMessage = new ODTMessage
            {
                TimeStamp = message.TimeStamp,
                Text = message.Text,
                RelativeTime = delta
            };

            Interlocked.Increment(ref _receivedSinceLastTick);

            UIThread.Invoke(() =>
            {
                const int MaxItems = 2000;

                InMessages.Insert(0,odtMessage);

                if (InMessages.Count > MaxItems)
                    InMessages.RemoveAt(InMessages.Count - 1);

                MessageCounter += 1;

                LastReceivedAt = odtMessage.TimeStamp;
                LastDelta = odtMessage.RelativeTime;
            });
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            _inboundService.OnInboundMessageReceived -= OnMessageArrived;
        }
    }
}
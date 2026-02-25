using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.ViewModels.Base;
using static UDPMonitor.App;

namespace UDPMonitor.ViewModels
{
    public class Outbound_ViewModel : ViewModelBase
    {
        private readonly IOutboundService _outboundService;
        private readonly IDialogService _dialogService;
        private string _textMessage;

        public string TextMessage
        {
            get => _textMessage;
            set
            {
                if (SetProperty(ref _textMessage, value))
                    Volatile.Write(ref _messageSnapshot, value);
            }
        }

        private string _ipAddress;

        public string IpAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }

        private string _port;

        public string Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    // se ti disconnetti, stoppa auto-send
                    if (!value) StopAutoSend();
                }
            }
        }

        private bool _isAutoSending;
        public bool IsAutoSending
        {
            get => _isAutoSending;
            set => SetProperty(ref _isAutoSending, value);
        }

        private string _autoSendHz;
        public string AutoSendHz
        {
            get => _autoSendHz;
            set => SetProperty(ref _autoSendHz, value);
        }

       

        public DelegateCommand SendCommand { get; private set; }
        public DelegateCommand ToggleConnectionCommand { get; private set; }

        public DelegateCommand ToggleAutoSendCommand { get; private set; }
        public DelegateCommand StartAutoSendCommand { get; private set; }
        public DelegateCommand StopAutoSendCommand { get; private set; }

        private CancellationTokenSource _sendLoopCts;
        private Task _sendLoopTask;
        private string _messageSnapshot;

        public Outbound_ViewModel(IOutboundService outboundService, IDialogService dialogService)
        {
            _outboundService = outboundService;
            _dialogService = dialogService;
            Port = _outboundService.Port.ToString();
            IpAddress = _outboundService.IPAddress;

            AutoSendHz = "30";
        }

        protected override void OnInitializeCommands()
        {
            SendCommand = new DelegateCommand(Send, CanSend)
                .ObservesProperty(() => TextMessage)
                .ObservesProperty(() => IsConnected);

            ToggleConnectionCommand = new DelegateCommand(ConnectDisconnect);

            StartAutoSendCommand = new DelegateCommand(StartAutoSend, CanStartAutoSend)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => IsAutoSending)
                .ObservesProperty(() => TextMessage);

            StopAutoSendCommand = new DelegateCommand(StopAutoSend, CanStopAutoSend)
                .ObservesProperty(() => IsAutoSending);

            ToggleAutoSendCommand = new DelegateCommand(ToggleAutoSend, CanToggleAutoSend)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => IsAutoSending)
                .ObservesProperty(() => TextMessage);
        }

        private void ToggleAutoSend()
        {
            if (IsAutoSending) StopAutoSend();
            else StartAutoSend();
        }

        private bool CanToggleAutoSend() => CanStartAutoSend() || CanStopAutoSend();

        private void StartAutoSend()
        {
            if (!CanStartAutoSend())
                return;

            IsAutoSending = true;

            StopSendLoopInternal(); // sicurezza
            _sendLoopCts = new CancellationTokenSource();
            var token = _sendLoopCts.Token;

            _sendLoopTask = Task.Run(async () =>
            {
                var period = TimeSpan.FromSeconds(1.0 / int.Parse(AutoSendHz));
                var sw = Stopwatch.StartNew();
                long tick = 0;

                while (!token.IsCancellationRequested)
                {
                    var next = TimeSpan.FromTicks(period.Ticks * tick);

                    // manda l'ultimo snapshot disponibile
                    if (IsConnected)
                    {
                        var msg = Volatile.Read(ref _messageSnapshot);
                        if (!string.IsNullOrWhiteSpace(msg))
                        {
                            try
                            {
                                _outboundService.SendMessage(msg);
                            }
                            catch
                            {
                                // opzionale: gestisci errori (log / stop autosend)
                                // StopAutoSend();
                            }
                        }
                    }

                    tick++;

                    var delay = next - sw.Elapsed;
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, token).ConfigureAwait(false);
                    else
                        await Task.Yield(); // siamo in ritardo, evita accumulo
                }
            }, token);
        }

        private bool CanStartAutoSend()
            => IsConnected && !IsAutoSending && !string.IsNullOrWhiteSpace(TextMessage);

        private void StopAutoSend()
        {
            if (!IsAutoSending)
                return;

            IsAutoSending = false;
            StopSendLoopInternal();
        }

        private bool CanStopAutoSend() => IsAutoSending;

        private void StopSendLoopInternal()
        {
            if (_sendLoopCts == null)
                return;

            try { _sendLoopCts.Cancel(); } catch { }
            _sendLoopCts.Dispose();
            _sendLoopCts = null;
        }

        private void ConnectDisconnect()
        {
            if (!IsConnected)
            {
                try
                {
                    Connect();
                    IsConnected = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to connect: {ex.Message}", "Connection Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                StopAutoSend(); // importante
                Disconnect();
                IsConnected = false;
            }
        }

        private void Disconnect() => _outboundService.Disconnect();

        private void Connect()
        {
            if (!int.TryParse(Port, NumberStyles.Integer, CultureInfo.InvariantCulture, out var port) ||
                port < 0 || port > 65535)
            {
                throw new ArgumentException("Port number must be between 0 and 65535.");
            }

            _outboundService.Port = port;
            _outboundService.IPAddress = IpAddress;

            _outboundService.Connect();
        }

        private void Send()
        {
            if (IsConnected)
                _outboundService.SendMessage(TextMessage);

            if(TextMessage == "Alessandro Gaspani")
            {
               _dialogService.ShowDialog(RegisteredViews.DialogCreator_View);
            }
        }

        private bool CanSend() => IsConnected && !string.IsNullOrWhiteSpace(TextMessage);
    }
}
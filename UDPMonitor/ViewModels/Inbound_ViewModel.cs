using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using TBMX.Core.Services;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.Business.Models;
using UDPMonitor.Core;
using UDPMonitor.Core.Export;
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

        private bool _isAutoScrollEnabled;

        public bool IsAutoScrollEnabled
        {
            get { return _isAutoScrollEnabled; }
            set
            {
                SetProperty(ref _isAutoScrollEnabled, value);
            }
        }

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand CopySelectedCommand { get; private set; }
        public DelegateCommand ShowDetailCommand { get; private set; }

        public DelegateCommand ExportCommand { get; private set; }

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
            CopySelectedCommand = new DelegateCommand(CopySelected);
            ShowDetailCommand = new DelegateCommand(ShowDetail);
            ExportCommand = new DelegateCommand(Export);
        }

        private const char Sep = ';';

        private void Export()
        {
            var data = InMessages; // oppure MessagesSelected se esporti solo i selezionati
            if (data == null || data.Count == 0)
                return;

            string initialDir = ApplicationService.LogFolderPath;

            // Crea la cartella se non esiste (opzionale ma comodo)
            if (!Directory.Exists(initialDir))
                Directory.CreateDirectory(initialDir);

            var dlg = new SaveFileDialog
            {
                Title = "Export log to CSV",
                Filter = "CSV file (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = ".csv",
                AddExtension = true,
                OverwritePrompt = true,
                FileName = $"UDPMonitor_Log_{DateTime.Now:yyyyMMdd_HHmmss}.csv",
                InitialDirectory = initialDir
            };

            bool? result = dlg.ShowDialog(); // owner opzionale
            if (result != true)
                return;

            var filePath = dlg.FileName;

            // Usa UTF8 per evitare problemi con accenti/caratteri speciali
            data.ToCSV(
                filePath,
                buildContent: BuildCsvContent,
                buildHeader: _ => $"TimeStamp{Sep}RelativeTime{Sep}Text",
                encoding: Encoding.UTF8);
        }

        private static string BuildCsvContent(IList<ODTMessage> messages)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < messages.Count; i++)
            {
                var m = messages[i];

                sb.Append(CsvEscape(m.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)));
                sb.Append(Sep);

                sb.Append(CsvEscape(FormatRelative(m.RelativeTime)));
                sb.Append(Sep);

                sb.Append(CsvEscape(m.Text));

                // newline tra record
                if (i < messages.Count - 1)
                    sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        private static string FormatRelative(TimeSpan ts)
        {
            // hh:mm:ss.fff
            return ts.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
        }

        private static string CsvEscape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            // Se contiene separatore, virgolette o newline => va quotato
            bool mustQuote =
                value.IndexOf(Sep) >= 0 ||
                value.IndexOf('"') >= 0 ||
                value.IndexOf('\r') >= 0 ||
                value.IndexOf('\n') >= 0;

            if (!mustQuote)
                return value;

            // raddoppia le virgolette interne
            value = value.Replace("\"", "\"\"");

            return $"\"{value}\"";
        }

        private void ShowDetail()
        {
            if (MessageSelected == null) return;

            var dialogParameters = new DialogParameters
                {
                    { "message", MessageSelected }
                };

            _dialogService.ShowDialog(RegisteredViews.DialogDetail_View, dialogParameters, null);
        }

        private void CopySelected()
        {
            if (MessageSelected == null) return;

            Clipboard.SetText($"{MessageSelected.Text}");
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
                last = InMessages[0].TimeStamp;

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

                InMessages.Insert(0, odtMessage);

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
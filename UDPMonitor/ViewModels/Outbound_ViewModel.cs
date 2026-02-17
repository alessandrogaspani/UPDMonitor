using Prism.Commands;
using System;
using System.Windows;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.ViewModels.Base;

namespace UDPMonitor.ViewModels
{
    public class Outbound_ViewModel : ViewModelBase
    {
        private readonly IOutboundService _outboundService;

        private string _textMessage;

        public string TextMessage
        {
            get { return _textMessage; }
            set { SetProperty(ref _textMessage, value); }
        }

        private string _ipAddress;

        public string IpAddress
        {
            get { return _ipAddress; }
            set { SetProperty(ref _ipAddress, value); }
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

        public DelegateCommand SendCommand { get; private set; }

        public DelegateCommand ConnectDisconnectCommand { get; private set; }

        public Outbound_ViewModel(IOutboundService outboundService)
        {
            _outboundService = outboundService;

            IpAddress = "127.0.0.1";
            Port = "5000";
        }

        protected override void OnInitializeCommands()
        {
            SendCommand = new DelegateCommand(Send, CanSend).ObservesProperty(() => TextMessage);
            ConnectDisconnectCommand = new DelegateCommand(ConnectDisconnect);
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
                    MessageBox.Show($"Failed to connect: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Disconnect();
                IsConnected = false;
            }
        }

        private void Disconnect()
        {
            _outboundService.Disconnect();
        }

        private void Connect()
        {
            if (int.Parse(_port) < 0 || int.Parse(_port) > 65535)
            {
                throw new ArgumentException("Port number must be between 0 and 65535.");
            }

            _outboundService.Connect(IpAddress, int.Parse(Port));
        }

        private void Send()
        {
            if (_isConnected)
            {
                _outboundService.SendMessage(_textMessage);
            }
        }

        private bool CanSend()
        {
            return IsConnected && !string.IsNullOrWhiteSpace(TextMessage);
        }
    }
}
using UDPMonitor.Business.Config;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.Business.Models;
using UDPMonitor.Core.Configuration;
using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Business
{
    public class InboundService : IInboundService
    {
        private const string InChannelTag = "inChannel";

        public event InMessageReceivedEventHandler OnInboundMessageReceived;

        public int Port { get; set; }

        public InboundService(IConfigurationManager<UDPManagerConfiguration> configManager)
        {
            Port = configManager.Configuration.Inbound.Port;
        }

        public void StartListen()
        {
            UdpService.AddInChannel(InChannelTag, Port, OnDataReceived);
            UdpService.StartReceiving(InChannelTag);
        }

        private void OnDataReceived(byte[] bytes)
        {
            var message = new InMessage()
            {
                TimeStamp = DateTime.Now,
                Text = System.Text.Encoding.UTF8.GetString(bytes),
            };

            OnInboundMessageReceived?.Invoke(message);
        }

        public void StopListen()
        {
            UdpService.StopReceiving(InChannelTag);
            UdpService.CloseInChannel(InChannelTag);
        }
    }
}
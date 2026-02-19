using UDPMonitor.Business.Config;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.Core.Configuration;
using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Business
{
    public class OutboundService : IOutboundService
    {
        private const string OutChanngelTag = "outChannel";

        public event OutMessageSentEventHandler OnOutMessageSent;

        public int Port { get; set; }
        public string IPAddress { get; set; }

        public OutboundService(IConfigurationManager<UDPManagerConfiguration> configManager)
        {
            Port = configManager.Configuration.Outbound.Port;
            IPAddress = configManager.Configuration.Outbound.IP;
        }

        public void Connect()
        {
            UdpService.AddOutChannel(OutChanngelTag, Port, IPAddress, OnMessageSent);
        }

        private void OnMessageSent(byte[] data)
        {
            string message = System.Text.Encoding.UTF8.GetString(data);

            OnOutMessageSent?.Invoke(message);
        }

        public void SendMessage(string message)
        {
            UdpService.Send(OutChanngelTag, message);
        }

        public void Disconnect()
        {
            UdpService.CloseOutChannel(OutChanngelTag);
        }
    }
}